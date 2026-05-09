using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks;

[OperationsPerSecond, PerElementMean]
public class GetFieldValue
{
    readonly NpgsqlConnection _conn;
    readonly NpgsqlCommand _cmd;
    readonly NpgsqlDataReader _reader;

    const int ArrayLength = 100;

    public GetFieldValue()
    {
        _conn = BenchmarkEnvironment.OpenConnection();
        var ints = string.Join(",", Enumerable.Range(0, ArrayLength));
        var timestamps = string.Join(",", Enumerable.Range(0, ArrayLength)
            .Select(i => $"'2026-01-01'::timestamp + interval '{i} days'"));
        var strings = string.Join(",", Enumerable.Range(0, ArrayLength)
            .Select(i => $"'some realistic text value #{i:0000}'"));
        _cmd = new NpgsqlCommand(
            "SELECT 0, 'str', " +
            $"ARRAY[{ints}]::integer[], " +
            $"ARRAY[{timestamps}], " +
            $"ARRAY[{strings}]::text[]",
            _conn);
        _reader = _cmd.ExecuteReader();
        _reader.Read();
    }

    [Benchmark]
    public void NullableField() => _reader.GetFieldValue<int?>(0);

    [Benchmark]
    public void ValueTypeField() => _reader.GetFieldValue<int>(0);

    [Benchmark]
    public void ReferenceTypeField() => _reader.GetFieldValue<string>(1);

    [Benchmark]
    public void ObjectField() => _reader.GetFieldValue<object>(1);

    [Benchmark, PerElement(ArrayLength)]
    public void IntArrayField() => _reader.GetFieldValue<int[]>(2);

    [Benchmark, PerElement(ArrayLength)]
    public void DateTimeArrayField() => _reader.GetFieldValue<DateTime[]>(3);

    [Benchmark, PerElement(ArrayLength)]
    public void StringArrayField() => _reader.GetFieldValue<string[]>(4);
}
