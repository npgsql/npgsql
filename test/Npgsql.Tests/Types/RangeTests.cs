using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Npgsql.Util;
using NpgsqlTypes;
using NUnit.Framework;

using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

/// <remarks>
/// https://www.postgresql.org/docs/current/static/rangetypes.html
/// </remarks>
class RangeTests : MultiplexingTestBase
{
    [Test, NUnit.Framework.Description("Resolves a range type handler via the different pathways")]
    public async Task Range_resolution()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing, ReloadTypes");

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Range_resolution), // Prevent backend type caching in TypeHandlerRegistry
            Pooling = false
        };

        using var conn = await OpenConnectionAsync(csb);

        // Resolve type by NpgsqlDbType
        using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            cmd.Parameters.AddWithValue("p", NpgsqlDbType.Range | NpgsqlDbType.Integer, DBNull.Value);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                Assert.That(reader.GetDataTypeName(0), Is.EqualTo("int4range"));
            }
        }

        // Resolve type by ClrType (type inference)
        conn.ReloadTypes();
        using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = new NpgsqlRange<int>(3, 5) });
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                Assert.That(reader.GetDataTypeName(0), Is.EqualTo("int4range"));
            }
        }

        // Resolve type by DataTypeName
        conn.ReloadTypes();
        using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName="p", DataTypeName = "int4range", Value = DBNull.Value });
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                Assert.That(reader.GetDataTypeName(0), Is.EqualTo("int4range"));
            }
        }

        // Resolve type by OID (read)
        conn.ReloadTypes();
        using (var cmd = new NpgsqlCommand("SELECT int4range(3, 5)", conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("int4range"));
            Assert.That(reader.GetFieldValue<NpgsqlRange<int>>(0), Is.EqualTo(new NpgsqlRange<int>(3, true, 5, false)));
        }
    }

    [Test]
    public async Task Range()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn);
        var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Range | NpgsqlDbType.Integer) { Value = NpgsqlRange<int>.Empty };
        var p2 = new NpgsqlParameter { ParameterName = "p2", Value = new NpgsqlRange<int>(1, 10) };
        var p3 = new NpgsqlParameter { ParameterName = "p3", Value = new NpgsqlRange<int>(1, false, 10, false) };
        var p4 = new NpgsqlParameter { ParameterName = "p4", Value = new NpgsqlRange<int>(0, false, true, 10, false, false) };
        Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Range | NpgsqlDbType.Integer));
        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);
        cmd.Parameters.Add(p3);
        cmd.Parameters.Add(p4);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        Assert.That(reader[0].ToString(), Is.EqualTo("empty"));
        Assert.That(reader[1].ToString(), Is.EqualTo("[1,11)"));
        Assert.That(reader[2].ToString(), Is.EqualTo("[2,10)"));
        Assert.That(reader[3].ToString(), Is.EqualTo("(,10)"));
    }

    [Test]
    [NonParallelizable]
    public async Task Range_with_long_subtype()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            MaxPoolSize = 1
        };
        await using var conn = await OpenConnectionAsync(csb);

        var typeName = await GetTempTypeName(conn);
        await conn.ExecuteNonQueryAsync($"CREATE TYPE {typeName} AS RANGE(subtype=text)");
        await Task.Yield(); // TODO: fix multiplexing deadlock bug
        conn.ReloadTypes();
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));

        var value = new NpgsqlRange<string>(
            new string('a', conn.Settings.WriteBufferSize + 10),
            new string('z', conn.Settings.WriteBufferSize + 10)
        );

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Range | NpgsqlDbType.Text) { Value = value });
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        await reader.ReadAsync();
        Assert.That(reader[0], Is.EqualTo(value));
    }

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

        //different upper bound (lower bound shoulnd't matter since it is infinite)
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
            @"{""[3,4)"",""[5,6)""}",
            "int4range[]",
            NpgsqlDbType.IntegerRange | NpgsqlDbType.Array,
            isDefaultForWriting: !supportsMultirange,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.2.0");
    }

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
        {
            Value = value;
        }

        public override string? ToString()
        {
            return Value;
        }

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
            new object[] { new NpgsqlRange<DateTime>(May_17_2018, false, false, May_18_2018, false, false) },

            // [2018-05-17, 2018-05-18]
            new object[] { new NpgsqlRange<DateTime>(May_17_2018, true, false, May_18_2018, true, false) },

            // [2018-05-17, 2018-05-18)
            new object[] { new NpgsqlRange<DateTime>(May_17_2018, true, false, May_18_2018, false, false) },

            // (2018-05-17, 2018-05-18]
            new object[] { new NpgsqlRange<DateTime>(May_17_2018, false, false, May_18_2018, true, false) },

            // (,)
            new object[] { new NpgsqlRange<DateTime>(default, false, true, default, false, true) },
            new object[] { new NpgsqlRange<DateTime>(May_17_2018, false, true, May_18_2018, false, true) },

            // (2018-05-17,)
            new object[] { new NpgsqlRange<DateTime>(May_17_2018, false, false, default, false, true) },
            new object[] { new NpgsqlRange<DateTime>(May_17_2018, false, false, May_18_2018, false, true) },

            // (,2018-05-18)
            new object[] { new NpgsqlRange<DateTime>(default, false, true, May_18_2018, false, false) },
            new object[] { new NpgsqlRange<DateTime>(May_17_2018, false, true, May_18_2018, false, false) }
        };

    #endregion

    public RangeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
