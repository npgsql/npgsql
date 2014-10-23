using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework ;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace NpgsqlTests
{
    [TestFixture("9.4")]
    [TestFixture("9.3")]
    [TestFixture("9.2")]
    [TestFixture("9.1")]
    [TestFixture("9.0")]
    [TestFixture("8.4")]    
    public class PostGisTests
    {
        private NpgsqlConnection Conn;
        private String _connectionString;      
        private Version _expectedPgisVer = new Version(2, 0, 0, 0);
        protected Version BackendVersion { get; private set; }

        public PostGisTests(String backendVersion)            
        {
            BackendVersion = new Version (backendVersion);            
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {            
            Version ver;             

            var connStringEnvVar = "NPGSQL_TEST_DB_" + BackendVersion;
            _connectionString = Environment.GetEnvironmentVariable(connStringEnvVar);
            if (_connectionString == null)
            {
                Assert.Ignore("Skipping tests for backend version {0}, environment variable {1} isn't defined", BackendVersion, connStringEnvVar);
            }
            else
                Console.WriteLine("Using connection string provided in env var {0}: {1}", connStringEnvVar, _connectionString);

            NpgsqlConnectionStringBuilder bld = new NpgsqlConnectionStringBuilder (_connectionString);
            bld.WithPostgis = true;
            Conn = new NpgsqlConnection (bld.ConnectionString);             
            Conn.Open ();

            try {
                ver = Conn.PostGisVersion;
            } catch (Exception ex) {
                Assert.Ignore(String.Format("No PostGis extension seems to be installed. Ignoring tests (exception was : {0}.",ex.Message));
                return;
            }   

            if (ver < _expectedPgisVer) {
                Assert.Ignore (String.Format ("Expected Postgis ver. {0} . Found {1} . Ignoring Tests.", _expectedPgisVer.ToString (), this.Conn.PostGisVersion.ToString ()));
            }                      
        }        
     
        [Test]
        public void PreparedStmtTest()
        {
            using (NpgsqlCommand c = this.Conn.CreateCommand())
            {
                NpgsqlParameter p = new NpgsqlParameter("geo", NpgsqlDbType.OgrPoint);
                OgrPoint p1 = new OgrPoint(10D, 10D);
                p.Value = p1;
                c.Parameters.Add(p);
                c.CommandText = "select :geo";
                c.Prepare();
                OgrPoint p2 = (OgrPoint)c.ExecuteScalar();
                Assert.IsTrue(p2 == p1, "Features are not equals");
            }
        }

        [Test]
        public void EnumerateLString()
        {
        OgrLineString l = GetFeature<OgrLineString>();
        OgrLineString l2 = new OgrLineString(l);
        Assert.IsTrue (l == l2, "LineString enumeration failed");
        }

        [Test]
        public void EnumerateMPoint()
        {
            OgrMultiPoint l = GetFeature<OgrMultiPoint>();
            OgrMultiPoint l2 = new OgrMultiPoint(l);
            Assert.IsTrue(l2 == l, "MultiPoint enumeration failed");
        }

        [Test]
        public void EnumerateMLineString()
        {
            OgrMultiLineString l = GetFeature<OgrMultiLineString>();
            OgrMultiLineString l2 = new OgrMultiLineString(l);
            Assert.IsTrue(l2 == l, "OgrMultiLineString enumeration failed");
        }

        [Test]
        public void EnumerateMPolygon()
        {
            OgrMultiPolygon l = GetFeature<OgrMultiPolygon>();
            OgrMultiPolygon l2 = new OgrMultiPolygon(l);
            Assert.IsTrue(l2 == l, "OgrMultiPolygon enumeration failed");
        }

        [Test]
        public void EnumerateGeoColl()
        {
            OgrGeometryCollection l = GetFeature<OgrGeometryCollection>();
            OgrGeometryCollection l2 = new OgrGeometryCollection(l);
            Assert.IsTrue(l2 == l, "OgrGeometryCollection enumeration failed");
        }

        [Test]
        public void TestConnectionString()
        {
            using (var cn = new NpgsqlConnection(_connectionString + ";WITH POSTGIS=true"))
            {
                NpgsqlConnectionStringBuilder b = new NpgsqlConnectionStringBuilder (cn.ConnectionString);
                Assert.IsTrue (b.WithPostgis, "Keyword WITH POSTGIS not set");
                cn.Open();
                using (var c = cn.CreateCommand ()) {

                    NpgsqlParameter p = new NpgsqlParameter() { ParameterName = "geo", Direction = System.Data.ParameterDirection.InputOutput };
                    c.Parameters.Add(p);
                    c.CommandText = "Select (st_point(10 ,10))";
                    c.ExecuteScalar();                

                    Assert.IsTrue (p.Value is OgrPoint, "WITH POSTGIS is not working");
                }
                cn.Close();
            }
        }

        private T GetFeature<T>()
        {
            object[] o = (object[])FeatureTestSource.First(x => (Type)((object[])x)[1] == typeof(T));
            return (T)o[0];
        }


        static object[] FeatureTestSource = 
        {
            new object[]{ new OgrPoint(10D, 10D),
                        typeof(OgrPoint),
                        NpgsqlDbType.OgrPoint 
                         },

            new object[] {new OgrLineString(new OgrPoint[2] { new OgrPoint(1D, 2D), new OgrPoint(3D, 4D) }),
                               typeof(OgrLineString),
                                NpgsqlDbType.OgrLineString 
                        },
        
            new object[]{new OgrPolygon(new OgrPoint[][] { new OgrPoint[] { new OgrPoint(0D, 0D), new OgrPoint(0D, 10D), new OgrPoint(10D, 10D), 
                                                                            new OgrPoint(10D, 0D), new OgrPoint(0D, 0D) } }),
                        typeof(OgrPolygon),
                        NpgsqlDbType.OgrPolygon 
                        },

            new object[]{new OgrMultiPoint(new OgrPoint[] { new OgrPoint(10D, 40D), new OgrPoint(40D, 30D), new OgrPoint(20D, 20D), new OgrPoint(30D, 10D) }),
                        typeof(OgrMultiPoint),
                        NpgsqlDbType.OgrMultiPoint 
                        },

            new object[]{new OgrMultiLineString(new OgrPoint[][] { new OgrPoint[]{  new OgrPoint(10D,10D),new OgrPoint(20D,20D),new OgrPoint(10D,40D) } ,
                                                                   new OgrPoint[]{  new OgrPoint(40D,40D),new OgrPoint(30D,30D),new OgrPoint(40D,20D) , 
                                                                                    new OgrPoint(30D,10D) }}),
                        typeof(OgrMultiLineString),
                        NpgsqlDbType.OgrMultiLineString 
                        },

            new object[]{new OgrMultiPolygon(new OgrPolygon[]{ new OgrPolygon( new OgrPoint[][] {new OgrPoint[] { new OgrPoint(30D,20D), new OgrPoint(45D,40D),
                                                                                                                  new OgrPoint(10D,40D),new OgrPoint(30D,20D)}}),
                                                               new OgrPolygon( new OgrPoint[][] {new OgrPoint[] { new OgrPoint(15D, 5D), new OgrPoint(40D,10D), 
                                                                                                                  new OgrPoint(10D,20D), new OgrPoint(5D,10D), 
                                                                                                                  new OgrPoint(15D,5D)}})       
                                                                             }),
                        typeof(OgrMultiPolygon),
                        NpgsqlDbType.OgrMultiPolygon 
                        },

            new object[]{ new OgrGeometryCollection(new IGeometry[]{ new OgrPoint(10D, 10D),new OgrPoint(10D, 10D),new OgrPoint(10D, 10D)}),
                          typeof(OgrGeometryCollection),
                          NpgsqlDbType.OgrGeometryCollection 
                },
        };

        [Test, TestCaseSource("FeatureTestSource")]
        public void TestFeature(IGeometry geom, Type geomType, NpgsqlDbType pgDbType)
        {            
            using (NpgsqlCommand c = this.Conn.CreateCommand())
            {                
                NpgsqlParameter p = new NpgsqlParameter() { ParameterName = "geo", Direction = System.Data.ParameterDirection.Input, NpgsqlDbType = pgDbType };
                c.Parameters.Add(p);
                p.Value = geom;
                p.NpgsqlDbType = pgDbType;
                c.CommandText = "SELECT :geo;";
                object ret = null;

                try
                {
                    ret = c.ExecuteScalar();
                }
                catch (NpgsqlException ex)
                {
                    Assert.Fail(String.Format("Impossible to use feature {0} as input parameter. Error was : {1},", geomType.ToString(), ex.Message));
                }

                Assert.IsTrue(p.DbType == DbType.Object, "DbType should be object.");        

                Assert.IsTrue(ret.GetType() == geomType, geomType.ToString() + " type inference doesn't seeems to be working");

                Assert.IsTrue((geom.Equals(ret)), "Input and output are not equals");                
            }
        }
    
    }
}
