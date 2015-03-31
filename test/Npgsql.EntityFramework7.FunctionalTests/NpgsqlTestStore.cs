using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Relational.FunctionalTests;
using Npgsql;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class NpgsqlTestStore : RelationalTestStore
    {
        public const int CommandTimeout = 30;

        private static int _scratchCount;

        public static Task<NpgsqlTestStore> GetOrCreateSharedAsync(string name, Func<Task> initializeDatabase)
        {
            return new NpgsqlTestStore(name).CreateSharedAsync(initializeDatabase);
        }

        public static NpgsqlTestStore GetOrCreateShared(string name, Action initializeDatabase)
        {
            return new NpgsqlTestStore(name).CreateShared(initializeDatabase);
        }

        /// <summary>
        ///     A non-transactional, transient, isolated test database. Use this in the case
        ///     where transactions are not appropriate.
        /// </summary>
        public static Task<NpgsqlTestStore> CreateScratchAsync(bool createDatabase = true)
        {
            var name = "Npgsql.Scratch_" + Interlocked.Increment(ref _scratchCount);
            return new NpgsqlTestStore(name).CreateTransientAsync(createDatabase);
        }

        public static NpgsqlTestStore CreateScratch(bool createDatabase = true)
        {
            var name = "Npgsql.Scratch_" + Interlocked.Increment(ref _scratchCount);
            return new NpgsqlTestStore(name).CreateTransient(createDatabase);
        }

        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;
        private readonly string _name;
        private bool _deleteDatabase;

        // Use async static factory method
        private NpgsqlTestStore(string name)
        {
            _name = name;
        }

        private async Task<NpgsqlTestStore> CreateSharedAsync(Func<Task> initializeDatabase)
        {
            await CreateSharedAsync(typeof(NpgsqlTestStore).Name + _name, initializeDatabase);

            _connection = new NpgsqlConnection(CreateConnectionString(_name));

            await _connection.OpenAsync();

            _transaction = _connection.BeginTransaction();

            return this;
        }

        private NpgsqlTestStore CreateShared(Action initializeDatabase)
        {
            CreateShared(typeof(NpgsqlTestStore).Name + _name, initializeDatabase);

            _connection = new NpgsqlConnection(CreateConnectionString(_name));

            _connection.Open();

            _transaction = _connection.BeginTransaction();

            return this;
        }

        public static async Task CreateDatabaseIfNotExistsAsync(string name, string scriptPath = null)
        {
            using (var master = new NpgsqlConnection(CreateAdminConnectionString()))
            {
                await master.OpenAsync();

                using (var command = master.CreateCommand())
                {
                    command.CommandTimeout = CommandTimeout;
                    command.CommandText
                        = string.Format(@"SELECT COUNT(*) FROM pg_database WHERE name = '{0}'", name);

                    var exists = (long)await command.ExecuteScalarAsync() > 0;

                    if (exists) { return; }

                    command.CommandText = string.Format(@"CREATE DATABASE ""{0}""", name);
                    await command.ExecuteNonQueryAsync();

                    if (scriptPath != null)
                    {
                        // HACK: Probe for script file as current dir
                        // is different between k build and VS run.

                        if (!File.Exists(scriptPath))
                        {
                            var appBase = Environment.GetEnvironmentVariable("DNX_APPBASE");

                            if (appBase != null)
                            {
                                scriptPath = Path.Combine(appBase, Path.GetFileName(scriptPath));
                            }
                        }

                        var script = File.ReadAllText(scriptPath);

                        foreach (var batch
                            in new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                                .Split(script))
                        {
                            command.CommandText = batch;

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }

        public static void CreateDatabaseIfNotExists(string name, string scriptPath = null)
        {
            using (var master = new NpgsqlConnection(CreateAdminConnectionString()))
            {
                master.Open();

                using (var command = master.CreateCommand())
                {
                    command.CommandTimeout = CommandTimeout;
                    command.CommandText
                        = string.Format(@"SELECT COUNT(*) FROM pg_database WHERE datname = '{0}'", name);

                    var exists = (long)command.ExecuteScalar() > 0;
                    if (exists) { return; }

                    command.CommandText = string.Format(@"CREATE DATABASE ""{0}""", name);
                    command.ExecuteNonQuery();
                }
            }

            using (var conn = new NpgsqlConnection(CreateConnectionString(name)))
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    if (scriptPath != null)
                    {
                        // HACK: Probe for script file as current dir
                        // is different between k build and VS run.

                        if (!File.Exists(scriptPath))
                        {
                            var appBase = Environment.GetEnvironmentVariable("DNX_APPBASE");

                            if (appBase != null)
                            {
                                scriptPath = Path.Combine(appBase, Path.GetFileName(scriptPath));
                            }
                        }

                        var script = File.ReadAllText(scriptPath);

                        foreach (var batch
                            in new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                                .Split(script))
                        {
                            command.CommandText = batch;

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private static async Task WaitForExistsAsync(NpgsqlConnection connection)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    await connection.OpenAsync();

                    connection.Close();

                    return;
                }
                catch (SqlException e)
                {
                    if (++retryCount >= 30
                        || (e.Number != 233 && e.Number != -2 && e.Number != 4060))
                    {
                        throw;
                    }

                    // TODO: SqlConnection.ClearPool(connection);

                    Thread.Sleep(100);
                }
            }
        }

        private static void WaitForExists(NpgsqlConnection connection)
        {
            var retryCount = 0;
            while (true)
            {
                try
                {
                    connection.Open();

                    connection.Close();

                    return;
                }
                catch (SqlException e)
                {
                    if (++retryCount >= 30
                        || (e.Number != 233 && e.Number != -2 && e.Number != 4060))
                    {
                        throw;
                    }

                    //TODO: SqlConnection.ClearPool(connection);

                    Thread.Sleep(100);
                }
            }
        }

        private async Task<NpgsqlTestStore> CreateTransientAsync(bool createDatabase)
        {
            await DeleteDatabaseAsync(_name);

            _connection = new NpgsqlConnection(CreateConnectionString(_name));

            if (createDatabase)
            {
                using (var master = new NpgsqlConnection(CreateAdminConnectionString()))
                {
                    await master.OpenAsync();
                    using (var command = master.CreateCommand())
                    {
                        command.CommandText = string.Format(@"{0}CREATE DATABASE ""{1}""", Environment.NewLine, _name);

                        await command.ExecuteNonQueryAsync();

                        await WaitForExistsAsync(_connection);
                    }
                }
                await _connection.OpenAsync();
            }

            _deleteDatabase = createDatabase;
            return this;
        }

        private NpgsqlTestStore CreateTransient(bool createDatabase)
        {
            DeleteDatabase(_name);

            _connection = new NpgsqlConnection(CreateConnectionString(_name));

            if (createDatabase)
            {
                using (var master = new NpgsqlConnection(CreateAdminConnectionString()))
                {
                    master.Open();
                    using (var command = master.CreateCommand())
                    {
                        command.CommandText = string.Format(@"{0}CREATE DATABASE ""{1}""", Environment.NewLine, _name);

                        command.ExecuteNonQuery();

                        WaitForExists(_connection);
                    }
                }
                _connection.Open();
            }

            _deleteDatabase = createDatabase;
            return this;
        }

        private async Task DeleteDatabaseAsync(string name)
        {
            using (var master = new NpgsqlConnection(CreateAdminConnectionString()))
            {
                await master.OpenAsync().WithCurrentCulture();

                using (var command = master.CreateCommand())
                {
                    command.CommandTimeout = CommandTimeout; // Query will take a few seconds if (and only if) there are active connections

                    // Kill all connection to the database
                    // TODO: Pre-9.2 PG has column name procid instead of pid
                    command.CommandText = String.Format(@"
                      SELECT pg_terminate_backend (pg_stat_activity.pid)
                      FROM pg_stat_activity
                      WHERE pg_stat_activity.datname = '{0}'", name);
                    await command.ExecuteNonQueryAsync().WithCurrentCulture();

                    command.CommandText = string.Format(@"DROP DATABASE IF EXISTS ""{0}""", name);
                    await command.ExecuteNonQueryAsync().WithCurrentCulture();
                }
            }
        }

        private void DeleteDatabase(string name)
        {
            using (var master = new NpgsqlConnection(CreateAdminConnectionString()))
            {
                master.Open();

                using (var command = master.CreateCommand())
                {
                    command.CommandTimeout = CommandTimeout; // Query will take a few seconds if (and only if) there are active connections

                    // Kill all connection to the database
                    // TODO: Pre-9.2 PG has column name procid instead of pid
                    command.CommandText = String.Format(@"
                      SELECT pg_terminate_backend (pg_stat_activity.pid)
                      FROM pg_stat_activity
                      WHERE pg_stat_activity.datname = '{0}'", name);
                    command.ExecuteNonQuery();

                    command.CommandText = string.Format(@"DROP DATABASE IF EXISTS ""{0}""", name);

                    command.ExecuteNonQuery();
                }
            }
        }

        public override DbConnection Connection
        {
            get { return _connection; }
        }

        public override DbTransaction Transaction
        {
            get { return _transaction; }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, CancellationToken cancellationToken, params object[] parameters)
        {
            using (var command = CreateCommand(sql, parameters))
            {
                return (T)await command.ExecuteScalarAsync(cancellationToken);
            }
        }

        public Task<int> ExecuteNonQueryAsync(string sql, params object[] parameters)
        {
            using (var command = CreateCommand(sql, parameters))
            {
                return command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] parameters)
        {
            using (var command = CreateCommand(sql, parameters))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var results = Enumerable.Empty<T>();

                    while (await dataReader.ReadAsync())
                    {
                        results = results.Concat(new[] { await dataReader.GetFieldValueAsync<T>(0) });
                    }

                    return results;
                }
            }
        }

        private DbCommand CreateCommand(string commandText, object[] parameters)
        {
            var command = _connection.CreateCommand();

            if (_transaction != null)
            {
                command.Transaction = _transaction;
            }

            command.CommandText = commandText;
            command.CommandTimeout = CommandTimeout;

            for (var i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue("p" + i, parameters[i]);
            }

            return command;
        }

        public override void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }

            _connection.Dispose();

            if (_deleteDatabase)
            {
                DeleteDatabaseAsync(_name).Wait();
            }
        }

        public static string CreateConnectionString(string name)
        {
            return new NpgsqlConnectionStringBuilder
            {
                Host="localhost",
                UserName="npgsql_tests",
                Password="npgsql_tests",
                Database=name,
            }.ConnectionString;
        }

        static string CreateAdminConnectionString()
        {
            return CreateConnectionString("postgres");
        }
    }
}