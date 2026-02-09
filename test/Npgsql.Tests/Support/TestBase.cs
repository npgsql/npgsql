using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Internal.Postgres;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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

    public Task<T> AssertType<T>(
        T value,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        Func<T, T, bool>? comparer = null,
        bool valueTypeEqualsFieldType = true,
        bool skipArrayCheck = false)
        => AssertTypeCore(OpenConnectionAsync(), disposeConnection: true, () => value, sqlLiteral, dataTypeName, dataTypeInference,
            dbType, comparer, valueTypeEqualsFieldType, skipArrayCheck);

    public Task<T> AssertType<T>(
        NpgsqlDataSource dataSource,
        T value,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        Func<T, T, bool>? comparer = null,
        bool valueTypeEqualsFieldType = true,
        bool skipArrayCheck = false)
        => AssertTypeCore(dataSource.OpenConnectionAsync(), disposeConnection: true, () => value, sqlLiteral, dataTypeName, dataTypeInference,
            dbType, comparer, valueTypeEqualsFieldType, skipArrayCheck);

    public Task<T> AssertType<T>(
        NpgsqlConnection connection,
        T value,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        Func<T, T, bool>? comparer = null,
        bool valueTypeEqualsFieldType = true,
        bool skipArrayCheck = false)
        => AssertTypeCore(new(connection), disposeConnection: false, () => value, sqlLiteral, dataTypeName, dataTypeInference,
            dbType, comparer, valueTypeEqualsFieldType, skipArrayCheck);

    public Task<T> AssertType<T>(
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        Func<T, T, bool>? comparer = null,
        bool valueTypeEqualsFieldType = true,
        bool skipArrayCheck = false)
        => AssertTypeCore(OpenConnectionAsync(), disposeConnection: true, valueFactory, sqlLiteral, dataTypeName, dataTypeInference,
            dbType, comparer, valueTypeEqualsFieldType, skipArrayCheck);

    public Task<T> AssertType<T>(
        NpgsqlDataSource dataSource,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        Func<T, T, bool>? comparer = null,
        bool valueTypeEqualsFieldType = true,
        bool skipArrayCheck = false)
        => AssertTypeCore(dataSource.OpenConnectionAsync(), disposeConnection: true, valueFactory, sqlLiteral, dataTypeName, dataTypeInference,
            dbType, comparer, valueTypeEqualsFieldType, skipArrayCheck);

    public Task<T> AssertType<T>(
        NpgsqlConnection connection,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        Func<T, T, bool>? comparer = null,
        bool valueTypeEqualsFieldType = true,
        bool skipArrayCheck = false)
        => AssertTypeCore(new(connection), disposeConnection: false, valueFactory, sqlLiteral, dataTypeName, dataTypeInference,
            dbType, comparer, valueTypeEqualsFieldType, skipArrayCheck);

    static async Task<T> AssertTypeCore<T>(
        ValueTask<NpgsqlConnection> connectionTask,
        bool disposeConnection,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        Func<T, T, bool>? comparer = null,
        bool valueTypeEqualsFieldType = true,
        bool skipArrayCheck = false)
    {
        var connection = await connectionTask;
        await using var _ = disposeConnection ? connection : null;

        await AssertTypeWriteCore(new(connection), disposeConnection: false, valueFactory, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);
        return await AssertTypeReadCore(new(connection), disposeConnection: false, sqlLiteral, dataTypeName, valueFactory(),
            valueTypeEqualsFieldType, comparer, skipArrayCheck);
    }

    public Task AssertTypeWrite<T>(
        T value,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(OpenConnectionAsync(), disposeConnection: true, () => value, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        NpgsqlDataSource dataSource,
        T value,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(dataSource.OpenConnectionAsync(), disposeConnection: true, () => value, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        NpgsqlConnection connection,
        T value,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(new(connection), disposeConnection: false, () => value, sqlLiteral, dataTypeName, dataTypeInference,
            dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(OpenConnectionAsync(), disposeConnection: true, valueFactory, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        NpgsqlDataSource dataSource,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(dataSource.OpenConnectionAsync(), disposeConnection: true, valueFactory, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        NpgsqlConnection connection,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        ExpectedDbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(new(connection), disposeConnection: false, valueFactory, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    static async Task AssertTypeWriteCore<T>(
        ValueTask<NpgsqlConnection> connectionTask,
        bool disposeConnection,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference,
        ExpectedDbTypes? dbType,
        bool skipArrayCheck)
    {
        var connection = await connectionTask;
        await using var _ = disposeConnection ? connection : null;

        await AssertTypeWriteCore(
            connection, valueFactory, sqlLiteral,
            dataTypeName, dataTypeInference ?? true,
            dbType);

        // Check the corresponding array type as well
        if (!skipArrayCheck && !dataTypeName.EndsWith("[]", StringComparison.Ordinal))
        {
            await AssertTypeWriteCore(
                connection,
                () => new[] { valueFactory(), valueFactory() },
                ArrayLiteral(sqlLiteral),
                dataTypeName + "[]", dataTypeInference ?? true,
                expectedDbTypes: null);
        }
    }

    public enum DataTypeInferenceKind
    {
        /// <summary>
        /// The exact PostgreSQL data type can be inferred from the CLR type alone.
        /// The CLR type has a mapping to a specific PostgreSQL data type.
        /// </summary>
        Exact,

        /// <summary>
        /// The CLR type maps to a well-known PostgreSQL type (it has a case in NpgsqlDbType) but does not map to the value type.
        /// This is used for negative matching without requiring the exact well-known type to be specified.
        /// </summary>
        WellKnown,

        /// <summary>
        /// The PostgreSQL data type cannot be inferred from the CLR type.
        /// Requires explicit type specification via NpgsqlDbType, DataTypeName, or DbType.
        /// </summary>
        Unknown,
    }

    public readonly struct DataTypeInference(DataTypeInferenceKind kind)
    {
        public DataTypeInferenceKind Kind { get; } = kind;
        public static implicit operator DataTypeInference(bool isDataTypeInferredFromValue)
            => new(isDataTypeInferredFromValue ? DataTypeInferenceKind.Exact : DataTypeInferenceKind.Unknown);
        public static implicit operator DataTypeInference(DataTypeInferenceKind kind)
            => new(kind);
    }

    public readonly struct ExpectedDbTypes(DbType dbType, DbType dataTypeNameDbType, DbType valueInferredDbType)
    {
        public DbType DbType { get; } = dbType;
        public DbType DataTypeNameDbType { get; } = dataTypeNameDbType;
        public DbType ValueInferredDbType { get; } = valueInferredDbType;

        public ExpectedDbTypes(DbType dataTypeNameDbType, DbType valueInferredDbType)
            : this(valueInferredDbType, dataTypeNameDbType, valueInferredDbType) {}

        ExpectedDbTypes(DbType dbType) : this(dbType, dbType, dbType) {}

        public static implicit operator ExpectedDbTypes(DbType dbType) => new(dbType);
    }

    static async Task AssertTypeWriteCore<T>(
        NpgsqlConnection connection,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference dataTypeInference,
        ExpectedDbTypes? expectedDbTypes)
    {
        var npgsqlDbType = DataTypeName.FromDisplayName(dataTypeName).ToNpgsqlDbType();

        // Strip any facet information (length/precision/scale)
        var parenIndex = dataTypeName.IndexOf('(');
        var pgTypeNameWithoutFacets = parenIndex > -1
            ? dataTypeName[..parenIndex] + dataTypeName[(dataTypeName.IndexOf(')') + 1)..]
            : dataTypeName;

        // For composite type with dots in name, Postgresql returns name with quotes - scheme."My.type.name"
        // but for npgsql mapping we should use names without quotes - scheme.My.type.name
        var pgTypeNameWithoutFacetsAndQuotes = pgTypeNameWithoutFacets.Replace("\"", string.Empty);

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

        // With data type name
        p = new NpgsqlParameter { Value = valueFactory(), DataTypeName = pgTypeNameWithoutFacetsAndQuotes };
        errorIdentifier[++errorIdentifierIndex] = $"DataTypeName={pgTypeNameWithoutFacetsAndQuotes}";
        CheckParameter();
        cmd.Parameters.Add(p);

        // With NpgsqlDbType
        if (npgsqlDbType is not null)
        {
            p = new NpgsqlParameter { Value = valueFactory(), NpgsqlDbType = npgsqlDbType.Value };
            errorIdentifier[++errorIdentifierIndex] = $"NpgsqlDbType={npgsqlDbType}";
            CheckParameter();
            cmd.Parameters.Add(p);
        }

        // With DbType
        if (expectedDbTypes?.DbType is { } dbType)
        {
            p = new NpgsqlParameter { Value = valueFactory(), DbType = dbType };
            errorIdentifier[++errorIdentifierIndex] = $"DbType={dbType}";
            CheckParameter(dbTypeApplied: true);
            if (dataTypeInference.Kind is DataTypeInferenceKind.Exact)
                cmd.Parameters.Add(p);
        }

        // With (non-generic) value only
        p = new NpgsqlParameter { Value = valueFactory() };
        errorIdentifier[++errorIdentifierIndex] = $"Value only (type {p.Value!.GetType().Name}, non-generic)";
        CheckParameter(valueSolelyApplied: true);
        if (dataTypeInference.Kind is DataTypeInferenceKind.Exact)
            cmd.Parameters.Add(p);

        // With (generic) value only
        p = new NpgsqlParameter<T> { TypedValue = valueFactory() };
        errorIdentifier[++errorIdentifierIndex] = $"Value only (type {p.Value!.GetType().Name}, generic)";
        CheckParameter(valueSolelyApplied: true);
        if (dataTypeInference.Kind is DataTypeInferenceKind.Exact)
            cmd.Parameters.Add(p);

        cmd.CommandText = "SELECT " + string.Join(", ", Enumerable.Range(1, cmd.Parameters.Count).Select(i =>
            "pg_typeof($1)::text, $1::text".Replace("$1", $"${i}")));

        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        await reader.ReadAsync();

        for (var i = 0; i < cmd.Parameters.Count * 2; i += 2)
        {
            var error = errorIdentifier[i / 2];
            Assert.That(reader[i], Is.EqualTo(pgTypeNameWithoutFacets), $"Got wrong PG type name when writing with {error}");
            Assert.That(reader[i+1], Is.EqualTo(sqlLiteral), $"Got wrong SQL literal when writing with {error}");
        }

        void CheckParameter(bool dbTypeApplied = false, bool valueSolelyApplied = false)
        {
            var expectedDataTypeName = pgTypeNameWithoutFacetsAndQuotes;
            var expectedNpgsqlDbType = npgsqlDbType ?? NpgsqlDbType.Unknown;
            if (valueSolelyApplied)
            {
                switch (dataTypeInference.Kind)
                {
                    // Only respect WellKnown if the type is actually well known, otherwise use the existing values so we'll error properly.
                    case DataTypeInferenceKind.WellKnown when p.NpgsqlDbType is not NpgsqlDbType.Unknown:
                        expectedDataTypeName = p.DataTypeName;
                        expectedNpgsqlDbType = p.NpgsqlDbType;
                        break;
                    case DataTypeInferenceKind.Unknown:
                        expectedDataTypeName = null;
                        expectedNpgsqlDbType = NpgsqlDbType.Unknown;
                        break;
                }
            }

            if (!dbTypeApplied || dataTypeInference.Kind is DataTypeInferenceKind.Exact)
            {
                Assert.That(p.DataTypeName, Is.EqualTo(expectedDataTypeName),
                    () => $"Got wrong inferred DataTypeName when inferring with {errorIdentifier[errorIdentifierIndex]}");
                Assert.That(p.NpgsqlDbType, Is.EqualTo(expectedNpgsqlDbType),
                    () => $"Got wrong inferred NpgsqlDbType when inferring with {errorIdentifier[errorIdentifierIndex]}");
            }

            DbType? dbType;
            var actualDbType = p.DbType;
            if (dbTypeApplied)
                dbType = expectedDbTypes?.DbType;
            else if (valueSolelyApplied)
                dbType = expectedDbTypes?.ValueInferredDbType ?? DbType.Object;
            else if (dataTypeInference.Kind is DataTypeInferenceKind.Exact || actualDbType != DbType.Object)
                dbType = expectedDbTypes?.DataTypeNameDbType ?? DbType.Object;
            // If data type is not inferrable from the value and the actual db type is object we'll skip the DbType check.
            // This allows callers to pass DbType through implicit conversion instead of requiring new ExpectedDbType(DbType.Object, #DbType#)
            else
                dbType = null;

            if (dbType is not null)
                Assert.That(actualDbType, Is.EqualTo(dbType),
                    () => $"Got wrong inferred DbType when inferring with {errorIdentifier[errorIdentifierIndex]}");
        }
    }

    public Task<T> AssertTypeRead<T>(string sqlLiteral, string dataTypeName, T expected,
        bool valueTypeEqualsFieldType = true, Func<T, T, bool>? comparer = null, bool skipArrayCheck = false)
        => AssertTypeReadCore(OpenConnectionAsync(), disposeConnection: true, sqlLiteral, dataTypeName,
            expected, valueTypeEqualsFieldType, comparer, skipArrayCheck);

    public Task<T> AssertTypeRead<T>(NpgsqlDataSource dataSource, string sqlLiteral, string dataTypeName, T expected,
        bool valueTypeEqualsFieldType = true, Func<T, T, bool>? comparer = null, bool skipArrayCheck = false)
        => AssertTypeReadCore(dataSource.OpenConnectionAsync(), disposeConnection: true, sqlLiteral, dataTypeName,
            expected, valueTypeEqualsFieldType, comparer, skipArrayCheck);

    public Task<T> AssertTypeRead<T>(NpgsqlConnection connection, string sqlLiteral, string dataTypeName, T expected,
        bool valueTypeEqualsFieldType = true, Func<T, T, bool>? comparer = null, bool skipArrayCheck = false)
        => AssertTypeReadCore(new(connection), disposeConnection: false, sqlLiteral, dataTypeName,
            expected, valueTypeEqualsFieldType, comparer, skipArrayCheck);

    static async Task<T> AssertTypeReadCore<T>(
        ValueTask<NpgsqlConnection> connectionTask,
        bool disposeConnection,
        string sqlLiteral,
        string dataTypeName,
        T expected,
        bool valueTypeEqualsFieldType,
        Func<T, T, bool>? comparer,
        bool skipArrayCheck)
    {
        var connection = await connectionTask;
        await using var _ = disposeConnection ? connection : null;

        var result = await AssertTypeReadCore(connection, sqlLiteral, dataTypeName, expected, valueTypeEqualsFieldType, comparer);

        // Check the corresponding array type as well
        if (!skipArrayCheck && !dataTypeName.EndsWith("[]", StringComparison.Ordinal))
        {
            await AssertTypeReadCore(
                connection,
                ArrayLiteral(sqlLiteral),
                dataTypeName + "[]",
                new[] { expected, expected },
                valueTypeEqualsFieldType,
                comparer is null ? null : (array1, array2) => comparer(array1[0], array2[0]) && comparer(array1[1], array2[1]));
        }
        return result;
    }

    static async Task<T> AssertTypeReadCore<T>(
        NpgsqlConnection connection,
        string sqlLiteral,
        string dataTypeName,
        T expected,
        bool valueTypeEqualsFieldType,
        Func<T, T, bool>? comparer)
    {
        if (sqlLiteral.Contains('\''))
            sqlLiteral = sqlLiteral.Replace("'", "''");

        await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{dataTypeName}", connection);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        var truncatedSqlLiteral = sqlLiteral.Length > 40 ? sqlLiteral[..40] + "..." : sqlLiteral;

        var readerTypeName = reader.GetDataTypeName(0);
        var dotIndex = readerTypeName.IndexOf('.');
        if (dotIndex > -1 && readerTypeName.Substring(0, dotIndex) is "pg_catalog" or "public")
            readerTypeName = readerTypeName.Substring(dotIndex + 1);

        // For composite type with dots, postgres works only with quoted name - scheme."My.type.name"
        // but npgsql converts it to name without quotes
        var pgTypeNameWithoutQuotes = readerTypeName.Replace("\"", string.Empty);
        Assert.That(readerTypeName, Is.EqualTo(pgTypeNameWithoutQuotes),
            $"Got wrong result from GetDataTypeName when reading '{truncatedSqlLiteral}'");

        // For arrays, GetFieldType always returns typeof(Array), since PG arrays can have arbitrary dimensionality.
        var isArray = readerTypeName.EndsWith("[]");
        Assert.That(reader.GetFieldType(0),
            (valueTypeEqualsFieldType || isArray ? new ConstraintExpression() : Is.Not)
                .EqualTo(isArray ? typeof(Array) : typeof(T)),
            $"Got wrong result from GetFieldType when reading '{truncatedSqlLiteral}'");

        T actual;
        if (valueTypeEqualsFieldType)
        {
            actual = (T)reader.GetValue(0);
            Assert.That(actual, comparer is null ? Is.EqualTo(expected) : Is.EqualTo(expected).Using(new SimpleComparer<T>(comparer)),
                $"Got wrong result from GetValue() value when reading '{truncatedSqlLiteral}'");

            actual = (T)reader.GetFieldValue<object>(0);
            Assert.That(actual, comparer is null ? Is.EqualTo(expected) : Is.EqualTo(expected).Using(new SimpleComparer<T>(comparer)),
                $"Got wrong result from GetFieldValue<object>() value when reading '{truncatedSqlLiteral}'");

            return actual;
        }

        actual = reader.GetFieldValue<T>(0);

        Assert.That(actual, comparer is null ? Is.EqualTo(expected) : Is.EqualTo(expected).Using(new SimpleComparer<T>(comparer)),
            $"Got wrong result from GetFieldValue<T>() value when reading '{truncatedSqlLiteral}'");

        return actual;
    }

    public async Task AssertTypeUnsupported<T>(T value, string sqlLiteral, string dataTypeName, NpgsqlDataSource? dataSource = null)
    {
        await AssertTypeUnsupportedRead<T>(sqlLiteral, dataTypeName, dataSource);
        await AssertTypeUnsupportedWrite(value, dataTypeName, dataSource);
    }

    public async Task<InvalidCastException> AssertTypeUnsupportedRead(string sqlLiteral, string dataTypeName, NpgsqlDataSource? dataSource = null)
    {
        dataSource ??= DataSource;

        await using var conn = await dataSource.OpenConnectionAsync();
        // Make sure we don't poison the connection with a fault, potentially terminating other perfectly passing tests as well.
        await using var tx = dataSource.Settings.Multiplexing ? await conn.BeginTransactionAsync() : null;
        await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{dataTypeName}", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return Assert.Throws<InvalidCastException>(() => reader.GetValue(0))!;
    }

    public Task<InvalidCastException> AssertTypeUnsupportedRead<T>(string sqlLiteral, string dataTypeName,
        NpgsqlDataSource? dataSource = null, bool skipArrayCheck = false)
        => AssertTypeUnsupportedRead<T, InvalidCastException>(sqlLiteral, dataTypeName, dataSource);

    public async Task<TException> AssertTypeUnsupportedRead<T, TException>(string sqlLiteral, string dataTypeName,
        NpgsqlDataSource? dataSource = null, bool skipArrayCheck = false)
        where TException : Exception
    {
        var result = await AssertTypeUnsupportedReadCore<T, TException>(sqlLiteral, dataTypeName, dataSource);

        // Check the corresponding array type as well
        if (!skipArrayCheck && !dataTypeName.EndsWith("[]", StringComparison.Ordinal))
        {
            await AssertTypeUnsupportedReadCore<T[], TException>(ArrayLiteral(sqlLiteral), dataTypeName + "[]", dataSource);
        }

        return result;
    }

    async Task<TException> AssertTypeUnsupportedReadCore<T, TException>(string sqlLiteral, string dataTypeName, NpgsqlDataSource? dataSource = null)
        where TException : Exception
    {
        dataSource ??= DataSource;

        await using var conn = await dataSource.OpenConnectionAsync();
        // Make sure we don't poison the connection with a fault, potentially terminating other perfectly passing tests as well.
        await using var tx = dataSource.Settings.Multiplexing ? await conn.BeginTransactionAsync() : null;
        await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{dataTypeName}", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return Assert.Throws<TException>(() => reader.GetFieldValue<T>(0))!;
    }

    public Task<InvalidCastException> AssertTypeUnsupportedWrite<T>(T value, string? dataTypeName = null, NpgsqlDataSource? dataSource = null,
        bool skipArrayCheck = false)
        => AssertTypeUnsupportedWrite<T, InvalidCastException>(value, dataTypeName, dataSource, skipArrayCheck: false);

    public async Task<TException> AssertTypeUnsupportedWrite<T, TException>(T value, string? dataTypeName = null,
        NpgsqlDataSource? dataSource = null, bool skipArrayCheck = false)
        where TException : Exception
    {
        var result = await AssertTypeUnsupportedWriteCore<T, TException>(value, dataTypeName, dataSource);

        // Check the corresponding array type as well
        if (!skipArrayCheck && !dataTypeName?.EndsWith("[]", StringComparison.Ordinal) == true)
        {
            await AssertTypeUnsupportedWriteCore<T[], TException>([value, value], dataTypeName + "[]", dataSource);
        }

        return result;
    }

    async Task<TException> AssertTypeUnsupportedWriteCore<T, TException>(T value, string? dataTypeName = null, NpgsqlDataSource? dataSource = null)
        where TException : Exception
    {
        dataSource ??= DataSource;

        await using var conn = await dataSource.OpenConnectionAsync();
        // Make sure we don't poison the connection with a fault, potentially terminating other perfectly passing tests as well.
        await using var tx = dataSource.Settings.Multiplexing ? await conn.BeginTransactionAsync() : null;
        await using var cmd = new NpgsqlCommand("SELECT $1", conn)
        {
            Parameters = { new() { Value = value } }
        };

        if (dataTypeName is not null)
            cmd.Parameters[0].DataTypeName = dataTypeName;

        return Assert.ThrowsAsync<TException>(() => cmd.ExecuteReaderAsync())!;
    }

    class SimpleComparer<T>(Func<T, T, bool> comparerDelegate) : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y)
            => x is null
                ? y is null
                : y is not null && comparerDelegate(x, y);

        public int GetHashCode(T obj) => throw new NotSupportedException();
    }

    // For array quoting rules, see array_out in https://github.com/postgres/postgres/blob/master/src/backend/utils/adt/arrayfuncs.c
    static string ArrayLiteral(string elementLiteral)
    {
        switch (elementLiteral)
        {
        case "":
            elementLiteral = "\"\"";
            break;
        case "NULL":
            elementLiteral = "\"NULL\"";
            break;
        default:
            // Escape quotes and backslashes, quote for special chars
            elementLiteral = elementLiteral.Replace("\\", "\\\\").Replace("\"", "\\\"");
            if (elementLiteral.Any(c => c is '{' or '}' or ',' or '"' or '\\' || char.IsWhiteSpace(c)))
            {
                elementLiteral = '"' + elementLiteral + '"';
            }

            break;
        }

        return $"{{{elementLiteral},{elementLiteral}}}";
    }

    #endregion Type testing

    #region Utilities for use by tests

    protected virtual NpgsqlDataSourceBuilder CreateDataSourceBuilder()
        => new(ConnectionString);

    protected virtual NpgsqlDataSource CreateDataSource()
        => CreateDataSource(ConnectionString);

    protected NpgsqlDataSource CreateDataSource(string connectionString)
        => NpgsqlDataSource.Create(connectionString);

    protected NpgsqlDataSource CreateDataSource(Action<NpgsqlConnectionStringBuilder> connectionStringBuilderAction)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString);
        connectionStringBuilderAction(connectionStringBuilder);
        return NpgsqlDataSource.Create(connectionStringBuilder);
    }

    protected NpgsqlDataSource CreateDataSource(Action<NpgsqlDataSourceBuilder> configure)
    {
        var builder = new NpgsqlDataSourceBuilder(ConnectionString);
        configure(builder);
        return builder.Build();
    }

    protected static NpgsqlDataSource GetDataSource(string connectionString)
    {
        if (!DataSources.TryGetValue(connectionString, out var dataSource))
        {
            lock (dataSourceLockObject)
            {
                if (!DataSources.TryGetValue(connectionString, out dataSource))
                {
                    var canonicalConnectionString = new NpgsqlConnectionStringBuilder(connectionString).ToString();
                    if (!DataSources.TryGetValue(canonicalConnectionString, out dataSource))
                    {
                        DataSources[canonicalConnectionString] = dataSource = NpgsqlDataSource.Create(connectionString);
                    }
                    DataSources[connectionString] = dataSource;
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

    protected virtual NpgsqlDataSource DataSource => DefaultDataSource;

    protected virtual NpgsqlConnection CreateConnection()
        => DataSource.CreateConnection();

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
