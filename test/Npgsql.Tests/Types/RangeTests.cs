using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Npgsql.Properties;
using Npgsql.Util;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

class RangeTests : MultiplexingTestBase
{
    static readonly TestCaseData[] RangeTestCases =
    [
        new TestCaseData(new NpgsqlRange<int>(1, true, 10, false), "[1,10)", "int4range", NpgsqlDbType.IntegerRange)
            .SetName("IntegerRange"),
        new TestCaseData(new NpgsqlRange<long>(1, true, 10, false), "[1,10)", "int8range", NpgsqlDbType.BigIntRange)
            .SetName("BigIntRange"),
        new TestCaseData(new NpgsqlRange<decimal>(1, true, 10, false), "[1,10)", "numrange", NpgsqlDbType.NumericRange)
            .SetName("NumericRange"),
        new TestCaseData(new NpgsqlRange<DateTime>(
                    new DateTime(2020, 1, 1, 12, 0, 0), true,
                    new DateTime(2020, 1, 3, 13, 0, 0), false),
                """["2020-01-01 12:00:00","2020-01-03 13:00:00")""", "tsrange", NpgsqlDbType.TimestampRange)
            .SetName("TimestampRange"),
        // Note that the below text representations are local (according to TimeZone, which is set to Europe/Berlin in this test class),
        // because that's how PG does timestamptz *text* representation.
        new TestCaseData(new NpgsqlRange<DateTime>(
                    new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc), true,
                    new DateTime(2020, 1, 3, 13, 0, 0, DateTimeKind.Utc), false),
                """["2020-01-01 13:00:00+01","2020-01-03 14:00:00+01")""", "tstzrange", NpgsqlDbType.TimestampTzRange)
            .SetName("TimestampTzRange"),

