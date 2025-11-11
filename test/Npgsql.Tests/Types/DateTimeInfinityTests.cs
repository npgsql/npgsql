using System;
using System.Data;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Util.Statics;

namespace Npgsql.Tests.Types;

[TestFixture(false)]
#if DEBUG
[TestFixture(true)]
[NonParallelizable]
#endif
public sealed class DateTimeInfinityTests : TestBase, IDisposable
{
    static readonly TestCaseData[] TimestampDateTimeValues =
    [
        new TestCaseData(DateTime.MinValue.AddYears(1), "0002-01-01 00:00:00", "0002-01-01 00:00:00")
            .SetName("MinValue_AddYear"),
        new TestCaseData(DateTime.MinValue, "0001-01-01 00:00:00", "-infinity")
            .SetName("MinValue"),
        new TestCaseData(DateTime.MaxValue, "9999-12-31 23:59:59.999999", "infinity")
            .SetName("MaxValue")
    ];

    static readonly TestCaseData[] TimestampTzDateTimeValues =
    [
        new TestCaseData(DateTime.MinValue.AddYears(1), "0002-01-01 00:00:00+00", "0002-01-01 00:00:00+00")
            .SetName("MinValue_AddYear"),
        new TestCaseData(DateTime.MinValue, "0001-01-01 00:00:00+00", "-infinity")
            .SetName("MinValue"),
        new TestCaseData(DateTime.MaxValue, "9999-12-31 23:59:59.999999+00", "infinity")
            .SetName("MaxValue")
    ];

    static readonly TestCaseData[] TimestampTzDateTimeOffsetValues =
    [
        new TestCaseData(DateTimeOffset.MinValue.ToUniversalTime().AddYears(1), "0002-01-01 00:00:00+00", "0002-01-01 00:00:00+00")
            .SetName("MinValue_AddYear"),
        new TestCaseData(DateTimeOffset.MinValue, "0001-01-01 00:00:00+00", "-infinity")
            .SetName("MinValue"),
        new TestCaseData(DateTimeOffset.MaxValue, "9999-12-31 23:59:59.999999+00", "infinity")
            .SetName("MaxValue")
    ];

    static readonly TestCaseData[] DateDateTimeValues =
    [
        new TestCaseData(DateTime.MinValue.AddYears(1), "0002-01-01", "0002-01-01")
            .SetName("MinValue_AddYear"),
        new TestCaseData(DateTime.MinValue, "0001-01-01", "-infinity")
            .SetName("MinValue"),
        new TestCaseData(DateTime.MaxValue, "9999-12-31", "infinity")
            .SetName("MaxValue")
    ];

    // As we can't roundtrip DateTime.MaxValue due to precision differences with postgres we are lenient with equality for this particular value.
    static readonly Func<DateTime, DateTime, bool> MaxValuePrecisionLenientComparer =
        (expected, actual) => expected == DateTime.MaxValue && actual == new DateTime(expected.Ticks - 9) || actual == expected;

    [Test, TestCaseSource(nameof(TimestampDateTimeValues))]
    public Task Timestamp_DateTime(DateTime dateTime, string sqlLiteral, string infinityConvertedSqlLiteral)
        => AssertType(dateTime, DisableDateTimeInfinityConversions ? sqlLiteral : infinityConvertedSqlLiteral,
            "timestamp without time zone",
            dbType: DbType.DateTime2,
            comparer: MaxValuePrecisionLenientComparer);

    [Test, TestCaseSource(nameof(TimestampTzDateTimeValues))]
    public Task TimestampTz_DateTime(DateTime dateTime, string sqlLiteral, string infinityConvertedSqlLiteral)
        => AssertType(new DateTime(dateTime.Ticks, DateTimeKind.Utc), DisableDateTimeInfinityConversions ? sqlLiteral : infinityConvertedSqlLiteral,
            "timestamp with time zone",
            dbType: DbType.DateTime,
            comparer: MaxValuePrecisionLenientComparer);

    [Test, TestCaseSource(nameof(TimestampTzDateTimeOffsetValues))]
    public Task TimestampTz_DateTimeOffset(DateTimeOffset dateTime, string sqlLiteral, string infinityConvertedSqlLiteral)
        => AssertType(dateTime, DisableDateTimeInfinityConversions ? sqlLiteral : infinityConvertedSqlLiteral,
            "timestamp with time zone",
            dbType: DbType.DateTime,
            comparer: (expected, actual) => MaxValuePrecisionLenientComparer(expected.DateTime, actual.DateTime),
            isValueTypeDefaultFieldType: false);

    [Test, TestCaseSource(nameof(DateDateTimeValues))]
    public Task Date_DateTime(DateTime dateTime, string sqlLiteral, string infinityConvertedSqlLiteral)
        => AssertType(DisableDateTimeInfinityConversions ? dateTime.Date : dateTime, DisableDateTimeInfinityConversions ? sqlLiteral : infinityConvertedSqlLiteral,
            "date", isDataTypeInferredFromValue: false,
            dbType: DbType.Date,
            isValueTypeDefaultFieldType: false);

    static readonly TestCaseData[] DateOnlyDateTimeValues =
    [
        new TestCaseData(DateOnly.MinValue.AddYears(1), "0002-01-01", "0002-01-01")
            .SetName("MinValue_AddYear"),
        new TestCaseData(DateOnly.MinValue, "0001-01-01", "-infinity")
            .SetName("MinValue"),
        new TestCaseData(DateOnly.MaxValue, "9999-12-31", "infinity")
            .SetName("MaxValue")
    ];

    [Test, TestCaseSource(nameof(DateOnlyDateTimeValues))]
    public Task Date_DateOnly(DateOnly dateTime, string sqlLiteral, string infinityConvertedSqlLiteral)
        => AssertType(dateTime, DisableDateTimeInfinityConversions ? sqlLiteral : infinityConvertedSqlLiteral,
            "date",
            dbType: DbType.Date);

    NpgsqlDataSource? _dataSource;
    protected override NpgsqlDataSource DataSource => _dataSource ??= CreateDataSource(csb => csb.Timezone = "UTC");

    public DateTimeInfinityTests(bool disableDateTimeInfinityConversions)
    {
#if DEBUG
        DisableDateTimeInfinityConversions = disableDateTimeInfinityConversions;
#else
        if (disableDateTimeInfinityConversions)
        {
            Assert.Ignore(
                "DateTimeInfinityTests rely on the Npgsql.DisableDateTimeInfinityConversions AppContext switch and can only be run in DEBUG builds");
        }
#endif
    }

    public void Dispose()
    {
#if DEBUG
        DisableDateTimeInfinityConversions = false;
#endif
        DataSource.Dispose();
    }
}
