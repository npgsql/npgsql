﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

// ReSharper disable AccessToModifiedClosure
// ReSharper disable AccessToDisposedClosure

namespace Npgsql.PluginTests
{
    /// <summary>
    /// Tests for the Npgsql.Json.NET mapping plugin
    /// </summary>
    [NonParallelizable]
    [TestFixture(NpgsqlDbType.Jsonb)]
    [TestFixture(NpgsqlDbType.Json)]
    public class JsonNetTests : TestBase
    {
        [Test]
        public void RoundtripObject()
        {
            var expected = new Foo { Bar = 8 };
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT @p1, @p2", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p1", _npgsqlDbType) { Value = expected });
                cmd.Parameters.Add(new NpgsqlParameter<Foo>
                {
                    ParameterName = "p2", NpgsqlDbType = _npgsqlDbType, TypedValue = expected
                });
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetFieldValue<Foo>(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldValue<Foo>(1), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void DeserializeFailure()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand($@"SELECT '[1, 2, 3]'::{_pgTypeName}", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                // Attempt to deserialize JSON array into object
                Assert.That(() => reader.GetFieldValue<Foo>(0), Throws.TypeOf<JsonSerializationException>());
                // State should still be OK to continue
                var actual = reader.GetFieldValue<JArray>(0);
                Assert.That((int)actual[0], Is.EqualTo(1));
            }
        }

        class Foo
        {
            public int Bar { get; set; }
            public override bool Equals(object? obj) => (obj as Foo)?.Bar == Bar;
            public override int GetHashCode() => Bar.GetHashCode();
        }

        class Bar
        {
            public int A { get; set; }
        }

        [Test]
        public void RoundtripJObject()
        {
            var expected = new JObject { ["Bar"] = 8 };

            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT @p", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p", _npgsqlDbType) { Value = expected });
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var actual = reader.GetFieldValue<JObject>(0);
                    Assert.That((int)actual["Bar"], Is.EqualTo(8));
                }
            }
        }

        [Test]
        public void RoundtripJArray()
        {
            var expected = new JArray(new[] { 1, 2, 3 });

            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT @p", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p", _npgsqlDbType) { Value = expected });
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var jarray = reader.GetFieldValue<JArray>(0);
                    Assert.That(jarray.ToObject<int[]>(), Is.EqualTo(new[] { 1, 2, 3 }));
                }
            }
        }

        [Test]
        public void ClrTypeMapping()
        {
            var expected = new Foo { Bar = 8 };
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT @p", conn))
            {
                conn.TypeMapper.UseJsonNet(new[] { typeof(Foo) });

                cmd.Parameters.AddWithValue("p", expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var actual = reader.GetFieldValue<Foo>(0);
                    Assert.That(actual.Bar, Is.EqualTo(8));
                }
            }
        }

        [Test, Ignore("https://github.com/npgsql/npgsql/issues/2568")]
        public void ClrTypeMappingTwoTypes()
        {
            var value1 = new Foo { Bar = 8 };
            var value2 = new Bar { A = 8 };
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT @p1, @p2", conn))
            {
                conn.TypeMapper.UseJsonNet(new[] { typeof(Foo), typeof(Bar) });

                cmd.Parameters.AddWithValue("p1", value1);
                cmd.Parameters.AddWithValue("p2", value1);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var actual1 = reader.GetFieldValue<Foo>(0);
                    Assert.That(actual1.Bar, Is.EqualTo(8));
                    var actual2 = reader.GetFieldValue<Bar>(1);
                    Assert.That(actual2.A, Is.EqualTo(8));
                }
            }
        }

        [Test]
        public void RoundtripClrArray()
        {
            var expected = new[] { 1, 2, 3 };

            using (var conn = OpenConnection())
            {
                conn.TypeMapper.UseJsonNet(new[] { typeof(int[]) });

                using (var cmd = new NpgsqlCommand($@"SELECT @p::{_pgTypeName}", conn))
                {
                    cmd.Parameters.AddWithValue("p", expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var actual = reader.GetFieldValue<int[]>(0);
                        Assert.That(actual, Is.EqualTo(expected));
                    }
                }
            }
        }

        class DateWrapper
        {
            public System.DateTime Date;
            public override bool Equals(object? obj) => (obj as DateWrapper)?.Date == Date;
            public override int GetHashCode() => Date.GetHashCode();
        }

        void RoundtripCustomSerializerSettings(bool asJsonb)
        {
            var expected = new DateWrapper() { Date = new System.DateTime(2018, 04, 20) };

            var settings = new JsonSerializerSettings()
            {
                DateFormatString = @"T\he d\t\h o\f MMMM, yyyy"
            };

            // If we serialize to JSONB, Postgres will not store the Json.NET formatting, and will add a space after ':'
            var expectedString = asJsonb ? "{\"Date\": \"The 20th of April, 2018\"}"
                                         : "{\"Date\":\"The 20th of April, 2018\"}";

            using (var conn = OpenConnection())
            {
                if (asJsonb)
                {
                    conn.TypeMapper.UseJsonNet(jsonbClrTypes : new[] { typeof(DateWrapper) }, settings : settings);
                }
                else
                {
                    conn.TypeMapper.UseJsonNet(jsonClrTypes : new[] { typeof(DateWrapper) }, settings : settings);
                }

                using (var cmd = new NpgsqlCommand($@"SELECT @p::{_pgTypeName}, @p::text", conn))
                {
                    cmd.Parameters.AddWithValue("p", expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var actual = reader.GetFieldValue<DateWrapper>(0);
                        var actualString = reader.GetFieldValue<string>(1);
                        Assert.That(actual, Is.EqualTo(expected));
                        Assert.That(actualString, Is.EqualTo(expectedString));
                    }
                }
            }
        }

        [Test]
        public void RoundtripJsonbCustomSerializerSettings() => RoundtripCustomSerializerSettings(asJsonb : true);

        [Test]
        public void RoundtripJsonCustomSerializerSettings() => RoundtripCustomSerializerSettings(asJsonb : false);

        protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        {
            var conn = base.OpenConnection(connectionString);
            conn.TypeMapper.UseJsonNet();
            return conn;
        }

        readonly NpgsqlDbType _npgsqlDbType;
        readonly string _pgTypeName;

        public JsonNetTests(NpgsqlDbType npgsqlDbType)
        {
            _npgsqlDbType = npgsqlDbType;
            _pgTypeName = npgsqlDbType.ToString().ToLower();
        }
    }
}
