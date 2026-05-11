using System;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

// ReSharper disable UnusedMember.Global

namespace Npgsql.Benchmarks;

[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
[OperationsPerSecond]
public class CommandExecuteBenchmarks
{
    readonly NpgsqlConnection _conn;
    readonly NpgsqlCommand _executeNonQueryCmd;
    readonly NpgsqlCommand _executeNonQueryWithParamCmd;
    readonly NpgsqlCommand _executeNonQueryPreparedCmd;
    readonly NpgsqlCommand _executeScalarCmd;
    readonly NpgsqlCommand _executeReaderCmd;

    public CommandExecuteBenchmarks()
    {
        _conn = BenchmarkEnvironment.OpenConnection();
        _executeNonQueryCmd = new NpgsqlCommand("SET lock_timeout = 1000", _conn);
        _executeNonQueryWithParamCmd = new NpgsqlCommand("SET lock_timeout = 1000", _conn);
        _executeNonQueryWithParamCmd.Parameters.AddWithValue("not_used", DBNull.Value);
        _executeNonQueryPreparedCmd = new NpgsqlCommand("SET lock_timeout = 1000", _conn);
        _executeNonQueryPreparedCmd.Prepare();
        _executeScalarCmd = new NpgsqlCommand("SELECT 1", _conn);
        _executeReaderCmd   = new NpgsqlCommand("SELECT 1", _conn);
    }

    [GlobalCleanup]
    public void Cleanup() => _conn.Dispose();

    [Benchmark]
    public int ExecuteNonQuery() => _executeNonQueryCmd.ExecuteNonQuery();

    [Benchmark]
    public int ExecuteNonQueryWithParam() => _executeNonQueryWithParamCmd.ExecuteNonQuery();

    [Benchmark]
    public int ExecuteNonQueryPrepared() => _executeNonQueryPreparedCmd.ExecuteNonQuery();

    [Benchmark]
    public object ExecuteScalar() => _executeScalarCmd.ExecuteScalar()!;

    [Benchmark]
    public object ExecuteReader()
    {
        using (var reader = _executeReaderCmd.ExecuteReader())
        {
            reader.Read();
            return reader.GetValue(0);
        }
    }
}
