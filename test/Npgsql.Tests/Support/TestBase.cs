using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public abstract class TestBase
    {
        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures, e.g. to set special connection parameters
        /// </summary>
        public virtual string ConnectionString => TestUtil.ConnectionString;

        static SemaphoreSlim DatabaseCreationLock = new(1);

        #region Type testing

        public async Task<T> AssertType<T>(
            T value,
            string sqlLiteral,
            string pgTypeName,
            NpgsqlDbType npgsqlDbType,
            DbType? dbType = null,
            DbType? inferredDbType = null,
            bool isDefaultForReading = true,
            bool isDefaultForWriting = true,
            bool? isDefault = null)
        {
            await using var connection = await OpenConnectionAsync();
            return await AssertType(
                connection, value, sqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefaultForReading, isDefaultForWriting,
                isDefault);
        }

        public async Task<T> AssertType<T>(
            NpgsqlConnection connection,
            T value,
            string sqlLiteral,
            string pgTypeName,
            NpgsqlDbType npgsqlDbType,
            DbType? dbType = null,
            DbType? inferredDbType = null,
            bool isDefaultForReading = true,
            bool isDefaultForWriting = true,
            bool? isDefault = null)
        {
            if (isDefault is not null)
                isDefaultForReading = isDefaultForWriting = isDefault.Value;

            await AssertTypeWrite(connection, value, sqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefaultForWriting);
            return await AssertTypeRead(connection, sqlLiteral, pgTypeName, value, isDefaultForReading);
        }


        public async Task<T> AssertTypeRead<T>(T expected, string sqlLiteral, string pgTypeName, bool isDefault = true)
        {
            await using var connection = await OpenConnectionAsync();
            return await AssertTypeRead(connection, sqlLiteral, pgTypeName, expected, isDefault);
        }

        public async Task AssertTypeWrite<T>(
            T value,
            string expectedSqlLiteral,
            string pgTypeName,
            NpgsqlDbType npgsqlDbType,
            DbType? dbType = null,
            DbType? inferredDbType = null,
            bool isDefault = true)
        {
            await using var connection = await OpenConnectionAsync();
            await AssertTypeWrite(connection, value, expectedSqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefault);
        }

        internal static async Task<T> AssertTypeRead<T>(
            NpgsqlConnection connection, string sqlLiteral, string pgTypeName, T expected, bool isDefault = true)
        {
            if (sqlLiteral.Contains('\''))
                sqlLiteral = sqlLiteral.Replace("'", "''");

            await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{pgTypeName}", connection);
            await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
            await reader.ReadAsync();

            var truncatedSqlLiteral = sqlLiteral.Length > 40 ? sqlLiteral[..40] + "..." : sqlLiteral;

            var dataTypeName = reader.GetDataTypeName(0);
            var dotIndex = dataTypeName.IndexOf('.');
            if (dotIndex > -1)
                dataTypeName = dataTypeName.Substring(dotIndex + 1);

            Assert.That(dataTypeName, Is.EqualTo(pgTypeName),
                $"Got wrong result from GetDataTypeName when reading '{truncatedSqlLiteral}'");

            if (isDefault)
            {
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(T)),
                    $"Got wrong result from GetFieldType when reading '{truncatedSqlLiteral}'");
            }

            var actual = isDefault ? (T)reader.GetValue(0) : reader.GetFieldValue<T>(0);

            Assert.That(actual, Is.EqualTo(expected),
                $"Got wrong result from GetFieldValue value when reading '{truncatedSqlLiteral}'");

            return actual;
        }

        internal static async Task AssertTypeWrite<T>(
            NpgsqlConnection connection,
            T value,
            string expectedSqlLiteral,
            string pgTypeName,
            NpgsqlDbType npgsqlDbType,
            DbType? dbType = null,
            DbType? inferredDbType = null,
            bool isDefault = true)
        {
            // TODO: Interferes with both multiplexing and connection-specific mapping (used e.g. in NodaTime)
            // Reset the type mapper to make sure we're resolving this type with a clean slate (for isolation, just in case)
            // connection.TypeMapper.Reset();

            // Strip any facet information (length/precision/scale)
            var parenIndex = pgTypeName.IndexOf('(');
            var pgTypeNameWithoutFacets = parenIndex > -1 ? pgTypeName[..parenIndex] : pgTypeName;

            // We test the following scenarios (between 2 and 5 in total):
            // 1. With NpgsqlDbType explicitly set
            // 2. With DataTypeName explicitly set
            // 3. With DbType explicitly set (if one was provided)
            // 4. With only the value set (if it's the default)
            // 5. With only the value set, using generic NpgsqlParameter<T> (if it's the default)

            var numParams = 2 + (dbType is not null ? 1 : 0) + (isDefault ? 2 : 0);
            var sql = "SELECT " + string.Join(", ", Enumerable.Range(1, numParams).Select(i =>
                "pg_typeof($1)::text, $1::text".Replace("$1", $"${i}")));

            var errorIdentifierIndex = 0;
            var errorIdentifier = new Dictionary<int, string>
            {
                { errorIdentifierIndex++, $"NpgsqlDbType={npgsqlDbType}" },
                { errorIdentifierIndex++, $"DataTypeName={pgTypeNameWithoutFacets}" }
            };

            await using var cmd = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new() { Value = value, NpgsqlDbType = npgsqlDbType },
                    new() { Value = value, DataTypeName = pgTypeNameWithoutFacets }
                }
            };

            if (dbType is not null)
            {
                cmd.Parameters.Add(new() { Value = value, DbType = dbType.Value });
                errorIdentifier[errorIdentifierIndex++] = $"DbType={dbType}";
            }

            if (isDefault)
            {
                cmd.Parameters.Add(new() { Value = value });
                cmd.Parameters.Add(new NpgsqlParameter<T> { TypedValue = value });
                errorIdentifier[errorIdentifierIndex++] = "Value only (default)";
                errorIdentifier[errorIdentifierIndex++] = "Value only (default)";
            }

            Debug.Assert(numParams == cmd.Parameters.Count && numParams == errorIdentifierIndex);

            // First check inference on the parameter
            for (var i = 0; i < numParams; i++)
            {
                var p = cmd.Parameters[i];

                Assert.That(p.NpgsqlDbType, Is.EqualTo(npgsqlDbType),
                    () => $"Got wrong inferred NpgsqlDbType when inferring with {errorIdentifier[i]}");
                Assert.That(p.DbType, Is.EqualTo(inferredDbType ?? dbType ?? DbType.Object),
                    () => $"Got wrong inferred DbType when inferring with {errorIdentifier[i]}");

                Assert.That(p.DataTypeName, Is.EqualTo(pgTypeNameWithoutFacets),
                    () => $"Got wrong inferred DataTypeName when inferring with {errorIdentifier[i]}");
            }

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            for (var i = 0; i < numParams * 2; i += 2)
            {
                Assert.That(reader[i], Is.EqualTo(pgTypeNameWithoutFacets), $"Got wrong PG type name when writing with {errorIdentifier[i / 2]}");
                Assert.That(reader[i+1], Is.EqualTo(expectedSqlLiteral), $"Got wrong SQL literal when writing with {errorIdentifier[i / 2]}");
            }
        }

        public async Task AssertTypeUnsupported<T>(T value, string sqlLiteral, string pgTypeName)
        {
            await AssertTypeUnsupportedRead<T>(sqlLiteral, pgTypeName);
            await AssertTypeUnsupportedWrite(value, pgTypeName);
        }

        public async Task AssertTypeUnsupportedRead(string sqlLiteral, string pgTypeName)
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{pgTypeName}", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<InvalidCastException>());
        }

        public Task<Exception> AssertTypeUnsupportedRead<T>(string sqlLiteral, string pgTypeName)
            => AssertTypeUnsupportedRead<T, InvalidCastException>(sqlLiteral, pgTypeName);

        public async Task<Exception> AssertTypeUnsupportedRead<T, TException>(string sqlLiteral, string pgTypeName)
            where TException : Exception
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{pgTypeName}", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            return Assert.Throws<TException>(() => reader.GetFieldValue<T>(0))!;
        }

        public Task<InvalidCastException> AssertTypeUnsupportedWrite<T>(T value, string? pgTypeName = null)
            => AssertTypeUnsupportedWrite<T, InvalidCastException>(value, pgTypeName);

        public async Task<TException> AssertTypeUnsupportedWrite<T, TException>(T value, string? pgTypeName = null)
            where TException : Exception
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1", conn)
            {
                Parameters = { new() { Value = value } }
            };

            if (pgTypeName is not null)
                cmd.Parameters[0].DataTypeName = pgTypeName;

            return Assert.ThrowsAsync<TException>(() => cmd.ExecuteReaderAsync())!;
        }

        #endregion Type testing

        #region Utilities for use by tests

        protected virtual NpgsqlConnection CreateConnection(string? connectionString = null)
            => new(connectionString ?? ConnectionString);

        protected virtual NpgsqlConnection CreateConnection(Action<NpgsqlConnectionStringBuilder> builderAction)
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            builderAction(builder);
            return new NpgsqlConnection(builder.ConnectionString);
        }

        protected virtual NpgsqlConnection OpenConnection(string? connectionString = null)
            => OpenConnection(connectionString, async: false).GetAwaiter().GetResult();

        protected virtual NpgsqlConnection OpenConnection(Action<NpgsqlConnectionStringBuilder> builderAction)
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            builderAction(builder);
            return OpenConnection(builder.ConnectionString, async: false).GetAwaiter().GetResult();
        }

        protected virtual ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
            => OpenConnection(connectionString, async: true);

        protected virtual ValueTask<NpgsqlConnection> OpenConnectionAsync(
            Action<NpgsqlConnectionStringBuilder> builderAction)
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            builderAction(builder);
            return OpenConnection(builder.ConnectionString, async: true);
        }

        ValueTask<NpgsqlConnection> OpenConnection(string? connectionString, bool async)
        {
            return OpenConnectionInternal(hasLock: false);

            async ValueTask<NpgsqlConnection> OpenConnectionInternal(bool hasLock)
            {
                var conn = CreateConnection(connectionString);
                try
                {
                    if (async)
                        await conn.OpenAsync();
                    else
                        conn.Open();
                    return conn;
                }
                catch (PostgresException e)
                {
                    if (e.SqlState == PostgresErrorCodes.InvalidPassword && connectionString == TestUtil.DefaultConnectionString)
                        throw new Exception("Please create a user npgsql_tests as follows: CREATE USER npgsql_tests PASSWORD 'npgsql_tests' SUPERUSER");

                    if (e.SqlState == PostgresErrorCodes.InvalidCatalogName)
                    {
                        if (!hasLock)
                        {
                            DatabaseCreationLock.Wait();
                            try
                            {
                                return await OpenConnectionInternal(hasLock: true);
                            }
                            finally
                            {
                                DatabaseCreationLock.Release();
                            }
                        }

                        // Database does not exist and we have the lock, proceed to creation
                        var builder = new NpgsqlConnectionStringBuilder(connectionString ?? ConnectionString)
                        {
                            Pooling = false,
                            Multiplexing = false,
                            Database = "postgres"
                        };

                        using var adminConn = new NpgsqlConnection(builder.ConnectionString);
                        adminConn.Open();
                        adminConn.ExecuteNonQuery("CREATE DATABASE " + conn.Database);
                        adminConn.Close();
                        Thread.Sleep(1000);

                        if (async)
                            await conn.OpenAsync();
                        else
                            conn.Open();
                        return conn;
                    }

                    throw;
                }
            }
        }

        protected NpgsqlConnection OpenConnection(NpgsqlConnectionStringBuilder csb)
            => OpenConnection(csb.ToString());

        protected virtual ValueTask<NpgsqlConnection> OpenConnectionAsync(NpgsqlConnectionStringBuilder csb)
            => OpenConnectionAsync(csb.ToString());

        // In PG under 9.1 you can't do SELECT pg_sleep(2) in binary because that function returns void and PG doesn't know
        // how to transfer that. So cast to text server-side.
        protected static NpgsqlCommand CreateSleepCommand(NpgsqlConnection conn, int seconds = 1000)
            => new($"SELECT pg_sleep({seconds}){(conn.PostgreSqlVersion < new Version(9, 1, 0) ? "::TEXT" : "")}", conn);

        protected bool IsRedshift => new NpgsqlConnectionStringBuilder(ConnectionString).ServerCompatibilityMode == ServerCompatibilityMode.Redshift;

        #endregion
    }
}
