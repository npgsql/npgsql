﻿using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public class InternalTypeTests : TestBase
    {
        [Test]
        public void ReadInternalChar()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT typdelim FROM pg_type WHERE typname='int4'", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetChar(0), Is.EqualTo(','));
                Assert.That(reader.GetValue(0), Is.EqualTo(','));
                Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(','));
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(char)));
            }
        }

        [Test]
        [TestCase(NpgsqlDbType.Oid)]
        [TestCase(NpgsqlDbType.Regtype)]
        [TestCase(NpgsqlDbType.Regconfig)]
        public void InternalUintTypes(NpgsqlDbType npgsqlDbType)
        {
            var postgresType = npgsqlDbType.ToString().ToLowerInvariant();
            using var conn = OpenConnection();
            using var cmd = new NpgsqlCommand($"SELECT @max, 4294967295::{postgresType}, @eight, 8::{postgresType}", conn);
            cmd.Parameters.AddWithValue("max", npgsqlDbType, uint.MaxValue);
            cmd.Parameters.AddWithValue("eight", npgsqlDbType, 8u);
            using var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < reader.FieldCount; i++)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(uint)));

            Assert.That(reader.GetValue(0), Is.EqualTo(uint.MaxValue));
            Assert.That(reader.GetValue(1), Is.EqualTo(uint.MaxValue));
            Assert.That(reader.GetValue(2), Is.EqualTo(8u));
            Assert.That(reader.GetValue(3), Is.EqualTo(8u));
        }

        [Test]
        public void Tid()
        {
            var expected = new NpgsqlTid(3, 5);
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT '(1234,40000)'::tid, @p::tid";
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Tid, expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual(1234, reader.GetFieldValue<NpgsqlTid>(0).BlockNumber);
                    Assert.AreEqual(40000, reader.GetFieldValue<NpgsqlTid>(0).OffsetNumber);
                    Assert.AreEqual(expected.BlockNumber, reader.GetFieldValue<NpgsqlTid>(1).BlockNumber);
                    Assert.AreEqual(expected.OffsetNumber, reader.GetFieldValue<NpgsqlTid>(1).OffsetNumber);
                }
            }
        }
    }
}
