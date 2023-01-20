using System;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

// ReSharper disable UnusedMember.Global

namespace Npgsql.Benchmarks;

[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
[Config(typeof(Config))]
public class CommandExecuteBenchmarks
{
    readonly NpgsqlCommandOrig _executeNonQueryCmd;
    readonly NpgsqlCommandOrig _executeNonQueryWithParamCmd;
    readonly NpgsqlCommandOrig _executeNonQueryPreparedCmd;
    readonly NpgsqlCommandOrig _executeScalarCmd;
    readonly NpgsqlCommandOrig _executeReaderCmd;

    public CommandExecuteBenchmarks()
    {
        var conn = BenchmarkEnvironment.OpenConnection();
        _executeNonQueryCmd = new NpgsqlCommandOrig("SET lock_timeout = 1000", conn);
        _executeNonQueryWithParamCmd = new NpgsqlCommandOrig("SET lock_timeout = 1000", conn);
        _executeNonQueryWithParamCmd.Parameters.AddWithValue("not_used", DBNull.Value);
        _executeNonQueryPreparedCmd = new NpgsqlCommandOrig("SET lock_timeout = 1000", conn);
        _executeNonQueryPreparedCmd.Prepare();
        _executeScalarCmd = new NpgsqlCommandOrig("SELECT 1", conn);
        _executeReaderCmd   = new NpgsqlCommandOrig("SELECT 1", conn);
    }

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

    class Config : ManualConfig
    {
        public Config()
        {
            AddColumn(StatisticColumn.OperationsPerSecond);
        }
    }
}