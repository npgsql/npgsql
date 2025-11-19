using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace Npgsql.Tests;

public class MetricTests : TestBase
{
    [Test]
    public async Task OperationDuration()
    {
        var exportedItems = new List<Metric>();
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("Npgsql")
            .AddInMemoryExporter(exportedItems)
            .Build();

        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1";
        await using (var reader = await cmd.ExecuteReaderAsync())
            while (await reader.ReadAsync());

        meterProvider.ForceFlush();

        var metric = exportedItems.SingleOrDefault(m => m.Name == "db.client.operation.duration");
        Assert.That(metric, Is.Not.Null, "Metric 'db.client.operation.duration' not found.");

        var point = GetFilteredPoints(metric.GetMetricPoints(), dataSource.Name).Single();

        Assert.That(point.GetHistogramSum(), Is.GreaterThan(0));
        Assert.That(point.GetHistogramCount(), Is.EqualTo(1));

        var tags = ToDictionary(point.Tags);

        using (Assert.EnterMultipleScope())
        {
            // TODO: Vary this for PG-like databases (e.g. CockroachDB)?
            Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));

            Assert.That(tags["server.address"], Is.EqualTo(dataSource.Settings.Host));
            Assert.That(tags["server.port"], Is.EqualTo(dataSource.Settings.Port));
            Assert.That(tags["db.client.connection.pool.name"], Is.EqualTo(dataSource.Name));
        }
    }

    [Test]
    public async Task ConnectionCount()
    {
        var exportedItems = new List<Metric>();
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("Npgsql")
            .AddInMemoryExporter(exportedItems)
            .Build();

        await using var dataSource = CreateDataSource();

        using (var _ = await dataSource.OpenConnectionAsync())
        {
            meterProvider.ForceFlush();

            var metric = exportedItems.Single(m => m.Name == "db.client.connection.count");
            var points = GetFilteredPoints(metric.GetMetricPoints(), dataSource.Name);

            var usedPoint = GetPoint(points, "used");
            Assert.That(usedPoint.GetSumLong(), Is.EqualTo(1), "Expected used connections to be 1");

            var idlePoint = GetPoint(points, "idle");
            Assert.That(idlePoint.GetSumLong(), Is.Zero, "Expected idle connections to be 0");

            exportedItems.Clear();
        }

        meterProvider.ForceFlush();

        {
            var metric = exportedItems.Single(m => m.Name == "db.client.connection.count");
            var points = GetFilteredPoints(metric.GetMetricPoints(), dataSource.Name);

            var usedPoint = GetPoint(points, "used");
            Assert.That(usedPoint.GetSumLong(), Is.Zero, "Expected used connections to be 0");

            var idlePoint = GetPoint(points, "idle");
            Assert.That(idlePoint.GetSumLong(), Is.EqualTo(1), "Expected idle connections to be 1");
        }

        static MetricPoint GetPoint(IEnumerable<MetricPoint> points, string state)
        {
            foreach (var point in points)
            {
                foreach (var tag in point.Tags)
                {
                    if (tag.Key == "db.client.connection.state" && (string?)tag.Value == state)
                        return point;
                }
            }

            Assert.Fail($"Point with state '{state}' not found");
            throw new UnreachableException();
        }
    }

    [Test]
    public async Task ConnectionMax()
    {
        var exportedItems = new List<Metric>();
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("Npgsql")
            .AddInMemoryExporter(exportedItems)
            .Build();

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConnectionStringBuilder.MaxPoolSize = 134;
        await using var dataSource = dataSourceBuilder.Build();

        meterProvider.ForceFlush();

        var metric = exportedItems.Single(m => m.Name == "db.client.connection.max");
        var point = GetFilteredPoints(metric.GetMetricPoints(), dataSource.Name).First(p => p.GetSumLong() == 134);
        var tags = ToDictionary(point.Tags);
        Assert.That(tags["db.client.connection.pool.name"], Is.EqualTo(dataSource.Name));
    }

    static Dictionary<string, object?> ToDictionary(ReadOnlyTagCollection tags)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var tag in tags)
            dict[tag.Key] = tag.Value;
        return dict;
    }

    protected override NpgsqlDataSourceBuilder CreateDataSourceBuilder()
    {
        var dataSourceBuilder = base.CreateDataSourceBuilder();
        dataSourceBuilder.Name = "MetricsDataSource" + _dataSourceCounter++;
        return dataSourceBuilder;
    }

    protected override NpgsqlDataSource CreateDataSource()
        => CreateDataSourceBuilder().Build();

    int _dataSourceCounter;

    static IEnumerable<MetricPoint> GetFilteredPoints(MetricPointsAccessor points, string dataSourceName)
    {
        foreach (var point in points)
        {
            foreach (var tag in point.Tags)
            {
                if (tag.Key == "db.client.connection.pool.name" && (string?)tag.Value == dataSourceName)
                    yield return point;
            }
        }
    }
}
