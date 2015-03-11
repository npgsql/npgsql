using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using EntityFramework.Npgsql.Extensions;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations.History;
using Microsoft.Data.Entity.Relational.Migrations.Operations;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Migrations
{
    // TODO: Log
    public class NpgsqlHistoryRepository : IHistoryRepository
    {
        private readonly NpgsqlEntityFrameworkConnection _connection;
        private readonly NpgsqlDataStoreCreator _creator;
        private readonly Type _contextType;

        public NpgsqlHistoryRepository(
            [NotNull] NpgsqlEntityFrameworkConnection connection,
            [NotNull] NpgsqlDataStoreCreator creator,
            [NotNull] DbContext context)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(creator, nameof(creator));
            Check.NotNull(context, nameof(context));

            _connection = connection;
            _creator = creator;
            _contextType = context.GetType();
        }

        public virtual bool Exists()
        {
            var exists = false;

            if (!_creator.Exists())
            {
                return exists;
            }

            var command = (SqlCommand)_connection.DbConnection.CreateCommand();
            command.CommandText =
                @"SELECT 1 FROM [INFORMATION_SCHEMA].[TABLES]
WHERE [TABLE_SCHEMA] = N'dbo' AND [TABLE_NAME] = '__MigrationHistory' AND [TABLE_TYPE] = 'BASE TABLE'";

            _connection.Open();
            try
            {
                exists = command.ExecuteScalar() != null;
            }
            finally
            {
                _connection.Close();
            }

            return exists;
        }

        public virtual IReadOnlyList<IHistoryRow> GetAppliedMigrations()
        {
            var rows = new List<HistoryRow>();

            if (!Exists())
            {
                return rows;
            }

            _connection.Open();
            try
            {
                var command = (SqlCommand)_connection.DbConnection.CreateCommand();
                command.CommandText =
                    @"SELECT [MigrationId], [ProductVersion]
FROM [dbo].[__MigrationHistory]
WHERE [ContextKey] = @ContextKey ORDER BY [MigrationId]";
                command.Parameters.AddWithValue("@ContextKey", _contextType.FullName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new HistoryRow(reader.GetString(0), reader.GetString(1)));
                    }
                }
            }
            finally
            {
                _connection.Close();
            }

            return rows;
        }

        public virtual MigrationOperation GetCreateOperation()
        {
            return new SqlOperation(
                @"CREATE TABLE [dbo].[__MigrationHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ContextKey] nvarchar(300) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK_MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
)",
                suppressTransaction: false);
        }

        public virtual MigrationOperation GetDeleteOperation(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            // TODO: Escape. Can we parameterize?
            return new SqlOperation(
                @"DELETE FROM [dbo].[__MigrationHistory]
WHERE [MigrationId] = '" + migrationId + "' AND [ContextKey] = '" + _contextType.FullName + "'",
                suppressTransaction: false);
        }

        public virtual MigrationOperation GetInsertOperation(IHistoryRow row)
        {
            Check.NotNull(row, nameof(row));

            // TODO: Escape. Can we parameterize?
            return new SqlOperation(
                @"INSERT INTO [dbo].[__MigrationHistory] ([MigrationId], [ContextKey], [ProductVersion])
VALUES ('" + row.MigrationId + "', '" + _contextType.FullName + "', '" + row.ProductVersion + "')",
                suppressTransaction: false);
        }
        
        public virtual string Create(bool ifNotExists)
        {
            if(!ifNotExists || (ifNotExists && !Exists()))
            {
            	return ((SqlOperation)GetCreateOperation()).Sql;
            }
            
            return string.Empty;
        }
        
        public virtual string BeginIfNotExists(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            // TODO: Escape
            return new StringBuilder()
                .Append("IF NOT EXISTS(SELECT * FROM [dbo].[__MigrationHistory] WHERE [MigrationId] = '")
                    .Append(migrationId).Append("' AND [ContextKey] = '").Append(_contextType.FullName).AppendLine("')")
                .Append("BEGIN")
                .ToString();
        }
        
        public virtual string BeginIfExists(string migrationId)
        {
            Check.NotEmpty(migrationId, nameof(migrationId));

            // TODO: Escape
            return new StringBuilder()
                .Append("IF EXISTS(SELECT * FROM [dbo].[__MigrationHistory] WHERE [MigrationId] = '")
                    .Append(migrationId).Append("' AND [ContextKey] = '").Append(_contextType.FullName).AppendLine("')")
                .Append("BEGIN")
                .ToString();
        }

        public virtual string EndIf()
        {
            return "END";
        }

    }
}
