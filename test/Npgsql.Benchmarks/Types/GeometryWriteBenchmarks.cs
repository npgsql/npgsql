using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
#if NET46
using BenchmarkDotNet.Diagnostics.Windows;
#endif
using NpgsqlTypes;

namespace Npgsql.Benchmarks.Types
{
    [Config(typeof(Config))]
    public class GeometryWriteBenchmarks
    {
        [Params(4, 100, 1000, 10000)]
        public int PolygonSize { get; set; }

        readonly NpgsqlConnection conn;
        PostgisPolygon p;

        public GeometryWriteBenchmarks()
        {
            conn = BenchmarkEnvironment.OpenConnection();

            using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE foo (bar geometry)", conn))
                cmd.ExecuteNonQuery();
        }

        [Setup]
        public void Setup()
        {
            var a = new Coordinate2D[PolygonSize];
            var points = PolygonSize - 1;
            for (int i = 0; i < points; i++)
            {
                double r = ((double)i / (double)points) * Math.PI * 2;
                a[i] = new Coordinate2D(Math.Sin(r), Math.Cos(r));
            }
            a[points] = a[0]; // last point must be equal to first
            p = new PostgisPolygon(new[] { a });
        }

        [Benchmark]
        public void WriteGeometry()
        {
            using (var writer = conn.BeginBinaryImport("COPY foo FROM STDIN(FORMAT BINARY)"))
                for (int i = 0; i < (100000 / PolygonSize); i++)
                {
                    writer.StartRow();
                    writer.Write(p, NpgsqlDbType.Geometry);
                }
        }

        class Config : ManualConfig
        {
            public Config()
            {
#if NET46
                Add(new MemoryDiagnoser());
#endif
                Add(StatisticColumn.OperationsPerSecond);
            }
        }
    }
}
