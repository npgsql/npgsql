using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    abstract class CompositeMemberHandler<TComposite>
    {
        public MemberInfo MemberInfo { get; }
        public PostgresType PostgresType { get; }

        protected CompositeMemberHandler(MemberInfo memberInfo, PostgresType postgresType)
        {
            MemberInfo = memberInfo;
            PostgresType = postgresType;
        }

        public abstract ValueTask Read(TComposite composite, NpgsqlReadBuffer buffer, bool async);

        public abstract ValueTask Read(ByReference<TComposite> composite, NpgsqlReadBuffer buffer, bool async);

        public abstract Task Write(TComposite composite, NpgsqlWriteBuffer buffer, NpgsqlLengthCache? lengthCache, bool async, CancellationToken cancellationToken = default);

        public abstract int ValidateAndGetLength(TComposite composite, ref NpgsqlLengthCache? lengthCache);
    }
}
