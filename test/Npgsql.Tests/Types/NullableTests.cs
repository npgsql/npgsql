using NUnit.Framework;
using System;

namespace Npgsql.Tests.Types
{
    public class NullableTests : TestBase
    {
        [Test]
        public void ReadNullable()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT NULL::integer", conn))
            using (var rdr = cmd.ExecuteRecord())
            {
                Assert.That(rdr.GetFieldType(0), Is.EqualTo(typeof(int)));
                Assert.That(rdr.GetDataTypeName(0), Is.EqualTo("integer"));
                Assert.That(() => rdr.GetFieldValue<object>(0), Throws.TypeOf<InvalidCastException>());
                Assert.That(() => rdr.GetFieldValue<int>(0), Throws.TypeOf<InvalidCastException>());
                Assert.That(() => rdr.GetFieldValue<int?>(0), Throws.Nothing);
                Assert.That(rdr.GetFieldValue<int?>(0), Is.Null);
            }
        }

        [Test]
        public void ReadNonNullable()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT NULL::text", conn))
            using (var rdr = cmd.ExecuteRecord())
            {
                Assert.That(rdr.GetFieldType(0), Is.EqualTo(typeof(string)));
                Assert.That(rdr.GetDataTypeName(0), Is.EqualTo("text"));
                Assert.That(() => rdr.GetFieldValue<object>(0), Throws.TypeOf<InvalidCastException>());
                Assert.That(() => rdr.GetFieldValue<string>(0), Throws.TypeOf<InvalidCastException>());
            }
        }

        [Test]
        public void WriteNullable()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<int?>("p1", null));
                cmd.Parameters.Add(new NpgsqlParameter<int?>("p2", 42));
                Assert.That(() => cmd.ExecuteNonQuery(), Throws.Nothing);
            }
        }

        [Test]
        public void WriteNonNullable()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter<string>("p", null));
                Assert.That(() => cmd.ExecuteNonQuery(), Throws.TypeOf<InvalidCastException>());
            }
        }
    }
}
