#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Text;
#if ENTITIES6
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Infrastructure.DependencyResolution;
#else
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
#endif
using Npgsql.SqlGenerators;
using DbConnection = System.Data.Common.DbConnection;
using DbCommand = System.Data.Common.DbCommand;

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
            return CreateCommandDefinition(CreateDbCommand(((NpgsqlProviderManifest)providerManifest).Version, commandTree));
        }

        internal DbCommand CreateDbCommand(Version serverVersion, DbCommandTree commandTree)
        {
            if (commandTree == null)
                throw new ArgumentNullException("commandTree");

            NpgsqlCommand command = new NpgsqlCommand();

            foreach (KeyValuePair<string, TypeUsage> parameter in commandTree.Parameters)
            {
                NpgsqlParameter dbParameter = new NpgsqlParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.NpgsqlDbType = NpgsqlProviderManifest.GetNpgsqlDbType(((PrimitiveType)parameter.Value.EdmType).PrimitiveTypeKind);
                command.Parameters.Add(dbParameter);
            }

            TranslateCommandTree(serverVersion, commandTree, command);

            return command;
        }

        internal void TranslateCommandTree(Version serverVersion, DbCommandTree commandTree, DbCommand command, bool createParametersForNonSelect = true)
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
            sqlGenerator._createParametersForConstants = select != null ? false : createParametersForNonSelect;
            sqlGenerator._command = (NpgsqlCommand)command;
            sqlGenerator.Version = serverVersion;

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
                var sb = new StringBuilder();
                sb.Append("CREATE DATABASE \"");
                sb.Append(connection.Database);
                sb.Append("\"");
                if (conn.EntityTemplateDatabase != null)
                {
                    sb.Append(" TEMPLATE \"");
                    sb.Append(conn.EntityTemplateDatabase);
                    sb.Append("\"");
                }

                using (NpgsqlCommand command = new NpgsqlCommand(sb.ToString(), conn))
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
                Database = connection.EntityAdminDatabase ?? "template1",
                Pooling = false
            };

            using (var masterConnection = new NpgsqlConnection(connectionBuilder.ConnectionString))
            {
                masterConnection.Open();//using's Dispose will close it even if exception...
                action(masterConnection);
            }
        }
    }
}
