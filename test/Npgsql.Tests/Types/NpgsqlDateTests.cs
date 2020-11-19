using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public sealed class NpgsqlDateTests : NpgsqlTypeTests
    {
        const long DaysMinValue = -2451545;
        const long DaysMaxValue = 2145031949 - 1;

        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        [TestCase(DaysMaxValue)]
        [TestCase(DaysMinValue)]
        public void ConstructionFromDays(int days) =>
            Assert.DoesNotThrow(() => new NpgsqlDate(days));

        [TestCase(DaysMaxValue + 1)]
        [TestCase(DaysMinValue - 1)]
        public void ConstructionFromDaysThrows(int days) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new NpgsqlDate(days));

        // General
        [TestCase(1, 01, 01)]
        [TestCase(1, 02, 01)]
        [TestCase(1, 03, 01)]
        [TestCase(1, 04, 01)]
        [TestCase(1, 05, 01)]
        [TestCase(1, 06, 01)]
        [TestCase(1, 07, 01)]
        [TestCase(1, 08, 01)]
        [TestCase(1, 09, 01)]
        [TestCase(1, 10, 01)]
        [TestCase(1, 11, 01)]
        [TestCase(1, 12, 01)]
        // Day cases
        [TestCase(1, 01, 31)]
        [TestCase(1, 02, 27)]
        [TestCase(1, 03, 31)]
        [TestCase(1, 04, 30)]
        [TestCase(1, 05, 31)]
        [TestCase(1, 06, 30)]
        [TestCase(1, 07, 31)]
        [TestCase(1, 08, 31)]
        [TestCase(1, 09, 30)]
        [TestCase(1, 10, 31)]
        [TestCase(1, 11, 30)]
        [TestCase(1, 12, 31)]
        [TestCase(4, 01, 31)]
        [TestCase(4, 02, 28)]
        [TestCase(4, 03, 31)]
        [TestCase(4, 04, 30)]
        [TestCase(4, 05, 31)]
        [TestCase(4, 06, 30)]
        [TestCase(4, 07, 31)]
        [TestCase(4, 08, 31)]
        [TestCase(4, 09, 30)]
        [TestCase(4, 10, 31)]
        [TestCase(4, 11, 30)]
        [TestCase(4, 12, 31)]
        // Edge cases
        [TestCase(-4714, 11, 24)]
        [TestCase(+5874897, 12, 31)]
        public void ConstructionFromParts(int year, int month, int day) =>
            Assert.DoesNotThrow(() => new NpgsqlDate(year, month, day));

        // General
        [TestCase(0, 01, 01, "year")]
        [TestCase(1, 00, 01, "month")]
        [TestCase(1, 01, 00, "day")]
        // Day cases
        [TestCase(1, 01, 32, "day")]
        [TestCase(1, 02, 29, "day")]
        [TestCase(1, 03, 32, "day")]
        [TestCase(1, 04, 31, "day")]
        [TestCase(1, 05, 32, "day")]
        [TestCase(1, 06, 31, "day")]
        [TestCase(1, 07, 32, "day")]
        [TestCase(1, 08, 32, "day")]
        [TestCase(1, 09, 31, "day")]
        [TestCase(1, 10, 32, "day")]
        [TestCase(1, 11, 31, "day")]
        [TestCase(1, 12, 32, "day")]
        [TestCase(4, 01, 32, "day")]
        [TestCase(4, 02, 30, "day")]
        [TestCase(4, 03, 32, "day")]
        [TestCase(4, 04, 31, "day")]
        [TestCase(4, 05, 32, "day")]
        [TestCase(4, 06, 31, "day")]
        [TestCase(4, 07, 32, "day")]
        [TestCase(4, 08, 32, "day")]
        [TestCase(4, 09, 31, "day")]
        [TestCase(4, 10, 32, "day")]
        [TestCase(4, 11, 31, "day")]
        [TestCase(4, 12, 32, "day")]
        // Edge cases
        [TestCase(-4714, 11, 23, "year")]
        [TestCase(+5874898, 01, 01, "year")]
        public void ConstructionFromPartsThrows(int year, int month, int day, string parameter) =>
            Assert.AreEqual(
                Assert
                    .Throws<ArgumentOutOfRangeException>(() => new NpgsqlDate(year, month, day))
                    .ParamName,
                parameter);

        public static IEnumerable PartsTestCases() => new[]
        {
            new object[] { NpgsqlDate.PostgreSqlEpoch, 2000, 1, 1 },
            new object[] { NpgsqlDate.UnixEpoch, 1970, 1, 1 },
            new object[] { new NpgsqlDate(1, 1, 1), 1, 1, 1 },
            new object[] { new NpgsqlDate(1, 1, 2), 1, 1, 2 },
            new object[] { new NpgsqlDate(1, 2, 1), 1, 2, 1 },
            new object[] { new NpgsqlDate(2, 1, 1), 2, 1, 1 },
            new object[] { new NpgsqlDate(2, 2, 1), 2, 2, 1 },
            new object[] { new NpgsqlDate(2, 1, 2), 2, 1, 2 },
            new object[] { new NpgsqlDate(1, 2, 2), 1, 2, 2 },
            new object[] { new NpgsqlDate(2, 2, 2), 2, 2, 2 },
        };

        [TestCaseSource(nameof(PartsTestCases))]
        public void Parts(NpgsqlDate value, int year, int month, int day)
        {
            Assert.AreEqual(year, value.Year);
            Assert.AreEqual(month, value.Month);
            Assert.AreEqual(day, value.Day);
        }

        public static IEnumerable EqualityTestCases => new[]
        {
            new object[] { NpgsqlDate.MinValue, NpgsqlDate.MinValue },
            new object[] { NpgsqlDate.MaxValue, NpgsqlDate.MaxValue },
            new object[] { NpgsqlDate.NegativeInfinity, NpgsqlDate.NegativeInfinity },
            new object[] { NpgsqlDate.PositiveInfinity, NpgsqlDate.PositiveInfinity },
            new object[] { NpgsqlDate.PostgreSqlEpoch, NpgsqlDate.PostgreSqlEpoch },
            new object[] { NpgsqlDate.UnixEpoch, NpgsqlDate.UnixEpoch },
            new object[] { new NpgsqlDate(1979, 05, 01), new NpgsqlDate(1979, 05, 01) },
            new object[] { new NpgsqlDate(1990, 06, 13), new NpgsqlDate(1990, 06, 13) },
            new object[] { new NpgsqlDate(1994, 08, 03), new NpgsqlDate(1994, 08, 03) },
            new object[] { new NpgsqlDate(1995, 01, 23), new NpgsqlDate(1995, 01, 23) },
            new object[] { new NpgsqlDate(2012, 08, 31), new NpgsqlDate(2012, 08, 31) },
            new object[] { new NpgsqlDate(2020, 02, 26), new NpgsqlDate(2020, 02, 26) },
        };

        [TestCaseSource(nameof(EqualityTestCases))]
        public async Task Equality(NpgsqlDate left, NpgsqlDate right)
        {
            Assert.True(left == right);
            Assert.True(left.Days == right.Days);
            Assert.True(left.Equals(right));
            Assert.True(left.CompareTo(right) == 0);
            Assert.True(left.GetHashCode() == right.GetHashCode());

            await BackendTrue(
                "SELECT @left = @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable InequalityTestCases => new[]
        {
            new object[] { NpgsqlDate.MinValue, NpgsqlDate.MaxValue },
            new object[] { NpgsqlDate.MaxValue, NpgsqlDate.MinValue },
            new object[] { NpgsqlDate.PositiveInfinity, NpgsqlDate.NegativeInfinity },
            new object[] { NpgsqlDate.NegativeInfinity, NpgsqlDate.PositiveInfinity },
            new object[] { NpgsqlDate.PostgreSqlEpoch, NpgsqlDate.UnixEpoch },
            new object[] { NpgsqlDate.UnixEpoch, NpgsqlDate.PostgreSqlEpoch },
            new object[] { new NpgsqlDate(1979, 05, 01), new NpgsqlDate(2020, 02, 26) },
            new object[] { new NpgsqlDate(1990, 06, 13), new NpgsqlDate(2012, 08, 31) },
            new object[] { new NpgsqlDate(1994, 08, 03), new NpgsqlDate(1995, 01, 23) },
            new object[] { new NpgsqlDate(1995, 01, 23), new NpgsqlDate(1994, 08, 03) },
            new object[] { new NpgsqlDate(2012, 08, 31), new NpgsqlDate(1990, 06, 13) },
            new object[] { new NpgsqlDate(2020, 02, 26), new NpgsqlDate(1979, 05, 01) },
        };

        [TestCaseSource(nameof(InequalityTestCases))]
        public async Task Inequality(NpgsqlDate left, NpgsqlDate right)
        {
            Assert.True(left != right);
            Assert.True(left.Days != right.Days);
            Assert.False(left.Equals(right));
            Assert.True(left.CompareTo(right) != 0);

            await BackendTrue(
                "SELECT @left != @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable LessThanTestCases => new[]
        {
            new object[] { NpgsqlDate.MinValue, NpgsqlDate.MaxValue },
            new object[] { NpgsqlDate.NegativeInfinity, NpgsqlDate.PositiveInfinity },
            new object[] { NpgsqlDate.UnixEpoch, NpgsqlDate.PostgreSqlEpoch },
            new object[] { new NpgsqlDate(1979, 05, 01), new NpgsqlDate(1990, 06, 13) },
            new object[] { new NpgsqlDate(1990, 06, 13), new NpgsqlDate(1994, 08, 03) },
            new object[] { new NpgsqlDate(1994, 08, 03), new NpgsqlDate(1995, 01, 23) },
            new object[] { new NpgsqlDate(1995, 01, 23), new NpgsqlDate(2012, 08, 31) },
            new object[] { new NpgsqlDate(2012, 08, 31), new NpgsqlDate(2020, 02, 26) },
        };

        [TestCaseSource(nameof(LessThanTestCases))]
        public async Task LessThan(NpgsqlDate left, NpgsqlDate right)
        {
            Assert.True(left < right);
            Assert.True(left.Days < right.Days);
            Assert.True(left.CompareTo(right) < 0);

            await BackendTrue(
                "SELECT @left < @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable LessThanOrEqualsTestCases =>
            Enumerable.Concat(
                LessThanTestCases.OfType<object[]>(),
                EqualityTestCases.OfType<object[]>());

        [TestCaseSource(nameof(LessThanOrEqualsTestCases))]
        public async Task LessThanOrEquals(NpgsqlDate left, NpgsqlDate right)
        {
            Assert.True(left <= right);
            Assert.True(left.Days <= right.Days);
            Assert.True(left.CompareTo(right) <= 0);

            await BackendTrue(
                "SELECT @left <= @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable GreaterThanTestCases => new[]
        {
            new object[] { NpgsqlDate.MaxValue, NpgsqlDate.MinValue },
            new object[] { NpgsqlDate.PositiveInfinity, NpgsqlDate.NegativeInfinity },
            new object[] { NpgsqlDate.PostgreSqlEpoch, NpgsqlDate.UnixEpoch },
            new object[] { new NpgsqlDate(1990, 06, 13), new NpgsqlDate(1979, 05, 01) },
            new object[] { new NpgsqlDate(1994, 08, 03), new NpgsqlDate(1990, 06, 13) },
            new object[] { new NpgsqlDate(1995, 01, 23), new NpgsqlDate(1994, 08, 03) },
            new object[] { new NpgsqlDate(2012, 08, 31), new NpgsqlDate(1995, 01, 23) },
            new object[] { new NpgsqlDate(2020, 02, 26), new NpgsqlDate(2012, 08, 31) },
        };

        [TestCaseSource(nameof(GreaterThanTestCases))]
        public async Task GreaterThan(NpgsqlDate left, NpgsqlDate right)
        {
            Assert.True(left > right);
            Assert.True(left.Days > right.Days);
            Assert.True(left.CompareTo(right) > 0);

            await BackendTrue(
                "SELECT @left > @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable GreaterThanOrEqualsTestCases =>
            Enumerable.Concat(
                GreaterThanTestCases.OfType<object[]>(),
                EqualityTestCases.OfType<object[]>());

        [TestCaseSource(nameof(GreaterThanOrEqualsTestCases))]
        public async Task GreaterThanOrEquals(NpgsqlDate left, NpgsqlDate right)
        {
            Assert.True(left >= right);
            Assert.True(left.Days >= right.Days);
            Assert.True(left.CompareTo(right) >= 0);

            await BackendTrue(
                "SELECT @left >= @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable DateTimeCastingTestCases() => new[]
        {
            new object[] { NpgsqlDate.UnixEpoch, DateTime.UnixEpoch },
            new object[] { new NpgsqlDate(1979, 05, 01), new DateTime(1979, 05, 01) },
            new object[] { new NpgsqlDate(1990, 06, 13), new DateTime(1990, 06, 13) },
            new object[] { new NpgsqlDate(1994, 08, 03), new DateTime(1994, 08, 03) },
            new object[] { new NpgsqlDate(1995, 01, 23), new DateTime(1995, 01, 23) },
            new object[] { new NpgsqlDate(2012, 08, 31), new DateTime(2012, 08, 31) },
            new object[] { new NpgsqlDate(2020, 02, 26), new DateTime(2020, 02, 26) },
        };

        [TestCaseSource(nameof(DateTimeCastingTestCases))]
        public void CastToDateTime(NpgsqlDate source, DateTime result) =>
            Assert.AreEqual(result, (DateTime)source);

        [TestCaseSource(nameof(DateTimeCastingTestCases))]
        public void CastFromDateTime(NpgsqlDate result, DateTime source) =>
            Assert.AreEqual(result, (NpgsqlDate)source);
    }
}
