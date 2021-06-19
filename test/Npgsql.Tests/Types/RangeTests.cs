using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
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
        public async Task RangeWithLongSubtype()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, ReloadTypes");

            using var conn = await OpenConnectionAsync();
            await conn.ExecuteNonQueryAsync("CREATE TYPE pg_temp.textrange AS RANGE(subtype=text)");
            conn.ReloadTypes();
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));

            var value = new NpgsqlRange<string>(
                new string('a', conn.Settings.WriteBufferSize + 10),
                new string('z', conn.Settings.WriteBufferSize + 10)
            );

            //var value = new NpgsqlRange<string>("bar", "foo");
            using var cmd = new NpgsqlCommand("SELECT @p", conn);
            cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Range | NpgsqlDbType.Text) { Value = value });
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
            reader.Read();
            Assert.That(reader[0], Is.EqualTo(value));
        }

        [Test]
        public void RangeEquality_FiniteRange()
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
        public void RangeEquality_InfiniteRange()
        {
            var r1 = new NpgsqlRange<int>(0, false, true, 1, false, false);

            //different upper bound (lower bound shoulnd't matter since it is infinite)
            var r2 = new NpgsqlRange<int>(1, false, true, 2, false, false);
            Assert.IsFalse(r1 == r2);

            //upper bound is inclusive
            var r3 = new NpgsqlRange<int>(0, false, true, 1, true, false);
            Assert.IsFalse(r1 == r3);

            //value of lower bound shoulnd't matter since it is infinite
            var r4 = new NpgsqlRange<int>(10, false, true, 1, false, false);
            Assert.IsTrue(r1 == r4);

            //check some other combinations while we are here
            Assert.IsFalse(r2 == r3);
            Assert.IsFalse(r2 == r4);
            Assert.IsFalse(r3 == r4);
        }

        [Test]
        public void RangeHashCode_ValueTypes()
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
        public void RangeHashCode_ReferenceTypes()
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
        public async Task TimestampTzRangeWithDateTimeOffset()
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

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            using var conn = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(conn, "9.2.0");
        }

        #region ParseTests

        [Theory]
        [TestCaseSource(nameof(DateTimeRangeTheoryData))]
        public void GivenDateRangeLiteral_WhenConverted_ThenReturnsDateRange(NpgsqlRange<DateTime> input)
        {
            // Arrange
            var wellKnownText = input.ToString();

            // Act
            var result = NpgsqlRange<DateTime>.Parse(wellKnownText);

            // Assert
            Assert.AreEqual(input, result);
        }

        [Theory]
        [TestCase("empty")]
        [TestCase("EMPTY")]
        [TestCase("  EmPtY  ")]
        public void GivenEmptyIntRangeLiteral_WhenParsed_ThenReturnsEmptyIntRange(string value)
        {
            // Act
            var result = NpgsqlRange<int>.Parse(value);

            // Assert
            Assert.AreEqual(NpgsqlRange<int>.Empty, result);
        }

        [Theory]
        [TestCase("(0,1)")]
        [TestCase("(0,1]")]
        [TestCase("[0,1)")]
        [TestCase("[0,1]")]
        [TestCase(" [ 0 , 1 ] ")]
        public void GivenIntRangeLiteral_WhenParsed_ThenReturnsIntRange(string input)
        {
            // Act
            var result = NpgsqlRange<int>.Parse(input);

            // Assert
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
        public void GivenPoorlyFormedIntRangeLiteral_WhenParsed_ThenReturnsIntRange(string input, string normalized)
        {
            // Act
            var result = NpgsqlRange<int>.Parse(input);

            // Assert
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
        public void GivenPoorlyFormedNullableIntRangeLiteral_WhenParsed_ThenReturnsNullableIntRange(string input, string normalized)
        {
            // Act
            var result = NpgsqlRange<int?>.Parse(input);

            // Assert
            Assert.AreEqual(normalized, result.ToString());
        }

        [Theory]
        [TestCase("(a,a)", "empty")]
        [TestCase("[a,a)", "empty")]
        [TestCase("[a,a]", "[a,a]")]
        [TestCase("(a,b)", "(a,b)")]
        public void GivenStringRangeLiteral_WhenParsed_ThenReturnsStringRange(string input, string normalized)
        {
            // Act
            var result = NpgsqlRange<string>.Parse(input);

            // Assert
            Assert.AreEqual(normalized, result.ToString());
        }

        [Theory]
        [TestCase("(one,two)")]
        public void GivenSimpleTypeRangeLiteral_WhenParsed_ThenReturnsSimpleTypeRange(string input)
        {
            // Act
            var result = NpgsqlRange<SimpleType>.Parse(input);

            // Assert
            Assert.AreEqual(input, result.ToString());
        }

        [Theory]
        [TestCase("0, 1)")]
        [TestCase("(0 1)")]
        [TestCase("(0, 1")]
        [TestCase(" 0, 1 ")]
        public void GivenMalformedRangeLiteral_WhenParsed_ThenThrowsFormatException(string input)
        {
            Assert.Throws<FormatException>(() => NpgsqlRange<int>.Parse(input));
        }

        [Test, Ignore("Fails only on build server, can't reproduce locally.")]
        public void CanGetTypeConverter()
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
}
