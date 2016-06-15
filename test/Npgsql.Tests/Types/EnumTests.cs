using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;

namespace Npgsql.Tests.Types
{
    class EnumTests : TestBase
    {
        enum Mood { Sad, Ok, Happy };

        [Test]
        public void LateRegistration()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')");
            Conn.ReloadTypes();
            Conn.RegisterEnum<Mood>("mood");
            const Mood expected = Mood.Ok;
            var cmd = new NpgsqlCommand("SELECT @p1::MOOD, @p2::MOOD", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Enum) { EnumType = typeof(Mood), Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof (Mood)));
                Assert.That(reader.GetFieldValue<Mood>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i), Is.EqualTo(expected));
            }

            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void DualEnums()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')");
            ExecuteNonQuery("CREATE TYPE pg_temp.test_enum AS ENUM ('label1', 'label2', 'label3')");
            Conn.ReloadTypes();
            Conn.RegisterEnum<Mood>("mood");
            Conn.RegisterEnum<TestEnum>("test_enum");
            var cmd = new NpgsqlCommand("SELECT @p1", Conn);
            var expected = new Mood[] { Mood.Ok, Mood.Sad };
            var p = new NpgsqlParameter("p1", NpgsqlDbType.Enum | NpgsqlDbType.Array) { EnumType = typeof(Mood), Value = expected };
            cmd.Parameters.Add(p);
            var result = cmd.ExecuteScalar();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GlobalRegistration()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')");
            NpgsqlConnection.RegisterEnumGlobally<Mood>();
            try
            {
                Conn.ReloadTypes();
                const Mood expected = Mood.Ok;
                using (var cmd = new NpgsqlCommand("SELECT @p::MOOD", Conn))
                {
                    var p = new NpgsqlParameter {ParameterName = "p", Value = expected};
                    cmd.Parameters.Add(p);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();

                        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (Mood)));
                        Assert.That(reader.GetFieldValue<Mood>(0), Is.EqualTo(expected));
                        Assert.That(reader.GetValue(0), Is.EqualTo(expected));

                    }
                }
            }
            finally
            {
                NpgsqlConnection.UnregisterEnumGlobally<Mood>();
            }
        }

        [Test]
        public void Array()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')");
            Conn.ReloadTypes();
            Conn.RegisterEnum<Mood>("mood");
            var expected = new[] { Mood.Ok, Mood.Happy };
            using (var cmd = new NpgsqlCommand("SELECT @p1::MOOD[], @p2::MOOD[]", Conn))
            {
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Enum | NpgsqlDbType.Array) {
                    EnumType = typeof (Mood),
                    Value = expected
                };
                var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof (Array)));
                        Assert.That(reader.GetFieldValue<Mood[]>(i), Is.EqualTo(expected));
                        Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                    }
                }
            }
        }

        [Test]
        public void ReadUnmappedEnumsAsString()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')", conn);
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT 'Sad'::MOOD, ARRAY['Ok', 'Happy']::MOOD[]", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader[0], Is.EqualTo("Sad"));
                    Assert.That(reader[1], Is.EqualTo(new[] { "Ok", "Happy" }));
                }
            }
        }

        [Test, Description("Test that a c# string can be written to a backend enum when DbType is unknown")]
        public void WriteStringToBackendEnum()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.fruit AS ENUM ('Banana', 'Apple', 'Orange')");
            ExecuteNonQuery("create table pg_temp.test_fruit ( id serial, value1 pg_temp.fruit, value2 pg_temp.fruit );");
            Conn.ReloadTypes();
            const string expected = "Banana";
            using (var cmd = new NpgsqlCommand("insert into pg_temp.test_fruit(id, value1, value2) values(default, @p1, @p2);", Conn))
            {
                cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Unknown, expected);
                var p2 = new NpgsqlParameter("p1", NpgsqlDbType.Unknown) {Value = expected};
                cmd.Parameters.Add(p2);
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Description("Tests that a a C# enum an be written to an enum backend when passed as dbUnknown")]
        public void WriteEnumAsDbUnknwown()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('Sad', 'Ok', 'Happy')", Conn);
            ExecuteNonQuery("create table pg_temp.test_mood_writes ( value1 pg_temp.mood);");
            Conn.ReloadTypes();
            var expected = Mood.Happy;
            using (var cmd = new NpgsqlCommand("insert into pg_temp.test_mood_writes(value1) values(@p1);", Conn))
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Unknown, expected);
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void TestEnumType()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.test_enum AS ENUM ('label1', 'label2', 'label3')");
            Conn.ReloadTypes();
            Conn.RegisterEnum<TestEnum>("test_enum");
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select :p1, :p2, :p3, :p4, :p5";

                cmd.Parameters.AddWithValue("p1", TestEnum.label1);
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", NpgsqlDbType = NpgsqlDbType.Enum, EnumType = typeof(TestEnum), Value = TestEnum.label2 });
                cmd.Parameters.AddWithValue("p3", new[] { TestEnum.label1, TestEnum.Label3 });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4", NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Enum, EnumType = typeof(TestEnum), Value = new[] { TestEnum.label1, TestEnum.Label3 } });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p5", NpgsqlDbType = NpgsqlDbType.Enum, EnumType = typeof(TestEnum), Value = DBNull.Value });

                Assert.AreEqual(NpgsqlDbType.Enum, cmd.Parameters[0].NpgsqlDbType);
                Assert.AreEqual(typeof(TestEnum), cmd.Parameters[0].EnumType);
                Assert.AreEqual(NpgsqlDbType.Array | NpgsqlDbType.Enum, cmd.Parameters[2].NpgsqlDbType);
                Assert.AreEqual(typeof(TestEnum), cmd.Parameters[2].EnumType);

                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    Assert.AreEqual(typeof(TestEnum), rdr.GetValue(0).GetType());
                    Assert.AreEqual(TestEnum.label1, rdr.GetValue(0));
                    Assert.AreEqual(typeof(TestEnum), rdr.GetValue(1).GetType());
                    Assert.AreEqual(TestEnum.label2, rdr.GetValue(1));
                    Assert.AreEqual(typeof(TestEnum[]), rdr.GetValue(2).GetType());
                    Assert.IsTrue(new[] { TestEnum.label1, TestEnum.Label3 }.SequenceEqual((TestEnum[])rdr.GetValue(2)));
                    Assert.AreEqual(typeof(TestEnum[]), rdr.GetValue(3).GetType());
                    Assert.IsTrue(new[] { TestEnum.label1, TestEnum.Label3 }.SequenceEqual((TestEnum[])rdr.GetValue(3)));
                    Assert.AreEqual(typeof(TestEnum), rdr.GetFieldType(4));
                }
            }
        }

        enum TestEnum
        {
            label1,
            label2,
            [EnumLabel("label3")]
            Label3
        }
    }
}
