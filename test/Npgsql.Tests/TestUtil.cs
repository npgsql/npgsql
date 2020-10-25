using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Npgsql.Tests
{
    public static class TestUtil
    {
        /// <summary>
        /// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
        /// test database.
        /// </summary>
        public const string DefaultConnectionString = "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

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
        public static void IgnoreExceptOnBuildServer(string message)
        {
            if (IsOnBuildServer)
                Assert.Fail(message);
            else
                Assert.Ignore(message);
        }

        public static void IgnoreExceptOnBuildServer(string message, params object[] args)
            => IgnoreExceptOnBuildServer(string.Format(message, args));

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

        static readonly Version MinCreateExtensionVersion = new Version(9, 1);

        public static bool IsPgPrerelease(NpgsqlConnection conn)
            => ((string)conn.ExecuteScalar("SELECT version()")!).Contains("beta");

        public static void EnsureExtension(NpgsqlConnection conn, string extension, string? minVersion = null)
            => EnsureExtension(conn, extension, minVersion, async: false).GetAwaiter().GetResult();

        public static Task EnsureExtensionAsync(NpgsqlConnection conn, string extension, string? minVersion = null)
            => EnsureExtension(conn, extension, minVersion, async: true);

        static async Task EnsureExtension(NpgsqlConnection conn, string extension, string? minVersion, bool async)
        {
            if (minVersion != null)
                MinimumPgVersion(conn, minVersion,
                    $"The extension '{extension}' only works for PostgreSQL {minVersion} and higher.");

            if (conn.PostgreSqlVersion < MinCreateExtensionVersion)
                Assert.Ignore($"The 'CREATE EXTENSION' command only works for PostgreSQL {MinCreateExtensionVersion} and higher.");

            if (async)
                await conn.ExecuteNonQueryAsync($"CREATE EXTENSION IF NOT EXISTS {extension}");
            else
                conn.ExecuteNonQuery($"CREATE EXTENSION IF NOT EXISTS {extension}");

            conn.ReloadTypes();
        }

        public static async Task EnsurePostgis(NpgsqlConnection conn)
        {
            try
            {
                await EnsureExtensionAsync(conn, "postgis");
            }
            catch (PostgresException e) when (e.SqlState == "58P01")
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
        /// Creates a table with a unique name, usable for a single test, and returns an <see cref="IDisposable"/> to
        /// drop it at the end of the test.
        /// </summary>
        internal static Task<IAsyncDisposable> CreateTempTable(NpgsqlConnection conn, string columns, out string tableName)
        {
            tableName = "temp_table" + Interlocked.Increment(ref _tempTableCounter);
            return conn.ExecuteNonQueryAsync($"DROP TABLE IF EXISTS {tableName} CASCADE; CREATE TABLE {tableName} ({columns})")
                .ContinueWith(
                    (t, name) => (IAsyncDisposable)new DatabaseObjectDropper(conn, (string)name!, "TABLE"),
                    tableName,
                    TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// Creates a schema with a unique name, usable for a single test, and returns an <see cref="IDisposable"/> to
        /// drop it at the end of the test.
        /// </summary>
        internal static Task<IAsyncDisposable> CreateTempSchema(NpgsqlConnection conn, out string schemaName)
        {
            schemaName = "temp_schema" + Interlocked.Increment(ref _tempSchemaCounter);
            return conn.ExecuteNonQueryAsync($"DROP SCHEMA IF EXISTS {schemaName} CASCADE; CREATE SCHEMA {schemaName}")
                .ContinueWith(
                    (t, name) => (IAsyncDisposable)new DatabaseObjectDropper(conn, (string)name!, "SCHEMA"),
                    schemaName,
                    TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// Generates a unique table name, usable for a single test, and drops it if it already exists.
        /// Actual creation of the table is the responsibility of the caller.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> to drop the table at the end of the test.
        /// </returns>
        internal static Task<IAsyncDisposable> GetTempTableName(NpgsqlConnection conn, out string tableName)
        {
            tableName = "temp_table" + Interlocked.Increment(ref _tempTableCounter);
            return conn.ExecuteNonQueryAsync($"DROP TABLE IF EXISTS {tableName} CASCADE")
                .ContinueWith(
                    (t, name) => (IAsyncDisposable)new DatabaseObjectDropper(conn, (string)name!, "TABLE"),
                    tableName,
                    TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// Generates a unique view name, usable for a single test, and drops it if it already exists.
        /// Actual creation of the view is the responsibility of the caller.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> to drop the view at the end of the test.
        /// </returns>
        internal static Task<IAsyncDisposable> GetTempViewName(NpgsqlConnection conn, out string viewName)
        {
            viewName = "temp_view" + Interlocked.Increment(ref _tempViewCounter);
            return conn.ExecuteNonQueryAsync($"DROP VIEW IF EXISTS {viewName} CASCADE")
                .ContinueWith(
                    (t, name) => (IAsyncDisposable)new DatabaseObjectDropper(conn, (string)name!, "VIEW"),
                    viewName,
                    TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        /// <summary>
        /// Generates a unique function name, usable for a single test.
        /// Actual creation of the function is the responsibility of the caller.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> to drop the function at the end of the test.
        /// </returns>
        internal static IAsyncDisposable GetTempFunctionName(NpgsqlConnection conn, out string functionName)
        {
            functionName = "temp_func" + Interlocked.Increment(ref _tempFunctionCounter);
            return new DatabaseObjectDropper(conn, functionName, "FUNCTION");
        }

        /// <summary>
        /// Generates a unique function name, usable for a single test.
        /// Actual creation of the function is the responsibility of the caller.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> to drop the function at the end of the test.
        /// </returns>
        internal static Task<IAsyncDisposable> GetTempTypeName(NpgsqlConnection conn, out string typeName)
        {
            typeName = "temp_type" + Interlocked.Increment(ref _tempTypeCounter);
            return conn.ExecuteNonQueryAsync($"DROP TYPE IF EXISTS {typeName} CASCADE")
                .ContinueWith(
                    (t, name) => (IAsyncDisposable)new DatabaseObjectDropper(conn, (string)name!, "TYPE"),
                    typeName,
                    TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        static volatile int _tempTableCounter;
        static volatile int _tempViewCounter;
        static volatile int _tempFunctionCounter;
        static volatile int _tempSchemaCounter;
        static volatile int _tempTypeCounter;

        readonly struct DatabaseObjectDropper : IAsyncDisposable
        {
            readonly NpgsqlConnection _conn;
            readonly string _type;
            readonly string _name;

            internal DatabaseObjectDropper(NpgsqlConnection conn, string name, string type)
                => (_conn, _name, _type) = (conn, name, type);

            public async ValueTask DisposeAsync()
            {
                try
                {
                    await _conn.ExecuteNonQueryAsync($"DROP {_type} {_name} CASCADE");
                }
                catch
                {
                    // Swallow to allow triggering exceptions to surface
                }
            }
        }

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
            var resetter = new EnvironmentVariableResetter(name, Environment.GetEnvironmentVariable(name));
            Environment.SetEnvironmentVariable(name, value);
            return resetter;
        }

        internal static IDisposable SetCurrentCulture(CultureInfo culture) =>
            new CultureSetter(culture);

        class EnvironmentVariableResetter : IDisposable
        {
            readonly string _name;
            readonly string? _value;

            internal EnvironmentVariableResetter(string name, string? value)
            {
                _name = name;
                _value = value;
            }

            public void Dispose() =>
                Environment.SetEnvironmentVariable(_name, _value);
        }

        class CultureSetter : IDisposable
        {
            readonly CultureInfo _oldCulture;

            internal CultureSetter(CultureInfo newCulture)
            {
                _oldCulture = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = newCulture;
            }

            public void Dispose() =>
                CultureInfo.CurrentCulture = _oldCulture;
        }
    }

    public static class NpgsqlConnectionExtensions
    {
        public static int ExecuteNonQuery(this NpgsqlConnection conn, string sql, NpgsqlTransaction? tx = null)
        {
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return cmd.ExecuteNonQuery();
        }

        public static object? ExecuteScalar(this NpgsqlConnection conn, string sql, NpgsqlTransaction? tx = null)
        {
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return cmd.ExecuteScalar();
        }

        public static async Task<int> ExecuteNonQueryAsync(this NpgsqlConnection conn, string sql, NpgsqlTransaction? tx = null)
        {
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<object?> ExecuteScalarAsync(this NpgsqlConnection conn, string sql, NpgsqlTransaction? tx = null)
        {
            var cmd = tx == null ? new NpgsqlCommand(sql, conn) : new NpgsqlCommand(sql, conn, tx);
            using (cmd)
                return await cmd.ExecuteScalarAsync();
        }
    }

    public static class CommandBehaviorExtensions
    {
        public static bool IsSequential(this CommandBehavior behavior)
            => (behavior & CommandBehavior.SequentialAccess) != 0;
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

    /// <summary>
    /// Causes the test to be ignored on mono
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class MonoIgnore : Attribute, ITestAction
    {
        readonly string? _ignoreText;

        public MonoIgnore(string? ignoreText = null) { _ignoreText = ignoreText; }

        public void BeforeTest(ITest test)
        {
            if (Type.GetType("Mono.Runtime") != null)
            {
                var msg = "Ignored on mono";
                if (_ignoreText != null)
                    msg += ": " + _ignoreText;
                Assert.Ignore(msg);
            }
        }

        public void AfterTest(ITest test) { }
        public ActionTargets Targets => ActionTargets.Test;
    }

    /// <summary>
    /// Causes the test to be ignored on Linux
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class LinuxIgnore : Attribute, ITestAction
    {
        readonly string? _ignoreText;

        public LinuxIgnore(string? ignoreText = null) { _ignoreText = ignoreText; }

        public void BeforeTest(ITest test)
        {
            var osEnvVar = Environment.GetEnvironmentVariable("OS");
            if (osEnvVar == null || osEnvVar != "Windows_NT")
            {
                var msg = "Ignored on Linux";
                if (_ignoreText != null)
                    msg += ": " + _ignoreText;
                Assert.Ignore(msg);
            }
        }

        public void AfterTest(ITest test) { }
        public ActionTargets Targets => ActionTargets.Test;
    }

    /// <summary>
    /// Causes the test to be ignored on Windows
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class WindowsIgnore : Attribute, ITestAction
    {
        readonly string? _ignoreText;

        public WindowsIgnore(string? ignoreText = null) { _ignoreText = ignoreText; }

        public void BeforeTest(ITest test)
        {
            var osEnvVar = Environment.GetEnvironmentVariable("OS");
            if (osEnvVar == "Windows_NT")
            {
                var msg = "Ignored on Windows";
                if (_ignoreText != null)
                    msg += ": " + _ignoreText;
                Assert.Ignore(msg);
            }
        }

        public void AfterTest(ITest test) { }
        public ActionTargets Targets => ActionTargets.Test;
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
}
