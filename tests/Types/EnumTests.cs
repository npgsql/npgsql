using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;

namespace NpgsqlTests.Types
{
    class EnumTests : TestBase
    {
        public EnumTests(string backendVersion) : base(backendVersion) {}

        enum Mood { Sad, Ok, Happy };

        [Test]
        public void LateRegistration()
        {
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
        public void GlobalRegistration()
        {
            NpgsqlConnection.RegisterEnumGlobally<Mood>();
            var myconn = new NpgsqlConnection(ConnectionString);
            myconn.Open();
            const Mood expected = Mood.Ok;
            var cmd = new NpgsqlCommand("SELECT @p::MOOD", myconn);
            var p = new NpgsqlParameter { ParameterName = "p", Value = expected };
            cmd.Parameters.Add(p);
            var reader = cmd.ExecuteReader();
            reader.Read();

            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Mood)));
            Assert.That(reader.GetFieldValue<Mood>(0), Is.EqualTo(expected));
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));

            reader.Close();
            cmd.Dispose();
            myconn.Dispose();
        }

        [Test]
        public void Array()
        {
            Conn.RegisterEnum<Mood>("mood");
            var expected = new[] { Mood.Ok, Mood.Happy };
            var cmd = new NpgsqlCommand("SELECT @p1::MOOD[], @p2::MOOD[]", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Enum | NpgsqlDbType.Array) { EnumType=typeof(Mood), Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Array)));
                Assert.That(reader.GetFieldValue<Mood[]>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i), Is.EqualTo(expected));
            }

            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void TestEnumType()
        {
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

        [SetUp]
        public void CreateEnum()
        {
            if (ExecuteScalar("SELECT 1 FROM pg_type WHERE typname = 'test_enum'") == null)
                ExecuteNonQuery("CREATE TYPE test_enum AS ENUM ('label1', 'label2', 'label3')");
            if (ExecuteScalar("SELECT 1 FROM pg_type WHERE typname = 'mood'") == null)
                ExecuteNonQuery("CREATE TYPE mood AS ENUM ('Sad', 'Ok', 'Happy')");
            // Need to clear the connection pool and the type registry's backend type cache to reload the types...
            Conn.Close();
            NpgsqlConnection.ClearAllPools();
            TypeHandlerRegistry.ClearBackendTypeCache();
            Conn = new NpgsqlConnection(ConnectionString);
            Conn.Open();
        }
    }
}
