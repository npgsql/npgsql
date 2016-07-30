using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks
{
    [Config("columns=OperationPerSecond")]
    public class ConnectionCloseOpenBenchmarks
    {
        readonly NpgsqlConnection _pooledConnection;
        readonly NpgsqlConnection _nonPooledConnection;

        public ConnectionCloseOpenBenchmarks()
        {
            _pooledConnection = BenchmarkEnvironment.GetConnection();
            var csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) { Pooling = false };
            _nonPooledConnection = new NpgsqlConnection(csb);
        }

        [Benchmark]
        public void OpenClosePooled()
        {
            _pooledConnection.Open();
            _pooledConnection.Close();
        }

        [Benchmark]
        public void OpenCloseNonPooled()
        {
            _nonPooledConnection.Open();
            _nonPooledConnection.Close();
        }
    }
}
