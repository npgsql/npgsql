using System;
using BenchmarkDotNet.Attributes;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Benchmarks;

// Mirrors NpgsqlBinaryImporter's per-row inner loop on a single column —
// TypedValue=, ResolveTypeInfo, Bind, Write — against a no-flush in-memory PgWriter so the substrate
// bind differences aren't swamped by network roundtrip cost. Uses internal APIs (visible to this assembly).
[OperationsPerSecond, PerElementMean]
public class WriteParameter
{
    readonly NpgsqlConnection _conn;
    readonly NpgsqlConnector _connector;
    readonly PgWriter _pgWriter;

    // DataTypeName pinned on all parameters so the resolver gets a decided pgTypeId — DateTime[] needs
    // this to exercise the pgTypeIdClassified erasure (without it the provider stays alive and every
    // iteration runs the per-value provider dispatch instead of the cached-default fast path).
    readonly NpgsqlParameter<int?> _nullableParam = new() { DataTypeName = DataTypeNames.Int4 };
    readonly NpgsqlParameter<int> _intParam = new() { DataTypeName = DataTypeNames.Int4 };
    readonly NpgsqlParameter<string> _stringParam = new() { DataTypeName = DataTypeNames.Text };
    readonly NpgsqlParameter<object> _objectParam = new() { DataTypeName = DataTypeNames.Text };
    readonly NpgsqlParameter<int[]> _intArrayParam = new() { DataTypeName = DataTypeNames.Int4.ToArrayName() };
    readonly NpgsqlParameter<DateTime[]> _dateTimeArrayParam = new() { DataTypeName = DataTypeNames.Timestamp.ToArrayName() };
    readonly NpgsqlParameter<string[]> _stringArrayParam = new() { DataTypeName = DataTypeNames.Text.ToArrayName() };

    // Undecided variants: no DataTypeName pinned. Resolution looks up by CLR type alone, returning a
    // PgProviderTypeInfo when the registration is provider-backed (DateTime[]), or a PgConcreteTypeInfo
    // for non-provider types (int[]). Each iteration's MakeConcreteForValue then either runs the
    // provider's per-value dispatch or just returns the cached concrete.
    readonly NpgsqlParameter<int[]> _intArrayUndecidedParam = new();
    readonly NpgsqlParameter<DateTime[]> _dateTimeArrayUndecidedParam = new();

    const int ArrayLength = 100;
    static readonly int[] s_intArray = MakeIntArray();
    static readonly DateTime[] s_dateTimeArray = MakeDateTimeArray();
    static readonly string[] s_stringArray = MakeStringArray();

    static int[] MakeIntArray()
    {
        var a = new int[ArrayLength];
        for (var i = 0; i < ArrayLength; i++) a[i] = i;
        return a;
    }

    static DateTime[] MakeDateTimeArray()
    {
        var a = new DateTime[ArrayLength];
        var d = new DateTime(2026, 1, 1);
        for (var i = 0; i < ArrayLength; i++) a[i] = d.AddDays(i);
        return a;
    }

    static string[] MakeStringArray()
    {
        var a = new string[ArrayLength];
        for (var i = 0; i < ArrayLength; i++) a[i] = $"some realistic text value #{i:0000}";
        return a;
    }

    public WriteParameter()
    {
        _conn = BenchmarkEnvironment.OpenConnection();
        _connector = _conn.Connector!;
        _pgWriter = _connector.WriteBuffer.GetWriter(_connector.DatabaseInfo, FlushMode.None);
    }

    [GlobalCleanup]
    public void Cleanup() => _conn.Dispose();

    [Benchmark]
    public void NullableField() => Write(_nullableParam, (int?)0);

    [Benchmark]
    public void ValueTypeField() => Write(_intParam, 0);

    [Benchmark]
    public void ReferenceTypeField() => Write(_stringParam, "str");

    [Benchmark]
    public void ObjectField() => Write(_objectParam, "str");

    // Array element modes: fixed-size, fixed-size + bind validation, full bind sizing.
    [Benchmark, PerElement(ArrayLength)]
    public void IntArrayField() => Write(_intArrayParam, s_intArray);

    [Benchmark, PerElement(ArrayLength)]
    public void DateTimeArrayField() => Write(_dateTimeArrayParam, s_dateTimeArray);

    [Benchmark, PerElement(ArrayLength)]
    public void StringArrayField() => Write(_stringArrayParam, s_stringArray);

    [Benchmark, PerElement(ArrayLength)]
    public void IntArrayFieldUndecided() => Write(_intArrayUndecidedParam, s_intArray);

    [Benchmark, PerElement(ArrayLength)]
    public void DateTimeArrayFieldUndecided() => Write(_dateTimeArrayUndecidedParam, s_dateTimeArray);

    void Write<T>(NpgsqlParameter<T> param, T value)
    {
        param.TypedValue = value;
        param.ResolveTypeInfo(_connector.SerializerOptions, _connector.DbTypeResolver);
        param.Bind(_connector.ConversionContext, out _, out _, requiredFormat: DataFormat.Binary);
        param.Write(async: false, _pgWriter, default).GetAwaiter().GetResult();
        _connector.WriteBuffer.Clear();
    }
}
