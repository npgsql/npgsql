using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.Internal.ResolverFactories;
using Npgsql.Util;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

[NonParallelizable]
public class LegacyDateAndTimeBehaviorTests : TestBase
{
    [Test]
    public Task Date_as_DateTime()
        => AssertType(new DateTime(2020, 1, 1), "2020-01-01", "date", NpgsqlDbType.Date, DbType.Date, isDefaultForWriting: false);

    [Test]
    public Task Daterange_as_NpgsqlRange_DateTime()
        => AssertType(
            new NpgsqlRange<DateTime>(new DateTime(2020, 1, 1), lowerBoundIsInclusive: true, new DateTime(2020, 1, 5), upperBoundIsInclusive: false),
            "[2020-01-01,2020-01-05)",
            "daterange",
            NpgsqlDbType.DateRange,
            isDefaultForWriting: false,
            skipArrayCheck: true);

    [Test]
    public Task Date_as_DateOnly_explicitly()
        => AssertType(new DateOnly(2020, 1, 1), "2020-01-01", "date", NpgsqlDbType.Date, DbType.Date, isDefaultForReading: false);

    [Test]
    public Task Time_as_TimeSpan()
        => AssertType(new TimeSpan(1, 2, 3), "01:02:03", "time without time zone", NpgsqlDbType.Time, DbType.Time, isDefaultForWriting: false);

    [Test]
    public Task Time_as_TimeOnly_explicitly()
        => AssertType(new TimeOnly(1, 2, 3), "01:02:03", "time without time zone", NpgsqlDbType.Time, DbType.Time, isDefaultForReading: false);

    [Test]
    public Task Datemultirange_as_NpgsqlRange_DateTime_array()
        => AssertType(
            new[]
            {
                new NpgsqlRange<DateTime>(
                    new DateTime(2020, 1, 1), lowerBoundIsInclusive: true,
                    new DateTime(2020, 1, 5), upperBoundIsInclusive: false),
                new NpgsqlRange<DateTime>(
                    new DateTime(2020, 1, 6), lowerBoundIsInclusive: true,
                    new DateTime(2020, 1, 10), upperBoundIsInclusive: false)
            },
            "{[2020-01-01,2020-01-05),[2020-01-06,2020-01-10)}",
            "datemultirange",
            NpgsqlDbType.DateMultirange,
            isDefaultForWriting: false);

    NpgsqlDataSource _dataSource = null!;
    protected override NpgsqlDataSource DataSource => _dataSource;

    [OneTimeSetUp]
    public void Setup()
    {
#if DEBUG
        Statics.EnableLegacyDateAndTimeBehavior = true;
        // Can't use the static AdoTypeInfoResolver instance, it already captured the feature flag.
        _dataSource = CreateDataSource(builder => builder.AddTypeInfoResolverFactory(new AdoTypeInfoResolverFactory()));
        NpgsqlDataSourceBuilder.ResetGlobalMappings(overwrite: true);
#else
        Assert.Ignore("LegacyDateAndTimeTests can only be run in DEBUG builds where Statics.EnableLegacyDateAndTimeBehavior is mutable");
#endif
    }

    [OneTimeTearDown]
    public void Teardown()
    {
#if DEBUG
        Statics.EnableLegacyDateAndTimeBehavior = false;
        _dataSource.Dispose();
        NpgsqlDataSourceBuilder.ResetGlobalMappings(overwrite: true);
#endif
    }
}
