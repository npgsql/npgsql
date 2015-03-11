using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Framework.Logging;
using Npgsql;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlEntityFrameworkConnection : RelationalConnection
    {
    	private readonly ILoggerFactory _loggerFactory;
    	
        public NpgsqlEntityFrameworkConnection([NotNull] IDbContextOptions options, [NotNull] ILoggerFactory loggerFactory)
            : base(options, loggerFactory)
        {
        	_loggerFactory = loggerFactory;
        }

        protected override DbConnection CreateDbConnection()
        {
            // TODO: Consider using DbProviderFactory to create connection instance
            // Issue #774
            return new NpgsqlConnection(ConnectionString);
        }

        public virtual NpgsqlEntityFrameworkConnection CreateMasterConnection()
        {
            var builder = new NpgsqlConnectionStringBuilder { ConnectionString = ConnectionString };
            //builder.InitialCatalog = "master";

            // TODO use clone connection method once implimented see #1406
            var options = new DbContextOptions();
            options.UseNpgsql(builder.ConnectionString).CommandTimeout(CommandTimeout);

            return new NpgsqlEntityFrameworkConnection(options, _loggerFactory);
        }
    }
}
