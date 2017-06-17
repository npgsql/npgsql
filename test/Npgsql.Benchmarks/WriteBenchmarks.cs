using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using NpgsqlTypes;

namespace Npgsql.Benchmarks.Types
{
    [Config(typeof(Config))]
    public class WriteBenchmarks
    {
        readonly NpgsqlConnection _conn;
        NpgsqlCommand _intCmd;
        NpgsqlCommand _text1Cmd;
        NpgsqlCommand _text100Cmd;
        NpgsqlCommand _text1000Cmd;
        NpgsqlCommand _text10000Cmd;

        #region Initialization

        public WriteBenchmarks()
        {
            _conn = BenchmarkEnvironment.OpenConnection();

            using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE foo (int INT, text TEXT)", _conn))
                cmd.ExecuteNonQuery();
            _intCmd = BuildCommand("int", NpgsqlDbType.Integer, 8);
            _text1Cmd = BuildCommand("text", NpgsqlDbType.Text, new string('x', 1));
            _text100Cmd = BuildCommand("text", NpgsqlDbType.Text, new string('x', 100));
            _text1000Cmd = BuildCommand("text", NpgsqlDbType.Text, new string('x', 1000));
            _text10000Cmd = BuildCommand("text", NpgsqlDbType.Text, new string('x', 10000));
        }

        [GlobalSetup]
        public void Setup()
        {
            using (var cmd = new NpgsqlCommand("TRUNCATE foo", _conn))
                cmd.ExecuteNonQuery();
        }

        NpgsqlCommand BuildCommand(string column, NpgsqlDbType npgsqlDbType, object value)
        {
            var cmd = new NpgsqlCommand($"INSERT INTO foo ({column}) VALUES (@p)", _conn);
            cmd.Parameters.AddWithValue("p", npgsqlDbType, value);
            return cmd;
        }

        #endregion

        [Benchmark]
        public void Int()
        {
            _intCmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void Text1()
        {
            _text1Cmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void Text100()
        {
            _text100Cmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void Text1000()
        {
            _text1000Cmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void Text10000()
        {
            _text10000Cmd.ExecuteNonQuery();
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
