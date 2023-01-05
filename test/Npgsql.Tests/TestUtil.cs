using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Npgsql.Tests;

public static class TestUtil
{
    /// <summary>
    /// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
    /// test database.
    /// </summary>
    public const string DefaultConnectionString =
        "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0;SSL Mode=Disable";

    /// <summary>
    /// The connection string that will be used when opening the connection to the tests database.
    /// May be overridden in fixtures, e.g. to set special connection parameters
    /// </summary>
    public static string ConnectionString { get; }
        = Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? DefaultConnectionString;

    public static bool IsOnBuildServer =>
        Environment.GetEnvironmentVariable("GITHUB_ACTIONS") != null ||
        Environment.GetEnvironmentVariable("CI") != null;

    /// <summary>
    /// Calls Assert.Ignore() unless we're on the build server, in which case calls
    /// Assert.Fail(). We don't to miss any regressions just because something was misconfigured
    /// at the build server and caused a test to be inconclusive.
    /// </summary>
    [DoesNotReturn]
    public static void IgnoreExceptOnBuildServer(string message)
    {
        if (IsOnBuildServer)
            Assert.Fail(message);
        else
            Assert.Ignore(message);

        throw new Exception("Should not occur");
    }

    public static void IgnoreExceptOnBuildServer(string message, params object[] args)
        => IgnoreExceptOnBuildServer(string.Format(message, args));

    public static void MinimumPgVersion(NpgsqlDataSource dataSource, string minVersion, string? ignoreText = null)
    {
        using var connection = dataSource.OpenConnection();
        MinimumPgVersion(connection, minVersion, ignoreText);
    }

    public static void MinimumPgVersion(NpgsqlConnection conn, string minVersion, string? ignoreText = null)
    {
        var min = new Version(minVersion);
        if (conn.PostgreSqlVersion < min)
        {
            var msg = $"Postgresql backend version {conn.PostgreSqlVersion} is less than the required {min}";
            if (ignoreText != null)
                msg += ": " + ignoreText;
            Assert.Ignore(msg);
        }
    }

    public static void MaximumPgVersionExclusive(NpgsqlConnection conn, string maxVersion, string? ignoreText = null)
    {
        var max = new Version(maxVersion);
        if (conn.PostgreSqlVersion >= max)
        {
            var msg = $"Postgresql backend version {conn.PostgreSqlVersion} is greater than or equal to the required (exclusive) maximum of {maxVersion}";
            if (ignoreText != null)
                msg += ": " + ignoreText;
            Assert.Ignore(msg);
        }
    }

    static readonly Version MinCreateExtensionVersion = new(9, 1);

    public static void IgnoreOnRedshift(NpgsqlConnection conn, string? ignoreText = null)
    {
        if (new NpgsqlConnectionStringBuilder(conn.ConnectionString).ServerCompatibilityMode == ServerCompatibilityMode.Redshift)
        {
            var msg = "Test ignored on Redshift";
            if (ignoreText != null)
                msg += ": " + ignoreText;
            Assert.Ignore(msg);
        }
    }

    public static bool IsPgPrerelease(NpgsqlConnection conn)
        => ((string)conn.ExecuteScalar("SELECT version()")!).Contains("beta");

    public static void EnsureExtension(NpgsqlConnection conn, string extension, string? minVersion = null)
        => EnsureExtension(conn, extension, minVersion, async: false).GetAwaiter().GetResult();

    public static Task EnsureExtensionAsync(NpgsqlConnection conn, string extension, string? minVersion = null)
        => EnsureExtension(conn, extension, minVersion, async: true);

    static async Task EnsureExtension(NpgsqlConnection conn, string extension, string? minVersion, bool async)
    {
        if (minVersion != null)
            MinimumPgVersion(conn, minVersion, $"The extension '{extension}' only works for PostgreSQL {minVersion} and higher.");

        if (conn.PostgreSqlVersion < MinCreateExtensionVersion)
            Assert.Ignore($"The 'CREATE EXTENSION' command only works for PostgreSQL {MinCreateExtensionVersion} and higher.");

        if (async)
            await conn.ExecuteNonQueryAsync($"CREATE EXTENSION IF NOT EXISTS {extension}");
        else
            conn.ExecuteNonQuery($"CREATE EXTENSION IF NOT EXISTS {extension}");

        conn.ReloadTypes();

        // Multiplexing doesn't really support reloading types, since each connector uses its own connector type mapper when reading,
        // which is different from the pool-wise connector mapper (which is used when writing).
        NpgsqlConnection.ClearPool(conn);
    }

