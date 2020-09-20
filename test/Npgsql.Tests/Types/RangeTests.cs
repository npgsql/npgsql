using System;
using System.Data;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    class RangeTests : MultiplexingTestBase
    {
        [Test, NUnit.Framework.Description("Resolves a range type handler via the different pathways")]
        public async Task RangeTypeResolution()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, ReloadTypes");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(RangeTypeResolution), // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = await OpenConnectionAsync(csb))
            {
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

                // Resolve type by OID (read)
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT int4range(3, 5)", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo("int4range"));
                }
            }
        }

        [Test]
        public async Task Range()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Range | NpgsqlDbType.Integer) { Value = NpgsqlRange.Empty<int>() };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = NpgsqlRange.CreateInclusive(1, 10) };
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = NpgsqlRange.CreateExclusive(1, 10) };
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = NpgsqlRange.Create(NpgsqlRangeBound.Infinite<int>(), 10) };

            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Range | NpgsqlDbType.Integer));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader[0], Is.EqualTo(NpgsqlRange.Empty<int>()));
            Assert.That(reader[1], Is.EqualTo(NpgsqlRange.Create(1, 11)));
            Assert.That(reader[2], Is.EqualTo(NpgsqlRange.Create(2, 10)));
            Assert.That(reader[3], Is.EqualTo(NpgsqlRange.Create(NpgsqlRangeBound.Infinite<int>(), 10)));
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

            using var cmd = new NpgsqlCommand("SELECT @p", conn) { Parameters = { new NpgsqlParameter("p", NpgsqlDbType.Range | NpgsqlDbType.Text) { Value = value } } };
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
            reader.Read();
            Assert.That(reader[0], Is.EqualTo(value));
        }

        [Test]
        public void RangeEquality_FiniteRange()
        {
            var r1 = new NpgsqlRange<int>(
                NpgsqlRangeBound.Inclusive(0),
                NpgsqlRangeBound.Exclusive(1));

            //different bounds
            var r2 = new NpgsqlRange<int>(
                NpgsqlRangeBound.Inclusive(1),
                NpgsqlRangeBound.Exclusive(2));
            Assert.IsFalse(r1 == r2);

            //lower bound is not inclusive
            var r3 = new NpgsqlRange<int>(
                NpgsqlRangeBound.Exclusive(0),
                NpgsqlRangeBound.Exclusive(1));

            //upper bound is inclusive
            var r4 = new NpgsqlRange<int>(
                NpgsqlRangeBound.Inclusive(0),
                NpgsqlRangeBound.Inclusive(1));
            Assert.IsFalse(r1 == r4);

            var r5 = new NpgsqlRange<int>(
                NpgsqlRangeBound.Inclusive(0),
                NpgsqlRangeBound.Exclusive(1));
            Assert.IsTrue(r1 == r5);

            //check some other combinations while we are here
            Assert.IsFalse(r2 == r3);
            Assert.IsFalse(r2 == r4);
            Assert.IsFalse(r3 == r4);
        }

        [Test]
        public void RangeEquality_InfiniteRange()
        {
            var r1 = new NpgsqlRange<int>(
                NpgsqlRangeBound.Infinite<int>(),
                NpgsqlRangeBound.Exclusive(1));

            //upper bound is inclusive
            var r2 = new NpgsqlRange<int>(
                NpgsqlRangeBound.Infinite<int>(),
                NpgsqlRangeBound.Inclusive(1));
            Assert.IsFalse(r1 == r2);

            //value of lower bound shoulnd't matter since it is infinite
            var r3 = new NpgsqlRange<int>(
                NpgsqlRangeBound.Infinite<int>(),
                NpgsqlRangeBound.Exclusive(1));
            Assert.IsTrue(r1 == r3);

            //check some other combinations while we are here
            Assert.IsFalse(r2 == r3);
        }

        [Test]
        public void RangeHashCode_ValueTypes()
        {
            NpgsqlRange<int> a = default;
            var b = NpgsqlRange.Empty<int>();

            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void RangeHashCode_ReferenceTypes()
        {
            NpgsqlRange<string> a = default;
            var b = NpgsqlRange.Empty<string>();

            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
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
            using (var conn = await OpenConnectionAsync())
                TestUtil.MinimumPgVersion(conn, "9.2.0");
        }

        public RangeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
