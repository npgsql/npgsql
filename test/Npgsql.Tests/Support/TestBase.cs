using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
        DbTypes? dbType = null,
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
        DbTypes? dbType = null,
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
        DbTypes? dbType = null,
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
        DbTypes? dbType = null,
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
        DbTypes? dbType = null,
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
        DbTypes? dbType = null,
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
        DbTypes? dbType = null,
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
        DbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(OpenConnectionAsync(), disposeConnection: true, () => value, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        NpgsqlDataSource dataSource,
        T value,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        DbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(dataSource.OpenConnectionAsync(), disposeConnection: true, () => value, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        NpgsqlConnection connection,
        T value,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        DbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(new(connection), disposeConnection: false, () => value, sqlLiteral, dataTypeName, dataTypeInference,
            dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        DbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(OpenConnectionAsync(), disposeConnection: true, valueFactory, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        NpgsqlDataSource dataSource,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        DbTypes? dbType = null,
        bool skipArrayCheck = false)
        => AssertTypeWriteCore(dataSource.OpenConnectionAsync(), disposeConnection: true, valueFactory, sqlLiteral,
            dataTypeName, dataTypeInference, dbType, skipArrayCheck);

    public Task AssertTypeWrite<T>(
        NpgsqlConnection connection,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference? dataTypeInference = null,
        DbTypes? dbType = null,
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
        DbTypes? dbType,
        bool skipArrayCheck)
    {
        var connection = await connectionTask;
        await using var _ = disposeConnection ? connection : null;

        await AssertTypeWriteCore(
            connection, valueFactory, sqlLiteral,
            dataTypeName, dataTypeInference ?? DataTypeInference.Match,
            dbType);

        // Check the corresponding array type as well
        if (!skipArrayCheck && !dataTypeName.EndsWith("[]", StringComparison.Ordinal))
        {
            await AssertTypeWriteCore(
                connection,
                () => new[] { valueFactory(), valueFactory() },
                ArrayLiteral(sqlLiteral),
                dataTypeName + "[]", dataTypeInference ?? DataTypeInference.Match,
                expectedDbTypes: null);
        }
    }

    public enum DataTypeInference
    {
        /// <summary>
        /// Data type is inferred from the CLR value and matches the data type under test.
        /// </summary>
        Match,

        /// <summary>
        /// Data type is inferred from the CLR value but differs from the data type under test.
        /// </summary>
        /// <remarks>
        /// Used when we get some inferred data type (e.g. CLR strings are inferred to be 'text') but this does not match the data type (e.g. 'json') under test.
        /// </remarks>
        Mismatch,

        /// <summary>
        /// Data type can not be inferred from the CLR value.
        /// </summary>
        /// <remarks>
        /// This is for CLR types that are statically unknown to Npgsql (plugin types: NodaTime/NTS, composite types, enums...),
        /// or where we specifically don't want to infer a data type because there's no good option
        /// (e.g. uint can be mapped to 'oid/xid/cid', but we don't want any of these as a default/inferred data type)
        /// </remarks>
        Nothing,
    }

    public readonly struct DbTypes(DbType dataTypeMappedDbType, DbType valueInferredDbType, DbType dbTypeToSet)
    {
        public DbType DataTypeMappedDbType { get; } = dataTypeMappedDbType;
        public DbType ValueInferredDbType { get; } = valueInferredDbType;

        // The DbType to explicitly set on the parameter. Usually same as ValueInferredDbType,
        // It differs when testing DbType aliases (e.g. VarNumeric → DbType.Decimal) as we want to test those also work correctly.
        public DbType DbTypeToSet { get; } = dbTypeToSet;

        public DbTypes(DbType dataTypeMappedDbType, DbType valueInferredDbType)
            : this(dataTypeMappedDbType, valueInferredDbType, valueInferredDbType) {}

        public static implicit operator DbTypes(DbType dbType) => new(dbType, dbType, dbType);
    }

    static async Task AssertTypeWriteCore<T>(
        NpgsqlConnection connection,
        Func<T> valueFactory,
        string sqlLiteral,
        string dataTypeName,
        DataTypeInference dataTypeInference,
        DbTypes? expectedDbTypes)
    {
        var npgsqlDbType = DataTypeName.FromDisplayName(dataTypeName).ToNpgsqlDbType();

        // Strip any facet information (length/precision/scale)
        var parenIndex = dataTypeName.IndexOf('(');
        var dataTypeNameWithoutFacets = parenIndex > -1
            ? dataTypeName[..parenIndex] + dataTypeName[(dataTypeName.IndexOf(')') + 1)..]
            : dataTypeName;

        // For composite type with dots in name, Postgresql returns name with quotes - scheme."My.type.name"
        // but for npgsql mapping we should use names without quotes - scheme.My.type.name
        var dataTypeNameWithoutFacetsAndQuotes = dataTypeNameWithoutFacets.Replace("\"", string.Empty);

        // We test the following scenarios (between 2 and 5 in total):
        // 1. With value and DataTypeName set
        // 2. With value and NpgsqlDbType set (when available)
        // 3. With value and DbType explicitly set
        // 4. With only the value set
        // 5. With only the value set, using generic NpgsqlParameter<T>

        // We only actually attempt to write to the database with a set DataTypeName, NpgsqlDbType, or when data type inference is exact.

        var errorIdentifierIndex = -1;
        var errorIdentifier = new Dictionary<int, string>();

        await using var cmd = new NpgsqlCommand { Connection = connection };
        NpgsqlParameter p;

        // With data type name
        p = new NpgsqlParameter { Value = valueFactory(), DataTypeName = dataTypeNameWithoutFacetsAndQuotes };
        errorIdentifier[++errorIdentifierIndex] = $"Value and DataTypeName={dataTypeNameWithoutFacetsAndQuotes}";
        DataTypeAsserts();
        cmd.Parameters.Add(p);

        // With NpgsqlDbType
        if (npgsqlDbType is not null)
        {
            p = new NpgsqlParameter { Value = valueFactory(), NpgsqlDbType = npgsqlDbType.Value };
            errorIdentifier[++errorIdentifierIndex] = $"Value and NpgsqlDbType={npgsqlDbType}";
            DataTypeAsserts();
            cmd.Parameters.Add(p);
        }

        // With DbType, if none was supplied we verify it's DbType.Object.
        p = new NpgsqlParameter { Value = valueFactory() };
        errorIdentifier[++errorIdentifierIndex] = $"Value and DbType={expectedDbTypes?.DbTypeToSet}";
        if (expectedDbTypes?.DbTypeToSet is { } expectedDbType)
            p.DbType = expectedDbType;
        DbTypeAsserts();
        if (dataTypeInference is DataTypeInference.Match)
            cmd.Parameters.Add(p);

        // With (non-generic) value only
        p = new NpgsqlParameter { Value = valueFactory() };
        errorIdentifier[++errorIdentifierIndex] = $"Value (type {p.Value!.GetType().Name}, non-generic)";
        ValueAsserts();
        if (dataTypeInference is DataTypeInference.Match)
            cmd.Parameters.Add(p);

        // With (generic) value only
        p = new NpgsqlParameter<T> { TypedValue = valueFactory() };
        errorIdentifier[++errorIdentifierIndex] = $"Value (type {p.Value!.GetType().Name}, generic)";
        ValueAsserts();
        if (dataTypeInference is DataTypeInference.Match)
            cmd.Parameters.Add(p);

        cmd.CommandText = "SELECT " + string.Join(", ", Enumerable.Range(1, cmd.Parameters.Count).Select(i =>
            "pg_typeof($1)::text, $1::text".Replace("$1", $"${i}")));

        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        await reader.ReadAsync();

        for (var i = 0; i < cmd.Parameters.Count * 2; i += 2)
        {
            var error = errorIdentifier[i / 2];
            Assert.That(reader[i], Is.EqualTo(dataTypeNameWithoutFacets), $"Got wrong data type name when writing with {error}");
            Assert.That(reader[i+1], Is.EqualTo(sqlLiteral), $"Got wrong SQL literal when writing with {error}");
        }

        void DataTypeAsserts()
        {
            var expectedDataTypeName = dataTypeNameWithoutFacetsAndQuotes;
            var expectedNpgsqlDbType = npgsqlDbType ?? NpgsqlDbType.Unknown;

            var expectedDbType = expectedDbTypes?.DataTypeMappedDbType ?? DbType.Object;

            AssertParameterProperties(expectedDataTypeName, expectedNpgsqlDbType, expectedDbType);
        }

        void DbTypeAsserts()
        {
            // If DbType was set it overrules any value based data type inference.
            // As DbType.Object never has any mapping either we check for null/Unknown when DbType.Object was set.
            var (expectedDataTypeName, expectedNpgsqlDbType) =
                expectedDbTypes is { DbTypeToSet: DbType.Object }
                    ? (null, NpgsqlDbType.Unknown)
                    : GetInferredDataType();

            var expectedDbType = expectedDbTypes?.DbTypeToSet ?? DbType.Object;

            AssertParameterProperties(expectedDataTypeName, expectedNpgsqlDbType, expectedDbType);
        }

        void ValueAsserts()
        {
            var (expectedDataTypeName, expectedNpgsqlDbType) = GetInferredDataType();

            var expectedDbType = expectedDbTypes?.ValueInferredDbType ?? DbType.Object;

            AssertParameterProperties(expectedDataTypeName, expectedNpgsqlDbType, expectedDbType);
        }

        void AssertParameterProperties(string? expectedDataTypeName, NpgsqlDbType expectedNpgsqlDbType, DbType expectedDbType)
        {
            Assert.That(p.DataTypeName, Is.EqualTo(expectedDataTypeName),
                $"Got wrong DataTypeName when checking with {errorIdentifier[errorIdentifierIndex]}");
            Assert.That(p.NpgsqlDbType, Is.EqualTo(expectedNpgsqlDbType),
                $"Got wrong NpgsqlDbType when checking with {errorIdentifier[errorIdentifierIndex]}");
            Assert.That(p.DbType, Is.EqualTo(expectedDbType),
                $"Got wrong DbType when checking with {errorIdentifier[errorIdentifierIndex]}");
        }

        (string? ExpectedDataTypeName, NpgsqlDbType ExpectedNpgsqlDbType) GetInferredDataType()
            => dataTypeInference switch
            {
                DataTypeInference.Match =>
                    (dataTypeNameWithoutFacetsAndQuotes, npgsqlDbType ?? NpgsqlDbType.Unknown),
                DataTypeInference.Mismatch =>
                    // Only respect Mismatch if the type is well known (for now that means it has an NpgsqlDbType).
                    // Otherwise use the exact values so we'll error with the right details.
                    p.NpgsqlDbType is not NpgsqlDbType.Unknown
                        ? (p.DataTypeName, p.NpgsqlDbType)
                        : (dataTypeNameWithoutFacetsAndQuotes, npgsqlDbType ?? NpgsqlDbType.Unknown),
                DataTypeInference.Nothing =>
                    (null, NpgsqlDbType.Unknown),
                _ => throw new UnreachableException($"Unknown case {dataTypeInference}")
            };
    }

    public Task<T> AssertTypeRead<T>(string sqlLiteral, string dataTypeName, T value,
        bool valueTypeEqualsFieldType = true, Func<T, T, bool>? comparer = null, bool skipArrayCheck = false)
        => AssertTypeReadCore(OpenConnectionAsync(), disposeConnection: true, sqlLiteral, dataTypeName,
            value, valueTypeEqualsFieldType, comparer, skipArrayCheck);

    public Task<T> AssertTypeRead<T>(NpgsqlDataSource dataSource, string sqlLiteral, string dataTypeName, T value,
        bool valueTypeEqualsFieldType = true, Func<T, T, bool>? comparer = null, bool skipArrayCheck = false)
        => AssertTypeReadCore(dataSource.OpenConnectionAsync(), disposeConnection: true, sqlLiteral, dataTypeName,
            value, valueTypeEqualsFieldType, comparer, skipArrayCheck);

    public Task<T> AssertTypeRead<T>(NpgsqlConnection connection, string sqlLiteral, string dataTypeName, T value,
        bool valueTypeEqualsFieldType = true, Func<T, T, bool>? comparer = null, bool skipArrayCheck = false)
        => AssertTypeReadCore(new(connection), disposeConnection: false, sqlLiteral, dataTypeName,
            value, valueTypeEqualsFieldType, comparer, skipArrayCheck);

    static async Task<T> AssertTypeReadCore<T>(
        ValueTask<NpgsqlConnection> connectionTask,
        bool disposeConnection,
        string sqlLiteral,
        string dataTypeName,
        T value,
        bool valueTypeEqualsFieldType,
        Func<T, T, bool>? comparer,
        bool skipArrayCheck)
    {
        var connection = await connectionTask;
        await using var _ = disposeConnection ? connection : null;

        var result = await AssertTypeReadCore(connection, sqlLiteral, dataTypeName, value, valueTypeEqualsFieldType, comparer);

        // Check the corresponding array type as well
        if (!skipArrayCheck && !dataTypeName.EndsWith("[]", StringComparison.Ordinal))
        {
            await AssertTypeReadCore(
                connection,
                ArrayLiteral(sqlLiteral),
                dataTypeName + "[]",
                new[] { value, value },
                valueTypeEqualsFieldType,
                comparer is null ? null : (array1, array2) => array1.SequenceEqual(array2, CreateEqualityComparer(comparer!)));
        }
        return result;
    }

    static async Task<T> AssertTypeReadCore<T>(
        NpgsqlConnection connection,
        string sqlLiteral,
        string dataTypeName,
        T value,
        bool valueTypeEqualsFieldType,
        Func<T, T, bool>? comparer)
    {
        if (sqlLiteral.Contains('\''))
            sqlLiteral = sqlLiteral.Replace("'", "''");

        await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{dataTypeName}", connection);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        var truncatedSqlLiteral = sqlLiteral.Length > 40 ? sqlLiteral[..40] + "..." : sqlLiteral;

        var actualDataTypeName = reader.GetDataTypeName(0);
        var dotIndex = actualDataTypeName.IndexOf('.');
        if (dotIndex > -1 && actualDataTypeName.Substring(0, dotIndex) is "pg_catalog" or "public")
            actualDataTypeName = actualDataTypeName.Substring(dotIndex + 1);

        // For composite type with dots, postgres works only with quoted name - scheme."My.type.name"
        // but npgsql converts it to name without quotes
        var dataTypeNameWithoutQuotes = dataTypeName.Replace("\"", string.Empty);
        Assert.That(actualDataTypeName, Is.EqualTo(dataTypeNameWithoutQuotes),
            $"Got wrong result from GetDataTypeName when reading '{truncatedSqlLiteral}'");

        // For arrays, GetFieldType always returns typeof(Array), since PG arrays can have arbitrary dimensionality.
        var isArrayTest = actualDataTypeName.EndsWith("[]", StringComparison.Ordinal) && typeof(T).IsArray;
        Assert.That(reader.GetFieldType(0),
            (valueTypeEqualsFieldType || isArrayTest ? new ConstraintExpression() : Is.Not)
                .EqualTo(isArrayTest ? typeof(Array) : typeof(T)),
            $"Got wrong result from GetFieldType when reading '{truncatedSqlLiteral}'");

        T actual;
        if (valueTypeEqualsFieldType)
        {
            actual = (T)reader.GetValue(0);
            Assert.That(actual, comparer is null ? Is.EqualTo(value) : Is.EqualTo(value).Using<T>(CreateEqualityComparer(comparer!)),
                $"Got wrong result from GetValue() value when reading '{truncatedSqlLiteral}'");

            actual = (T)reader.GetFieldValue<object>(0);
            Assert.That(actual, comparer is null ? Is.EqualTo(value) : Is.EqualTo(value).Using<T>(CreateEqualityComparer(comparer)),
                $"Got wrong result from GetFieldValue<object>() value when reading '{truncatedSqlLiteral}'");

            return actual;
        }

        actual = reader.GetFieldValue<T>(0);

        Assert.That(actual, comparer is null ? Is.EqualTo(value) : Is.EqualTo(value).Using<T>(CreateEqualityComparer(comparer!)),
            $"Got wrong result from GetFieldValue<T>() value when reading '{truncatedSqlLiteral}'");

        return actual;
    }

    static EqualityComparer<T> CreateEqualityComparer<T>(Func<T, T, bool> comparer)
        => EqualityComparer<T>.Create((x, y) =>
        {
            if (x is null && y is null)
                return true;
            if (x is null || y is null)
                return false;
            return comparer(x, y);
        });

    public async Task AssertTypeUnsupported<T>(T value, string sqlLiteral, string dataTypeName, NpgsqlDataSource? dataSource = null, bool skipArrayCheck = false)
    {
        await AssertTypeUnsupportedRead<T>(sqlLiteral, dataTypeName, dataSource, skipArrayCheck);
        await AssertTypeUnsupportedWrite(value, dataTypeName, dataSource, skipArrayCheck);
    }

    public Task<InvalidCastException> AssertTypeUnsupportedRead<T>(string sqlLiteral, string dataTypeName,
        NpgsqlDataSource? dataSource = null, bool skipArrayCheck = false)
        => AssertTypeUnsupportedRead<T, InvalidCastException>(sqlLiteral, dataTypeName, dataSource, skipArrayCheck);

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

        return Assert.Throws<TException>(() =>
        {
            _ = typeof(T) == typeof(object) ? reader.GetValue(0) : reader.GetFieldValue<T>(0);
        })!;
    }

    public Task<InvalidCastException> AssertTypeUnsupportedWrite<T>(T value, string? dataTypeName = null, NpgsqlDataSource? dataSource = null,
        bool skipArrayCheck = false)
        => AssertTypeUnsupportedWrite<T, InvalidCastException>(value, dataTypeName, dataSource, skipArrayCheck);

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
