using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;

namespace Npgsql.Benchmarks
{
    [Config(typeof(Config))]
    public class ConnectionOpenCloseBenchmarks
    {
        readonly NpgsqlCommand _noOpenCloseCmd;

        readonly NpgsqlConnection _openCloseConn;
        readonly NpgsqlCommand _openCloseCmd;

        readonly NpgsqlConnection _persistentPreparedConn;
        readonly NpgsqlCommand _persistentPreparedCmd;

        readonly NpgsqlConnection _noResetConn;
        readonly NpgsqlCommand _noResetCmd;

        readonly NpgsqlConnection _nonPooledConnection;
        readonly NpgsqlCommand _nonPooledCmd;

        [Params(0, 1, 5, 10)]
        public int StatementsToSend { get; set; }

        public ConnectionOpenCloseBenchmarks()
        {
            NpgsqlConnectionStringBuilder csb;

            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) { ApplicationName = nameof(NoOpenClose)};
            var noOpenCloseConn = new NpgsqlConnection(csb);
            noOpenCloseConn.Open();
            _noOpenCloseCmd = new NpgsqlCommand("SET lock_timeout = 1000", noOpenCloseConn);

            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) { ApplicationName = nameof(OpenClose) };
            _openCloseConn = new NpgsqlConnection(csb);
            _openCloseCmd = new NpgsqlCommand("SET lock_timeout = 1000", _openCloseConn);

            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) { ApplicationName = nameof(WithPersistentPrepared) };
            _persistentPreparedConn = new NpgsqlConnection(csb);
            _persistentPreparedConn.Open();
            using (var persistent = new NpgsqlCommand("SELECT 1", _persistentPreparedConn))
                persistent.Prepare(true);
            _persistentPreparedConn.Close();
            _persistentPreparedCmd = new NpgsqlCommand("SET lock_timeout = 1000", _persistentPreparedConn);

            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString)
            {
                ApplicationName = nameof(NoResetOnClose),
                NoResetOnClose = true
            };
            _noResetConn = new NpgsqlConnection(csb);
            _noResetCmd = new NpgsqlCommand("SET lock_timeout = 1000", _noResetConn);
            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) {
                ApplicationName = nameof(NonPooled),
                Pooling = false
            };
            _nonPooledConnection = new NpgsqlConnection(csb);
            _nonPooledCmd = new NpgsqlCommand("SET lock_timeout = 1000", _nonPooledConnection);
        }

        [Benchmark]
        public void NoOpenClose()
        {
            for (var i = 0; i < StatementsToSend; i++)
                _noOpenCloseCmd.ExecuteNonQuery();
        }

        [Benchmark(Baseline = true)]
        public void OpenClose()
        {
            _openCloseConn.Open();
            for (var i = 0; i < StatementsToSend; i++)
                _openCloseCmd.ExecuteNonQuery();
            _openCloseConn.Close();
        }

        /// <summary>
        /// Having persistent prepared statements alters the connection reset when closing.
        /// </summary>
        [Benchmark]
        public void WithPersistentPrepared()
        {
            _persistentPreparedConn.Open();
            for (var i = 0; i < StatementsToSend; i++)
                _persistentPreparedCmd.ExecuteNonQuery();
            _persistentPreparedConn.Close();
        }

        /// <summary>
        /// Having persistent prepared statements alters the connection reset when closing.
        /// </summary>
        [Benchmark]
        public void NoResetOnClose()
        {
            _noResetConn.Open();
            for (var i = 0; i < StatementsToSend; i++)
                _noResetCmd.ExecuteNonQuery();
            _noResetConn.Close();
        }

        [Benchmark]
        public void NonPooled()
        {
            _nonPooledConnection.Open();
            for (var i = 0; i < StatementsToSend; i++)
                _nonPooledCmd.ExecuteNonQuery();
            _nonPooledConnection.Close();
        }

        class Config : ManualConfig
        {
            public Config()
            {
                Add(new MemoryDiagnoser());
                Add(StatisticColumn.OperationsPerSecond);
            }
        }
    }
}
