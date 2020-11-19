using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public sealed class NpgsqlTimetzTests : NpgsqlTypeTests
    {
        const int TimeZoneSecondsMinValue = -(((15 * 60) + 59) * 60 + 59);
        const int TimeZoneSecondsMaxValue = +(((15 * 60) + 59) * 60 + 59);

        [TestCase(86400000000, 0000)]
        [TestCase(3600000000, 0000)]
        [TestCase(60000000, 0000)]
        [TestCase(1000000, 0000)]
        [TestCase(1000, 0000)]
        [TestCase(1, 0000)]
        [TestCase(0, 0000)]
        [TestCase(0, TimeZoneSecondsMinValue)]
        [TestCase(0, TimeZoneSecondsMaxValue)]
        public void ConstructionFromMicroseconds(long microseconds, int offsetSeconds) =>
            Assert.DoesNotThrow(() => new NpgsqlTimetz(microseconds, offsetSeconds));

        [TestCase(86400000001, 0)]
        [TestCase(-1, 0)]
        [TestCase(0, TimeZoneSecondsMinValue - 1)]
        [TestCase(0, TimeZoneSecondsMaxValue + 1)]
        public void ConstructionFromMicrosecondsThrows(long microseconds, int offsetSeconds) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new NpgsqlTimetz(microseconds, offsetSeconds));

        // General
        [TestCase(00, 00, 00, 000, 000, TimeZoneSecondsMinValue)]
        [TestCase(00, 00, 00, 000, 000, TimeZoneSecondsMaxValue)]
        [TestCase(00, 00, 00, 000, 000, 0)]
        [TestCase(00, 00, 00, 000, 001, 0)]
        [TestCase(00, 00, 00, 001, 000, 0)]
        [TestCase(00, 00, 01, 000, 000, 0)]
        [TestCase(00, 01, 00, 000, 000, 0)]
        [TestCase(01, 00, 00, 000, 000, 0)]
        [TestCase(00, 00, 00, 000, 999, 0)]
        [TestCase(00, 00, 00, 999, 000, 0)]
        [TestCase(00, 00, 60, 000, 000, 0)]
        [TestCase(00, 59, 00, 000, 000, 0)]
        [TestCase(24, 00, 00, 000, 000, 0)]
        // Edge cases
        [TestCase(23, 59, 60, 000, 000, 0)]
        public void ConstructionFromParts(int hour, int minute, int second, int millisecond, int microsecond, int timeZoneSeconds) =>
            Assert.DoesNotThrow(() => new NpgsqlTimetz(hour, minute, second, millisecond, microsecond, new(timeZoneSeconds)));

        // General
        [TestCase(00, 00, 00, 000, 000, TimeZoneSecondsMinValue - 1, "seconds")]
        [TestCase(00, 00, 00, 000, 000, TimeZoneSecondsMaxValue + 1, "seconds")]
        [TestCase(00, 00, 00, 000, -001, 0, "microsecond")]
        [TestCase(00, 00, 00, 000, 1000, 0, "microsecond")]
        [TestCase(00, 00, 00, -001, 000, 0, "millisecond")]
        [TestCase(00, 00, 00, 1000, 000, 0, "millisecond")]
        [TestCase(00, 00, -1, 000, 000, 0, "second")]
        [TestCase(00, 00, 61, 000, 000, 0, "second")]
        [TestCase(00, -1, 00, 000, 000, 0, "minute")]
        [TestCase(00, 60, 00, 000, 000, 0, "minute")]
        [TestCase(-1, 00, 00, 000, 000, 0, "hour")]
        [TestCase(25, 00, 00, 000, 000, 0, "hour")]
        // Edge cases
        [TestCase(24, 00, 01, 000, 000, 0, "hour")]
        public void ConstructionFromPartsThrows(int hour, int minute, int second, int millisecond, int microsecond, int timeZoneSeconds, string parameter) =>
            Assert.AreEqual(
                Assert
                    .Throws<ArgumentOutOfRangeException>(() => new NpgsqlTimetz(hour, minute, second, millisecond, microsecond, new(timeZoneSeconds)))
                    .ParamName,
                parameter);

        public static IEnumerable PartsTestCases() => new[]
        {
            new object[] { new NpgsqlTimetz(0, 0, 0, 0, 0, new(1)), 0, 0, 0, 0, 0, 1 },
            new object[] { new NpgsqlTimetz(0, 0, 0, 0, 1, new(0)), 0, 0, 0, 0, 1, 0 },
            new object[] { new NpgsqlTimetz(0, 0, 0, 1, 0, new(0)), 0, 0, 0, 1, 0, 0 },
            new object[] { new NpgsqlTimetz(0, 0, 1, 0, 0, new(0)), 0, 0, 1, 0, 0, 0 },
            new object[] { new NpgsqlTimetz(0, 1, 0, 0, 0, new(0)), 0, 1, 0, 0, 0, 0 },
            new object[] { new NpgsqlTimetz(1, 0, 0, 0, 0, new(0)), 1, 0, 0, 0, 0, 0 },
            new object[] { new NpgsqlTimetz(1, 1, 1, 1, 1, new(0)), 1, 1, 1, 1, 1, 0 },
        };

        [TestCaseSource(nameof(PartsTestCases))]
        public void Parts(NpgsqlTimetz value, int hour, int minute, int second, int millisecond, int microsecond, int timeZoneSeconds)
        {
            Assert.AreEqual(hour, value.Hour);
            Assert.AreEqual(minute, value.Minute);
            Assert.AreEqual(second, value.Second);
            Assert.AreEqual(millisecond, value.Millisecond);
            Assert.AreEqual(microsecond, value.Microsecond);
            Assert.AreEqual(timeZoneSeconds, value.TimeZoneSeconds);
        }

        public static IEnumerable EqualityTestCases => new[]
        {
            new object[] { NpgsqlTimetz.MinValue, NpgsqlTimetz.MinValue },
            new object[] { NpgsqlTimetz.MaxValue, NpgsqlTimetz.MaxValue },
            new object[] { new NpgsqlTimetz(01, 27, 27, default), new NpgsqlTimetz(01, 27, 27, default) },
            new object[] { new NpgsqlTimetz(02, 55, 52, default), new NpgsqlTimetz(02, 55, 52, default) },
            new object[] { new NpgsqlTimetz(11, 17, 51, default), new NpgsqlTimetz(11, 17, 51, default) },
            new object[] { new NpgsqlTimetz(11, 47, 33, default), new NpgsqlTimetz(11, 47, 33, default) },
            new object[] { new NpgsqlTimetz(12, 57, 32, default), new NpgsqlTimetz(12, 57, 32, default) },
            new object[] { new NpgsqlTimetz(12, 58, 00, default), new NpgsqlTimetz(12, 58, 00, default) },
            new object[] { new NpgsqlTimetz(01, 27, 27, new(3600)), new NpgsqlTimetz(01, 27, 27, new(3600)) },
            new object[] { new NpgsqlTimetz(02, 55, 52, new(3600)), new NpgsqlTimetz(02, 55, 52, new(3600)) },
            new object[] { new NpgsqlTimetz(11, 17, 51, new(3600)), new NpgsqlTimetz(11, 17, 51, new(3600)) },
            new object[] { new NpgsqlTimetz(11, 47, 33, new(3600)), new NpgsqlTimetz(11, 47, 33, new(3600)) },
            new object[] { new NpgsqlTimetz(12, 57, 32, new(3600)), new NpgsqlTimetz(12, 57, 32, new(3600)) },
            new object[] { new NpgsqlTimetz(12, 58, 00, new(3600)), new NpgsqlTimetz(12, 58, 00, new(3600)) },
        };

        [TestCaseSource(nameof(EqualityTestCases))]
        public async Task Equality(NpgsqlTimetz left, NpgsqlTimetz right)
        {
            Assert.True(left == right);
            Assert.True(
                left.Microseconds == right.Microseconds &&
                left.TimeZoneSeconds == right.TimeZoneSeconds);

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
            new object[] { NpgsqlTimetz.MinValue, NpgsqlTimetz.MaxValue },
            new object[] { NpgsqlTimetz.MaxValue, NpgsqlTimetz.MinValue },
            new object[] { new NpgsqlTimetz(01, 27, 27, default), new NpgsqlTimetz(12, 58, 00, default) },
            new object[] { new NpgsqlTimetz(02, 55, 52, default), new NpgsqlTimetz(12, 57, 32, default) },
            new object[] { new NpgsqlTimetz(11, 17, 51, default), new NpgsqlTimetz(11, 47, 33, default) },
            new object[] { new NpgsqlTimetz(11, 47, 33, default), new NpgsqlTimetz(11, 17, 51, default) },
            new object[] { new NpgsqlTimetz(12, 57, 32, default), new NpgsqlTimetz(02, 55, 52, default) },
            new object[] { new NpgsqlTimetz(12, 58, 00, default), new NpgsqlTimetz(01, 27, 27, default) },
            new object[] { new NpgsqlTimetz(01, 27, 27, new(+3600)), new NpgsqlTimetz(01, 27, 27, new(-3600)) },
            new object[] { new NpgsqlTimetz(02, 55, 52, new(+3600)), new NpgsqlTimetz(02, 55, 52, new(-3600)) },
            new object[] { new NpgsqlTimetz(11, 17, 51, new(+3600)), new NpgsqlTimetz(11, 17, 51, new(-3600)) },
            new object[] { new NpgsqlTimetz(11, 47, 33, new(+3600)), new NpgsqlTimetz(11, 47, 33, new(-3600)) },
            new object[] { new NpgsqlTimetz(12, 57, 32, new(+3600)), new NpgsqlTimetz(12, 57, 32, new(-3600)) },
            new object[] { new NpgsqlTimetz(12, 58, 00, new(+3600)), new NpgsqlTimetz(12, 58, 00, new(-3600)) },
        };

        [TestCaseSource(nameof(InequalityTestCases))]
        public async Task Inequality(NpgsqlTimetz left, NpgsqlTimetz right)
        {
            Assert.True(left != right);
            Assert.True(
                left.Microseconds != right.Microseconds ||
                left.TimeZoneSeconds != right.TimeZoneSeconds);

            Assert.False(left.Equals(right));
            Assert.True(left.CompareTo(right) != 0);
            Assert.True(left.GetHashCode() != right.GetHashCode());

            await BackendTrue(
                "SELECT @left != @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable LessThanTestCases => new[]
        {
            new object[] { NpgsqlTimetz.MinValue, NpgsqlTimetz.MaxValue },
            new object[] { new NpgsqlTimetz(01, 27, 27, default), new NpgsqlTimetz(02, 55, 52, default) },
            new object[] { new NpgsqlTimetz(02, 55, 52, default), new NpgsqlTimetz(11, 17, 51, default) },
            new object[] { new NpgsqlTimetz(11, 17, 51, default), new NpgsqlTimetz(11, 47, 33, default) },
            new object[] { new NpgsqlTimetz(11, 47, 33, default), new NpgsqlTimetz(12, 57, 32, default) },
            new object[] { new NpgsqlTimetz(12, 57, 32, default), new NpgsqlTimetz(12, 58, 00, default) },
            // Offset difference
            new object[] { new NpgsqlTimetz(01, 27, 27, new(0)), new NpgsqlTimetz(01, 27, 27, new(1)) },
            new object[] { new NpgsqlTimetz(02, 55, 52, new(0)), new NpgsqlTimetz(02, 55, 52, new(1)) },
            new object[] { new NpgsqlTimetz(11, 17, 51, new(0)), new NpgsqlTimetz(11, 17, 51, new(1)) },
            new object[] { new NpgsqlTimetz(11, 47, 33, new(0)), new NpgsqlTimetz(11, 47, 33, new(1)) },
            new object[] { new NpgsqlTimetz(12, 57, 32, new(0)), new NpgsqlTimetz(12, 57, 32, new(1)) },
            new object[] { new NpgsqlTimetz(12, 58, 00, new(0)), new NpgsqlTimetz(12, 58, 00, new(1)) },
            // Offset difference, same local time
            new object[] { new NpgsqlTimetz(01, 27, 28, new(0)), new NpgsqlTimetz(01, 27, 27, new(1)) },
            new object[] { new NpgsqlTimetz(02, 55, 53, new(0)), new NpgsqlTimetz(02, 55, 52, new(1)) },
            new object[] { new NpgsqlTimetz(11, 17, 52, new(0)), new NpgsqlTimetz(11, 17, 51, new(1)) },
            new object[] { new NpgsqlTimetz(11, 47, 34, new(0)), new NpgsqlTimetz(11, 47, 33, new(1)) },
            new object[] { new NpgsqlTimetz(12, 57, 33, new(0)), new NpgsqlTimetz(12, 57, 32, new(1)) },
            new object[] { new NpgsqlTimetz(12, 58, 01, new(0)), new NpgsqlTimetz(12, 58, 00, new(1)) },
        };

        [TestCaseSource(nameof(LessThanTestCases))]
        public async Task LessThan(NpgsqlTimetz left, NpgsqlTimetz right)
        {
            Assert.True(left < right);
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
        public async Task LessThanOrEquals(NpgsqlTimetz left, NpgsqlTimetz right)
        {
            Assert.True(left <= right);
            Assert.True(left.CompareTo(right) <= 0);

            await BackendTrue(
                "SELECT @left <= @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable GreaterThanTestCases => new[]
        {
            new object[] { NpgsqlTimetz.MaxValue, NpgsqlTimetz.MinValue },
            new object[] { new NpgsqlTimetz(02, 55, 52, default), new NpgsqlTimetz(01, 27, 27, default) },
            new object[] { new NpgsqlTimetz(11, 17, 51, default), new NpgsqlTimetz(02, 55, 52, default) },
            new object[] { new NpgsqlTimetz(11, 47, 33, default), new NpgsqlTimetz(11, 17, 51, default) },
            new object[] { new NpgsqlTimetz(12, 57, 32, default), new NpgsqlTimetz(11, 47, 33, default) },
            new object[] { new NpgsqlTimetz(12, 58, 00, default), new NpgsqlTimetz(12, 57, 32, default) },
            // Offset difference
            new object[] { new NpgsqlTimetz(01, 27, 27, new(1)), new NpgsqlTimetz(01, 27, 27, new(0)) },
            new object[] { new NpgsqlTimetz(02, 55, 52, new(1)), new NpgsqlTimetz(02, 55, 52, new(0)) },
            new object[] { new NpgsqlTimetz(11, 17, 51, new(1)), new NpgsqlTimetz(11, 17, 51, new(0)) },
            new object[] { new NpgsqlTimetz(11, 47, 33, new(1)), new NpgsqlTimetz(11, 47, 33, new(0)) },
            new object[] { new NpgsqlTimetz(12, 57, 32, new(1)), new NpgsqlTimetz(12, 57, 32, new(0)) },
            new object[] { new NpgsqlTimetz(12, 58, 00, new(1)), new NpgsqlTimetz(12, 58, 00, new(0)) },
            // Offset difference, same local time
            new object[] { new NpgsqlTimetz(01, 27, 27, new(1)), new NpgsqlTimetz(01, 27, 28, new(0)) },
            new object[] { new NpgsqlTimetz(02, 55, 52, new(1)), new NpgsqlTimetz(02, 55, 53, new(0)) },
            new object[] { new NpgsqlTimetz(11, 17, 51, new(1)), new NpgsqlTimetz(11, 17, 52, new(0)) },
            new object[] { new NpgsqlTimetz(11, 47, 33, new(1)), new NpgsqlTimetz(11, 47, 34, new(0)) },
            new object[] { new NpgsqlTimetz(12, 57, 32, new(1)), new NpgsqlTimetz(12, 57, 33, new(0)) },
            new object[] { new NpgsqlTimetz(12, 58, 00, new(1)), new NpgsqlTimetz(12, 58, 01, new(0)) },
        };

        [TestCaseSource(nameof(GreaterThanTestCases))]
        public async Task GreaterThan(NpgsqlTimetz left, NpgsqlTimetz right)
        {
            Assert.True(left > right);
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
        public async Task GreaterThanOrEquals(NpgsqlTimetz left, NpgsqlTimetz right)
        {
            Assert.True(left >= right);
            Assert.True(left.CompareTo(right) >= 0);

            await BackendTrue(
                "SELECT @left >= @right",
                Parameter(left, "left"),
                Parameter(right, "right"));
        }

        public static IEnumerable DateTimeOffsetCastingTestCases() => new[]
        {
            new object[] { new NpgsqlTimetz(01, 27, 27, default), new DateTimeOffset(1, 1, 1, 01, 27, 27, default) },
            new object[] { new NpgsqlTimetz(02, 55, 52, default), new DateTimeOffset(1, 1, 1, 02, 55, 52, default) },
            new object[] { new NpgsqlTimetz(11, 17, 51, default), new DateTimeOffset(1, 1, 1, 11, 17, 51, default) },
            new object[] { new NpgsqlTimetz(11, 47, 33, default), new DateTimeOffset(1, 1, 1, 11, 47, 33, default) },
            new object[] { new NpgsqlTimetz(12, 57, 32, default), new DateTimeOffset(1, 1, 1, 12, 57, 32, default) },
            new object[] { new NpgsqlTimetz(12, 58, 00, default), new DateTimeOffset(1, 1, 1, 12, 58, 00, default) },
            new object[] { new NpgsqlTimetz(01, 27, 27, new NpgsqlTimeZone(01, 00, 00)), new DateTimeOffset(1, 1, 1, 01, 27, 27, new TimeSpan(01, 00, 00)) },
            new object[] { new NpgsqlTimetz(02, 55, 52, new NpgsqlTimeZone(01, 00, 00)), new DateTimeOffset(1, 1, 1, 02, 55, 52, new TimeSpan(01, 00, 00)) },
            new object[] { new NpgsqlTimetz(11, 17, 51, new NpgsqlTimeZone(01, 00, 00)), new DateTimeOffset(1, 1, 1, 11, 17, 51, new TimeSpan(01, 00, 00)) },
            new object[] { new NpgsqlTimetz(11, 47, 33, new NpgsqlTimeZone(01, 00, 00)), new DateTimeOffset(1, 1, 1, 11, 47, 33, new TimeSpan(01, 00, 00)) },
            new object[] { new NpgsqlTimetz(12, 57, 32, new NpgsqlTimeZone(01, 00, 00)), new DateTimeOffset(1, 1, 1, 12, 57, 32, new TimeSpan(01, 00, 00)) },
            new object[] { new NpgsqlTimetz(12, 58, 00, new NpgsqlTimeZone(01, 00, 00)), new DateTimeOffset(1, 1, 1, 12, 58, 00, new TimeSpan(01, 00, 00)) },
        };

        [TestCaseSource(nameof(DateTimeOffsetCastingTestCases))]
        public void CastToDateTimeOffset(NpgsqlTimetz source, DateTimeOffset result) =>
            Assert.AreEqual(result, (DateTimeOffset)source);

        [TestCaseSource(nameof(DateTimeOffsetCastingTestCases))]
        public void CastFromDateTimeOffset(NpgsqlTimetz result, DateTimeOffset source) =>
            Assert.AreEqual(result, (NpgsqlTimetz)source);
    }
}
