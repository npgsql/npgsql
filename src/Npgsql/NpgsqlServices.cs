#if ENTITIES
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using Npgsql.SqlGenerators;

namespace Npgsql
{
    internal class NpgsqlServices : DbProviderServices
    {
        private static readonly NpgsqlServices _instance = new NpgsqlServices();

        public static NpgsqlServices Instance
        {
            get { return _instance; }
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(DbConnection connection, DbCommandTree commandTree)
        {
            return CreateCommandDefinition(CreateDbCommand(connection, commandTree));
        }

        private DbCommand CreateDbCommand(DbConnection connection, DbCommandTree commandTree)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (commandTree == null)
                throw new ArgumentNullException("commandTree");

            DbCommand command = connection.CreateCommand();

            foreach (KeyValuePair<string, TypeUsage> parameter in commandTree.Parameters)
            {
                DbParameter dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
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
            }
            else if ((delete = commandTree as DbDeleteCommandTree) != null)
            {
            }
            else
            {
                // TODO: get a message (unsupported DbCommandTree type)
                throw new ArgumentException();
            }

            sqlGenerator.BuildCommand(command);
        }

        protected override DbProviderManifest GetDbProviderManifest(DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            return new NpgsqlProviderManifest();
        }

        protected override DbProviderManifest GetDbProviderManifest(string versionHint)
        {
            if (versionHint == null)
                throw new ArgumentNullException("versionHint");
            return new NpgsqlProviderManifest();
        }
    }
}

#endif