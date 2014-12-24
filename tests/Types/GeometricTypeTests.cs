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
        public void TestPointSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_point) VALUES ( '(4, 3)' )");
            var command = new NpgsqlCommand("select field_point from data", Conn);
            var p = (NpgsqlPoint)command.ExecuteScalar();
            Assert.AreEqual(4, p.X);
            Assert.AreEqual(3, p.Y);
        }

        [Test]
        public void TestBoxSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_box) VALUES ( '(4, 3), (5, 4)'::box )");
            var command = new NpgsqlCommand("select field_box from data", Conn);
            var box = (NpgsqlBox)command.ExecuteScalar();
            Assert.AreEqual(5, box.UpperRight.X);
            Assert.AreEqual(4, box.UpperRight.Y);
            Assert.AreEqual(4, box.LowerLeft.X);
            Assert.AreEqual(3, box.LowerLeft.Y);
        }

        [Test]
        public void TestLSegSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_lseg) VALUES ( '(4, 3), (5, 4)'::lseg )");
            var command = new NpgsqlCommand("select field_lseg from data", Conn);
            var lseg = (NpgsqlLSeg)command.ExecuteScalar();
            Assert.AreEqual(4, lseg.Start.X);
            Assert.AreEqual(3, lseg.Start.Y);
            Assert.AreEqual(5, lseg.End.X);
            Assert.AreEqual(4, lseg.End.Y);
        }

        [Test]
        public void TestClosedPathSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_path) VALUES ( '((4, 3), (5, 4))'::path )");
            var command = new NpgsqlCommand("select field_path from data", Conn);
            var path = (NpgsqlPath)command.ExecuteScalar();
            Assert.AreEqual(false, path.Open);
            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(4, path[0].X);
            Assert.AreEqual(3, path[0].Y);
            Assert.AreEqual(5, path[1].X);
            Assert.AreEqual(4, path[1].Y);
        }

        [Test]
        public void TestOpenPathSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_path) VALUES ( '[(4, 3), (5, 4)]'::path )");
            var command = new NpgsqlCommand("select field_path from data", Conn);
            var path = (NpgsqlPath)command.ExecuteScalar();
            Assert.AreEqual(true, path.Open);
            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(4, path[0].X);
            Assert.AreEqual(3, path[0].Y);
            Assert.AreEqual(5, path[1].X);
            Assert.AreEqual(4, path[1].Y);
        }

        [Test]
        public void TestPolygonSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_polygon) VALUES ( '((4, 3), (5, 4))'::polygon )");
            var command = new NpgsqlCommand("select field_polygon from data", Conn);
            var polygon = (NpgsqlPolygon)command.ExecuteScalar();
            Assert.AreEqual(2, polygon.Count);
            Assert.AreEqual(4, polygon[0].X);
            Assert.AreEqual(3, polygon[0].Y);
            Assert.AreEqual(5, polygon[1].X);
            Assert.AreEqual(4, polygon[1].Y);
        }

        [Test]
        public void TestCircleSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_circle) VALUES ( '< (4, 3), 5 >'::circle )");
            var command = new NpgsqlCommand("select field_circle from data", Conn);
            var circle = (NpgsqlCircle)command.ExecuteScalar();
            Assert.AreEqual(4, circle.Center.X);
            Assert.AreEqual(3, circle.Center.Y);
            Assert.AreEqual(5, circle.Radius);
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