        // Note that numrange is a non-discrete range, and therefore doesn't undergo normalization to inclusive/exclusive in PG
        new TestCaseData(NpgsqlRange<decimal>.Empty, "empty", "numrange", NpgsqlDbType.NumericRange)
            .SetName("EmptyRange"),
        new TestCaseData(new NpgsqlRange<decimal>(1, true, 10, true), "[1,10]", "numrange", NpgsqlDbType.NumericRange)
            .SetName("Inclusive"),
        new TestCaseData(new NpgsqlRange<decimal>(1, false, 10, false), "(1,10)", "numrange", NpgsqlDbType.NumericRange)
            .SetName("Exclusive"),
        new TestCaseData(new NpgsqlRange<decimal>(1, true, 10, false), "[1,10)", "numrange", NpgsqlDbType.NumericRange)
            .SetName("InclusiveExclusive"),
        new TestCaseData(new NpgsqlRange<decimal>(1, false, 10, true), "(1,10]", "numrange", NpgsqlDbType.NumericRange)
            .SetName("ExclusiveInclusive"),
        new TestCaseData(new NpgsqlRange<decimal>(1, false, true, 10, false, false), "(,10)", "numrange", NpgsqlDbType.NumericRange)
            .SetName("InfiniteLowerBound"),
        new TestCaseData(new NpgsqlRange<decimal>(1, true, false, 10, false, true), "[1,)", "numrange", NpgsqlDbType.NumericRange)
            .SetName("InfiniteUpperBound")
    ];

    // See more test cases in DateTimeTests
    [Test, TestCaseSource(nameof(RangeTestCases))]
    public Task Range<T>(T range, string sqlLiteral, string pgTypeName, NpgsqlDbType? npgsqlDbType)
        => AssertType(range, sqlLiteral, pgTypeName, npgsqlDbType,
            // NpgsqlRange<T>[] is mapped to multirange by default, not array, so the built-in AssertType testing for arrays fails
            // (see below)
            skipArrayCheck: true);

    // This re-executes the same scenario as above, but with isDefaultForWriting: false and without skipArrayCheck: true.
    // This tests coverage of range arrays (as opposed to multiranges).
    [Test, TestCaseSource(nameof(RangeTestCases))]
    public Task Range_array<T>(T range, string sqlLiteral, string pgTypeName, NpgsqlDbType? npgsqlDbType)
        => AssertType(range, sqlLiteral, pgTypeName, npgsqlDbType, isDefaultForWriting: false);

    [Test]
    public void Equality_finite()
    {
        var r1 = new NpgsqlRange<int>(0, true, false, 1, false, false);

        //different bounds
        var r2 = new NpgsqlRange<int>(1, true, false, 2, false, false);
        Assert.IsFalse(r1 == r2);

        //lower bound is not inclusive
        var r3 = new NpgsqlRange<int>(0, false, false, 1, false, false);
        Assert.IsFalse(r1 == r3);

        //upper bound is inclusive
        var r4 = new NpgsqlRange<int>(0, true, false, 1, true, false);
        Assert.IsFalse(r1 == r4);

        var r5 = new NpgsqlRange<int>(0, true, false, 1, false, false);
        Assert.IsTrue(r1 == r5);

        //check some other combinations while we are here
        Assert.IsFalse(r2 == r3);
        Assert.IsFalse(r2 == r4);
        Assert.IsFalse(r3 == r4);
    }

    [Test]
    public void Equality_infinite()
    {
        var r1 = new NpgsqlRange<int>(0, false, true, 1, false, false);

        //different upper bound (lower bound shouldn't matter since it is infinite)
        var r2 = new NpgsqlRange<int>(1, false, true, 2, false, false);
        Assert.IsFalse(r1 == r2);

        //upper bound is inclusive
        var r3 = new NpgsqlRange<int>(0, false, true, 1, true, false);
        Assert.IsFalse(r1 == r3);

        //value of lower bound shouldn't matter since it is infinite
        var r4 = new NpgsqlRange<int>(10, false, true, 1, false, false);
        Assert.IsTrue(r1 == r4);

        //check some other combinations while we are here
        Assert.IsFalse(r2 == r3);
        Assert.IsFalse(r2 == r4);
        Assert.IsFalse(r3 == r4);
    }

    [Test]
    public void GetHashCode_value_types()
    {
        NpgsqlRange<int> a = default;
        NpgsqlRange<int> b = NpgsqlRange<int>.Empty;
        NpgsqlRange<int> c = NpgsqlRange<int>.Parse("(,)");

        Assert.IsFalse(a.Equals(b));
        Assert.IsFalse(a.Equals(c));
        Assert.IsFalse(b.Equals(c));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        Assert.AreNotEqual(b.GetHashCode(), c.GetHashCode());
    }

    [Test]
    public void GetHashCode_reference_types()
    {
        NpgsqlRange<string> a= default;
        NpgsqlRange<string> b = NpgsqlRange<string>.Empty;
        NpgsqlRange<string> c = NpgsqlRange<string>.Parse("(,)");

        Assert.IsFalse(a.Equals(b));
        Assert.IsFalse(a.Equals(c));
        Assert.IsFalse(b.Equals(c));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
        Assert.AreNotEqual(b.GetHashCode(), c.GetHashCode());
    }

    [Test]
    public async Task TimestampTz_range_with_DateTimeOffset()
    {
        // The default CLR mapping for timestamptz is DateTime, but it also supports DateTimeOffset.
        // The range should also support both, defaulting to the first.
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p", conn);

        var dto1 = new DateTimeOffset(2010, 1, 3, 10, 0, 0, TimeSpan.Zero);
        var dto2 = new DateTimeOffset(2010, 1, 4, 10, 0, 0, TimeSpan.Zero);
        var range = new NpgsqlRange<DateTimeOffset>(dto1, dto2);
        cmd.Parameters.AddWithValue("p", range);
        using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();
        var actual = reader.GetFieldValue<NpgsqlRange<DateTimeOffset>>(0);
        Assert.That(actual, Is.EqualTo(range));
    }

    [Test]
    [NonParallelizable]
    public async Task Unmapped_range_with_mapped_subtype()
    {
        await using var dataSource = CreateDataSource(b => b.EnableUnmappedTypes().ConnectionStringBuilder.MaxPoolSize = 1);
        await using var conn = await dataSource.OpenConnectionAsync();

        var typeName = await GetTempTypeName(conn);
        await conn.ExecuteNonQueryAsync($"CREATE TYPE {typeName} AS RANGE(subtype=text)");
        await Task.Yield(); // TODO: fix multiplexing deadlock bug
        conn.ReloadTypes();
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));

        var value = new NpgsqlRange<char[]>(
            new string('a', conn.Settings.WriteBufferSize + 10).ToCharArray(),
            new string('z', conn.Settings.WriteBufferSize + 10).ToCharArray()
        );

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter { DataTypeName = typeName, ParameterName = "p", Value = value });
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        await reader.ReadAsync();

        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlRange<string>)));
        var result = reader.GetFieldValue<NpgsqlRange<char[]>>(0);
        Assert.That(result, Is.EqualTo(value).Using<NpgsqlRange<char[]>>((actual, expected) =>
            actual.LowerBound!.SequenceEqual(expected.LowerBound!) && actual.UpperBound!.SequenceEqual(expected.UpperBound!)));
    }

    [Test]
    public async Task Unmapped_range_supported_only_with_EnableUnmappedTypes()
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var rangeType = await GetTempTypeName(connection);
        await connection.ExecuteNonQueryAsync($"CREATE TYPE {rangeType} AS RANGE(subtype=text)");
        await Task.Yield(); // TODO: fix multiplexing deadlock bug
        await connection.ReloadTypesAsync();

        var errorMessage = string.Format(
            NpgsqlStrings.UnmappedRangesNotEnabled,
            nameof(NpgsqlSlimDataSourceBuilder.EnableUnmappedTypes),
            nameof(NpgsqlDataSourceBuilder));

        var exception = await AssertTypeUnsupportedWrite(new NpgsqlRange<string>("bar", "foo"), rangeType);
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));

        exception = await AssertTypeUnsupportedRead("""["bar","foo"]""", rangeType);
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));

        exception = await AssertTypeUnsupportedRead<NpgsqlRange<string>>("""["bar","foo"]""", rangeType);
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4441")]
    public async Task Array_of_range()
    {
        bool supportsMultirange;

        await using (var conn = await OpenConnectionAsync())
        {
            supportsMultirange = conn.PostgreSqlVersion.IsGreaterOrEqual(14);
        }

        // Starting with PG14, we map CLR NpgsqlRange<T>[] to PG multiranges by default, but also support mapping to PG array of range.
        // (wee also MultirangeTests for additional multirange-specific tests).
        // Earlier versions don't have multirange, so the default mapping is to PG array of range.

        // Note that when NpgsqlDbType inference, we don't know the PG version (since NpgsqlParameter can exist in isolation). So
        // if NpgsqlParameter.Value is set to NpgsqlRange<T>[], NpgsqlDbType always returns multirange (hence
        // isNpgsqlDbTypeInferredFromClrType is false).
        await AssertType(
            new NpgsqlRange<int>[]
            {
                new(3, lowerBoundIsInclusive: true, 4, upperBoundIsInclusive: false),
                new(5, lowerBoundIsInclusive: true, 6, upperBoundIsInclusive: false)
            },
            """{"[3,4)","[5,6)"}""",
            "int4range[]",
            NpgsqlDbType.IntegerRange | NpgsqlDbType.Array,
            isDefaultForWriting: !supportsMultirange,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    [Test]
    public async Task Ranges_not_supported_by_default_on_NpgsqlSlimSourceBuilder()
    {
        var errorMessage = string.Format(
            NpgsqlStrings.RangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRanges), nameof(NpgsqlSlimDataSourceBuilder));

        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        await using var dataSource = dataSourceBuilder.Build();

        var exception = await AssertTypeUnsupportedRead<NpgsqlRange<int>>("[1,10)", "int4range", dataSource);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));
        exception = await AssertTypeUnsupportedWrite(new NpgsqlRange<int>(1, true, 10, false), "int4range", dataSource);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task NpgsqlSlimSourceBuilder_EnableRanges()
    {
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableRanges();
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(
            dataSource,
            new NpgsqlRange<int>(1, true, 10, false), "[1,10)", "int4range", NpgsqlDbType.IntegerRange, skipArrayCheck: true);
    }

    protected override NpgsqlConnection OpenConnection()
        => throw new NotSupportedException();

    #region ParseTests

    [Theory]
    [TestCaseSource(nameof(DateTimeRangeTheoryData))]
    public void Roundtrip_DateTime_ranges_through_ToString_and_Parse(NpgsqlRange<DateTime> input)
    {
        var wellKnownText = input.ToString();
        var result = NpgsqlRange<DateTime>.Parse(wellKnownText);
        Assert.AreEqual(input, result);
    }

    [Theory]
    [TestCase("empty")]
    [TestCase("EMPTY")]
    [TestCase("  EmPtY  ")]
    public void Parse_empty(string value)
    {
        var result = NpgsqlRange<int>.Parse(value);
        Assert.AreEqual(NpgsqlRange<int>.Empty, result);
    }

    [Theory]
    [TestCase("(0,1)")]
    [TestCase("(0,1]")]
    [TestCase("[0,1)")]
    [TestCase("[0,1]")]
    [TestCase(" [ 0 , 1 ] ")]
    public void Roundtrip_int_ranges_through_ToString_and_Parse(string input)
    {
        var result = NpgsqlRange<int>.Parse(input);
        Assert.AreEqual(input.Replace(" ", null), result.ToString());
    }

    [Theory]
    [TestCase("(1,1)", "empty")]
    [TestCase("[1,1)", "empty")]
    [TestCase("[,1]", "(,1]")]
    [TestCase("[1,]", "[1,)")]
    [TestCase("[,]", "(,)")]
    [TestCase("[-infinity,infinity]", "(,)")]
    [TestCase("[ -infinity , infinity ]", "(,)")]
    [TestCase("[-infinity,infinity)", "(,)")]
    [TestCase("(-infinity,infinity]", "(,)")]
    [TestCase("(-infinity,infinity)", "(,)")]
    [TestCase("[null,null]", "(,)")]
    [TestCase("[null,infinity]", "(,)")]
    [TestCase("[-infinity,null]", "(,)")]
    public void Int_range_Parse_ToString_returns_normalized_representations(string input, string normalized)
    {
        var result = NpgsqlRange<int>.Parse(input);
        Assert.AreEqual(normalized, result.ToString());
    }

    [Theory]
    [TestCase("(1,1)", "empty")]
    [TestCase("[1,1)", "empty")]
    [TestCase("[,1]", "(,1]")]
    [TestCase("[1,]", "[1,)")]
    [TestCase("[,]", "(,)")]
    [TestCase("[-infinity,infinity]", "(,)")]
    [TestCase("[ -infinity , infinity ]", "(,)")]
    [TestCase("[-infinity,infinity)", "(,)")]
    [TestCase("(-infinity,infinity]", "(,)")]
    [TestCase("(-infinity,infinity)", "(,)")]
    [TestCase("[null,null]", "(,)")]
    [TestCase("[null,infinity]", "(,)")]
    [TestCase("[-infinity,null]", "(,)")]
    public void Nullable_int_range_Parse_ToString_returns_normalized_representations(string input, string normalized)
    {
        var result = NpgsqlRange<int?>.Parse(input);
        Assert.AreEqual(normalized, result.ToString());
    }

    [Theory]
    [TestCase("(a,a)", "empty")]
    [TestCase("[a,a)", "empty")]
    [TestCase("[a,a]", "[a,a]")]
    [TestCase("(a,b)", "(a,b)")]
    public void String_range_Parse_ToString_returns_normalized_representations(string input, string normalized)
    {
        var result = NpgsqlRange<string>.Parse(input);
        Assert.AreEqual(normalized, result.ToString());
    }

    [Theory]
    [TestCase("(one,two)")]
    public void Roundtrip_string_ranges_through_ToString_and_Parse2(string input)
    {
        var result = NpgsqlRange<SimpleType>.Parse(input);
        Assert.AreEqual(input, result.ToString());
    }

    [Theory]
    [TestCase("0, 1)")]
    [TestCase("(0 1)")]
    [TestCase("(0, 1")]
    [TestCase(" 0, 1 ")]
    public void Parse_malformed_range_throws(string input)
        => Assert.Throws<FormatException>(() => NpgsqlRange<int>.Parse(input));

    [Test, Ignore("Fails only on build server, can't reproduce locally.")]
    public void TypeConverter()
    {
        // Arrange
        NpgsqlRange<int>.RangeTypeConverter.Register();
        var converter = TypeDescriptor.GetConverter(typeof(NpgsqlRange<int>));

        // Act
        Assert.IsInstanceOf<NpgsqlRange<int>.RangeTypeConverter>(converter);
        Assert.IsTrue(converter.CanConvertFrom(typeof(string)));
        var result = converter.ConvertFromString("empty");

        // Assert
        Assert.AreEqual(NpgsqlRange<int>.Empty, result);
    }

    #endregion

    #region TheoryData

    [TypeConverter(typeof(SimpleTypeConverter))]
    class SimpleType
    {
        string? Value { get; }

        SimpleType(string? value)
            => Value = value;

        public override string? ToString()
            => Value;

        class SimpleTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => typeof(string) == sourceType;

            public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
                => new SimpleType(value.ToString());
        }
    }

    // ReSharper disable once InconsistentNaming
    static readonly DateTime May_17_2018 = DateTime.Parse("2018-05-17");

    // ReSharper disable once InconsistentNaming
    static readonly DateTime May_18_2018 = DateTime.Parse("2018-05-18");

    /// <summary>
    /// Provides theory data for <see cref="NpgsqlRange{T}"/> of <see cref="DateTime"/>.
    /// </summary>
    static object[][] DateTimeRangeTheoryData =>
        new object[][]
        {
            // (2018-05-17, 2018-05-18)
            [new NpgsqlRange<DateTime>(May_17_2018, false, false, May_18_2018, false, false)],

            // [2018-05-17, 2018-05-18]
            [new NpgsqlRange<DateTime>(May_17_2018, true, false, May_18_2018, true, false)],

            // [2018-05-17, 2018-05-18)
            [new NpgsqlRange<DateTime>(May_17_2018, true, false, May_18_2018, false, false)],

            // (2018-05-17, 2018-05-18]
            [new NpgsqlRange<DateTime>(May_17_2018, false, false, May_18_2018, true, false)],

            // (,)
            [new NpgsqlRange<DateTime>(default, false, true, default, false, true)],
            [new NpgsqlRange<DateTime>(May_17_2018, false, true, May_18_2018, false, true)],

            // (2018-05-17,)
            [new NpgsqlRange<DateTime>(May_17_2018, false, false, default, false, true)],
            [new NpgsqlRange<DateTime>(May_17_2018, false, false, May_18_2018, false, true)],

            // (,2018-05-18)
            [new NpgsqlRange<DateTime>(default, false, true, May_18_2018, false, false)],
            [new NpgsqlRange<DateTime>(May_17_2018, false, true, May_18_2018, false, false)]
        };

    #endregion

    protected override NpgsqlDataSource DataSource { get; }

    public RangeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode)
        => DataSource = CreateDataSource(builder =>
        {
            builder.ConnectionStringBuilder.Timezone = "Europe/Berlin";
        });

    [OneTimeTearDown]
    public void TearDown() => DataSource.Dispose();
}
