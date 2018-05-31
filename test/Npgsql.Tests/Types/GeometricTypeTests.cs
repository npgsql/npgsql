#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
            using (var conn = OpenConnection())
            {
                var expected = new NpgsqlPoint(1.2, 3.4);
                var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Point) {Value = expected};
                var p2 = new NpgsqlParameter {ParameterName = "p2", Value = expected};
                Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Point));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = cmd.ExecuteReader()) {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlPoint)));
                        var actual = reader.GetFieldValue<NpgsqlPoint>(i);
                        AssertPointsEqual(actual, expected);
                    }
                }
            }
        }

        [Test]
        public void LineSegment()
        {
            using (var conn = OpenConnection())
            {
                var expected = new NpgsqlLSeg(1, 2, 3, 4);
                var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.LSeg) {Value = expected};
                var p2 = new NpgsqlParameter {ParameterName = "p2", Value = expected};
                Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.LSeg));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlLSeg)));
                        var actual = reader.GetFieldValue<NpgsqlLSeg>(i);
                        AssertPointsEqual(actual.Start, expected.Start);
                        AssertPointsEqual(actual.End, expected.End);
                    }
                }
            }
        }

        [Test]
        public void Box()
        {
            using (var conn = OpenConnection())
            {
                var expected = new NpgsqlBox(2, 4, 1, 3);
                var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Box) {Value = expected};
                var p2 = new NpgsqlParameter {ParameterName = "p2", Value = expected};
                Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Box));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlBox)));
                        var actual = reader.GetFieldValue<NpgsqlBox>(i);
                        AssertPointsEqual(actual.UpperRight, expected.UpperRight);
                    }
                }
            }
        }

        [Test]
        public void Path()
        {
            using (var conn = OpenConnection())
            {
                var expectedOpen = new NpgsqlPath(new[] {new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4)}, true);
                var expectedClosed = new NpgsqlPath(new[] {new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4)}, false);
                var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Path) {Value = expectedOpen};
                var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Path) {Value = expectedClosed};
                var p3 = new NpgsqlParameter {ParameterName = "p3", Value = expectedClosed};
                Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Path));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        var expected = i == 0 ? expectedOpen : expectedClosed;
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlPath)));
                        var actual = reader.GetFieldValue<NpgsqlPath>(i);
                        Assert.That(actual.Open, Is.EqualTo(expected.Open));
                        Assert.That(actual, Has.Count.EqualTo(expected.Count));
                        for (var j = 0; j < actual.Count; j++)
                            AssertPointsEqual(actual[j], expected[j]);
                    }
                }
            }
        }

        [Test]
        public void Polygon()
        {
            using (var conn = OpenConnection())
            {
                var expected = new NpgsqlPolygon(new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4));
                var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Polygon) {Value = expected};
                var p2 = new NpgsqlParameter {ParameterName = "p2", Value = expected};
                Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Polygon));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlPolygon)));
                        var actual = reader.GetFieldValue<NpgsqlPolygon>(i);
                        Assert.That(actual, Has.Count.EqualTo(expected.Count));
                        for (var j = 0; j < actual.Count; j++)
                            AssertPointsEqual(actual[j], expected[j]);
                    }
                }
            }
        }

        [Test]
        public void Circle()
        {
            using (var conn = OpenConnection())
            {
                var expected = new NpgsqlCircle(1, 2, 0.5);
                var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Circle) {Value = expected};
                var p2 = new NpgsqlParameter {ParameterName = "p2", Value = expected};
                Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Circle));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(NpgsqlCircle)));
                        var actual = reader.GetFieldValue<NpgsqlCircle>(i);
                        Assert.That(actual.X, Is.EqualTo(expected.X).Within(1).Ulps);
                        Assert.That(actual.Y, Is.EqualTo(expected.Y).Within(1).Ulps);
                        Assert.That(actual.Radius, Is.EqualTo(expected.Radius).Within(1).Ulps);
                    }
                }
            }
        }

        void AssertPointsEqual(NpgsqlPoint actual, NpgsqlPoint expected)
        {
            Assert.That(actual.X, Is.EqualTo(expected.X).Within(1).Ulps);
            Assert.That(actual.Y, Is.EqualTo(expected.Y).Within(1).Ulps);
        }
    }
}
