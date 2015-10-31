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

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Storage.Internal
{
    public class NpgsqlDatabaseCreator : RelationalDatabaseCreator
    {
        private readonly NpgsqlRelationalConnection _connection;
        private readonly IMigrationsSqlGenerator _migrationsSqlGenerator;
        private readonly ISqlCommandBuilder _sqlCommandBuilder;

        public NpgsqlDatabaseCreator(
            [NotNull] NpgsqlRelationalConnection connection,
            [NotNull] IMigrationsModelDiffer modelDiffer,
            [NotNull] IMigrationsSqlGenerator migrationsSqlGenerator,
            [NotNull] ISqlCommandBuilder sqlCommandBuilder,
            [NotNull] IModel model)
            : base(model, connection, modelDiffer, migrationsSqlGenerator)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(modelDiffer, nameof(modelDiffer));
            Check.NotNull(migrationsSqlGenerator, nameof(migrationsSqlGenerator));
            Check.NotNull(sqlCommandBuilder, nameof(sqlCommandBuilder));

            _connection = connection;
            _migrationsSqlGenerator = migrationsSqlGenerator;
            _sqlCommandBuilder = sqlCommandBuilder;
        }

        public override void Create()
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                CreateCreateOperations().ExecuteNonQuery(masterConnection);

                ClearPool();
            }
        }

        public override async Task CreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await CreateCreateOperations().ExecuteNonQueryAsync(masterConnection, cancellationToken);

                ClearPool();
            }
        }

        protected override bool HasTables()
            => (bool)CreateHasTablesCommand().ExecuteScalar(_connection);

        protected override async Task<bool> HasTablesAsync(CancellationToken cancellationToken = default(CancellationToken))
            => (bool)(await CreateHasTablesCommand().ExecuteScalarAsync(_connection, cancellationToken));

        private IRelationalCommand CreateHasTablesCommand()
            => _sqlCommandBuilder
                .Build(@"
                    SELECT CASE WHEN COUNT(*) = 0 THEN FALSE ELSE TRUE END
                    FROM information_schema.tables
                    WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')
                ");

        private IEnumerable<IRelationalCommand> CreateCreateOperations()
            => _migrationsSqlGenerator.Generate(new[] { new NpgsqlCreateDatabaseOperation { Name = _connection.DbConnection.Database } });

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
                CreateDropCommands().ExecuteNonQuery(masterConnection);
            }
        }

        public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ClearAllPools();

            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await CreateDropCommands().ExecuteNonQueryAsync(masterConnection, cancellationToken);
            }
        }

        private IEnumerable<IRelationalCommand> CreateDropCommands()
        {
            var operations = new MigrationOperation[]
            {
                // TODO Check DbConnection.Database always gives us what we want
                // Issue #775
                new NpgsqlDropDatabaseOperation { Name = _connection.DbConnection.Database }
            };

            var masterCommands = _migrationsSqlGenerator.Generate(operations);
            return masterCommands;
        }

        // Clear connection pools in case there are active connections that are pooled
        private static void ClearAllPools() => NpgsqlConnection.ClearAllPools();

        // Clear connection pool for the database connection since after the 'create database' call, a previously
        // invalid connection may now be valid.
        private void ClearPool() => NpgsqlConnection.ClearPool((NpgsqlConnection)_connection.DbConnection);
    }
}
