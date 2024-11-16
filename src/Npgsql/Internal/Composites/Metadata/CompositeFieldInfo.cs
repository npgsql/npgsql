using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using Npgsql.Util;

namespace Npgsql.Internal.Composites;

abstract class CompositeFieldInfo
{
    protected PgTypeInfo PgTypeInfo { get; }
    protected PgConverter? Converter { get; }
    protected BufferRequirements _binaryBufferRequirements;

    /// <summary>
    /// CompositeFieldInfo constructor.
    /// </summary>
    /// <param name="name">Name of the field.</param>
    /// <param name="typeInfo">Type info for reading/writing.</param>
    /// <param name="nominalPgTypeId">The nominal field type, this may differ from the typeInfo.PgTypeId when the field is a domain type.</param>
    private protected CompositeFieldInfo(string name, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId)
    {
        Name = name;
        PgTypeInfo = typeInfo;
        PgTypeId = nominalPgTypeId;

        if (typeInfo.PgTypeId is null)
            ThrowHelper.ThrowArgumentException("PgTypeInfo must have a PgTypeId.");

        if (!typeInfo.IsResolverInfo)
        {
            var resolution = typeInfo.GetResolution();
            if (typeInfo.GetBufferRequirements(resolution.Converter, DataFormat.Binary) is not { } bufferRequirements)
            {
                ThrowHelper.ThrowInvalidOperationException("Converter must support binary format to participate in composite types.");
                return;
            }
            _binaryBufferRequirements = bufferRequirements;
            Converter = resolution.Converter;
        }
    }

    public PgConverter GetReadInfo(out Size readRequirement)
    {
        if (Converter is not null)
        {
            readRequirement = _binaryBufferRequirements.Read;
            return Converter;
        }

        if (!PgTypeInfo.TryBind(new Field(Name, PgTypeInfo.PgTypeId.GetValueOrDefault(), -1), DataFormat.Binary, out var converterInfo))
            ThrowHelper.ThrowInvalidOperationException("Converter must support binary format to participate in composite types.");

        readRequirement = converterInfo.BufferRequirement;
        return converterInfo.Converter;
    }

    public PgConverter GetWriteInfo(object instance, out Size writeRequirement)
    {
        if (Converter is null)
            return BindValue(instance, out writeRequirement);

        writeRequirement = _binaryBufferRequirements.Write;
        return Converter;

    }

    protected ValueTask ReadAsObject(bool async, PgConverter converter, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken)
    {
        if (async)
        {
            var task = converter.ReadAsObjectAsync(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return Core(builder, task);

            AddValue(builder, task.Result);
        }
        else
            AddValue(builder, converter.ReadAsObject(reader));
        return new();

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask Core(CompositeBuilder builder, ValueTask<object> task)
        {
            builder.AddValue(await task.ConfigureAwait(false));
        }
    }

    protected ValueTask WriteAsObject(bool async, PgConverter converter, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        if (async)
            return converter.WriteAsObjectAsync(writer, value, cancellationToken);

        converter.WriteAsObject(writer, value);
        return new();
    }

    public string Name { get; }
    public PgTypeId PgTypeId { get; }
    public Size BinaryReadRequirement => Converter is not null ? _binaryBufferRequirements.Read : Size.Unknown;
    public Size BinaryWriteRequirement => Converter is not null ? _binaryBufferRequirements.Write : Size.Unknown;

    public abstract Type Type { get; }

    protected abstract PgConverter BindValue(object instance, out Size writeRequirement);
    protected abstract void AddValue(CompositeBuilder builder, object value);

    public abstract StrongBox CreateBox();
    public abstract void Set(object instance, StrongBox value);
    public abstract int? ConstructorParameterIndex { get; }
    public abstract bool IsDbNullable { get; }

    public abstract void ReadDbNull(CompositeBuilder builder);
    public abstract ValueTask Read(bool async, PgConverter converter, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken = default);
    public abstract bool IsDbNull(PgConverter converter, object instance, ref object? writeState);
    public abstract Size? GetSizeOrDbNull(PgConverter converter, DataFormat format, Size writeRequirement, object instance, ref object? writeState);
    public abstract ValueTask Write(bool async, PgConverter converter, PgWriter writer, object instance, CancellationToken cancellationToken);
}

sealed class CompositeFieldInfo<T> : CompositeFieldInfo
{
    readonly Action<object, T>? _setter;
    readonly int _parameterIndex;
    readonly Func<object, T> _getter;
    readonly bool _asObject;

