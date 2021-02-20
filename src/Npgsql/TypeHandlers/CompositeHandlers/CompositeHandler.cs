using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    class CompositeHandler<T> : NpgsqlTypeHandler<T>, ICompositeHandler
    {
        readonly ConnectorTypeMapper _typeMapper;
        readonly INpgsqlNameTranslator _nameTranslator;

        Func<T>? _constructor;
        CompositeConstructorHandler<T>? _constructorHandler;
        CompositeMemberHandler<T>[] _memberHandlers = null!;

        public Type CompositeType => typeof(T);

        public CompositeHandler(PostgresCompositeType postgresType, ConnectorTypeMapper typeMapper, INpgsqlNameTranslator nameTranslator)
            : base(postgresType)
        {
            _typeMapper = typeMapper;
            _nameTranslator = nameTranslator;
        }

        public override ValueTask<T> Read(NpgsqlReadBuffer buffer, int length, bool async, FieldDescription? fieldDescription = null)
        {
            Initialize();

            return _constructorHandler is null
                ? ReadUsingMemberHandlers()
                : _constructorHandler.Read(buffer, async);

            async ValueTask<T> ReadUsingMemberHandlers()
            {
                await buffer.Ensure(sizeof(int), async);

                var fieldCount = buffer.ReadInt32();
                if (fieldCount != _memberHandlers.Length)
                    throw new InvalidOperationException($"pg_attributes contains {_memberHandlers.Length} fields for type {PgDisplayName}, but {fieldCount} fields were received.");

                if (IsValueType<T>.Value)
                {
                    var composite = new ByReference<T> { Value = _constructor!() };
                    foreach (var member in _memberHandlers)
                        await member.Read(composite, buffer, async);

                    return composite.Value;
                }
                else
                {
                    var composite = _constructor!();
                    foreach (var member in _memberHandlers)
                        await member.Read(composite, buffer, async);

                    return composite;
                }
            }
        }

        public override async Task Write(T value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            Initialize();

            if (buffer.WriteSpaceLeft < sizeof(int))
                await buffer.Flush(async, cancellationToken);

            buffer.WriteInt32(_memberHandlers.Length);

            foreach (var member in _memberHandlers)
                await member.Write(value, buffer, lengthCache, async, cancellationToken);
        }

        public override int ValidateAndGetLength(T value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            Initialize();

            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);

            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            // Leave empty slot for the entire composite type, and go ahead an populate the element slots
            var position = lengthCache.Position;
            lengthCache.Set(0);

            // number of fields + (type oid + field length) * member count
            var length = sizeof(int) + sizeof(int) * 2 * _memberHandlers.Length;
            foreach (var member in _memberHandlers)
                length += member.ValidateAndGetLength(value, ref lengthCache);

            return lengthCache.Lengths[position] = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Initialize()
        {
            if (_memberHandlers is null)
                InitializeCore();

            void InitializeCore()
            {
                var pgType = (PostgresCompositeType)PostgresType;

                _memberHandlers = CreateMemberHandlers(pgType, _typeMapper, _nameTranslator);
                _constructorHandler = CreateConstructorHandler(pgType, _typeMapper, _nameTranslator);
                _constructor = _constructorHandler is null
                    ? Expression
                        .Lambda<Func<T>>(Expression.New(typeof(T)))
                        .Compile()
                    : null;
            }
        }

        static CompositeConstructorHandler<T>? CreateConstructorHandler(PostgresCompositeType pgType, ConnectorTypeMapper typeMapper, INpgsqlNameTranslator nameTranslator)
        {
            var pgFields = pgType.Fields;
            var clrType = typeof(T);

            ConstructorInfo? clrDefaultConstructor = null;

            foreach (var clrConstructor in clrType.GetConstructors())
            {
                var clrParameters = clrConstructor.GetParameters();
                if (clrParameters.Length != pgFields.Count)
                {
                    if (clrParameters.Length == 0)
                        clrDefaultConstructor = clrConstructor;

                    continue;
                }

                var clrParameterHandlerCount = 0;
                var clrParametersMapped = new ParameterInfo[pgFields.Count];

                foreach (var clrParameter in clrParameters)
                {
                    var attr = clrParameter.GetCustomAttribute<PgNameAttribute>();
                    var name = attr?.PgName ?? (clrParameter.Name is string clrName ? nameTranslator.TranslateMemberName(clrName) : null);
                    if (name is null)
                        break;

                    for (var pgFieldIndex = pgFields.Count - 1; pgFieldIndex >= 0; --pgFieldIndex)
                    {
                        var pgField = pgFields[pgFieldIndex];
                        if (pgField.Name != name)
                            continue;

                        if (clrParametersMapped[pgFieldIndex] != null)
                            throw new AmbiguousMatchException($"Multiple constructor parameters are mapped to the '{pgField.Name}' field.");

                        clrParameterHandlerCount++;
                        clrParametersMapped[pgFieldIndex] = clrParameter;

                        break;
                    }
                }

                if (clrParameterHandlerCount < pgFields.Count)
                    continue;

                var clrParameterHandlers = new CompositeParameterHandler[pgFields.Count];
                for (var pgFieldIndex = 0; pgFieldIndex < pgFields.Count; ++pgFieldIndex)
                {
                    var pgField = pgFields[pgFieldIndex];

                    if (!typeMapper.TryGetByOID(pgField.Type.OID, out var handler))
                        throw new NpgsqlException($"PostgreSQL composite type {pgType.DisplayName} has field {pgField.Type.DisplayName} with an unknown type (OID = {pgField.Type.OID}).");

                    var clrParameter = clrParametersMapped[pgFieldIndex];
                    var clrParameterHandlerType = typeof(CompositeParameterHandler<>)
                        .MakeGenericType(clrParameter.ParameterType);

                    clrParameterHandlers[pgFieldIndex] = (CompositeParameterHandler)Activator.CreateInstance(
                        clrParameterHandlerType,
                        BindingFlags.Instance | BindingFlags.Public,
                        binder: null,
                        args: new object[] { handler, clrParameter },
                        culture: null)!;
                }

                return CompositeConstructorHandler<T>.Create(pgType, clrConstructor, clrParameterHandlers);
            }

            if (clrDefaultConstructor is null && !clrType.IsValueType)
                throw new InvalidOperationException($"No parameterless constructor defined for type '{clrType}'.");

            return null;
        }

        static CompositeMemberHandler<T>[] CreateMemberHandlers(PostgresCompositeType pgType, ConnectorTypeMapper typeMapper, INpgsqlNameTranslator nameTranslator)
        {
            var pgFields = pgType.Fields;

            var clrType = typeof(T);
            var clrMemberHandlers = new CompositeMemberHandler<T>[pgFields.Count];
            var clrMemberHandlerCount = 0;
            var clrMemberHandlerType = IsValueType<T>.Value
                ? typeof(CompositeStructMemberHandler<,>)
                : typeof(CompositeClassMemberHandler<,>);

            foreach (var clrProperty in clrType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                CreateMemberHandler(clrProperty, clrProperty.PropertyType);

            foreach (var clrField in clrType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                CreateMemberHandler(clrField, clrField.FieldType);

            if (clrMemberHandlerCount != pgFields.Count)
            {
                var notMappedFields = string.Join(", ", clrMemberHandlers
                    .Select((member, memberIndex) => member == null ? $"'{pgFields[memberIndex].Name}'" : null)
                    .Where(member => member != null));
                throw new InvalidOperationException($"PostgreSQL composite type {pgType.DisplayName} contains fields {notMappedFields} which could not match any on CLR type {clrType.Name}");
            }

            return clrMemberHandlers;

            void CreateMemberHandler(MemberInfo clrMember, Type clrMemberType)
            {
                var attr = clrMember.GetCustomAttribute<PgNameAttribute>();
                var name = attr?.PgName ?? nameTranslator.TranslateMemberName(clrMember.Name);

                for (var pgFieldIndex = pgFields.Count - 1; pgFieldIndex >= 0; --pgFieldIndex)
                {
                    var pgField = pgFields[pgFieldIndex];
                    if (pgField.Name != name)
                        continue;

                    if (clrMemberHandlers[pgFieldIndex] != null)
                        throw new AmbiguousMatchException($"Multiple class members are mapped to the '{pgField.Name}' field.");

                    if (!typeMapper.TryGetByOID(pgField.Type.OID, out var handler))
                        throw new NpgsqlException($"PostgreSQL composite type {pgType.DisplayName} has field {pgField.Type.DisplayName} with an unknown type (OID = {pgField.Type.OID}).");

                    clrMemberHandlerCount++;
                    clrMemberHandlers[pgFieldIndex] = (CompositeMemberHandler<T>)Activator.CreateInstance(
                        clrMemberHandlerType.MakeGenericType(clrType, clrMemberType),
                        BindingFlags.Instance | BindingFlags.Public,
                        binder: null,
                        args: new object[] { clrMember, pgField.Type, handler },
                        culture: null)!;

                    break;
                }
            }
        }
    }
}
