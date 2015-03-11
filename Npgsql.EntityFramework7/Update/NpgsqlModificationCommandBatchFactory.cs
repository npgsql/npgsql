using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlModificationCommandBatchFactory : ModificationCommandBatchFactory
    {
        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected NpgsqlModificationCommandBatchFactory()
        {
        }

        public NpgsqlModificationCommandBatchFactory([NotNull] NpgsqlSqlGenerator sqlGenerator)
            : base(sqlGenerator)
        {
        }

        public override ModificationCommandBatch Create([NotNull] IDbContextOptions options)
        {
            Check.NotNull(options, "options");

            var optionsExtension = options.Extensions.OfType<NpgsqlOptionsExtension>().FirstOrDefault();

            int? maxBatchSize = optionsExtension?.MaxBatchSize;

            return new NpgsqlModificationCommandBatch((NpgsqlSqlGenerator)SqlGenerator, maxBatchSize);
        }
    }
}