    /// <summary>
    /// Causes the test to be ignored if the supplied query fails with SqlState 0A000 (feature_not_supported)
    /// </summary>
    /// <param name="conn">The connection to execute the test query. The connection needs to be open.</param>
    /// <param name="testQuery">The query to test for the feature.
    /// This query needs to fail with SqlState 0A000 (feature_not_supported) if the feature isn't present.</param>
    public static void IgnoreIfFeatureNotSupported(NpgsqlConnection conn, string testQuery)
        => IgnoreIfFeatureNotSupported(conn, testQuery, async: false).GetAwaiter().GetResult();

    /// <summary>
    /// Causes the test to be ignored if the supplied query fails with SqlState 0A000 (feature_not_supported)
    /// </summary>
    /// <param name="conn">The connection to execute the test query. The connection needs to be open.</param>
    /// <param name="testQuery">The query to test for the feature.
    /// This query needs to fail with SqlState 0A000 (feature_not_supported) if the feature isn't present.</param>
    public static Task IgnoreIfFeatureNotSupportedAsync(NpgsqlConnection conn, string testQuery)
        => IgnoreIfFeatureNotSupported(conn, testQuery, async: true);

    static async Task IgnoreIfFeatureNotSupported(NpgsqlConnection conn, string testQuery, bool async)
    {
        try
        {
            if (async)
                await conn.ExecuteNonQueryAsync(testQuery);
            else
                conn.ExecuteNonQuery(testQuery);
        }
        catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.FeatureNotSupported)
        {
            Assert.Ignore(e.Message);
        }
    }

    public static async Task EnsurePostgis(NpgsqlConnection conn)
    {
        try
        {
            await EnsureExtensionAsync(conn, "postgis");
        }
        catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.UndefinedFile)
        {
            // PostGIS packages aren't available for PostgreSQL prereleases
            if (IsPgPrerelease(conn))
            {
                Assert.Ignore($"PostGIS could not be installed, but PostgreSQL is prerelease ({conn.ServerVersion}), ignoring test suite.");
            }
        }
    }

    public static string GetUniqueIdentifier(string prefix)
        => prefix + Interlocked.Increment(ref _counter);

    static int _counter;

    /// <summary>
    /// Creates a table with a unique name, usable for a single test.
    /// </summary>
    internal static async Task<string> CreateTempTable(NpgsqlConnection conn, string columns)
    {
        var tableName = "temp_table" + Interlocked.Increment(ref _tempTableCounter);

        await conn.ExecuteNonQueryAsync(@$"
START TRANSACTION;
SELECT pg_advisory_xact_lock(0);
DROP TABLE IF EXISTS {tableName} CASCADE;
COMMIT;
CREATE TABLE {tableName} ({columns});");

        return tableName;
    }

    /// <summary>
    /// Generates a unique table name, usable for a single test, and drops it if it already exists.
    /// Actual creation of the table is the responsibility of the caller.
    /// </summary>
    internal static async Task<string> GetTempTableName(NpgsqlConnection conn)
    {
        var tableName = "temp_table" + Interlocked.Increment(ref _tempTableCounter);
        await conn.ExecuteNonQueryAsync(@$"
START TRANSACTION;
SELECT pg_advisory_xact_lock(0);
DROP TABLE IF EXISTS {tableName} CASCADE;
COMMIT");
        return tableName;
    }

    /// <summary>
    /// Creates a table with a unique name, usable for a single test, and returns an <see cref="IDisposable"/> to
    /// drop it at the end of the test.
    /// </summary>
    internal static async Task<string> CreateTempTable(NpgsqlDataSource dataSource, string columns)
    {
        var tableName = "temp_table" + Interlocked.Increment(ref _tempTableCounter);
        await dataSource.ExecuteNonQueryAsync(@$"
START TRANSACTION;
SELECT pg_advisory_xact_lock(0);
DROP TABLE IF EXISTS {tableName} CASCADE;
COMMIT;
CREATE TABLE {tableName} ({columns});");
        return tableName;
    }

    /// <summary>
    /// Creates a schema with a unique name, usable for a single test.
    /// </summary>
    internal static async Task<string> CreateTempSchema(NpgsqlConnection conn)
    {
        var schemaName = "temp_schema" + Interlocked.Increment(ref _tempSchemaCounter);
        await conn.ExecuteNonQueryAsync($"DROP SCHEMA IF EXISTS {schemaName} CASCADE; CREATE SCHEMA {schemaName}");
        return schemaName;
    }

    /// <summary>
    /// Generates a unique view name, usable for a single test, and drops it if it already exists.
    /// Actual creation of the view is the responsibility of the caller.
    /// </summary>
    internal static async Task<string> GetTempViewName(NpgsqlConnection conn)
    {
        var viewName = "temp_view" + Interlocked.Increment(ref _tempViewCounter);
        await conn.ExecuteNonQueryAsync($"DROP VIEW IF EXISTS {viewName} CASCADE");
        return viewName;
    }

    /// <summary>
    /// Generates a unique function name, usable for a single test.
    /// Actual creation of the function is the responsibility of the caller.
    /// </summary>
    internal static async Task<string> GetTempFunctionName(NpgsqlConnection conn)
    {
        var functionName = "temp_func" + Interlocked.Increment(ref _tempFunctionCounter);
        await conn.ExecuteNonQueryAsync($"DROP FUNCTION IF EXISTS {functionName} CASCADE");
        return functionName;
    }

    /// <summary>
    /// Generates a unique function name, usable for a single test.
    /// Actual creation of the function is the responsibility of the caller.
    /// </summary>
    /// <returns>
    /// An <see cref="IDisposable"/> to drop the function at the end of the test.
    /// </returns>
    internal static async Task<string> GetTempProcedureName(NpgsqlDataSource dataSource)
    {
        var procedureName = "temp_procedure" + Interlocked.Increment(ref _tempProcedureCounter);
        await dataSource.ExecuteNonQueryAsync($"DROP PROCEDURE IF EXISTS {procedureName} CASCADE");
        return procedureName;
    }

    /// <summary>
    /// Generates a unique function name, usable for a single test.
    /// Actual creation of the function is the responsibility of the caller.
    /// </summary>
    /// <returns>
    /// An <see cref="IDisposable"/> to drop the function at the end of the test.
    /// </returns>
    internal static async Task<string> GetTempProcedureName(NpgsqlConnection connection)
    {
        var procedureName = "temp_procedure" + Interlocked.Increment(ref _tempProcedureCounter);
        await connection.ExecuteNonQueryAsync($"DROP PROCEDURE IF EXISTS {procedureName} CASCADE");
        return procedureName;
    }

    /// <summary>
    /// Generates a unique type name, usable for a single test.
    /// Actual creation of the type is the responsibility of the caller.
    /// </summary>
    internal static async Task<string> GetTempTypeName(NpgsqlConnection conn)
    {
        var typeName = "temp_type" + Interlocked.Increment(ref _tempTypeCounter);
        await conn.ExecuteNonQueryAsync($"DROP TYPE IF EXISTS {typeName} CASCADE");
        return typeName;
    }

    internal static volatile int _tempTableCounter;
    static volatile int _tempViewCounter;
    static volatile int _tempFunctionCounter;
    static volatile int _tempProcedureCounter;
    static volatile int _tempSchemaCounter;
    static volatile int _tempTypeCounter;

    /// <summary>
    /// Creates a pool with a unique application name, usable for a single test, and returns an
    /// <see cref="IDisposable"/> to drop it at the end of the test.
    /// </summary>
    internal static IDisposable CreateTempPool(string origConnectionString, out string tempConnectionString)
        => CreateTempPool(new NpgsqlConnectionStringBuilder(origConnectionString), out tempConnectionString);

    /// <summary>
    /// Creates a pool with a unique application name, usable for a single test, and returns an
    /// <see cref="IDisposable"/> to drop it at the end of the test.
    /// </summary>
    internal static IDisposable CreateTempPool(NpgsqlConnectionStringBuilder builder, out string tempConnectionString)
    {
        builder.ApplicationName = (builder.ApplicationName ?? "TempPool") + Interlocked.Increment(ref _tempPoolCounter);
        tempConnectionString = builder.ConnectionString;
        return new PoolDisposer(tempConnectionString);
    }

    static volatile int _tempPoolCounter;

    readonly struct PoolDisposer : IDisposable
    {
        readonly string _connectionString;

        internal PoolDisposer(string connectionString) => _connectionString = connectionString;

        public void Dispose()
        {
            var conn = new NpgsqlConnection(_connectionString);
            NpgsqlConnection.ClearPool(conn);
        }
    }

    /// <summary>
    /// Utility to generate a bytea literal in Postgresql hex format
    /// See https://www.postgresql.org/docs/current/static/datatype-binary.html
    /// </summary>
    internal static string EncodeByteaHex(ICollection<byte> buf)
    {
        var hex = new StringBuilder(@"E'\\x", buf.Count * 2 + 3);
        foreach (var b in buf)
            hex.Append($"{b:x2}");
        hex.Append("'");
        return hex.ToString();
    }

    internal static IDisposable SetEnvironmentVariable(string name, string? value)
    {
        var oldValue = Environment.GetEnvironmentVariable(name);
        Environment.SetEnvironmentVariable(name, value);
        return new DeferredExecutionDisposable(() => Environment.SetEnvironmentVariable(name, oldValue));
    }

    internal static IDisposable SetCurrentCulture(CultureInfo culture)
    {
        var oldCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = culture;

        return new DeferredExecutionDisposable(() => CultureInfo.CurrentCulture = oldCulture);
    }

    internal static IDisposable DisableSqlRewriting()
    {
#if DEBUG
        NpgsqlCommand.EnableSqlRewriting = false;
        return new DeferredExecutionDisposable(() => NpgsqlCommand.EnableSqlRewriting = true);
#else
            Assert.Ignore("Cannot disable SQL rewriting in RELEASE builds");
            throw new NotSupportedException("Cannot disable SQL rewriting in RELEASE builds");
#endif
    }

    class DeferredExecutionDisposable : IDisposable
    {
        readonly Action _action;

        internal DeferredExecutionDisposable(Action action) => _action = action;

        public void Dispose()
            => _action();
    }

    internal static object AssertLoggingStateContains(
        (LogLevel Level, EventId Id, string Message, object? State, Exception? Exception) log,
        string key)
    {
        if (log.State is not IEnumerable<KeyValuePair<string, object>> keyValuePairs || keyValuePairs.All(kvp => kvp.Key != key))
        {
            Assert.Fail($@"Dod not find logging state key ""{key}""");
            throw new Exception();
        }

        return keyValuePairs.Single(kvp => kvp.Key == key).Value;
    }

    internal static void AssertLoggingStateContains<T>(
        (LogLevel Level, EventId Id, string Message, object? State, Exception? Exception) log,
        string key,
        T value)
        => Assert.That(log.State, Contains.Item(new KeyValuePair<string, T>(key, value)));

    internal static void AssertLoggingStateDoesNotContain(
        (LogLevel Level, EventId Id, string Message, object? State, Exception? Exception) log,
        string key)
    {
        var value = log.State is IEnumerable<KeyValuePair<string, object>> keyValuePairs &&
                    keyValuePairs.FirstOrDefault(kvp => kvp.Key == key) is { } kvpPair
            ? kvpPair.Value
            : null;

        Assert.That(value, Is.Null, $@"Found logging state (""{key}"", {value}");
    }
}

public static class NpgsqlConnectionExtensions
{
    public static int ExecuteNonQuery(this NpgsqlConnection conn, string sql, NpgsqlTransaction? tx = null)
    {
        using var command = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
        return command.ExecuteNonQuery();
    }

    public static object? ExecuteScalar(this NpgsqlConnection conn, string sql, NpgsqlTransaction? tx = null)
    {
        using var command = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
        return command.ExecuteScalar();
    }

    public static async Task<int> ExecuteNonQueryAsync(
        this NpgsqlConnection conn, string sql, NpgsqlTransaction? tx = null, CancellationToken cancellationToken = default)
    {
        await using var command = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public static async Task<object?> ExecuteScalarAsync(
        this NpgsqlConnection conn, string sql, NpgsqlTransaction? tx = null, CancellationToken cancellationToken = default)
    {
        await using var command = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
        return await command.ExecuteScalarAsync(cancellationToken);
    }
}

public static class NpgsqlDataSourceExtensions
{
    public static int ExecuteNonQuery(this NpgsqlDataSource dataSource, string sql)
    {
        using var command = dataSource.CreateCommand(sql);
        return command.ExecuteNonQuery();
    }

    public static object? ExecuteScalar(this NpgsqlDataSource dataSource, string sql)
    {
        using var command = dataSource.CreateCommand(sql);
        return command.ExecuteScalar();
    }

    public static async Task<int> ExecuteNonQueryAsync(
        this NpgsqlDataSource dataSource, string sql, CancellationToken cancellationToken = default)
    {
        await using var command = dataSource.CreateCommand(sql);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public static async Task<object?> ExecuteScalarAsync(
        this NpgsqlDataSource dataSource, string sql, CancellationToken cancellationToken = default)
    {
        await using var command = dataSource.CreateCommand(sql);
        return await command.ExecuteScalarAsync(cancellationToken);
    }
}

public static class CommandBehaviorExtensions
{
    public static bool IsSequential(this CommandBehavior behavior)
        => (behavior & CommandBehavior.SequentialAccess) != 0;
}

public static class NpgsqlCommandExtensions
{
    public static void WaitUntilCommandIsInProgress(this NpgsqlCommand command)
    {
        while (command.State != CommandState.InProgress)
            Thread.Sleep(50);
    }
}

/// <summary>
/// Semantic attribute that points to an issue linked with this test (e.g. this
/// test reproduces the issue)
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class IssueLink : Attribute
{
    public string LinkAddress { get; private set; }
    public IssueLink(string linkAddress)
    {
        LinkAddress = linkAddress;
    }
}

public enum PrepareOrNot
{
    Prepared,
    NotPrepared
}

public enum PooledOrNot
{
    Pooled,
    Unpooled
}

#if NETSTANDARD2_0
    static class QueueExtensions
    {
        public static bool TryDequeue<T>(this Queue<T> queue, out T result)
        {
            if (queue.Count == 0)
            {
                result = default;
                return false;
            }

            result = queue.Dequeue();
            return true;
        }
    }
#endif
