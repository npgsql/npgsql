using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql.Internal.Composites;

[RequiresDynamicCode("Serializing arbitrary types can require creating new generic types or methods. This may not work when AOT compiling.")]
static class ReflectionCompositeInfoFactory
{
    public static CompositeInfo<T> CreateCompositeInfo<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        PostgresCompositeType pgType, INpgsqlNameTranslator nameTranslator, PgSerializerOptions options)
    {
        var pgFields = pgType.Fields;
        var propertyMap = MapProperties<T>(pgFields, nameTranslator);
        var fieldMap = MapFields<T>(pgFields, nameTranslator);

        var duplicates = propertyMap.Keys.Intersect(fieldMap.Keys).ToArray();
        if (duplicates.Length > 0)
            throw new AmbiguousMatchException($"Property {propertyMap[duplicates[0]].Name} and field {fieldMap[duplicates[0]].Name} map to the same '{pgFields[duplicates[0]].Name}' composite field name.");

        var (constructorInfo, parameterFieldMap) = MapBestMatchingConstructor<T>(pgFields, nameTranslator);
        var constructorParameters = constructorInfo?.GetParameters() ?? [];
        var compositeFields = new CompositeFieldInfo?[pgFields.Count];
        for (var i = 0; i < parameterFieldMap.Length; i++)
        {
            var fieldIndex = parameterFieldMap[i];
            var pgField = pgFields[fieldIndex];
            var parameter = constructorParameters[i];
            var reprTypeId = options.ToCanonicalTypeId(pgField.Type.GetRepresentationalType());
            PgTypeInfo pgTypeInfo;
            Delegate getter;
            if (propertyMap.TryGetValue(fieldIndex, out var property) && property.GetMethod is not null)
            {
                if (property.PropertyType != parameter.ParameterType)
                    throw new InvalidOperationException($"Could not find a matching getter for constructor parameter {parameter.Name} and type {parameter.ParameterType} mapped to composite field {pgFields[fieldIndex].Name}.");

                pgTypeInfo = options.GetTypeInfoInternal(property.PropertyType, reprTypeId) ?? throw NotSupportedField(pgType, pgField, isField: false, property.Name, property.PropertyType);
                getter = CreateGetter<T>(property);
            }
            else if (fieldMap.TryGetValue(fieldIndex, out var field))
            {
                if (field.FieldType != parameter.ParameterType)
                    throw new InvalidOperationException($"Could not find a matching getter for constructor parameter {parameter.Name} and type {parameter.ParameterType} mapped to composite field {pgFields[fieldIndex].Name}.");

                pgTypeInfo = options.GetTypeInfoInternal(field.FieldType, reprTypeId) ?? throw NotSupportedField(pgType, pgField, isField: true, field.Name, field.FieldType);
                getter = CreateGetter<T>(field);
            }
            else
                throw new InvalidOperationException($"Cannot find property or field for composite field {pgFields[fieldIndex].Name}.");

            compositeFields[fieldIndex] = CreateCompositeFieldInfo(pgField.Name, pgTypeInfo.Type, pgTypeInfo, options.ToCanonicalTypeId(pgField.Type), getter, i);
        }

        for (var fieldIndex = 0; fieldIndex < pgFields.Count; fieldIndex++)
        {
            // Handled by constructor.
            if (compositeFields[fieldIndex] is not null)
                continue;

            var pgField = pgFields[fieldIndex];
            var reprTypeId = options.ToCanonicalTypeId(pgField.Type.GetRepresentationalType());
            PgTypeInfo pgTypeInfo;
            Delegate getter;
            Delegate setter;
            if (propertyMap.TryGetValue(fieldIndex, out var property))
            {
                pgTypeInfo = options.GetTypeInfoInternal(property.PropertyType, reprTypeId)
                             ?? throw NotSupportedField(pgType, pgField, isField: false, property.Name, property.PropertyType);
                getter = CreateGetter<T>(property);
                setter = CreateSetter<T>(property);
            }
            else if (fieldMap.TryGetValue(fieldIndex, out var field))
            {
                pgTypeInfo = options.GetTypeInfoInternal(field.FieldType, reprTypeId)
                             ?? throw NotSupportedField(pgType, pgField, isField: true, field.Name, field.FieldType);
                getter = CreateGetter<T>(field);
                setter = CreateSetter<T>(field);
            }
            else
                throw new InvalidOperationException($"Cannot find property or field for composite field '{pgFields[fieldIndex].Name}'.");

            compositeFields[fieldIndex] = CreateCompositeFieldInfo(pgField.Name, pgTypeInfo.Type, pgTypeInfo, options.ToCanonicalTypeId(pgField.Type), getter, setter);
        }

        Debug.Assert(compositeFields.All(x => x is not null));

        var constructor = constructorInfo is null ? _ => Activator.CreateInstance<T>() : CreateStrongBoxConstructor<T>(constructorInfo);
        return new CompositeInfo<T>(compositeFields!, constructorInfo is null ? 0 : constructorParameters.Length, constructor);

        static NotSupportedException NotSupportedField(PostgresCompositeType composite, PostgresCompositeType.Field field, bool isField, string name, Type type)
            => new($"No mapping could be found for ('{type.FullName}', '{field.Type.FullName}'). Mapping: CLR {(isField ? "field" : "property")} '{typeof(T).FullName}.{name}' <-> Composite field '{composite.Name}.{field.Name}'");
    }

    static Delegate CreateGetter<T>(FieldInfo info)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        return Expression
            .Lambda(typeof(Func<,>).MakeGenericType(typeof(object), info.FieldType),
                Expression.Field(UnboxAny(instance, typeof(T)), info),
                instance)
            .Compile();
    }

    static Delegate CreateSetter<T>(FieldInfo info)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var value = Expression.Parameter(info.FieldType, "value");

        return Expression
            .Lambda(typeof(Action<,>).MakeGenericType(typeof(object), info.FieldType),
                Expression.Assign(Expression.Field(UnboxAny(instance, typeof(T)), info), value), instance, value)
            .Compile();
    }

    static Delegate CreateGetter<T>(PropertyInfo info)
    {
        var invalidOpExceptionMessageConstructor = typeof(InvalidOperationException).GetConstructor([typeof(string)])!;
        var instance = Expression.Parameter(typeof(object), "instance");
        var body = info.GetMethod is null || !info.GetMethod.IsPublic
            ? (Expression)Expression.Throw(Expression.New(invalidOpExceptionMessageConstructor,
                Expression.Constant($"No (public) getter for '{info}' on type {typeof(T)}")), info.PropertyType)
            : Expression.Property(UnboxAny(instance, typeof(T)), info);

        return Expression
            .Lambda(typeof(Func<,>).MakeGenericType(typeof(object), info.PropertyType), body, instance)
            .Compile();
    }

    static Delegate CreateSetter<T>(PropertyInfo info)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var value = Expression.Parameter(info.PropertyType, "value");

        var invalidOpExceptionMessageConstructor = typeof(InvalidOperationException).GetConstructor([typeof(string)])!;
        var body = info.SetMethod is null || !info.SetMethod.IsPublic
            ? (Expression)Expression.Throw(Expression.New(invalidOpExceptionMessageConstructor,
                Expression.Constant($"No (public) setter for '{info}' on type {typeof(T)}")), info.PropertyType)
            : Expression.Call(UnboxAny(instance, typeof(T)), info.SetMethod, value);

        return Expression
            .Lambda(typeof(Action<,>).MakeGenericType(typeof(object), info.PropertyType), body, instance, value)
            .Compile();
    }

    static Expression UnboxAny(Expression expression, Type type)
        => type.IsValueType ? Expression.Unbox(expression, type) : Expression.Convert(expression, type, null);

    [DynamicDependency(nameof(StrongBox<object>.TypedValue), typeof(StrongBox<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicProperties, typeof(StrongBox[]))]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "DynamicDependencies in place for the System.Linq.Expression.Property calls")]
    static Func<StrongBox[], T> CreateStrongBoxConstructor<T>(ConstructorInfo constructorInfo)
    {
        var values = Expression.Parameter(typeof(StrongBox[]), "values");

        var parameters = constructorInfo.GetParameters();
        var parameterCount = Expression.Constant(parameters.Length);
        var argumentExceptionNameMessageConstructor = typeof(ArgumentException).GetConstructor([typeof(string), typeof(string)])!;
        return Expression
            .Lambda<Func<StrongBox[], T>>(
                Expression.Block(
                    Expression.IfThen(
                        Expression.LessThan(Expression.Property(values, nameof(Array.Length)), parameterCount),

                        Expression.Throw(Expression.New(argumentExceptionNameMessageConstructor,
                            Expression.Constant("Passed fewer arguments than there are constructor parameters."), Expression.Constant(values.Name)))
                    ),
                    Expression.New(constructorInfo, parameters.Select((parameter, i) =>
                        Expression.Property(
                            UnboxAny(
                                Expression.ArrayIndex(values, Expression.Constant(i)),
                                typeof(StrongBox<>).MakeGenericType(parameter.ParameterType)
                            ),
                            nameof(StrongBox<object>.TypedValue)
                        )
                    ))
                ), values)
            .Compile();
    }
    static CompositeFieldInfo CreateCompositeFieldInfo(string name, Type type, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId, Delegate getter, int constructorParameterIndex)
        => (CompositeFieldInfo)Activator.CreateInstance(
            typeof(CompositeFieldInfo<>).MakeGenericType(type), name, typeInfo, nominalPgTypeId, getter, constructorParameterIndex)!;

    static CompositeFieldInfo CreateCompositeFieldInfo(string name, Type type, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId, Delegate getter, Delegate setter)
        => (CompositeFieldInfo)Activator.CreateInstance(
            typeof(CompositeFieldInfo<>).MakeGenericType(type), name, typeInfo, nominalPgTypeId, getter, setter)!;

    static Dictionary<int, PropertyInfo> MapProperties<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(IReadOnlyList<PostgresCompositeType.Field> fields, INpgsqlNameTranslator nameTranslator)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertiesAndNames = properties.Select(x =>
        {
            var attr = x.GetCustomAttribute<PgNameAttribute>();
            var name = attr?.PgName ?? nameTranslator.TranslateMemberName(x.Name);
            return new KeyValuePair<string, PropertyInfo>(name, x);
        }).ToArray();

        var duplicates = propertiesAndNames.GroupBy(x => x.Key).Where(g => g.Count() > 1).ToArray();
        if (duplicates.Length > 0)
            throw new AmbiguousMatchException($"Multiple properties are mapped to the '{duplicates[0].Key}' field.");

        var propertiesMap = propertiesAndNames.ToDictionary(x => x.Key, x => x.Value);
        var result = new Dictionary<int, PropertyInfo>();
        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            if (!propertiesMap.TryGetValue(field.Name, out var value))
                continue;

            result[i] = value;
        }

        return result;
    }

    static Dictionary<int, FieldInfo> MapFields<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(IReadOnlyList<PostgresCompositeType.Field> fields, INpgsqlNameTranslator nameTranslator)
    {
        var clrFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
        var clrFieldsAndNames = clrFields.Select(x =>
        {
            var attr = x.GetCustomAttribute<PgNameAttribute>();
            var name = attr?.PgName ?? nameTranslator.TranslateMemberName(x.Name);
            return new KeyValuePair<string, FieldInfo>(name, x);
        }).ToArray();

        var duplicates = clrFieldsAndNames.GroupBy(x => x.Key).Where(g => g.Count() > 1).ToArray();
        if (duplicates.Length > 0)
            throw new AmbiguousMatchException($"Multiple properties are mapped to the '{duplicates[0].Key}' field.");

        var clrFieldsMap = clrFieldsAndNames.ToDictionary(x => x.Key, x => x.Value);
        var result = new Dictionary<int, FieldInfo>();
        for (var i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            if (!clrFieldsMap.TryGetValue(field.Name, out var value))
                continue;

            result[i] = value;
        }

        return result;
    }

    static (ConstructorInfo? ConstructorInfo, int[] ParameterFieldMap) MapBestMatchingConstructor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(IReadOnlyList<PostgresCompositeType.Field> fields, INpgsqlNameTranslator nameTranslator)
    {
        ConstructorInfo? clrDefaultConstructor = null;
        Exception? duplicatesException = null;
        foreach (var constructor in typeof(T).GetConstructors().OrderByDescending(x => x.GetParameters().Length))
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
                clrDefaultConstructor = constructor;

            var parametersMap = new int[parameters.Length];
            Array.Fill(parametersMap, -1);
            for (var i = 0; i < parameters.Length; i++)
            {
                var clrParameter = parameters[i];
                var attr = clrParameter.GetCustomAttribute<PgNameAttribute>();
                var name = attr?.PgName ?? (clrParameter.Name is { } clrName ? nameTranslator.TranslateMemberName(clrName) : null);
                if (name is null)
                    break;

                for (var pgFieldIndex = 0; pgFieldIndex < fields.Count; pgFieldIndex++)
                {
                    if (fields[pgFieldIndex].Name == name)
                    {
                        parametersMap[i] = pgFieldIndex;
                        break;
                    }
                }
            }

            if (parametersMap.Any(x => x is -1))
                continue;

            var duplicates = parametersMap.GroupBy(x => x).Where(g => g.Count() > 1).ToArray();
            if (duplicates.Length is 0)
                return (constructor, parametersMap);

            duplicatesException = new AmbiguousMatchException($"Multiple parameters are mapped to the field '{fields[duplicates[0].Key].Name}' in constructor: {constructor}.");
        }

        if (duplicatesException is not null)
            throw duplicatesException;

        if (clrDefaultConstructor is null && !typeof(T).IsValueType)
            throw new InvalidOperationException($"No parameterless constructor defined for type '{typeof(T)}'.");

        return (clrDefaultConstructor, []);
    }
}
