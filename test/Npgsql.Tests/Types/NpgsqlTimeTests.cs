using System;
using System.Collections;
using System.Linq;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public sealed class NpgsqlTimeTests
    {
        // General
        [TestCase(00, 00, 00, 000, 000)]
        [TestCase(00, 00, 00, 000, 001)]
        [TestCase(00, 00, 00, 001, 000)]
        [TestCase(00, 00, 01, 000, 000)]
        [TestCase(00, 01, 00, 000, 000)]
        [TestCase(01, 00, 00, 000, 000)]
        [TestCase(00, 00, 00, 000, 999)]
        [TestCase(00, 00, 00, 999, 000)]
        [TestCase(00, 00, 60, 000, 000)]
        [TestCase(00, 59, 00, 000, 000)]
        [TestCase(24, 00, 00, 000, 000)]
        // Edge cases
        [TestCase(23, 59, 60, 000, 000)]
        public void ConstructionFromParts(int hour, int minute, int second, int millisecond, int microsecond) =>
            Assert.DoesNotThrow(() => new NpgsqlTime(hour, minute, second, millisecond, microsecond));

        // General
        [TestCase(00, 00, 00, 000, -001, "microsecond")]
        [TestCase(00, 00, 00, 000, 1000, "microsecond")]
        [TestCase(00, 00, 00, -001, 000, "millisecond")]
        [TestCase(00, 00, 00, 1000, 000, "millisecond")]
        [TestCase(00, 00, -1, 000, 000, "second")]
        [TestCase(00, 00, 61, 000, 000, "second")]
        [TestCase(00, -1, 00, 000, 000, "minute")]
        [TestCase(00, 60, 00, 000, 000, "minute")]
        [TestCase(-1, 00, 00, 000, 000, "hour")]
        [TestCase(25, 00, 00, 000, 000, "hour")]
        // Edge cases
        [TestCase(24, 00, 01, 000, 000, "hour")]
        public void ConstructionFromPartsThrows(int hour, int minute, int second, int millisecond, int microsecond, string parameter) =>
            Assert.AreEqual(
                Assert
                    .Throws<ArgumentOutOfRangeException>(() => new NpgsqlTime(hour, minute, second, millisecond, microsecond))
                    .ParamName,
                parameter);

        public static IEnumerable PartsTestCases() => new[]
        {
            new object[] { new NpgsqlTime(0, 0, 0, 0, 1), 0, 0, 0, 0, 1 },
            new object[] { new NpgsqlTime(0, 0, 0, 1, 0), 0, 0, 0, 1, 0 },
            new object[] { new NpgsqlTime(0, 0, 1, 0, 0), 0, 0, 1, 0, 0 },
            new object[] { new NpgsqlTime(0, 1, 0, 0, 0), 0, 1, 0, 0, 0 },
            new object[] { new NpgsqlTime(1, 0, 0, 0, 0), 1, 0, 0, 0, 0 },
            new object[] { new NpgsqlTime(1, 1, 1, 1, 1), 1, 1, 1, 1, 1 },
        };

        [TestCaseSource(nameof(PartsTestCases))]
        public void Parts(NpgsqlTime value, int hour, int minute, int second, int millisecond, int microsecond)
        {
            Assert.AreEqual(hour, value.Hour);
            Assert.AreEqual(minute, value.Minute);
            Assert.AreEqual(second, value.Second);
            Assert.AreEqual(millisecond, value.Millisecond);
            Assert.AreEqual(microsecond, value.Microsecond);
        }

        public static IEnumerable EqualityTestCases => new[]
        {
            new object[] { NpgsqlTime.MinValue, NpgsqlTime.MinValue },
            new object[] { NpgsqlTime.MaxValue, NpgsqlTime.MaxValue },
            new object[] { new NpgsqlTime(01, 27, 27), new NpgsqlTime(01, 27, 27) },
            new object[] { new NpgsqlTime(02, 55, 52), new NpgsqlTime(02, 55, 52) },
            new object[] { new NpgsqlTime(11, 17, 51), new NpgsqlTime(11, 17, 51) },
            new object[] { new NpgsqlTime(11, 47, 33), new NpgsqlTime(11, 47, 33) },
            new object[] { new NpgsqlTime(12, 57, 32), new NpgsqlTime(12, 57, 32) },
            new object[] { new NpgsqlTime(12, 58, 00), new NpgsqlTime(12, 58, 00) },
        };

        [TestCaseSource(nameof(EqualityTestCases))]
        public void Equality(NpgsqlTime left, NpgsqlTime right)
        {
            Assert.True(left == right);
            Assert.True(left.Microseconds == right.Microseconds);
            Assert.True(left.Equals(right));
            Assert.True(left.CompareTo(right) == 0);
            Assert.True(left.GetHashCode() == right.GetHashCode());
        }

        public static IEnumerable InequalityTestCases => new[]
        {
            new object[] { NpgsqlTime.MinValue, NpgsqlTime.MaxValue },
            new object[] { NpgsqlTime.MaxValue, NpgsqlTime.MinValue },
            new object[] { new NpgsqlTime(01, 27, 27), new NpgsqlTime(12, 58, 00) },
            new object[] { new NpgsqlTime(02, 55, 52), new NpgsqlTime(12, 57, 32) },
            new object[] { new NpgsqlTime(11, 17, 51), new NpgsqlTime(11, 47, 33) },
            new object[] { new NpgsqlTime(11, 47, 33), new NpgsqlTime(11, 17, 51) },
            new object[] { new NpgsqlTime(12, 57, 32), new NpgsqlTime(02, 55, 52) },
            new object[] { new NpgsqlTime(12, 58, 00), new NpgsqlTime(01, 27, 27) },
        };

        [TestCaseSource(nameof(InequalityTestCases))]
        public void Inequality(NpgsqlTime left, NpgsqlTime right)
        {
            Assert.True(left != right);
            Assert.True(left.Microseconds != right.Microseconds);
            Assert.False(left.Equals(right));
            Assert.True(left.CompareTo(right) != 0);
            Assert.True(left.GetHashCode() != right.GetHashCode());
        }

        public static IEnumerable LessThanTestCases => new[]
        {
            new object[] { NpgsqlTime.MinValue, NpgsqlTime.MaxValue },
            new object[] { new NpgsqlTime(01, 27, 27), new NpgsqlTime(02, 55, 52) },
            new object[] { new NpgsqlTime(02, 55, 52), new NpgsqlTime(11, 17, 51) },
            new object[] { new NpgsqlTime(11, 17, 51), new NpgsqlTime(11, 47, 33) },
            new object[] { new NpgsqlTime(11, 47, 33), new NpgsqlTime(12, 57, 32) },
            new object[] { new NpgsqlTime(12, 57, 32), new NpgsqlTime(12, 58, 00) },
        };

        [TestCaseSource(nameof(LessThanTestCases))]
        public void LessThan(NpgsqlTime left, NpgsqlTime right)
        {
            Assert.True(left < right);
            Assert.True(left.Microseconds < right.Microseconds);
            Assert.True(left.CompareTo(right) < 0);
        }

        public static IEnumerable LessThanOrEqualsTestCases =>
            Enumerable.Concat(
                LessThanTestCases.OfType<object[]>(),
                EqualityTestCases.OfType<object[]>());

        [TestCaseSource(nameof(LessThanOrEqualsTestCases))]
        public void LessThanOrEquals(NpgsqlTime left, NpgsqlTime right)
        {
            Assert.True(left <= right);
            Assert.True(left.Microseconds <= right.Microseconds);
            Assert.True(left.CompareTo(right) <= 0);
        }

        public static IEnumerable GreaterThanTestCases => new[]
        {
            new object[] { NpgsqlTime.MaxValue, NpgsqlTime.MinValue },
            new object[] { new NpgsqlTime(02, 55, 52), new NpgsqlTime(01, 27, 27) },
            new object[] { new NpgsqlTime(11, 17, 51), new NpgsqlTime(02, 55, 52) },
            new object[] { new NpgsqlTime(11, 47, 33), new NpgsqlTime(11, 17, 51) },
            new object[] { new NpgsqlTime(12, 57, 32), new NpgsqlTime(11, 47, 33) },
            new object[] { new NpgsqlTime(12, 58, 00), new NpgsqlTime(12, 57, 32) },
        };

        [TestCaseSource(nameof(GreaterThanTestCases))]
        public void GreaterThan(NpgsqlTime left, NpgsqlTime right)
        {
            Assert.True(left > right);
            Assert.True(left.Microseconds > right.Microseconds);
            Assert.True(left.CompareTo(right) > 0);
        }

        public static IEnumerable GreaterThanOrEqualsTestCases =>
            Enumerable.Concat(
                GreaterThanTestCases.OfType<object[]>(),
                EqualityTestCases.OfType<object[]>());

        [TestCaseSource(nameof(GreaterThanOrEqualsTestCases))]
        public void GreaterThanOrEquals(NpgsqlTime left, NpgsqlTime right)
        {
            Assert.True(left >= right);
            Assert.True(left.Microseconds >= right.Microseconds);
            Assert.True(left.CompareTo(right) >= 0);
        }

        public static IEnumerable DateTimeCastingTestCases() => new[]
        {
            new object[] { new NpgsqlTime(01, 27, 27), new DateTime(1, 1, 1, 01, 27, 27) },
            new object[] { new NpgsqlTime(02, 55, 52), new DateTime(1, 1, 1, 02, 55, 52) },
            new object[] { new NpgsqlTime(11, 17, 51), new DateTime(1, 1, 1, 11, 17, 51) },
            new object[] { new NpgsqlTime(11, 47, 33), new DateTime(1, 1, 1, 11, 47, 33) },
            new object[] { new NpgsqlTime(12, 57, 32), new DateTime(1, 1, 1, 12, 57, 32) },
            new object[] { new NpgsqlTime(12, 58, 00), new DateTime(1, 1, 1, 12, 58, 00) },
        };

        [TestCaseSource(nameof(DateTimeCastingTestCases))]
        public void CastToDateTime(NpgsqlTime source, DateTime result) =>
            Assert.AreEqual(result, (DateTime)source);

        [TestCaseSource(nameof(DateTimeCastingTestCases))]
        public void CastFromDateTime(NpgsqlTime result, DateTime source) =>
            Assert.AreEqual(result, (NpgsqlTime)source);
    }
}
