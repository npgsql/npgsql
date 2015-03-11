using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Framework.Logging;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlDatabase : RelationalDatabase
    {
        public NpgsqlDatabase(
            [NotNull] DbContext context,
            [NotNull] NpgsqlDataStoreCreator dataStoreCreator,
            [NotNull] NpgsqlEntityFrameworkConnection connection,
            [NotNull] Migrator migrator,
            [NotNull] ILoggerFactory loggerFactory)
            : base(context, dataStoreCreator, connection, migrator, loggerFactory)
        {
        }

        public new virtual NpgsqlEntityFrameworkConnection Connection
        {
            get { return (NpgsqlEntityFrameworkConnection)base.Connection; }
        }
    }
}
