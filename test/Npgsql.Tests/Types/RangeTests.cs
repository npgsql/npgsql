using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    [MinPgVersion(9, 2, 0, "Ranges supported only starting PostgreSQL 9.2")]
    class RangeTests : TestBase
    {
        public RangeTests(string backendVersion) : base(backendVersion) { }

        [Test]
        public void Range()
        {
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Range | NpgsqlDbType.Integer) { Value = NpgsqlRange<int>.Empty() };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = new NpgsqlRange<int>(1, 10) };
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = new NpgsqlRange<int>(1, false, 10, false) };
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = new NpgsqlRange<int>(0, false, true, 10, false, false) };
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Range | NpgsqlDbType.Integer));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            var reader = cmd.ExecuteReader();
            reader.Read();

            Assert.That(reader[0].ToString(), Is.EqualTo("empty"));
            Assert.That(reader[1].ToString(), Is.EqualTo("[1,11)"));
            Assert.That(reader[2].ToString(), Is.EqualTo("[2,10)"));
            Assert.That(reader[3].ToString(), Is.EqualTo("(,10)"));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void RangeWithLongSubtype()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.textrange AS RANGE(subtype=text)");
            Conn.ReloadTypes();
            Assert.That(ExecuteScalar("SELECT 1"), Is.EqualTo(1));

            var value = new NpgsqlRange<string>(
                new string('a', Conn.BufferSize + 10),
                new string('z', Conn.BufferSize + 10)
            );

            //var value = new NpgsqlRange<string>("bar", "foo");
            using (var cmd = new NpgsqlCommand("SELECT @p", Conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Range | NpgsqlDbType.Text) { Value = value });
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    reader.Read();
                    Assert.That(reader[0], Is.EqualTo(value));
                }
            }
        }

        [Test]
        public void TestRange()
        {
            using (var cmd = Conn.CreateCommand())
            {
                object obj;

                cmd.CommandText = "select '[2,10)'::int4range";
                cmd.Prepare();
                obj = cmd.ExecuteScalar();
                Assert.AreEqual(new NpgsqlRange<int>(2, true, false, 10, false, false), obj);

                cmd.CommandText = "select array['[2,10)'::int4range, '[3,9)'::int4range]";
                cmd.Prepare();
                obj = cmd.ExecuteScalar();
                Assert.AreEqual(new NpgsqlRange<int>(3, true, false, 9, false, false), ((NpgsqlRange<int>[])obj)[1]);
            }
        }
    }
}
