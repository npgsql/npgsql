using System.Collections.Generic;
using NpgsqlTypes;
using NUnit.Framework;

#if !NET461
using System.Collections.Immutable;
#endif

namespace Npgsql.Tests.Types
{
    public class HstoreTests : TestBase
    {
        [Test]
        public void Basic()
        {
            using var conn = OpenConnection();

            var expected = new Dictionary<string, string?> {
                {"a", "3"},
                {"b", null},
                {"cd", "hello"}
            };

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Hstore, expected);
            cmd.Parameters.AddWithValue("p2", expected);

            using var reader = cmd.ExecuteReader();
            reader.Read();
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Dictionary<string, string>)));
                Assert.That(reader.GetFieldValue<Dictionary<string, string>>(i), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<IDictionary<string, string>>(i), Is.EqualTo(expected));
            }
        }

        [Test]
        public void Empty()
        {
            using var conn = OpenConnection();

            var expected = new Dictionary<string, string?>();

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Hstore, expected);
            cmd.Parameters.AddWithValue("p2", expected);

            using var reader = cmd.ExecuteReader();
            reader.Read();
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Dictionary<string, string>)));
                Assert.That(reader.GetFieldValue<Dictionary<string, string>>(i), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<IDictionary<string, string>>(i), Is.EqualTo(expected));
            }
        }

#if !NET461
        [Test]
        public void ImmutableDictionary()
        {
            using var conn = OpenConnection();

            var builder = ImmutableDictionary<string, string?>.Empty;
            builder.Add("a", "3");
            builder.Add("b", null);
            builder.Add("cd", "hello");
            var expected = builder.ToImmutableDictionary();

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Hstore, expected);
            cmd.Parameters.AddWithValue("p2", expected);

            using var reader = cmd.ExecuteReader();
            reader.Read();
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Dictionary<string, string>)));
                Assert.That(reader.GetFieldValue<ImmutableDictionary<string, string?>>(i), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<ImmutableDictionary<string, string?>>(i), Is.EqualTo(expected));
            }
        }
#endif

        [OneTimeSetUp]
        public void SetUp()
        {
            using var conn = OpenConnection();
            TestUtil.MinimumPgVersion(conn, "9.1", "Hstore introduced in PostgreSQL 9.1");
            TestUtil.EnsureExtension(conn, "hstore", "9.1");
        }
    }
}