    CompositeFieldInfo(string name, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId, Func<object, T> getter)
        : base(name, typeInfo, nominalPgTypeId)
    {
        if (typeInfo.Type != typeof(T))
            throw new InvalidOperationException($"PgTypeInfo type '{typeInfo.Type.FullName}' must be equal to field type '{typeof(T)}'.");

        if (!typeInfo.IsResolverInfo)
        {
            var resolution = typeInfo.GetResolution();
            var typeToConvert = resolution.Converter.TypeToConvert;
            _asObject = typeToConvert != typeof(T);
            if (!typeToConvert.IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException($"Converter type '{typeToConvert.FullName}' must be assignable from field type '{typeof(T)}'.");
        }

        _getter = getter;
    }

    // Accessed through reflection (ReflectionCompositeInfoFactory)
    public CompositeFieldInfo(string name, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId, Func<object, T> getter, int parameterIndex)
        : this(name, typeInfo, nominalPgTypeId, getter)
        => _parameterIndex = parameterIndex;

    // Accessed through reflection (ReflectionCompositeInfoFactory)
    public CompositeFieldInfo(string name, PgTypeInfo typeInfo, PgTypeId nominalPgTypeId, Func<object, T> getter, Action<object, T> setter)
        : this(name, typeInfo, nominalPgTypeId, getter)
        => _setter = setter;

    bool AsObject(PgConverter converter)
        => ReferenceEquals(Converter, converter) ? _asObject : converter.TypeToConvert != typeof(T);

    public override Type Type => typeof(T);

    public override int? ConstructorParameterIndex => _setter is not null ? null : _parameterIndex;

    public T Get(object instance) => _getter(instance);

    public override StrongBox CreateBox() => new Util.StrongBox<T>();

    public void Set(object instance, T value)
    {
        if (_setter is null)
            throw new InvalidOperationException("Not a composite field for a clr field.");

        _setter(instance, value);
    }

    public override void Set(object instance, StrongBox value)
    {
        if (_setter is null)
            throw new InvalidOperationException("Not a composite field for a clr field.");

        _setter(instance, ((Util.StrongBox<T>)value).TypedValue!);
    }

    public override void ReadDbNull(CompositeBuilder builder)
    {
        if (default(T) != null)
            throw new InvalidCastException($"Type {typeof(T).FullName} does not have null as a possible value.");

        builder.AddValue((T?)default);
    }

    protected override PgConverter BindValue(object instance, out Size writeRequirement)
    {
        var value = _getter(instance);
        var resolution = PgTypeInfo.IsBoxing ? PgTypeInfo.GetObjectResolution(value) : PgTypeInfo.GetResolution(value);
        if (PgTypeInfo.GetBufferRequirements(resolution.Converter, DataFormat.Binary) is not { } bufferRequirements)
        {
            ThrowHelper.ThrowInvalidOperationException("Converter must support binary format to participate in composite types.");
            writeRequirement = default;
            return default;
        }

        writeRequirement = bufferRequirements.Write;
        return resolution.Converter;
    }

    protected override void AddValue(CompositeBuilder builder, object value) => builder.AddValue((T)value);

    public override ValueTask Read(bool async, PgConverter converter, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken = default)
    {
        if (AsObject(converter))
            return ReadAsObject(async, converter, builder, reader, cancellationToken);

        if (async)
        {
            var task = ((PgConverter<T>)converter).ReadAsync(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return Core(builder, task);

            builder.AddValue(task.Result);
        }
        else
            builder.AddValue(((PgConverter<T>)converter).Read(reader));
        return new();

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask Core(CompositeBuilder builder, ValueTask<T> task)
        {
            builder.AddValue(await task.ConfigureAwait(false));
        }
    }

    public override bool IsDbNullable => Converter?.IsDbNullable ?? true;

    public override bool IsDbNull(PgConverter converter, object instance, ref object? writeState)
    {
        var value = _getter(instance);
        return AsObject(converter) ? converter.IsDbNullAsObject(value, ref writeState) : ((PgConverter<T>)converter).IsDbNull(value, ref writeState);
    }

    public override Size? GetSizeOrDbNull(PgConverter converter, DataFormat format, Size writeRequirement, object instance, ref object? writeState)
    {
        var value = _getter(instance);
        return AsObject(converter)
            ? converter.GetSizeOrDbNullAsObject(format, writeRequirement, value, ref writeState)
            : ((PgConverter<T>)converter).GetSizeOrDbNull(format, writeRequirement, value, ref writeState);
    }

    public override ValueTask Write(bool async, PgConverter converter, PgWriter writer, object instance, CancellationToken cancellationToken)
    {
        var value = _getter(instance);
        if (AsObject(converter))
            return WriteAsObject(async, converter, writer, value!, cancellationToken);

        if (async)
            return ((PgConverter<T>)converter).WriteAsync(writer, value!, cancellationToken);

        ((PgConverter<T>)converter).Write(writer, value!);
        return new();
    }
}
