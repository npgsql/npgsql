using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests;

public abstract class TestBase
{
    /// <summary>
    /// The connection string that will be used when opening the connection to the tests database.
    /// May be overridden in fixtures, e.g. to set special connection parameters
    /// </summary>
    public virtual string ConnectionString => TestUtil.ConnectionString;

    static readonly SemaphoreSlim DatabaseCreationLock = new(1);

    static readonly object dataSourceLockObject = new();

    static ConcurrentDictionary<string, NpgsqlDataSource> DataSources = new(StringComparer.Ordinal);

    #region Type testing

    public async Task<T> AssertType<T>(
        T value,
        string sqlLiteral,
        string pgTypeName,
        NpgsqlDbType? npgsqlDbType,
        DbType? dbType = null,
        DbType? inferredDbType = null,
        bool isDefaultForReading = true,
        bool isDefaultForWriting = true,
        bool? isDefault = null,
        bool isNpgsqlDbTypeInferredFromClrType = true,
        Func<T, T, bool>? comparer = null)
    {
        await using var connection = await OpenConnectionAsync();
        return await AssertType(
            connection, value, sqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefaultForReading, isDefaultForWriting,
            isDefault, isNpgsqlDbTypeInferredFromClrType, comparer);
    }

    public async Task<T> AssertType<T>(
        NpgsqlDataSource dataSource,
        T value,
        string sqlLiteral,
        string pgTypeName,
        NpgsqlDbType? npgsqlDbType,
        DbType? dbType = null,
        DbType? inferredDbType = null,
        bool isDefaultForReading = true,
        bool isDefaultForWriting = true,
        bool? isDefault = null,
        bool isNpgsqlDbTypeInferredFromClrType = true,
        Func<T, T, bool>? comparer = null)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        return await AssertType(connection, value, sqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefaultForReading,
            isDefaultForWriting, isDefault, isNpgsqlDbTypeInferredFromClrType, comparer);
    }

    public async Task<T> AssertType<T>(
        NpgsqlConnection connection,
        T value,
        string sqlLiteral,
        string pgTypeName,
        NpgsqlDbType? npgsqlDbType,
        DbType? dbType = null,
        DbType? inferredDbType = null,
        bool isDefaultForReading = true,
        bool isDefaultForWriting = true,
        bool? isDefault = null,
        bool isNpgsqlDbTypeInferredFromClrType = true,
        Func<T, T, bool>? comparer = null)
    {
        if (isDefault is not null)
            isDefaultForReading = isDefaultForWriting = isDefault.Value;

        await AssertTypeWrite(connection, () => value, sqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefaultForWriting, isNpgsqlDbTypeInferredFromClrType);
        return await AssertTypeRead(connection, sqlLiteral, pgTypeName, value, isDefaultForReading, comparer);
    }

    public async Task<T> AssertTypeRead<T>(string sqlLiteral, string pgTypeName, T expected, bool isDefault = true)
    {
        await using var connection = await OpenConnectionAsync();
        return await AssertTypeRead(connection, sqlLiteral, pgTypeName, expected, isDefault);
    }

    public async Task AssertTypeWrite<T>(
        NpgsqlDataSource dataSource,
        T value,
        string expectedSqlLiteral,
        string pgTypeName,
        NpgsqlDbType npgsqlDbType,
        DbType? dbType = null,
        DbType? inferredDbType = null,
        bool isDefault = true,
        bool isNpgsqlDbTypeInferredFromClrType = true)
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        await AssertTypeWrite(connection, () => value, expectedSqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefault,
            isNpgsqlDbTypeInferredFromClrType);
    }

    public Task AssertTypeWrite<T>(
        T value,
        string expectedSqlLiteral,
        string pgTypeName,
        NpgsqlDbType npgsqlDbType,
        DbType? dbType = null,
        DbType? inferredDbType = null,
        bool isDefault = true,
        bool isNpgsqlDbTypeInferredFromClrType = true)
        => AssertTypeWrite(() => value, expectedSqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefault,
            isNpgsqlDbTypeInferredFromClrType);

    public async Task AssertTypeWrite<T>(
        Func<T> valueFactory,
        string expectedSqlLiteral,
        string pgTypeName,
        NpgsqlDbType npgsqlDbType,
        DbType? dbType = null,
        DbType? inferredDbType = null,
        bool isDefault = true,
        bool isNpgsqlDbTypeInferredFromClrType = true)
    {
        await using var connection = await OpenConnectionAsync();
        await AssertTypeWrite(connection, valueFactory, expectedSqlLiteral, pgTypeName, npgsqlDbType, dbType, inferredDbType, isDefault, isNpgsqlDbTypeInferredFromClrType);
    }

    internal static async Task<T> AssertTypeRead<T>(
        NpgsqlConnection connection,
        string sqlLiteral,
        string pgTypeName,
        T expected,
        bool isDefault = true,
        Func<T, T, bool>? comparer = null)
    {
        if (sqlLiteral.Contains('\''))
            sqlLiteral = sqlLiteral.Replace("'", "''");

        await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{pgTypeName}", connection);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        await reader.ReadAsync();

        var truncatedSqlLiteral = sqlLiteral.Length > 40 ? sqlLiteral[..40] + "..." : sqlLiteral;

        var dataTypeName = reader.GetDataTypeName(0);
        var dotIndex = dataTypeName.IndexOf('.');
        if (dotIndex > -1 && dataTypeName.Substring(0, dotIndex) is "pg_catalog" or "public")
            dataTypeName = dataTypeName.Substring(dotIndex + 1);

        Assert.That(dataTypeName, Is.EqualTo(pgTypeName),
            $"Got wrong result from GetDataTypeName when reading '{truncatedSqlLiteral}'");

        if (isDefault)
        {
            // For arrays, GetFieldType always returns typeof(Array), since PG arrays can have arbitrary dimensionality
            Assert.That(reader.GetFieldType(0), Is.EqualTo(dataTypeName.EndsWith("[]") ? typeof(Array) : typeof(T)),
                $"Got wrong result from GetFieldType when reading '{truncatedSqlLiteral}'");
        }

        var actual = isDefault ? (T)reader.GetValue(0) : reader.GetFieldValue<T>(0);

        Assert.That(actual, comparer is null ? Is.EqualTo(expected) : Is.EqualTo(expected).Using(new SimpleComparer<T>(comparer)),
            $"Got wrong result from GetFieldValue value when reading '{truncatedSqlLiteral}'");

        return actual;
    }

    internal static async Task AssertTypeWrite<T>(
        NpgsqlConnection connection,
        Func<T> valueFactory,
        string expectedSqlLiteral,
        string pgTypeName,
        NpgsqlDbType? npgsqlDbType,
        DbType? dbType = null,
        DbType? inferredDbType = null,
        bool isDefault = true,
        bool isNpgsqlDbTypeInferredFromClrType = true)
    {
        if (npgsqlDbType is null)
            isNpgsqlDbTypeInferredFromClrType = false;

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

        var errorIdentifierIndex = -1;
        var errorIdentifier = new Dictionary<int, string>();

        await using var cmd = new NpgsqlCommand { Connection = connection };
        NpgsqlParameter p;
        // With NpgsqlDbType
        if (npgsqlDbType is not null)
        {
            p = new NpgsqlParameter { Value = valueFactory(), NpgsqlDbType = npgsqlDbType.Value };
            cmd.Parameters.Add(p);
            errorIdentifier[++errorIdentifierIndex] = $"NpgsqlDbType={npgsqlDbType}";
            CheckInference();
        }

        // With data type name
        p = new NpgsqlParameter { Value = valueFactory(), DataTypeName = pgTypeNameWithoutFacets };
        cmd.Parameters.Add(p);
        errorIdentifier[++errorIdentifierIndex] = $"DataTypeName={pgTypeNameWithoutFacets}";
        CheckInference();

        // With DbType
        if (dbType is not null)
        {
            p = new NpgsqlParameter { Value = valueFactory(), DbType = dbType.Value };
            cmd.Parameters.Add(p);
            errorIdentifier[++errorIdentifierIndex] = $"DbType={dbType}";
            CheckInference();
        }

        if (isDefault)
        {
            // With (non-generic) value only
            p = new NpgsqlParameter { Value = valueFactory() };
            cmd.Parameters.Add(p);
            errorIdentifier[++errorIdentifierIndex] = "Value only (non-generic)";
            if (isNpgsqlDbTypeInferredFromClrType)
                CheckInference();

            // With (generic) value only
            p = new NpgsqlParameter<T> { TypedValue = valueFactory() };
            cmd.Parameters.Add(p);
            errorIdentifier[++errorIdentifierIndex] = "Value only (generic)";
            if (isNpgsqlDbTypeInferredFromClrType)
                CheckInference();
        }

        Debug.Assert(cmd.Parameters.Count == errorIdentifierIndex + 1);

        cmd.CommandText = "SELECT " + string.Join(", ", Enumerable.Range(1, cmd.Parameters.Count).Select(i =>
            "pg_typeof($1)::text, $1::text".Replace("$1", $"${i}")));

        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        await reader.ReadAsync();

        for (var i = 0; i < cmd.Parameters.Count * 2; i += 2)
        {
            Assert.That(reader[i], Is.EqualTo(pgTypeNameWithoutFacets), $"Got wrong PG type name when writing with {errorIdentifier[i / 2]}");
            Assert.That(reader[i+1], Is.EqualTo(expectedSqlLiteral), $"Got wrong SQL literal when writing with {errorIdentifier[i / 2]}");
        }

        void CheckInference()
        {
            if (npgsqlDbType is not null)
            {
                Assert.That(p.NpgsqlDbType, Is.EqualTo(npgsqlDbType),
                    () => $"Got wrong inferred NpgsqlDbType when inferring with {errorIdentifier[errorIdentifierIndex]}");
            }

            Assert.That(p.DbType, Is.EqualTo(inferredDbType ?? dbType ?? DbType.Object),
                () => $"Got wrong inferred DbType when inferring with {errorIdentifier[errorIdentifierIndex]}");

            Assert.That(p.DataTypeName, Is.EqualTo(pgTypeNameWithoutFacets),
                () => $"Got wrong inferred DataTypeName when inferring with {errorIdentifier[errorIdentifierIndex]}");
        }
    }

    public async Task AssertTypeUnsupported<T>(T value, string sqlLiteral, string pgTypeName, NpgsqlDataSource? dataSource = null)
    {
        await AssertTypeUnsupportedRead<T>(sqlLiteral, pgTypeName, dataSource);
        await AssertTypeUnsupportedWrite(value, pgTypeName, dataSource);
    }

    public async Task AssertTypeUnsupportedRead(string sqlLiteral, string pgTypeName, NpgsqlDataSource? dataSource = null)
    {
        dataSource ??= DefaultDataSource;

        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{pgTypeName}", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<InvalidCastException>());
    }

    public Task<Exception> AssertTypeUnsupportedRead<T>(string sqlLiteral, string pgTypeName, NpgsqlDataSource? dataSource = null)
        => AssertTypeUnsupportedRead<T, InvalidCastException>(sqlLiteral, pgTypeName, dataSource);

    public async Task<Exception> AssertTypeUnsupportedRead<T, TException>(string sqlLiteral, string pgTypeName, NpgsqlDataSource? dataSource = null)
        where TException : Exception
    {
        dataSource ??= DefaultDataSource;

        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{pgTypeName}", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return Assert.Throws<TException>(() => reader.GetFieldValue<T>(0))!;
    }

    public Task<InvalidCastException> AssertTypeUnsupportedWrite<T>(T value, string? pgTypeName = null, NpgsqlDataSource? dataSource = null)
        => AssertTypeUnsupportedWrite<T, InvalidCastException>(value, pgTypeName, dataSource);

    public async Task<TException> AssertTypeUnsupportedWrite<T, TException>(T value, string? pgTypeName = null, NpgsqlDataSource? dataSource = null)
        where TException : Exception
    {
        dataSource ??= DefaultDataSource;

        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1", conn)
        {
            Parameters = { new() { Value = value } }
        };

        if (pgTypeName is not null)
            cmd.Parameters[0].DataTypeName = pgTypeName;

        return Assert.ThrowsAsync<TException>(() => cmd.ExecuteReaderAsync())!;
    }

    class SimpleComparer<T> : IEqualityComparer<T>
    {
        readonly Func<T, T, bool> _comparerDelegate;

        public SimpleComparer(Func<T, T, bool> comparerDelegate)
            => _comparerDelegate = comparerDelegate;

        public bool Equals(T? x, T? y)
            => x is null
                ? y is null
                : y is not null && _comparerDelegate(x, y);

        public int GetHashCode(T obj) => throw new NotSupportedException();
    }

    #endregion Type testing

    #region Utilities for use by tests

    protected static readonly NpgsqlDataSource SharedDataSource = NpgsqlDataSource.Create(TestUtil.ConnectionString);

    protected virtual NpgsqlDataSourceBuilder CreateDataSourceBuilder()
        => new(ConnectionString);

    protected virtual NpgsqlDataSource CreateDataSource()
        => CreateDataSource(ConnectionString);

    protected virtual NpgsqlDataSource CreateDataSource(string connectionString)
        => NpgsqlDataSource.Create(connectionString);

    protected virtual NpgsqlDataSource CreateDataSource(Action<NpgsqlConnectionStringBuilder> connectionStringBuilderAction)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString);
        connectionStringBuilderAction(connectionStringBuilder);
        return NpgsqlDataSource.Create(connectionStringBuilder);
    }

    protected static NpgsqlDataSource GetDataSource(string connectionString)
    {
        if (!DataSources.TryGetValue(connectionString, out var dataSource))
        {
            lock (dataSourceLockObject)
            {
                if (!DataSources.TryGetValue(connectionString, out dataSource))
                {
                    DataSources[connectionString] = dataSource = NpgsqlDataSource.Create(connectionString);
                }
            }
        }

        return dataSource;
    }

    protected virtual NpgsqlDataSource CreateLoggingDataSource(
        out ListLoggerProvider listLoggerProvider,
        string? connectionString = null,
        bool sensitiveDataLoggingEnabled = true)
    {
        var builder = new NpgsqlDataSourceBuilder(connectionString ?? ConnectionString);
        var provider = listLoggerProvider = new ListLoggerProvider();

        builder.UseLoggerFactory(LoggerFactory.Create(loggerFactoryBuilder =>
        {
            loggerFactoryBuilder.SetMinimumLevel(LogLevel.Trace);
            loggerFactoryBuilder.AddProvider(provider);
        }));

        builder.EnableParameterLogging(sensitiveDataLoggingEnabled);

        return builder.Build();
    }

    protected NpgsqlDataSource DefaultDataSource
        => GetDataSource(ConnectionString);

    protected virtual NpgsqlConnection CreateConnection()
        => DefaultDataSource.CreateConnection();

    protected virtual NpgsqlConnection OpenConnection()
    {
        var connection = CreateConnection();
        try
        {
            OpenConnection(connection, async: false).GetAwaiter().GetResult();
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    protected virtual async ValueTask<NpgsqlConnection> OpenConnectionAsync()
    {
        var connection = CreateConnection();
        try
        {
            await OpenConnection(connection, async: true);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    static Task OpenConnection(NpgsqlConnection conn, bool async)
    {
        return OpenConnectionInternal(hasLock: false);

        async Task OpenConnectionInternal(bool hasLock)
        {
            try
            {
                if (async)
                    await conn.OpenAsync();
                else
                    conn.Open();
            }
            catch (PostgresException e)
            {
                if (e.SqlState == PostgresErrorCodes.InvalidPassword)
                    throw new Exception("Please create a user npgsql_tests as follows: CREATE USER npgsql_tests PASSWORD 'npgsql_tests' SUPERUSER");

                if (e.SqlState == PostgresErrorCodes.InvalidCatalogName)
                {
                    if (!hasLock)
                    {
                        DatabaseCreationLock.Wait();
                        try
                        {
                            await OpenConnectionInternal(hasLock: true);
                        }
                        finally
                        {
                            DatabaseCreationLock.Release();
                        }
                    }

                    // Database does not exist and we have the lock, proceed to creation
                    var builder = new NpgsqlConnectionStringBuilder(TestUtil.ConnectionString)
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
                    return;
                }

                throw;
            }
        }
    }

    // In PG under 9.1 you can't do SELECT pg_sleep(2) in binary because that function returns void and PG doesn't know
    // how to transfer that. So cast to text server-side.
    protected static NpgsqlCommand CreateSleepCommand(NpgsqlConnection conn, int seconds = 1000)
        => new($"SELECT pg_sleep({seconds}){(conn.PostgreSqlVersion < new Version(9, 1, 0) ? "::TEXT" : "")}", conn);

    #endregion
}
