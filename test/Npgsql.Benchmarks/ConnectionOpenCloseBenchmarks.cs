using System.Data.SqlClient;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Npgsql.Benchmarks
{
    [Config(typeof(Config))]
    public class ConnectionOpenCloseBenchmarks
    {
        const string SqlClientConnectionString = @"Data Source=(localdb)\mssqllocaldb";

        NpgsqlCommand _noOpenCloseCmd;

        readonly string _openCloseConnString = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) { ApplicationName = nameof(OpenClose) }.ToString();
        readonly NpgsqlCommand _openCloseCmd = new NpgsqlCommand("SET lock_timeout = 1000");
        readonly SqlCommand _sqlOpenCloseCmd = new SqlCommand("SET LOCK_TIMEOUT 1000");

        NpgsqlConnection _openCloseSameConn;
        NpgsqlCommand _openCloseSameCmd;

        SqlConnection _sqlOpenCloseSameConn;
        SqlCommand _sqlOpenCloseSameCmd;

        NpgsqlConnection _connWithPrepared;
        NpgsqlCommand _withPreparedCmd;

        NpgsqlConnection _noResetConn;
        NpgsqlCommand _noResetCmd;

        NpgsqlConnection _nonPooledConnection;
        NpgsqlCommand _nonPooledCmd;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [Params(0, 1, 5, 10)]
        public int StatementsToSend { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) { ApplicationName = nameof(NoOpenClose)};
            var noOpenCloseConn = new NpgsqlConnection(csb.ToString());
            noOpenCloseConn.Open();
            _noOpenCloseCmd = new NpgsqlCommand("SET lock_timeout = 1000", noOpenCloseConn);

            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) { ApplicationName = nameof(OpenCloseSameConnection) };
            _openCloseSameConn = new NpgsqlConnection(csb.ToString());
            _openCloseSameCmd = new NpgsqlCommand("SET lock_timeout = 1000", _openCloseSameConn);

            _sqlOpenCloseSameConn = new SqlConnection(SqlClientConnectionString);
            _sqlOpenCloseSameCmd = new SqlCommand("SET LOCK_TIMEOUT 1000", _sqlOpenCloseSameConn);

            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) { ApplicationName = nameof(WithPrepared) };
            _connWithPrepared = new NpgsqlConnection(csb.ToString());
            _connWithPrepared.Open();
            using (var somePreparedCmd = new NpgsqlCommand("SELECT 1", _connWithPrepared))
                somePreparedCmd.Prepare();
            _connWithPrepared.Close();
            _withPreparedCmd = new NpgsqlCommand("SET lock_timeout = 1000", _connWithPrepared);

            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString)
            {
                ApplicationName = nameof(NoResetOnClose),
                NoResetOnClose = true
            };
            _noResetConn = new NpgsqlConnection(csb.ToString());
            _noResetCmd = new NpgsqlCommand("SET lock_timeout = 1000", _noResetConn);
            csb = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString) {
                ApplicationName = nameof(NonPooled),
                Pooling = false
            };
            _nonPooledConnection = new NpgsqlConnection(csb.ToString());
            _nonPooledCmd = new NpgsqlCommand("SET lock_timeout = 1000", _nonPooledConnection);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _noOpenCloseCmd.Connection?.Close();
            NpgsqlConnection.ClearAllPools();
            SqlConnection.ClearAllPools();
        }

        [Benchmark]
        public void NoOpenClose()
        {
            for (var i = 0; i < StatementsToSend; i++)
                _noOpenCloseCmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void OpenClose()
        {
            using (var conn = new NpgsqlConnection(_openCloseConnString))
            {
                conn.Open();
                _openCloseCmd.Connection = conn;
                for (var i = 0; i < StatementsToSend; i++)
                    _openCloseCmd.ExecuteNonQuery();
            }
        }

        [Benchmark(Baseline = true)]
        public void SqlClientOpenClose()
        {
            using (var conn = new SqlConnection(SqlClientConnectionString))
            {
                conn.Open();
                _sqlOpenCloseCmd.Connection = conn;
                for (var i = 0; i < StatementsToSend; i++)
                    _sqlOpenCloseCmd.ExecuteNonQuery();
            }
        }

        [Benchmark]
        public void OpenCloseSameConnection()
        {
            _openCloseSameConn.Open();
            for (var i = 0; i < StatementsToSend; i++)
                _openCloseSameCmd.ExecuteNonQuery();
            _openCloseSameConn.Close();
        }

        [Benchmark]
        public void SqlClientOpenCloseSameConnection()
        {
            _sqlOpenCloseSameConn.Open();
            for (var i = 0; i < StatementsToSend; i++)
                _sqlOpenCloseSameCmd.ExecuteNonQuery();
            _sqlOpenCloseSameConn.Close();
        }

        /// <summary>
        /// Having prepared statements alters the connection reset when closing.
        /// </summary>
        [Benchmark]
        public void WithPrepared()
        {
            _connWithPrepared.Open();
            for (var i = 0; i < StatementsToSend; i++)
                _withPreparedCmd.ExecuteNonQuery();
            _connWithPrepared.Close();
        }

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
                Add(StatisticColumn.OperationsPerSecond);
            }
        }
    }
}
