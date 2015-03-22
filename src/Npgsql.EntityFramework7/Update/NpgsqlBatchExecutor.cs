using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Framework.Logging;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlBatchExecutor : BatchExecutor
    {
        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected NpgsqlBatchExecutor()
        {
        }

        public NpgsqlBatchExecutor(
            [NotNull] NpgsqlTypeMapper typeMapper,
            [NotNull] DbContext context,
            [NotNull] ILoggerFactory loggerFactory)
            : base(typeMapper, context, loggerFactory)
        {
        }
    }
}
