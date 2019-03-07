using NUnit.Framework;
using System;
using System.Linq;
using Npgsql.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Npgsql.CrateDb.Tests
{
    /// <summary>
    /// Tests for the CrateDb plugin.
    /// </summary>
    class CrateDbTypesTest : CrateIntegrationTest
    {
        [OneTimeSetUp]
        public void Init()
        {
            NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Trace);

            CreateTestTable();
            InsertIntoTestTable();

            CreateArrayTestTable();
            InsertIntoArrayTestTable();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TearDownTestTable();
            TearDownArrayTestTable();
        }

        [Test]
        public void TestSelectStringType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select string_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo("Youri"));
            }
        }

        [Test]
        public void TestSelectBooleanType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select boolean_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo(true));
            }
        }

        [Test]
        public void TestSelectByteType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select byte_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo((byte)120));
            }
        }

        [Test]
        public void TestSelectShortType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select short_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo((short)1000));
            }
        }

        [Test]
        public void TestSelectIntegerType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select integer_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo(1200000));
            }
        }

        [Test]
        public void TestSelectLongType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select long_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo(120000000000L));
            }
        }

        [Test]
        public void TestSelectFloatType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select float_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo(1.4f));
            }
        }

        [Test]
        public void TestSelectDoubleType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select double_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo(3.456789d));
            }
        }

        [Test]
        public void TestSelectTimestampType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select timestamp_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo(new DateTime(2018, 10, 11).ToUniversalTime()));
            }
        }

        [Test]
        public void TestSelectIPType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select ip_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EqualTo("127.0.0.1"));
            }
        }

        [Test]
        public void TestSelectGeoPoint()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select geo_point_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new double[] { 9.7419021d, 47.4048045d }));
            }
        }

        [Test]
        public void TestSelectGeoShape()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select geo_shape_field from test", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.InstanceOf(typeof(string)));

                var gs = JsonConvert.DeserializeObject<GeoShape>(r.ToString());
                Assert.That(gs.type, Is.EqualTo("Polygon"));
                Assert.That(gs.coordinates, Is.EqualTo(new double[][][] {
                    new double[][]
                    {
                        new double[] { 30.0, 10.0 },
                        new double[] { 40.0, 40.0 },
                        new double[] { 20.0, 40.0 },
                        new double[] { 10.0, 20.0 },
                        new double[] { 30.0, 10.0 }
                    }
                }));
            }
        }

        [Test]
        public void SelectStringArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select str_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new string[] { "a", "b", "c", "d" }));
            }
        }

        [Test]
        public void SelectBooleanArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select bool_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new bool[] { true, false }));
            }
        }

        [Test]
        public void SelectByteArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select byte_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                // Crate returns byte arrays as array of int2.
                Assert.That(r.GetType(), Is.EqualTo(typeof(char[])));
                Assert.That(r, Is.EquivalentTo(new byte[] { 120, 100 }));
            }
        }
        
        [Test]
        public void SelectByteArrayTypeWithNpgsqlGetBytes()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select byte_array from arrayTest", con))
            using (var rdr = cmd.ExecuteReader())
            {
                Assert.That(rdr.Read(), Is.EqualTo(true));
                Assert.Throws(typeof(InvalidCastException), () =>
                {
                    var buffer = new byte[10];
                    var bytesRead = rdr.GetBytes(0, 0, buffer, 0, 10);
                });
                rdr.Close();
            }
        }


        [Test]
        public void SelectShortArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select short_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new short[] { 1300, 1200 }));
            }
        }

        [Test]
        public void SelectIntegerArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select integer_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new int[] { 2147483647, 234583 }));
            }
        }

        [Test]
        public void SelectLongArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select long_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new long[] { 9223372036854775807L, 4L }));
            }
        }

        [Test]
        public void SelectFloatArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select float_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new float[] { 3.402f, 3.403f, 1.4f }));
            }
        }

        [Test]
        public void SelectDoubleArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select double_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new double[] { 1.79769313486231570e+308, 1.69769313486231570e+308 }));
            }
        }

        [Test]
        public void SelectTimestampArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select timestamp_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new DateTime[] { new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(1970, 1, 1).ToUniversalTime() }));
            }
        }

        [Test]
        public void SelectIPArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select ip_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.EquivalentTo(new string[] { "127.142.132.9", "127.0.0.1" }));
            }
        }

        [Test]
        public void SelectObjectType()
        {
            using (var con = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("select object_field from test", con))
                using (var rdr = cmd.ExecuteReader())
                {
                    Assert.That(rdr.Read(), Is.True);
                    var result = rdr.GetFieldValue<string>(0);
                    Assert.That(result, Is.InstanceOf(typeof(string)));
                    var testObject = JsonConvert.DeserializeObject<TestObject>(result);
                    Assert.That(testObject, Is.EqualTo(new TestObject { inner = "Zoon" }));
                }
            }
        }

        [Test]
        public void SelectObjectTypeWithExecuteScalar()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select object_field from test", con))
            {
                var r = cmd.ExecuteScalar();

                Assert.That(r, Is.InstanceOf(typeof(string)));

                var obj = JsonConvert.DeserializeObject<TestObject>((string)r);
                Assert.That(obj, Is.EqualTo(new TestObject { inner = "Zoon" }));
            }
        }

        [Test]
        public void SelectObjectArrayType()
        {
            using (var con = OpenConnection())
            {

                using (var cmd = new NpgsqlCommand("select obj_array from arrayTest", con))
                using (var rdr = cmd.ExecuteReader())
                {
                    Assert.That(rdr.Read(), Is.True);

                    var result = rdr.GetFieldValue<string>(0);

                    Assert.That(result, Is.InstanceOf(typeof(string)));
                    var testObjects = JsonConvert.DeserializeObject<TestObject[]>(result);
                    Assert.That(testObjects, Is.EquivalentTo(new TestObject[]
                    {
                    new TestObject { inner = "Zoon1" },
                    new TestObject { inner = "Zoon2" }
                    }));
                }
            }
        }

        [Test]
        public void SelectObjectArrayTypeWithExecuteScalar()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select obj_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();

                Assert.That(r, Is.InstanceOf(typeof(string)));

                var objArr = JsonConvert.DeserializeObject<TestObject[]>((string)r);
                Assert.That(objArr, Is.EquivalentTo(new TestObject[]
                {
                    new TestObject { inner = "Zoon1" },
                    new TestObject { inner = "Zoon2" }
                }));
            }
        }

        [Test]
        public void SelectGeoShapeArrayType()
        {
            using (var con = OpenConnection())
            using (var cmd = new NpgsqlCommand("select geo_shape_array from arrayTest", con))
            {
                var r = cmd.ExecuteScalar();
                Assert.That(r, Is.InstanceOf(typeof(string[])));

                var gs = ((string[])r).Select(s => JsonConvert.DeserializeObject<GeoShape>(s)).ToArray();

                Assert.That(gs.Length, Is.EqualTo(2));
                Assert.That(gs[0].type, Is.EqualTo("Polygon"));
                Assert.That(gs[0].coordinates, Is.EqualTo(new double[][][] {
                    new double[][]
                    {
                        new double[] { 30.0, 10.0 },
                        new double[] { 40.0, 40.0 },
                        new double[] { 20.0, 40.0 },
                        new double[] { 10.0, 20.0 },
                        new double[] { 30.0, 10.0 }
                    }
                }));
                Assert.That(gs[1].type, Is.EqualTo("Polygon"));
                Assert.That(gs[1].coordinates, Is.EqualTo(new double[][][] {
                    new double[][]
                    {
                        new double[] { 40.0, 20.0 },
                        new double[] { 50.0, 50.0 },
                        new double[] { 30.0, 50.0 },
                        new double[] { 20.0, 30.0 },
                        new double[] { 40.0, 20.0 }
                    }
                }));
            }
        }

        [Test]
        public void SelectGeoShapeArrayTypeWithDataReader()
        {
            using (var con = OpenConnection())
            {

                using (var cmd = new NpgsqlCommand("select geo_shape_array from arrayTest", con))
                using (var rdr = cmd.ExecuteReader())
                {
                    Assert.That(rdr.Read(), Is.True);

                    var result = rdr.GetFieldValue<string[]>(0);
                    Assert.That(result, Is.InstanceOf(typeof(string[])));
                    Assert.That(result.Length, Is.EqualTo(2));

                    var geoShapes = new List<GeoShape>();

                    foreach(var g in result)
                    {
                        geoShapes.Add(JsonConvert.DeserializeObject<GeoShape>(g));
                    }

                    
                    Assert.That(geoShapes[0].type, Is.EqualTo("Polygon"));
                    Assert.That(geoShapes[1].type, Is.EqualTo("Polygon"));

                    var expectedCoordinates = new double[][][] {
                        new double[][]
                        {
                            new double[] { 30.0, 10.0 },
                            new double[] { 40.0, 40.0 },
                            new double[] { 20.0, 40.0 },
                            new double[] { 10.0, 20.0 },
                            new double[] { 30.0, 10.0 }
                        }
                    };
                    Assert.That(geoShapes[0].coordinates, Is.EqualTo(expectedCoordinates));
                }
            }
        }

        class GeoShape
        {
            public string type { get; set; }
            public double[][][] coordinates { get; set; }
        }

        class TestObject
        {
            public string inner { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is TestObject))
                    return false;
                return inner.Equals(((TestObject)obj).inner);
            }

            public override int GetHashCode()
            {
                return inner.GetHashCode();
            }
        }
    }
}
