using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Npgsql.Benchmarks
{
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [Config("columns=OperationPerSecond")]
    public class CommandExecuteBenchmarks
    {
        readonly NpgsqlCommand _executeNonQueryCmd;
        readonly NpgsqlCommand _executeNonQueryWithParamCmd;
        readonly NpgsqlCommand _executeNonQueryPreparedCmd;
        readonly NpgsqlCommand _executeScalarCmd;
        readonly NpgsqlCommand _executeReaderCmd;

        public CommandExecuteBenchmarks()
        {
            var conn = BenchmarkEnvironment.OpenConnection();
            _executeNonQueryCmd = new NpgsqlCommand("SET lock_timeout = 1000", conn);
            _executeNonQueryWithParamCmd = new NpgsqlCommand("SET lock_timeout = 1000", conn);
            _executeNonQueryWithParamCmd.Parameters.AddWithValue("not_used", DBNull.Value);
            _executeNonQueryPreparedCmd = new NpgsqlCommand("SET lock_timeout = 1000", conn);
            _executeNonQueryPreparedCmd.Prepare();
            _executeScalarCmd = new NpgsqlCommand("SELECT 1", conn);
            _executeReaderCmd   = new NpgsqlCommand("SELECT 1", conn);
        }

        [Benchmark]
        public int ExecuteNonQuery() => _executeNonQueryCmd.ExecuteNonQuery();

        [Benchmark]
        public int ExecuteNonQueryWithParam() => _executeNonQueryWithParamCmd.ExecuteNonQuery();

        [Benchmark]
        public int ExecuteNonQueryPrepared() => _executeNonQueryPreparedCmd.ExecuteNonQuery();

        [Benchmark]
        public object ExecuteScalar() => _executeScalarCmd.ExecuteScalar();

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
}
