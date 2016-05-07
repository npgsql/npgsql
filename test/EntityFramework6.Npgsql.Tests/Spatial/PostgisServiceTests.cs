using NpgsqlTypes;
using NUnit.Framework;
using System;
using Npgsql.Spatial;

namespace Npgsql.Tests.Spatial
{
    public class PostgisServiceTests : TestBase
    {
        NpgsqlConnection _conn;
        NpgsqlConnection Conn
        {
            get
            {
                if (_conn == null)
                {
                    _conn = new NpgsqlConnection(ConnectionString);
                    _conn.Open();
                }
                return _conn;
            }
        }

        [Test()]
        public void AsBinaryTest()
        {
            var p = new PostgisPoint(1D, 1D);
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.AsBinary(svcs.GeometryFromProviderValue(p));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void AsBinaryTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void AsGmlTest()
        {
            var p = new PostgisPoint(1D, 1D);
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.AsGml(svcs.GeometryFromProviderValue(p));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void AsGmlTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void AsTextTest()
        {
            var p = new PostgisPoint(1D, 1D);
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.AsText(svcs.GeometryFromProviderValue(p));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void AsTextTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void BufferTest()
        {
            var p = new PostgisPoint(1D, 1D);
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.Buffer(svcs.GeometryFromProviderValue(p), 19D);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void BufferTest1()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void ContainsTest()
        {
            var p = new PostgisPoint(1D, 1D);
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            var pol = new PostgisPolygon(new Coordinate2D[1][]
            {new Coordinate2D[5]
                {
                    new Coordinate2D(0D,0D),
                    new Coordinate2D(5D,0D),
                    new Coordinate2D(5D,5D),
                    new Coordinate2D(0D,5D),
                    new Coordinate2D(0D,0D)
                }
            });
            try
            {
                Assert.True(svcs.Contains(svcs.GeometryFromProviderValue(pol), svcs.GeometryFromProviderValue(p)));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void CreateProviderValueTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.CreateProviderValue(
                svcs.CreateWellKnownValue(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)))
                );
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void CreateProviderValueTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void CreateWellKnownValueTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.CreateWellKnownValue(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void CreateWellKnownValueTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void CrossesTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.Crosses(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)),
                              svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void DifferenceTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.Difference(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)),
                              svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void DifferenceTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void DisjointTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.Disjoint(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)),
                              svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void DisjointTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void DistanceTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.Distance(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)),
                              svcs.GeometryFromProviderValue(new PostgisPoint(1D, 1D)));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void DistanceTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void ElementAtTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.ElementAt(svcs.GeometryFromProviderValue(
                    new PostgisGeometryCollection(new PostgisGeometry[1] { new PostgisPoint(0D, 0D) })),
                    1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void ElementAtTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyCollectionFromBinaryTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyCollectionFromTextTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyFromBinaryTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyFromBinaryTest1()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyFromGmlTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyFromGmlTest1()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyFromProviderValueTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyFromTextTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyFromTextTest1()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyLineFromBinaryTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyLineFromTextTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyMultiLineFromBinaryTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyMultiLineFromTextTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyMultiPointFromBinaryTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyMultiPointFromTextTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyMultiPolygonFromBinaryTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyMultiPolygonFromTextTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyPointFromBinaryTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyPointFromTextTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyPolygonFromBinaryTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeographyPolygonFromTextTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeometryCollectionFromBinaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsBinary(svcs.GeometryFromProviderValue(new PostgisGeometryCollection
                    (new PostgisGeometry[] { new PostgisPoint(0D, 0D) })));
                svcs.GeometryCollectionFromBinary(b, 1);
            }
            catch (NotImplementedException exi)
            {
                Assert.Ignore("not implemented");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryCollectionFromTextTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(new PostgisGeometryCollection
                    (new PostgisGeometry[] { new PostgisPoint(0D, 0D) })));
                svcs.GeometryCollectionFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryFromBinaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsBinary(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)));
                svcs.GeometryFromBinary(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryFromBinaryTestGeog()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeometryFromGmlTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)));
                svcs.GeometryCollectionFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryFromGmlTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeometryFromProviderValueTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                svcs.GeometryFromProviderValue(new PostgisPoint(0d, 0d));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryFromTextTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(new PostgisPoint(0D, 0D)));
                svcs.GeometryCollectionFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryFromTextTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GeometryLineFromBinaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsBinary(svcs.GeometryFromProviderValue(
                    new PostgisLineString(new Coordinate2D[] { new Coordinate2D(0D, 0D),
                                           new Coordinate2D(0d,0d) })));
                svcs.GeometryLineFromBinary(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryLineFromTextTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(
                    new PostgisLineString(new Coordinate2D[] { new Coordinate2D(0D, 0D),
                                           new Coordinate2D(0d,0d) })));
                svcs.GeometryLineFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryMultiLineFromBinaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsBinary(svcs.GeometryFromProviderValue(
                    new PostgisMultiLineString(
                        new Coordinate2D[][] {
                            new Coordinate2D[] {
                                new Coordinate2D(0D, 0D),
                                new Coordinate2D(0d,0d)
                                }
                            })));
                svcs.GeometryMultiLineFromBinary(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryMultiLineFromTextTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(
                    new PostgisMultiLineString(
                        new Coordinate2D[][] {
                            new Coordinate2D[] {
                                new Coordinate2D(0D, 0D),
                                new Coordinate2D(0d,0d)
                                }
                            })));
                svcs.GeometryMultiLineFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryMultiPointFromBinaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsBinary(svcs.GeometryFromProviderValue(
                    new PostgisMultiPoint(
                            new Coordinate2D[] {
                                new Coordinate2D(0D, 0D),
                                new Coordinate2D(0d,0d)
                                }
                            )));
                svcs.GeometryMultiPointFromBinary(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryMultiPointFromTextTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(
                    new PostgisMultiPoint(
                            new Coordinate2D[] {
                                new Coordinate2D(0D, 0D),
                                new Coordinate2D(0d,0d)
                                }
                            )));
                svcs.GeometryMultiPointFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryMultiPolygonFromBinaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsBinary(svcs.GeometryFromProviderValue(
                    new PostgisMultiPolygon(new PostgisPolygon[]
                    {new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }
                            )
                    }
                            )));
                svcs.GeometryMultiPolygonFromBinary(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryMultiPolygonFromTextTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(
                    new PostgisMultiPolygon(new PostgisPolygon[]
                    {new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }
                            )
                    }
                            )));
                svcs.GeometryMultiPolygonFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryPointFromBinaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsBinary(svcs.GeometryFromProviderValue(new PostgisPoint(1D, 1D)));
                svcs.GeometryPointFromBinary(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryPointFromTextTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(new PostgisPoint(1D, 1D)));
                svcs.GeometryPointFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryPolygonFromBinaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsBinary(svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                })));
                svcs.GeometryPolygonFromBinary(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GeometryPolygonFromTextTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.AsText(svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                })));
                svcs.GeometryPolygonFromText(b, 1);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetAreaTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetArea(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetAreaTestGeom()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetBoundaryTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetBoundary(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetCentroidTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetCentroid(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetConvexHullTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetConvexHull(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetCoordinateSystemIdTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                })
                    { SRID = 3742 });
                svcs.GetCoordinateSystemId(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetCoordinateSystemIdTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetDimensionTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetDimension(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetDimensionTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetElementCountTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisGeometryCollection(
                        new PostgisGeometry[] {
                            new PostgisPolygon(
                                new Coordinate2D[][] {
                                    new Coordinate2D[] {
                                        new Coordinate2D(0D, 0D),
                                        new Coordinate2D(0d,1d),
                                        new Coordinate2D(1d,1d),
                                        new Coordinate2D(1d,0d),
                                        new Coordinate2D(0d,0d)
                                        }
                                })
                            })
                        );
                svcs.GetElementCount(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetElementCountTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetElevationTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetElevationTest1()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetEndPointTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetEndPoint(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetEndPointTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetEnvelopeTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetEnvelope(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetExteriorRingTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetExteriorRing(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetInteriorRingCountTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetInteriorRingCount(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetIsClosedTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetIsClosed(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetIsClosedTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetIsEmptyTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetIsEmpty(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetIsEmptyTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetIsRingTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisLineString(
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }));
                svcs.GetIsRing(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetIsSimpleTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetIsSimple(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetIsValidTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetIsSimple(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetLatitudeTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetLengthTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetLength(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetLengthTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetLongitudeTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetMeasureTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetMeasureTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetPointCountTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetPointCount(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetPointCountTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetPointOnSurfaceTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetPointOnSurface(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetSpatialTypeNameTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                Assert.IsTrue(svcs.GetSpatialTypeName(b).ToLower() == "polygon");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetSpatialTypeNameTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetStartPointTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.GetStartPoint(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetStartPointTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void GetXCoordinateTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPoint(0D, 0D));
                svcs.GetXCoordinate(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void GetYCoordinateTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPoint(0D, 0D));
                svcs.GetYCoordinate(b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void InteriorRingAtTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                ,new Coordinate2D[] {
                                    new Coordinate2D(0.5D, 0.5D),
                                    new Coordinate2D(0.5d,0.8d),
                                    new Coordinate2D(0.8d,0.8d),
                                    new Coordinate2D(0.8d,0.5d),
                                    new Coordinate2D(0.5d,0.5d)
                                    }
                                }));
                svcs.InteriorRingAt(b, 2);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void IntersectionTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.Intersection(b, b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void IntersectionTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void IntersectsTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.Intersection(b, b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void IntersectsTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void OverlapsTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.Overlaps(b, b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void PointAtTest()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void PointAtTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void RelateTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.Relate(b, b, "0FFFFF212");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void SpatialEqualsTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.SpatialEquals(b, b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void SpatialEqualsTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void SymmetricDifferenceTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.SymmetricDifference(b, b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void SymmetricDifferenceTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void TouchesTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.Touches(b, b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void UnionTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.Union(b, b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void UnionTestGeog()
        {
            Assert.Ignore("not implemented");
        }

        [Test()]
        public void WithinTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            try
            {
                var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
                svcs.Within(b, b);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test()]
        public void InstanceTest()
        {
            var svcs = new PostgisServices();
            svcs.SetConnection(Conn);
            var b = svcs.GeometryFromProviderValue(
                    new PostgisPolygon(
                            new Coordinate2D[][] {
                                new Coordinate2D[] {
                                    new Coordinate2D(0D, 0D),
                                    new Coordinate2D(0d,1d),
                                    new Coordinate2D(1d,1d),
                                    new Coordinate2D(1d,0d),
                                    new Coordinate2D(0d,0d)
                                    }
                                }));
            try
            {
                var x = b.Area;
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
