// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using Microsoft.Data.Entity.Migrations.Operations;
using Microsoft.Data.Entity.Migrations.Sql;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Npgsql;
using EntityFramework7.Npgsql.Migrations;

namespace EntityFramework7.Npgsql
{
    public class NpgsqlDatabaseCreator : RelationalDatabaseCreator
    {
        private readonly NpgsqlDatabaseConnection _connection;
        private readonly IModelDiffer _modelDiffer;
        private readonly IMigrationSqlGenerator _sqlGenerator;
        private readonly ISqlStatementExecutor _statementExecutor;

        public NpgsqlDatabaseCreator(
            [NotNull] NpgsqlDatabaseConnection connection,
            [NotNull] IModelDiffer modelDiffer,
            [NotNull] IMigrationSqlGenerator sqlGenerator,
            [NotNull] ISqlStatementExecutor statementExecutor,
            [NotNull] IModel model)
            : base(model)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(modelDiffer, nameof(modelDiffer));
            Check.NotNull(sqlGenerator, nameof(sqlGenerator));
            Check.NotNull(statementExecutor, nameof(statementExecutor));

            _connection = connection;
            _modelDiffer = modelDiffer;
            _sqlGenerator = sqlGenerator;
            _statementExecutor = statementExecutor;
        }

        public override void Create()
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                _statementExecutor.ExecuteNonQuery(masterConnection, null, CreateCreateOperations());
                ClearPool();
            }
        }

        public override async Task CreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await _statementExecutor
                    .ExecuteNonQueryAsync(masterConnection, null, CreateCreateOperations(), cancellationToken);
                ClearPool();
            }
        }

        public override void CreateTables()
            => _statementExecutor.ExecuteNonQuery(
                _connection,
                _connection.DbTransaction,
                CreateSchemaCommands());

        public override async Task CreateTablesAsync(CancellationToken cancellationToken = default(CancellationToken))
            => await _statementExecutor
                .ExecuteNonQueryAsync(
                    _connection,
                    _connection.DbTransaction,
                    CreateSchemaCommands(),
                    cancellationToken);

        public override bool HasTables()
            => (int)_statementExecutor.ExecuteScalar(_connection, _connection.DbTransaction, CreateHasTablesCommand()) != 0;

        public override async Task<bool> HasTablesAsync(CancellationToken cancellationToken = default(CancellationToken))
            => (int)(await _statementExecutor
                .ExecuteScalarAsync(_connection, _connection.DbTransaction, CreateHasTablesCommand(), cancellationToken)) != 0;

        private IEnumerable<SqlBatch> CreateSchemaCommands()
            => _sqlGenerator.Generate(_modelDiffer.GetDifferences(null, Model), Model);

        private string CreateHasTablesCommand()
            => @"
                 SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END
                 FROM information_schema.tables
                 WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')
               ";

        private IEnumerable<SqlBatch> CreateCreateOperations()
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
                _statementExecutor.ExecuteNonQuery(masterConnection, null, CreateDropCommands());
            }
        }

        public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await _statementExecutor
                    .ExecuteNonQueryAsync(masterConnection, null, CreateDropCommands(), cancellationToken);
            }
        }

        private IEnumerable<SqlBatch> CreateDropCommands()
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
