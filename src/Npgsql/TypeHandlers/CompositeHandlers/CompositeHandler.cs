using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    class CompositeHandler<T> : NpgsqlTypeHandler<T>, ICompositeHandler
        where T : new()
    {
        static readonly Func<T> Constructor = Expression
            .Lambda<Func<T>>(Expression.New(typeof(T)))
            .Compile();

        readonly ConnectorTypeMapper _typeMapper;
        readonly INpgsqlNameTranslator _nameTranslator;
        CompositeMemberHandler<T>[]? _memberHandlers;

        public Type CompositeType => typeof(T);

        public CompositeHandler(PostgresCompositeType postgresType, ConnectorTypeMapper typeMapper, INpgsqlNameTranslator nameTranslator)
            : base(postgresType)
        {
            _typeMapper = typeMapper;
            _nameTranslator = nameTranslator;
        }

        public override async ValueTask<T> Read(NpgsqlReadBuffer buffer, int length, bool async, FieldDescription? fieldDescription = null)
        {
            if (_memberHandlers == null)
                _memberHandlers = CreateMemberHandlers();

            await buffer.Ensure(sizeof(int), async);

            var fieldCount = buffer.ReadInt32();
            if (fieldCount != _memberHandlers.Length)
                throw new InvalidOperationException($"pg_attributes contains {_memberHandlers.Length} fields for type {PgDisplayName}, but {fieldCount} fields were received.");

            if (IsValueType<T>.Value)
            {
                var composite = new ByReference<T> { Value = Constructor() };
                foreach (var member in _memberHandlers)
                    await member.Read(composite, buffer, async);

                return composite.Value;
            }
            else
            {
                var composite = Constructor();
                foreach (var member in _memberHandlers)
                    await member.Read(composite, buffer, async);

                return composite;
            }
        }

        public override async Task Write(T value, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (_memberHandlers == null)
                _memberHandlers = CreateMemberHandlers();

            if (buffer.WriteSpaceLeft < sizeof(int))
                await buffer.Flush(async);

            buffer.WriteInt32(_memberHandlers.Length);

            foreach (var member in _memberHandlers)
                await member.Write(value, buffer, lengthCache, async);
        }

        public override int ValidateAndGetLength(T value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (_memberHandlers == null)
                _memberHandlers = CreateMemberHandlers();

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

        CompositeMemberHandler<T>[] CreateMemberHandlers() => CreateMemberHandlers((PostgresCompositeType)PostgresType, _typeMapper, _nameTranslator);

        static CompositeMemberHandler<T>[] CreateMemberHandlers(PostgresCompositeType pgType, ConnectorTypeMapper typeMapper, INpgsqlNameTranslator nameTranslator)
        {
            var clrType = typeof(T);
            var pgFields = pgType.Fields;
            var clrHandlers = new CompositeMemberHandler<T>[pgFields.Count];
            var clrHandlerCount = 0;

            var clrHandlerType = IsValueType<T>.Value
                ? typeof(CompositeStructMemberHandler<,>)
                : typeof(CompositeClassMemberHandler<,>);

            foreach (var clrMember in clrType.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                Type clrMemberType;
                switch (clrMember)
                {
                    case FieldInfo clrField:
                        clrMemberType = clrField.FieldType;
                        break;
                    case PropertyInfo clrProperty:
                        clrMemberType = clrProperty.PropertyType;
                        break;
                    default:
                        continue;
                }

                var attr = clrMember.GetCustomAttribute<PgNameAttribute>();
                var name = attr?.PgName ?? nameTranslator.TranslateMemberName(clrMember.Name);

                for (var pgFieldIndex = pgFields.Count - 1; pgFieldIndex >= 0; --pgFieldIndex)
                {
                    var pgField = pgFields[pgFieldIndex];
                    if (pgField.Name != name)
                        continue;

                    if (clrHandlers[pgFieldIndex] != null)
                        throw new AmbiguousMatchException($"Multiple class members are mapped to the '{pgField.Name}' field.");

                    if (!typeMapper.TryGetByOID(pgField.Type.OID, out var handler))
                        throw new Exception($"PostgreSQL composite type {pgType.DisplayName} has field {pgField.Type.DisplayName} with an unknown type (OID = {pgField.Type.OID}).");

                    clrHandlerCount++;
                    clrHandlers[pgFieldIndex] = (CompositeMemberHandler<T>)Activator.CreateInstance(
                        clrHandlerType.MakeGenericType(clrType, clrMemberType),
                        BindingFlags.Instance | BindingFlags.Public,
                        binder: null,
                        args: new object[] { clrMember, pgField.Type, handler },
                        culture: null)!;

                    break;
                }
            }

            if (clrHandlerCount == pgFields.Count)
                return clrHandlers;

            var notMappedFields = string.Join(", ", clrHandlers
                .Select((member, memberIndex) => member == null ? $"'{pgFields[memberIndex].Name}'" : null)
                .Where(member => member != null));
            throw new InvalidOperationException($"PostgreSQL composite type {pgType.DisplayName} contains fields {notMappedFields} which could not match any on CLR type {clrType.Name}");
        }
    }
}
