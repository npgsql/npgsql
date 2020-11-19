using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public sealed class NpgsqlTimestampTests : NpgsqlTypeTests
    {
        const long MicrosecondsMinValue = -211813488000000000;
        const long MicrosecondsMaxValue = 9223371331200000000 - 1;

        [TestCase(long.MinValue)]
        [TestCase(long.MaxValue)]
        [TestCase(MicrosecondsMinValue)]
        [TestCase(MicrosecondsMaxValue)]
        [TestCase(0)]
        public void ConstructionFromMicroseconds(long microseconds) =>
            Assert.DoesNotThrow(() => new NpgsqlTimestamp(microseconds));

        [TestCase(MicrosecondsMinValue - 1)]
        [TestCase(MicrosecondsMaxValue + 1)]
        public void ConstructionFromMicrosecondsThrows(long microseconds) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new NpgsqlTimestamp(microseconds));

        // Time cases
        [TestCase(1, 01, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 01, 01, 00, 00, 00, 000, 001)]
        [TestCase(1, 01, 01, 00, 00, 00, 001, 000)]
        [TestCase(1, 01, 01, 00, 00, 01, 000, 000)]
        [TestCase(1, 01, 01, 00, 01, 00, 000, 000)]
        [TestCase(1, 01, 01, 01, 00, 00, 000, 000)]
        [TestCase(1, 01, 01, 00, 00, 00, 000, 999)]
        [TestCase(1, 01, 01, 00, 00, 00, 999, 000)]
        [TestCase(1, 01, 01, 00, 00, 60, 000, 000)]
        [TestCase(1, 01, 01, 00, 59, 00, 000, 000)]
        [TestCase(1, 01, 01, 24, 00, 00, 000, 000)]
        // Month cases
        [TestCase(1, 01, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 02, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 03, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 04, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 05, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 06, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 07, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 08, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 09, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 10, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 11, 01, 00, 00, 00, 000, 000)]
        [TestCase(1, 12, 01, 00, 00, 00, 000, 000)]
        // Day cases
        [TestCase(1, 01, 31, 00, 00, 00, 000, 000)]
        [TestCase(1, 02, 27, 00, 00, 00, 000, 000)]
        [TestCase(1, 03, 31, 00, 00, 00, 000, 000)]
        [TestCase(1, 04, 30, 00, 00, 00, 000, 000)]
        [TestCase(1, 05, 31, 00, 00, 00, 000, 000)]
        [TestCase(1, 06, 30, 00, 00, 00, 000, 000)]
        [TestCase(1, 07, 31, 00, 00, 00, 000, 000)]
        [TestCase(1, 08, 31, 00, 00, 00, 000, 000)]
        [TestCase(1, 09, 30, 00, 00, 00, 000, 000)]
        [TestCase(1, 10, 31, 00, 00, 00, 000, 000)]
        [TestCase(1, 11, 30, 00, 00, 00, 000, 000)]
        [TestCase(1, 12, 31, 00, 00, 00, 000, 000)]
        [TestCase(4, 01, 31, 00, 00, 00, 000, 000)]
        [TestCase(4, 02, 28, 00, 00, 00, 000, 000)]
        [TestCase(4, 03, 31, 00, 00, 00, 000, 000)]
        [TestCase(4, 04, 30, 00, 00, 00, 000, 000)]
        [TestCase(4, 05, 31, 00, 00, 00, 000, 000)]
        [TestCase(4, 06, 30, 00, 00, 00, 000, 000)]
        [TestCase(4, 07, 31, 00, 00, 00, 000, 000)]
        [TestCase(4, 08, 31, 00, 00, 00, 000, 000)]
        [TestCase(4, 09, 30, 00, 00, 00, 000, 000)]
        [TestCase(4, 10, 31, 00, 00, 00, 000, 000)]
        [TestCase(4, 11, 30, 00, 00, 00, 000, 000)]
        [TestCase(4, 12, 31, 00, 00, 00, 000, 000)]
        // Edge cases
        [TestCase(1, 01, 01, 23, 59, 60, 000, 000)]
        [TestCase(-4713, 11, 24, 00, 00, 00, 000, 000)]
        [TestCase(294276, 12, 31, 23, 59, 59, 999, 999)]
        public void ConstructionFromParts(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond) =>
            Assert.DoesNotThrow(() => new NpgsqlTimestamp(year, month, day, hour, minute, second, millisecond, microsecond));

        // Date cases
        [TestCase(0, 01, 01, 00, 00, 00, 000, 000, "year")]
        [TestCase(1, 00, 01, 00, 00, 00, 000, 000, "month")]
        [TestCase(1, 01, 00, 00, 00, 00, 000, 000, "day")]
        // Time cases
        [TestCase(1, 01, 01, 00, 00, 00, 000, -001, "microsecond")]
        [TestCase(1, 01, 01, 00, 00, 00, 000, 1000, "microsecond")]
        [TestCase(1, 01, 01, 00, 00, 00, -001, 000, "millisecond")]
        [TestCase(1, 01, 01, 00, 00, 00, 1000, 000, "millisecond")]
        [TestCase(1, 01, 01, 00, 00, -1, 000, 000, "second")]
        [TestCase(1, 01, 01, 00, 00, 61, 000, 000, "second")]
        [TestCase(1, 01, 01, 00, -1, 00, 000, 000, "minute")]
        [TestCase(1, 01, 01, 00, 60, 00, 000, 000, "minute")]
        [TestCase(1, 01, 01, -1, 00, 00, 000, 000, "hour")]
        [TestCase(1, 01, 01, 25, 00, 00, 000, 000, "hour")]
        // Day cases
        [TestCase(1, 01, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 02, 29, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 03, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 04, 31, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 05, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 06, 31, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 07, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 08, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 09, 31, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 10, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 11, 31, 00, 00, 00, 000, 000, "day")]
        [TestCase(1, 12, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 01, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 02, 30, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 03, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 04, 31, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 05, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 06, 31, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 07, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 08, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 09, 31, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 10, 32, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 11, 31, 00, 00, 00, 000, 000, "day")]
        [TestCase(4, 12, 32, 00, 00, 00, 000, 000, "day")]
        // Edge cases
        [TestCase(1, 01, 01, 24, 00, 01, 000, 000, "hour")]
        [TestCase(-4713, 11, 23, 23, 59, 59, 999, 999, "year")]
        [TestCase(294276, 12, 31, 00, 00, 00, 000, 000, "year")]
        public void ConstructionFromPartsThrows(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, string parameter) =>
            Assert.AreEqual(
                Assert
                    .Throws<ArgumentOutOfRangeException>(() => new NpgsqlTimestamp(year, month, day, hour, minute, second, millisecond, microsecond))
                    .ParamName,
                parameter);

        public static IEnumerable PartsTestCases() => new[]
        {
            // Default
            new object[] { new NpgsqlTimestamp(), 2000, 01, 01, 00, 00, 00, 000, 000 },
            // Constants
            new object[] { NpgsqlTimestamp.PostgreSqlEpoch, 2000, 01, 01, 00, 00, 00, 000, 000 },
            new object[] { NpgsqlTimestamp.UnixEpoch, 1970, 01, 01, 00, 00, 00, 000, 000 },
            // Time
            new object[] { new NpgsqlTimestamp(0001, 01, 01, 00, 00, 00, 000, 001), 0001, 01, 01, 00, 00, 00, 000, 001 },
            new object[] { new NpgsqlTimestamp(0001, 01, 01, 00, 00, 00, 001, 000), 0001, 01, 01, 00, 00, 00, 001, 000 },
            new object[] { new NpgsqlTimestamp(0001, 01, 01, 00, 00, 01, 000, 000), 0001, 01, 01, 00, 00, 01, 000, 000 },
            new object[] { new NpgsqlTimestamp(0001, 01, 01, 00, 01, 00, 000, 000), 0001, 01, 01, 00, 01, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0001, 01, 01, 01, 00, 00, 000, 000), 0001, 01, 01, 01, 00, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0001, 01, 01, 01, 01, 01, 001, 001), 0001, 01, 01, 01, 01, 01, 001, 001 },
            // Date
            new object[] { new NpgsqlTimestamp(0001, 01, 01, 00, 00, 00, 000, 000), 0001, 01, 01, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0001, 01, 02, 00, 00, 00, 000, 000), 0001, 01, 02, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0001, 02, 01, 00, 00, 00, 000, 000), 0001, 02, 01, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0002, 01, 01, 00, 00, 00, 000, 000), 0002, 01, 01, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0002, 02, 01, 00, 00, 00, 000, 000), 0002, 02, 01, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0002, 01, 02, 00, 00, 00, 000, 000), 0002, 01, 02, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0001, 02, 02, 00, 00, 00, 000, 000), 0001, 02, 02, 00, 00, 00, 000, 000 },
            new object[] { new NpgsqlTimestamp(0002, 02, 02, 00, 00, 00, 000, 000), 0002, 02, 02, 00, 00, 00, 000, 000 },
        };

        [TestCaseSource(nameof(PartsTestCases))]
        public void Parts(NpgsqlTimestamp value, int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond)
        {
            Assert.AreEqual(year, value.Year);
            Assert.AreEqual(month, value.Month);
            Assert.AreEqual(day, value.Day);
            Assert.AreEqual(hour, value.Hour);
            Assert.AreEqual(minute, value.Minute);
            Assert.AreEqual(second, value.Second);
            Assert.AreEqual(millisecond, value.Millisecond);
            Assert.AreEqual(microsecond, value.Microsecond);
            Assert.AreEqual(new NpgsqlDate(year, month, day), value.Date);
            Assert.AreEqual(new NpgsqlTime(hour, minute, second, millisecond, microsecond), value.Time);
        }

        public static IEnumerable EqualityTestCases => new[]
        {
            new object[] { NpgsqlTimestamp.MinValue, NpgsqlTimestamp.MinValue },
            new object[] { NpgsqlTimestamp.MaxValue, NpgsqlTimestamp.MaxValue },
            new object[] { NpgsqlTimestamp.NegativeInfinity, NpgsqlTimestamp.NegativeInfinity },
            new object[] { NpgsqlTimestamp.PositiveInfinity, NpgsqlTimestamp.PositiveInfinity },
            new object[] { NpgsqlTimestamp.PostgreSqlEpoch, NpgsqlTimestamp.PostgreSqlEpoch },
            new object[] { NpgsqlTimestamp.UnixEpoch, NpgsqlTimestamp.UnixEpoch },
            new object[] { new NpgsqlTimestamp(1979, 05, 01, 11, 47, 33), new NpgsqlTimestamp(1979, 05, 01, 11, 47, 33) },
            new object[] { new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00), new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52), new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32), new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51), new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51) },
            new object[] { new NpgsqlTimestamp(2020, 02, 26, 01, 27, 27), new NpgsqlTimestamp(2020, 02, 26, 01, 27, 27) },
        };

        [TestCaseSource(nameof(EqualityTestCases))]
        public async Task Equality(NpgsqlTimestamp left, NpgsqlTimestamp right)
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
            new object[] { NpgsqlTimestamp.MinValue, NpgsqlTimestamp.MaxValue },
            new object[] { NpgsqlTimestamp.MaxValue, NpgsqlTimestamp.MinValue },
            new object[] { NpgsqlTimestamp.PositiveInfinity, NpgsqlTimestamp.NegativeInfinity },
            new object[] { NpgsqlTimestamp.NegativeInfinity, NpgsqlTimestamp.PositiveInfinity },
            new object[] { NpgsqlTimestamp.PostgreSqlEpoch, NpgsqlTimestamp.UnixEpoch },
            new object[] { NpgsqlTimestamp.UnixEpoch, NpgsqlTimestamp.PostgreSqlEpoch },
            new object[] { new NpgsqlTimestamp(1979, 05, 01, 11, 47, 33), new NpgsqlTimestamp(2020, 02, 26, 01, 27, 27) },
            new object[] { new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00), new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51) },
            new object[] { new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52), new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32), new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51), new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlTimestamp(2020, 02, 26, 01, 27, 27), new NpgsqlTimestamp(1979, 05, 01, 11, 47, 33) },
        };

        [TestCaseSource(nameof(InequalityTestCases))]
        public async Task Inequality(NpgsqlTimestamp left, NpgsqlTimestamp right)
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
            new object[] { NpgsqlTimestamp.MinValue, NpgsqlTimestamp.MaxValue },
            new object[] { NpgsqlTimestamp.NegativeInfinity, NpgsqlTimestamp.PositiveInfinity },
            new object[] { NpgsqlTimestamp.UnixEpoch, NpgsqlTimestamp.PostgreSqlEpoch },
            new object[] { new NpgsqlTimestamp(1979, 05, 01, 11, 47, 33), new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00), new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52), new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32), new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51) },
            new object[] { new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51), new NpgsqlTimestamp(2020, 02, 26, 01, 27, 27) },
        };

        [TestCaseSource(nameof(LessThanTestCases))]
        public async Task LessThan(NpgsqlTimestamp left, NpgsqlTimestamp right)
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
        public async Task LessThanOrEquals(NpgsqlTimestamp left, NpgsqlTimestamp right)
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
            new object[] { NpgsqlTimestamp.MaxValue, NpgsqlTimestamp.MinValue },
            new object[] { NpgsqlTimestamp.PositiveInfinity, NpgsqlTimestamp.NegativeInfinity },
            new object[] { NpgsqlTimestamp.PostgreSqlEpoch, NpgsqlTimestamp.UnixEpoch },
            new object[] { new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00), new NpgsqlTimestamp(1979, 05, 01, 11, 47, 33) },
            new object[] { new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52), new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32), new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51), new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlTimestamp(2020, 02, 26, 01, 27, 27), new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51) },
        };

        [TestCaseSource(nameof(GreaterThanTestCases))]
        public async Task GreaterThan(NpgsqlTimestamp left, NpgsqlTimestamp right)
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
        public async Task GreaterThanOrEquals(NpgsqlTimestamp left, NpgsqlTimestamp right)
        {
            Assert.True(left >= right);
            Assert.True(left.Microseconds >= right.Microseconds);
            Assert.True(left.CompareTo(right) >= 0);

            await BackendTrue(
                "SELECT @left >= @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable DateTimeCastingTestCases() => new[]
        {
            new object[] { NpgsqlTimestamp.UnixEpoch, DateTime.UnixEpoch },
            new object[] { new NpgsqlTimestamp(1979, 05, 01, 11, 47, 33), new DateTime(1979, 05, 01, 11, 47, 33) },
            new object[] { new NpgsqlTimestamp(1990, 06, 13, 12, 58, 00), new DateTime(1990, 06, 13, 12, 58, 00) },
            new object[] { new NpgsqlTimestamp(1994, 08, 03, 02, 55, 52), new DateTime(1994, 08, 03, 02, 55, 52) },
            new object[] { new NpgsqlTimestamp(1995, 01, 23, 12, 57, 32), new DateTime(1995, 01, 23, 12, 57, 32) },
            new object[] { new NpgsqlTimestamp(2012, 08, 31, 11, 17, 51), new DateTime(2012, 08, 31, 11, 17, 51) },
            new object[] { new NpgsqlTimestamp(2020, 02, 26, 01, 27, 27), new DateTime(2020, 02, 26, 01, 27, 27) },
        };

        [TestCaseSource(nameof(DateTimeCastingTestCases))]
        public void CastToDateTime(NpgsqlTimestamp source, DateTime result) =>
            Assert.AreEqual(result, (DateTime)source);

        [TestCaseSource(nameof(DateTimeCastingTestCases))]
        public void CastFromDateTime(NpgsqlTimestamp result, DateTime source) =>
            Assert.AreEqual(result, (NpgsqlTimestamp)source);
    }
}
