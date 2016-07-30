using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks
{
    public class Connection
    {
        static readonly NpgsqlConnectionStringBuilder ConnectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString);
        const string ConnectionString = "Host=foo;Database=bar;Username=user;Password=password";

        [Benchmark]
        public NpgsqlConnection CreateWithoutConnectionString()
            => new NpgsqlConnection(ConnectionStringBuilder);

        [Benchmark]
        public NpgsqlConnection CreateWithConnectionString()
            => new NpgsqlConnection(ConnectionString);
    }
}
