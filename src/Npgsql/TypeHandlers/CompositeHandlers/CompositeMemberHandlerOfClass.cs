using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    sealed class CompositeClassMemberHandler<TComposite, TMember> : CompositeMemberHandler<TComposite>
        where TComposite : class
    {
        delegate TMember GetMember(TComposite composite);
        delegate void SetMember(TComposite composite, TMember value);

        readonly GetMember? _get;
        readonly SetMember? _set;
        readonly NpgsqlTypeHandler _handler;

        public CompositeClassMemberHandler(FieldInfo fieldInfo, PostgresType postgresType, NpgsqlTypeHandler handler)
            : base(fieldInfo, postgresType)
        {
            var composite = Expression.Parameter(typeof(TComposite), "composite");
            var value = Expression.Parameter(typeof(TMember), "value");

            _get = Expression
                .Lambda<GetMember>(Expression.Field(composite, fieldInfo), composite)
                .Compile();
            _set = Expression
                .Lambda<SetMember>(Expression.Assign(Expression.Field(composite, fieldInfo), value), composite, value)
                .Compile();
            _handler = handler;
        }

        public CompositeClassMemberHandler(PropertyInfo propertyInfo, PostgresType postgresType, NpgsqlTypeHandler handler)
            : base(propertyInfo, postgresType)
        {
            var getMethod = propertyInfo.GetGetMethod();
            if (getMethod != null)
                _get = (GetMember)Delegate.CreateDelegate(typeof(GetMember), getMethod);

            var setMethod = propertyInfo.GetSetMethod();
            if (setMethod != null)
                _set = (SetMember)Delegate.CreateDelegate(typeof(SetMember), setMethod);

            Debug.Assert(setMethod != null || getMethod != null);

            _handler = handler;
        }

        public override async ValueTask Read(TComposite composite, NpgsqlReadBuffer buffer, bool async)
        {
            if (_set == null)
                ThrowHelper.ThrowInvalidOperationException_NoPropertySetter(typeof(TComposite), MemberInfo);

            await buffer.Ensure(sizeof(uint) + sizeof(int), async);

            var oid = buffer.ReadUInt32();
            Debug.Assert(oid == PostgresType.OID);

            var length = buffer.ReadInt32();
            if (length == -1)
                return;

            var value = NullableHandler<TMember>.Exists
                ? await NullableHandler<TMember>.ReadAsync(_handler, buffer, length, async)
                : await _handler.Read<TMember>(buffer, length, async);

            _set(composite, value);
        }

        public override ValueTask Read(ByReference<TComposite> composite, NpgsqlReadBuffer buffer, bool async)
            => throw new NotSupportedException();

        public override async Task Write(TComposite composite, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, bool async, CancellationToken cancellationToken = default)
        {
            if (_get == null)
                ThrowHelper.ThrowInvalidOperationException_NoPropertyGetter(typeof(TComposite), MemberInfo);

            if (buffer.WriteSpaceLeft < sizeof(int))
                await buffer.Flush(async, cancellationToken);

            buffer.WriteUInt32(PostgresType.OID);
            if (NullableHandler<TMember>.Exists)
                await NullableHandler<TMember>.WriteAsync(_handler, _get(composite), buffer, lengthCache, null, async, cancellationToken);
            else
                await _handler.WriteWithLengthInternal(_get(composite), buffer, lengthCache, null, async, cancellationToken);
        }

        public override int ValidateAndGetLength(TComposite composite, ref NpgsqlLengthCache? lengthCache)
        {
            if (_get == null)
                ThrowHelper.ThrowInvalidOperationException_NoPropertyGetter(typeof(TComposite), MemberInfo);

            var value = _get(composite);
            if (value == null)
                return 0;

            return NullableHandler<TMember>.Exists
                ? NullableHandler<TMember>.ValidateAndGetLength(_handler, value, ref lengthCache, null)
                : _handler.ValidateAndGetLength(value, ref lengthCache, null);
        }
    }
}
