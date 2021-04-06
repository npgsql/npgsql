using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public sealed class NpgsqlIntervalTests : NpgsqlTypeTests
    {
        public static IEnumerable PartsTestCases() => new[]
        {
            // Default
            new object[] { new NpgsqlInterval(), 0000, 00, 00, 00, 00, 00, 000, 000 },
            // Time
            new object[] { new NpgsqlInterval(0001, 01, 01, 00, 00, 00, 000, 001), 0001, 01, 01, 00, 00, 00, 000, 001 },
            new object[] { new NpgsqlInterval(0001, 01, 01, 00, 00, 00, 001, 000), 0001, 01, 01, 00, 00, 00, 001, 000 },
            new object[] { new NpgsqlInterval(0001, 01, 01, 00, 00, 01, 000, 000), 0001, 01, 01, 00, 00, 01, 000, 000 },
            new object[] { new NpgsqlInterval(0001, 01, 01, 00, 01, 00, 000, 000), 0001, 01, 01, 00, 01, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0001, 01, 01, 01, 00, 00, 000, 000), 0001, 01, 01, 01, 00, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0001, 01, 01, 01, 01, 01, 001, 001), 0001, 01, 01, 01, 01, 01, 001, 001 },
            // Date
            new object[] { new NpgsqlInterval(0001, 01, 01, 00, 00, 00, 000, 000), 0001, 01, 01, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0001, 01, 02, 00, 00, 00, 000, 000), 0001, 01, 02, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0001, 02, 01, 00, 00, 00, 000, 000), 0001, 02, 01, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0002, 01, 01, 00, 00, 00, 000, 000), 0002, 01, 01, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0002, 02, 01, 00, 00, 00, 000, 000), 0002, 02, 01, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0002, 01, 02, 00, 00, 00, 000, 000), 0002, 01, 02, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0001, 02, 02, 00, 00, 00, 000, 000), 0001, 02, 02, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlInterval(0002, 02, 02, 00, 00, 00, 000, 000), 0002, 02, 02, 00, 00, 00, 000, 000 },
        };

        [TestCaseSource(nameof(PartsTestCases))]
        public void Parts(NpgsqlInterval value, int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond)
        {
            Assert.AreEqual(year, value.Year);
            Assert.AreEqual(month, value.Month);
            Assert.AreEqual(day, value.Day);
            Assert.AreEqual(hour, value.Hour);
            Assert.AreEqual(minute, value.Minute);
            Assert.AreEqual(second, value.Second);
            Assert.AreEqual(millisecond, value.Millisecond);
            Assert.AreEqual(microsecond, value.Microsecond);
        }

        public static IEnumerable EqualityTestCases => new[]
        {
            new object[] { NpgsqlInterval.MinValue, NpgsqlInterval.MinValue },
            new object[] { NpgsqlInterval.MaxValue, NpgsqlInterval.MaxValue },
            new object[] { new NpgsqlInterval(1979, 05, 01, 11, 47, 33), new NpgsqlInterval(1979, 05, 01, 11, 47, 33) },
            new object[] { new NpgsqlInterval(1990, 06, 13, 12, 58, 00), new NpgsqlInterval(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlInterval(1994, 08, 03, 02, 55, 52), new NpgsqlInterval(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlInterval(1995, 01, 23, 12, 57, 32), new NpgsqlInterval(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlInterval(2012, 08, 31, 11, 17, 51), new NpgsqlInterval(2012, 08, 31, 11, 17, 51) },
            new object[] { new NpgsqlInterval(2020, 02, 26, 01, 27, 27), new NpgsqlInterval(2020, 02, 26, 01, 27, 27) },
        };

        [TestCaseSource(nameof(EqualityTestCases))]
        public async Task Equality(NpgsqlInterval left, NpgsqlInterval right)
        {
            Assert.True(left == right);
            Assert.True(left.Microseconds == right.Microseconds);
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
            new object[] { NpgsqlInterval.MinValue, NpgsqlInterval.MaxValue },
            new object[] { NpgsqlInterval.MaxValue, NpgsqlInterval.MinValue },
            new object[] { new NpgsqlInterval(1979, 05, 01, 11, 47, 33), new NpgsqlInterval(2020, 02, 26, 01, 27, 27) },
            new object[] { new NpgsqlInterval(1990, 06, 13, 12, 58, 00), new NpgsqlInterval(2012, 08, 31, 11, 17, 51) },
            new object[] { new NpgsqlInterval(1994, 08, 03, 02, 55, 52), new NpgsqlInterval(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlInterval(1995, 01, 23, 12, 57, 32), new NpgsqlInterval(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlInterval(2012, 08, 31, 11, 17, 51), new NpgsqlInterval(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlInterval(2020, 02, 26, 01, 27, 27), new NpgsqlInterval(1979, 05, 01, 11, 47, 33) },
        };

        [TestCaseSource(nameof(InequalityTestCases))]
        public async Task Inequality(NpgsqlInterval left, NpgsqlInterval right)
        {
            Assert.True(left != right);
            Assert.True(left.Microseconds != right.Microseconds);
            Assert.False(left.Equals(right));
            Assert.True(left.CompareTo(right) != 0);

            await BackendTrue(
                "SELECT @left != @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable LessThanTestCases => new[]
        {
            new object[] { NpgsqlInterval.MinValue, NpgsqlInterval.MaxValue },
            new object[] { new NpgsqlInterval(1979, 05, 01, 11, 47, 33), new NpgsqlInterval(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlInterval(1990, 06, 13, 12, 58, 00), new NpgsqlInterval(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlInterval(1994, 08, 03, 02, 55, 52), new NpgsqlInterval(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlInterval(1995, 01, 23, 12, 57, 32), new NpgsqlInterval(2012, 08, 31, 11, 17, 51) },
            new object[] { new NpgsqlInterval(2012, 08, 31, 11, 17, 51), new NpgsqlInterval(2020, 02, 26, 01, 27, 27) },
        };

        [TestCaseSource(nameof(LessThanTestCases))]
        public async Task LessThan(NpgsqlInterval left, NpgsqlInterval right)
        {
            Assert.True(left < right);
            Assert.True(left.Microseconds < right.Microseconds);
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
        public async Task LessThanOrEquals(NpgsqlInterval left, NpgsqlInterval right)
        {
            Assert.True(left <= right);
            Assert.True(left.Microseconds <= right.Microseconds);
            Assert.True(left.CompareTo(right) <= 0);

            await BackendTrue(
                "SELECT @left <= @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable GreaterThanTestCases => new[]
        {
            new object[] { NpgsqlInterval.MaxValue, NpgsqlInterval.MinValue },
            new object[] { new NpgsqlInterval(1990, 06, 13, 12, 58, 00), new NpgsqlInterval(1979, 05, 01, 11, 47, 33) },
            new object[] { new NpgsqlInterval(1994, 08, 03, 02, 55, 52), new NpgsqlInterval(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlInterval(1995, 01, 23, 12, 57, 32), new NpgsqlInterval(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlInterval(2012, 08, 31, 11, 17, 51), new NpgsqlInterval(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlInterval(2020, 02, 26, 01, 27, 27), new NpgsqlInterval(2012, 08, 31, 11, 17, 51) },
        };

        [TestCaseSource(nameof(GreaterThanTestCases))]
        public async Task GreaterThan(NpgsqlInterval left, NpgsqlInterval right)
        {
            Assert.True(left > right);
            Assert.True(left.Microseconds > right.Microseconds);
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
        public async Task GreaterThanOrEquals(NpgsqlInterval left, NpgsqlInterval right)
        {
            Assert.True(left >= right);
            Assert.True(left.Microseconds >= right.Microseconds);
            Assert.True(left.CompareTo(right) >= 0);

            await BackendTrue(
                "SELECT @left >= @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable TimeSpanCastingTestCases() => new[]
        {
            new object[] { new NpgsqlTime(01, 27, 27), new TimeSpan(01, 27, 27) },
            new object[] { new NpgsqlTime(02, 55, 52), new TimeSpan(02, 55, 52) },
            new object[] { new NpgsqlTime(11, 17, 51), new TimeSpan(11, 17, 51) },
            new object[] { new NpgsqlTime(11, 47, 33), new TimeSpan(11, 47, 33) },
            new object[] { new NpgsqlTime(12, 57, 32), new TimeSpan(12, 57, 32) },
            new object[] { new NpgsqlTime(12, 58, 00), new TimeSpan(12, 58, 00) },
        };

        [TestCaseSource(nameof(TimeSpanCastingTestCases))]
        public void CastToTimeSpan(NpgsqlInterval source, TimeSpan result) =>
            Assert.AreEqual(result, (TimeSpan)source);

        [TestCaseSource(nameof(TimeSpanCastingTestCases))]
        public void CastFromTimeSpan(NpgsqlInterval result, TimeSpan source) =>
            Assert.AreEqual(result, (NpgsqlInterval)source);
    }
}
