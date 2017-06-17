using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Npgsql.Benchmarks
{
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [Config("columns=OperationPerSecond")]
    public class Prepare
    {
        NpgsqlConnection _conn, _autoPreparingConn;
        static readonly string[] Queries;
        string _query;
        NpgsqlCommand _preparedCmd;

        /// <summary>
        /// The more tables are joined, the more complex the query is to plan, and therefore the more
        /// impact statement preparation should have.
        /// </summary>
        [Params(0, 1, 2, 5, 10)]
        public int TablesToJoin { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _conn = BenchmarkEnvironment.OpenConnection();
            _autoPreparingConn = new NpgsqlConnection(new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString)
            {
                MaxAutoPrepare = 10
            }.ToString());
            _autoPreparingConn.Open();

            foreach (var conn in new[] { _conn, _autoPreparingConn })
            {
                using (var cmd = new NpgsqlCommand { Connection = conn })
                {
                    for (var i = 0; i < 100; i++)
                    {
                        cmd.CommandText = $@"
CREATE TEMP TABLE table{i} (id INT PRIMARY KEY, data INT);
INSERT INTO table{i} (id, data) VALUES (1, {i});
";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            _query = Queries[TablesToJoin];
            _preparedCmd = new NpgsqlCommand(_query, _conn);
            _preparedCmd.Prepare();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _conn.Dispose();            
        }

        public Prepare()
        {
            // Create tables and data
            using (var conn = BenchmarkEnvironment.OpenConnection())
            using (var cmd = new NpgsqlCommand {Connection = conn})
            {
                for (var i = 0; i < TablesToJoinValues.Max(); i++)
                {
                    cmd.CommandText = $@"
DROP TABLE IF EXISTS table{i};
CREATE TABLE table{i} (id INT PRIMARY KEY, data INT);
INSERT INTO table{i} (id, data) VALUES (1, {i});
";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [Benchmark(Baseline = true)]
        public object Unprepared()
        {
            using (var cmd = new NpgsqlCommand(_query, _conn))
                return cmd.ExecuteScalar();
        }

        [Benchmark]
        public object AutoPrepared()
        {
            using (var cmd = new NpgsqlCommand(_query, _autoPreparingConn))
                return cmd.ExecuteScalar();
        }

        [Benchmark]
        public object Prepared() => _preparedCmd.ExecuteScalar();

        static Prepare()
        {
            Queries = new string[TablesToJoinValues.Max() + 1];
            Queries[0] = "SELECT 1";

            foreach (var tablesToJoin in TablesToJoinValues.Where(i => i != 0))
                Queries[tablesToJoin] = GenerateQuery(tablesToJoin);
        }

        static string GenerateQuery(int tablesToJoin)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT ");
            sb.AppendLine(string.Join("+", Enumerable.Range(0, tablesToJoin).Select(i => $"table{i}.data")));
            sb.AppendLine("FROM table0");
            for (var i = 1; i < tablesToJoin; i++)
                sb.AppendLine($"JOIN table{i} ON table{i}.id = table{i - 1}.id");
            return sb.ToString();
        }

        static readonly int[] TablesToJoinValues = typeof(Prepare)
            .GetProperty(nameof(TablesToJoin))
            .GetCustomAttribute<ParamsAttribute>()
            .Values
            .Cast<int>()
            .ToArray();
    }
}
