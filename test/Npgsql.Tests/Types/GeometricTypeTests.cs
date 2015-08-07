#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL geometric types
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    class GeometricTypeTests : TestBase
    {
        [Test]
        public void Point()
        {
            var expected = new NpgsqlPoint(1.2, 3.4);
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Point) { Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Point));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlPoint)));
                Assert.That(reader[i], Is.EqualTo(expected));
            }
        }

        [Test]
        public void LineSegment()
        {
            var expected = new NpgsqlLSeg(1, 2 ,3, 4);
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.LSeg) { Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.LSeg));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++) {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlLSeg)));
                Assert.That(reader[i], Is.EqualTo(expected));
            }
        }

        [Test]
        public void Box()
        {
            var expected = new NpgsqlBox(2, 4, 1, 3);
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Box) { Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Box));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++) {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlBox)));
                Assert.That(reader[i], Is.EqualTo(expected));
            }
        }

        [Test]
        public void Path()
        {
            var expectedOpen = new NpgsqlPath(new[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }, true);
            var expectedClosed = new NpgsqlPath(new[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }, false);
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Path) { Value = expectedOpen };
            var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Path) { Value = expectedClosed };
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expectedClosed };
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Path));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                var expected = i == 0 ? expectedOpen : expectedClosed;
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlPath)));
                Assert.That(reader[i], Is.EqualTo(expected));
            }
        }

        [Test]
        public void Polygon()
        {
            var expected = new NpgsqlPolygon(new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4));
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Polygon) { Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Polygon));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++) {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlPolygon)));
                Assert.That(reader[i], Is.EqualTo(expected));
            }
        }

        [Test]
        public void Circle()
        {
            var expected = new NpgsqlCircle(1, 2, 0.5);
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Circle) { Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Circle));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++) {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlCircle)));
                Assert.That(reader[i], Is.EqualTo(expected));
            }
        }

#if NOT_IMPLEMENTED_BY_POSTGRESQL
        [Test]
        public void Line()
        {
            var expected = new NpgsqlLine(1, 2, 3);
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Line) { Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Line));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++) {
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(NpgsqlLine)));
                Assert.That(reader[i], Is.EqualTo(expected));
            }
        }
#endif

        public GeometricTypeTests(string backendVersion) : base(backendVersion) {}
    }
}
