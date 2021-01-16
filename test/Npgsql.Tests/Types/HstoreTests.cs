using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public class HstoreTests : MultiplexingTestBase
    {
        [Test]
        public async Task Basic()
        {
            using var conn = await OpenConnectionAsync();

            var expected = new Dictionary<string, string?> {
                {"a", "3"},
                {"b", null},
                {"cd", "hello"}
            };

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Hstore, expected);
            cmd.Parameters.AddWithValue("p2", expected);

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Dictionary<string, string>)));
                Assert.That(reader.GetFieldValue<Dictionary<string, string>>(i), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<IDictionary<string, string>>(i), Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task Empty()
        {
            using var conn = await OpenConnectionAsync();

            var expected = new Dictionary<string, string?>();

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Hstore, expected);
            cmd.Parameters.AddWithValue("p2", expected);

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Dictionary<string, string>)));
                Assert.That(reader.GetFieldValue<Dictionary<string, string>>(i), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<IDictionary<string, string>>(i), Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task ImmutableDictionary()
        {
            using var conn = await OpenConnectionAsync();

            var builder = ImmutableDictionary<string, string?>.Empty;
            builder.Add("a", "3");
            builder.Add("b", null);
            builder.Add("cd", "hello");
            var expected = builder.ToImmutableDictionary();

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Hstore, expected);
            cmd.Parameters.AddWithValue("p2", expected);

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Dictionary<string, string>)));
                Assert.That(reader.GetFieldValue<ImmutableDictionary<string, string?>>(i), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<ImmutableDictionary<string, string?>>(i), Is.EqualTo(expected));
            }
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            using var conn = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(conn, "9.1", "Hstore introduced in PostgreSQL 9.1");
            await TestUtil.EnsureExtensionAsync(conn, "hstore", "9.1");
        }

        public HstoreTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
