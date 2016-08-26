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
    public class PreparedStatementAcrossOpenCloseBenchmarks
    {
        readonly NpgsqlConnection _unpreparedConn;
        readonly NpgsqlCommand _unpreparedCommand;

        readonly NpgsqlConnection _preparedConn;
        readonly NpgsqlCommand _preparedCommand;

        readonly NpgsqlConnection _preparedPersistedConn;
        readonly NpgsqlCommand _preparedPersistedCommand;

        /// <summary>
        /// The more tables are joined, the more complex the query is to plan, and therefore the more
        /// impact statement preparation should have.
        /// </summary>
        [Params(0, 5, 10, 20)]
        public int TablesToJoin { get; set; }

        public PreparedStatementAcrossOpenCloseBenchmarks()
        {
            // Create tables and data
            using (var conn = BenchmarkEnvironment.OpenConnection())
            using (var cmd = new NpgsqlCommand {Connection = conn})
            {
                for (var i = 0; i < 100; i++)
                {
                    cmd.CommandText = $@"
DROP TABLE IF EXISTS table{i};
CREATE TABLE table{i} (id INT PRIMARY KEY, data INT);
INSERT INTO table{i} (id, data) VALUES (1, {i});
";
                    cmd.ExecuteNonQuery();
                }
            }

            _unpreparedConn = BenchmarkEnvironment.GetConnection();
            _unpreparedCommand = new NpgsqlCommand { Connection = _unpreparedConn };

            _preparedConn = BenchmarkEnvironment.GetConnection();
            _preparedCommand = new NpgsqlCommand { Connection = _preparedConn };

            _preparedPersistedConn = BenchmarkEnvironment.GetConnection();
            _preparedPersistedCommand = new NpgsqlCommand { Connection = _preparedPersistedConn };
        }

        [Benchmark(Baseline = true)]
        public object Unprepared()
        {
            _unpreparedConn.Open();
            _unpreparedCommand.CommandText = Queries[TablesToJoin];
            var o = _unpreparedCommand.ExecuteScalar();
            _unpreparedConn.Close();
            return o;
        }

        [Benchmark]
        public object Prepared()
        {
            _preparedConn.Open();
            _preparedCommand.CommandText = Queries[TablesToJoin];
            _preparedCommand.Prepare();
            var o = _preparedCommand.ExecuteScalar();
            _preparedConn.Close();
            return o;
        }

        [Benchmark]
        public object PreparedPersisted()
        {
            _preparedPersistedConn.Open();
            _preparedPersistedCommand.CommandText = Queries[TablesToJoin];
            _preparedPersistedCommand.Prepare(true);
            var o = _preparedPersistedCommand.ExecuteScalar();
            _preparedPersistedConn.Close();
            return o;
        }

        static PreparedStatementAcrossOpenCloseBenchmarks()
        {
            var paramValues = typeof(PreparedStatementAcrossOpenCloseBenchmarks)
                .GetProperty(nameof(TablesToJoin))
                .GetCustomAttribute<ParamsAttribute>()
                .Values
                .Cast<int>()
                .ToList();

            Queries = new string[paramValues.Max() + 1];
            Queries[0] = "SELECT 1";

            foreach (var tablesToJoin in paramValues.Where(i => i != 0))
                Queries[tablesToJoin] = GenerateQuery(tablesToJoin);
        }

        static readonly string[] Queries;

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
    }
}
