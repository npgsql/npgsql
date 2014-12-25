using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests.Types
{
    /// <summary>
    /// Tests on PostgreSQL geometric types
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    class GeometricTypeTests : TestBase
    {
        [Test]
        public void ReadPoint([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT '(1.2,3.4)'::POINT", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            var p = (NpgsqlPoint)reader.GetValue(0);
            Assert.That(p.X, Is.EqualTo(1.2));
            Assert.That(p.Y, Is.EqualTo(3.4));
            Assert.That(reader.GetString(0), Is.EqualTo("(1.2,3.4)"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlPoint)));
        }

        [Test]
        public void ReadLine([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT '{1,2,3}'::LINE", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            var l = (NpgsqlLine)reader.GetValue(0);
            Assert.That(l, Is.EqualTo(new NpgsqlLine(1, 2, 3)));
            Assert.That(reader.GetString(0), Is.EqualTo("{1,2,3}"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlLine)));
        }

        [Test]
        public void ReadLineSegment([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT '[(1,2),(3,4)]'::LSEG", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            var l = (NpgsqlLSeg)reader.GetValue(0);
            Assert.That(l, Is.EqualTo(new NpgsqlLSeg(1, 2, 3, 4)));
            Assert.That(reader.GetString(0), Is.EqualTo("[(1,2),(3,4)]"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlLSeg)));
        }

        [Test]
        public void ReadBox([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT '(4,3),(2,1)'::BOX", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            var b = (NpgsqlBox)reader.GetValue(0);
            //var expected = new NpgsqlBox(new NpgsqlPoint(4, 1), new NpgsqlPoint(2, 3));
            Assert.That(b, Is.EqualTo(new NpgsqlBox(new NpgsqlPoint(4, 3), new NpgsqlPoint(2, 1))));
            Assert.That(reader.GetString(0), Is.EqualTo("(4,3),(2,1)"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlBox)));
        }

        [Test]
        public void ReadPath([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT '((1,2),(3,4))'::PATH, '[(1,2),(3,4)]'::PATH", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            var closed = (NpgsqlPath)reader.GetValue(0);
            Assert.That(closed.Open, Is.False);
            Assert.That(closed, Is.EqualTo(new NpgsqlPath(new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4))));
            Assert.That(reader.GetString(0), Is.EqualTo("((1,2),(3,4))"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlPath)));

            var open = (NpgsqlPath)reader.GetValue(1);
            Assert.That(open.Open, Is.True);
            Assert.That(open, Is.EqualTo(new NpgsqlPath(new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4))));
            Assert.That(reader.GetString(1), Is.EqualTo("[(1,2),(3,4)]"));
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(NpgsqlPath)));
        }

        [Test]
        public void ReadPolygon([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT '((1,2),(3,4))'::POLYGON", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            var p = (NpgsqlPolygon)reader.GetValue(0);
            Assert.That(p, Is.EqualTo(new NpgsqlPolygon(new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4))));
            Assert.That(reader.GetString(0), Is.EqualTo("((1,2),(3,4))"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlPolygon)));
        }

        [Test]
        public void ReadCircle([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT '<(1,2),0.5>'::CIRCLE", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            var c = (NpgsqlCircle)reader.GetValue(0);
            Assert.That(c, Is.EqualTo(new NpgsqlCircle(1, 2, 0.5)));
            Assert.That(reader.GetString(0), Is.EqualTo("<(1,2),0.5>"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlCircle)));
        }

        [Test]
        public void Int32WithoutQuotesPolygon()
        {
            var a = new NpgsqlCommand("select 'polygon ((:a :b))' ", Conn);
            a.Parameters.Add(new NpgsqlParameter("a", 1));
            a.Parameters.Add(new NpgsqlParameter("b", 1));
            a.ExecuteScalar();
        }

        [Test]
        public void Int32WithoutQuotesPolygon2()
        {
            var a = new NpgsqlCommand("select 'polygon ((:a :b))' ", Conn);
            a.Parameters.Add(new NpgsqlParameter("a", 1)).DbType = DbType.Int32;
            a.Parameters.Add(new NpgsqlParameter("b", 1)).DbType = DbType.Int32;
            a.ExecuteScalar();
        }

        public GeometricTypeTests(string backendVersion) : base(backendVersion) {}
    }
}
