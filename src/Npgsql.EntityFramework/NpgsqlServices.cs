using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml;
#if ENTITIES6
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Infrastructure.DependencyResolution;
#else
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
#endif
using Npgsql.SqlGenerators;

namespace Npgsql
{
#if ENTITIES6
    public class NpgsqlServices : DbProviderServices
#else
    internal class NpgsqlServices : DbProviderServices
#endif
    {
        private static readonly NpgsqlServices _instance = new NpgsqlServices();

#if ENTITIES6
        public NpgsqlServices()
        {
            AddDependencyResolver(new SingletonDependencyResolver<Func<MigrationSqlGenerator>>(
                () => new NpgsqlMigrationSqlGenerator(), "Npgsql"));
        }
#endif

        public static NpgsqlServices Instance
        {
            get { return _instance; }
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            return CreateCommandDefinition(CreateDbCommand(commandTree));
        }

        internal DbCommand CreateDbCommand(DbCommandTree commandTree)
        {
            if (commandTree == null)
                throw new ArgumentNullException("commandTree");

            DbCommand command = NpgsqlFactory.Instance.CreateCommand();

            foreach (KeyValuePair<string, TypeUsage> parameter in commandTree.Parameters)
            {
                DbParameter dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.DbType = NpgsqlProviderManifest.GetDbType(((PrimitiveType)parameter.Value.EdmType).PrimitiveTypeKind);
                command.Parameters.Add(dbParameter);
            }

            TranslateCommandTree(commandTree, command);

            return command;
        }

        private void TranslateCommandTree(DbCommandTree commandTree, DbCommand command)
        {
            SqlBaseGenerator sqlGenerator = null;

            DbQueryCommandTree select;
            DbInsertCommandTree insert;
            DbUpdateCommandTree update;
            DbDeleteCommandTree delete;
            if ((select = commandTree as DbQueryCommandTree) != null)
            {
                sqlGenerator = new SqlSelectGenerator(select);
            }
            else if ((insert = commandTree as DbInsertCommandTree) != null)
            {
                sqlGenerator = new SqlInsertGenerator(insert);
            }
            else if ((update = commandTree as DbUpdateCommandTree) != null)
            {
                sqlGenerator = new SqlUpdateGenerator(update);
            }
            else if ((delete = commandTree as DbDeleteCommandTree) != null)
            {
                sqlGenerator = new SqlDeleteGenerator(delete);
            }
            else
            {
                // TODO: get a message (unsupported DbCommandTree type)
                throw new ArgumentException();
            }

            sqlGenerator.BuildCommand(command);
        }

        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            string serverVersion = "";
            UsingPostgresDBConnection((NpgsqlConnection)connection, conn =>
            {
                serverVersion = conn.ServerVersion;
            });
            return serverVersion;
        }

        protected override DbProviderManifest GetDbProviderManifest(string versionHint)
        {
            if (versionHint == null)
                throw new ArgumentNullException("versionHint");
            return new NpgsqlProviderManifest(versionHint);
        }

#if ENTITIES6
        protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            bool exists = false;
            UsingPostgresDBConnection((NpgsqlConnection)connection, conn =>
            {
                using (NpgsqlCommand command = new NpgsqlCommand("select count(*) from pg_catalog.pg_database where datname = '" + connection.Database + "';", conn))
                {
                    exists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            });
            return exists;
        }

        protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            UsingPostgresDBConnection((NpgsqlConnection)connection, conn =>
            {
                using (NpgsqlCommand command = new NpgsqlCommand("CREATE DATABASE \"" + connection.Database + "\";", conn))
                {
                    command.ExecuteNonQuery();
                }
            });
        }

        protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            UsingPostgresDBConnection((NpgsqlConnection)connection, conn =>
            {
                //Close all connections in pool or exception "database used by another user appears"
                NpgsqlConnection.ClearAllPools();
                using (NpgsqlCommand command = new NpgsqlCommand("DROP DATABASE \"" + connection.Database + "\";", conn))
                {
                    command.ExecuteNonQuery();
                }
            });
        }
#endif

        private static void UsingPostgresDBConnection(NpgsqlConnection connection, Action<NpgsqlConnection> action)
        {
            var connectionBuilder = new NpgsqlConnectionStringBuilder(connection.ConnectionString)
            {
                Database = "template1"
            };

            using (var masterConnection = new NpgsqlConnection(connectionBuilder.ConnectionString))
            {
                masterConnection.Open();//using's Dispose will close it even if exception...
                action(masterConnection);
            }
        }
    }
}
