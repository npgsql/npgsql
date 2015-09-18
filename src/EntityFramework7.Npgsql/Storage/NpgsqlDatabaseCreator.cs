// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Operations;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.Migrations;
using Npgsql;

namespace Microsoft.Data.Entity.Storage
{
    public class NpgsqlDatabaseCreator : RelationalDatabaseCreator
    {
        private readonly NpgsqlDatabaseConnection _connection;
        private readonly IMigrationsSqlGenerator _sqlGenerator;

        public NpgsqlDatabaseCreator(
            [NotNull] NpgsqlDatabaseConnection connection,
            [NotNull] IMigrationsModelDiffer modelDiffer,
            [NotNull] IMigrationsSqlGenerator sqlGenerator,
            [NotNull] ISqlStatementExecutor statementExecutor,
            [NotNull] IModel model)
            : base(model, connection, modelDiffer, sqlGenerator, statementExecutor)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(modelDiffer, nameof(modelDiffer));
            Check.NotNull(sqlGenerator, nameof(sqlGenerator));
            Check.NotNull(statementExecutor, nameof(statementExecutor));

            _connection = connection;
            _sqlGenerator = sqlGenerator;
        }

        public override void Create()
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                SqlStatementExecutor.ExecuteNonQuery(masterConnection, CreateCreateOperations());
                ClearPool();
            }
        }

        public override async Task CreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await SqlStatementExecutor
                    .ExecuteNonQueryAsync(masterConnection, CreateCreateOperations(), cancellationToken);
                ClearPool();
            }
        }

        protected override bool HasTables()
            => (int)SqlStatementExecutor.ExecuteScalar(_connection, CreateHasTablesCommand()) != 0;

        protected override async Task<bool> HasTablesAsync(CancellationToken cancellationToken = default(CancellationToken))
            => (int)(await SqlStatementExecutor
                .ExecuteScalarAsync(_connection, CreateHasTablesCommand(), cancellationToken)) != 0;

        private string CreateHasTablesCommand()
            => @"
                 SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END
                 FROM information_schema.tables
                 WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')
               ";

        private IEnumerable<RelationalCommand> CreateCreateOperations()
            => _sqlGenerator.Generate(new[] { new CreateDatabaseOperation { Name = _connection.DbConnection.Database } });

        public override bool Exists()
        {
            try
            {
                _connection.Open();
                _connection.Close();
                return true;
            }
            catch (NpgsqlException e)
            {
                if (IsDoesNotExist(e))
                {
                    return false;
                }

                throw;
            }
        }

        public override async Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _connection.OpenAsync(cancellationToken);
                _connection.Close();
                return true;
            }
            catch (NpgsqlException e)
            {
                if (IsDoesNotExist(e))
                {
                    return false;
                }

                throw;
            }
        }

        // Login failed is thrown when database does not exist (See Issue #776)
        private static bool IsDoesNotExist(NpgsqlException exception) => exception.Code == "3D000";

        public override void Delete()
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection())
            {
                SqlStatementExecutor.ExecuteNonQuery(masterConnection, CreateDropCommands());
            }
        }

        public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await SqlStatementExecutor
                    .ExecuteNonQueryAsync(masterConnection, CreateDropCommands(), cancellationToken);
            }
        }

        private IEnumerable<RelationalCommand> CreateDropCommands()
        {
            var operations = new MigrationOperation[]
            {
                // TODO Check DbConnection.Database always gives us what we want
                // Issue #775
                new DropDatabaseOperation { Name = _connection.DbConnection.Database }
            };

            var masterCommands = _sqlGenerator.Generate(operations);
            return masterCommands;
        }

        // Clear connection pools in case there are active connections that are pooled
        private static void ClearAllPools() => NpgsqlConnection.ClearAllPools();

        // Clear connection pool for the database connection since after the 'create database' call, a previously
        // invalid connection may now be valid.
        private void ClearPool() => NpgsqlConnection.ClearPool((NpgsqlConnection)_connection.DbConnection);
    }
}
