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
    [Config("columns=OperationPerSecond"), MemoryDiagnoser]
    public class WriteVaryingNumberOfParameters
    {
        NpgsqlConnection _conn;
        NpgsqlCommand _cmd;

        //[Params(0, 1, 10, 100)]
        [Params(10)]
        public int NumParams { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _conn = BenchmarkEnvironment.OpenConnection();

            var funcParams = string.Join(",",
                Enumerable.Range(0, NumParams)
                .Select(i => $"IN p{i} int4")
            );
            using (var cmd = new NpgsqlCommand($"CREATE FUNCTION pg_temp.swallow({funcParams}) RETURNS void AS 'BEGIN END;' LANGUAGE 'plpgsql'", _conn))
                cmd.ExecuteNonQuery();

            var cmdParams = string.Join(",", Enumerable.Range(0, NumParams).Select(i => $"@p{i}"));
            _cmd = new NpgsqlCommand($"SELECT pg_temp.swallow({cmdParams})", _conn);
            for (var i = 0; i < NumParams; i++)
                _cmd.Parameters.Add(new NpgsqlParameter("p" + i, NpgsqlDbType.Integer));
            _cmd.Prepare();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _cmd.Unprepare();
            _conn.Close();
        }

        [Benchmark]
        public void WriteParameters()
        {
            for (var i = 0; i < NumParams; i++)
                _cmd.Parameters[i].Value = i;
            _cmd.ExecuteNonQuery();
        }
    }
}
