using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

public class MultirangeTests : TestBase
{
    static readonly TestCaseData[] MultirangeTestCases =
    {
        // int4multirange
        new TestCaseData(
                new NpgsqlRange<int>[]
                {
                    new(3, true, false, 7, false, false),
                    new(9, true, false, 0, false, true)
                },
                "{[3,7),[9,)}", "int4multirange", NpgsqlDbType.IntegerMultirange, true, default(NpgsqlRange<int>))
            .SetName("Int"),

        // int8multirange
        new TestCaseData(
                new NpgsqlRange<long>[]
                {
                    new(3, true, false, 7, false, false),
                    new(9, true, false, 0, false, true)
                },
                "{[3,7),[9,)}", "int8multirange", NpgsqlDbType.BigIntMultirange, true, default(NpgsqlRange<long>))
            .SetName("Long"),

        // nummultirange
        // numeric is non-discrete so doesn't undergo normalization, use that to test bound scenarios which otherwise get normalized
        new TestCaseData(
                new NpgsqlRange<decimal>[]
                {
                    new(3, true, false, 7, true, false),
                    new(9, false, false, 0, false, true)
                },
                "{[3,7],(9,)}", "nummultirange", NpgsqlDbType.NumericMultirange, true, default(NpgsqlRange<decimal>))
            .SetName("Decimal"),

        // daterange
        new TestCaseData(
                new NpgsqlRange<DateTime>[]
                {
                    new(new(2020, 1, 1), true, false, new(2020, 1, 5), false, false),
                    new(new(2020, 1, 10), true, false, default, false, true)
                },
                "{[2020-01-01,2020-01-05),[2020-01-10,)}", "datemultirange", NpgsqlDbType.DateMultirange, true, default(NpgsqlRange<DateTime>))
            .SetName("DateTime"),

#if NET6_0_OR_GREATER
        new TestCaseData(
                new NpgsqlRange<DateOnly>[]
                {
                    new(new(2020, 1, 1), true, false, new(2020, 1, 5), false, false),
                    new(new(2020, 1, 10), true, false, default, false, true)
                },
                "{[2020-01-01,2020-01-05),[2020-01-10,)}", "datemultirange", NpgsqlDbType.DateMultirange, false, default(NpgsqlRange<DateOnly>))
            .SetName("DateOnly"),
#endif
    };

    [Test, TestCaseSource(nameof(MultirangeTestCases))]
    public Task Multirange_as_array<T, TRange>(
        T multirangeAsArray, string sqlLiteral, string pgTypeName, NpgsqlDbType? npgsqlDbType, bool isDefaultForReading, TRange _)
        => AssertType(multirangeAsArray, sqlLiteral, pgTypeName, npgsqlDbType, isDefaultForReading: isDefaultForReading);

    [Test, TestCaseSource(nameof(MultirangeTestCases))]
    public Task Multirange_as_list<T, TRange>(
        T multirangeAsArray, string sqlLiteral, string pgTypeName, NpgsqlDbType? npgsqlDbType, bool isDefaultForReading, TRange _)
        where T : IList<TRange>
        => AssertType(
            new List<TRange>(multirangeAsArray),
            sqlLiteral, pgTypeName, npgsqlDbType, isDefaultForReading: false);

    [OneTimeSetUp]
    public async Task Setup()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");
    }
}
