using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NpgsqlTests
{
    public class SimpleTypeTests : TestBase
    {
        public SimpleTypeTests(string backendVersion) : base(backendVersion) {}

        [Test]
        public void ReadInt16([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 8::SMALLINT", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt64(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt16(0),                 Is.EqualTo(8));
            Assert.That(reader.GetByte(0),                  Is.EqualTo(8));
            Assert.That(reader.GetFloat(0),                 Is.EqualTo(8.0f));
            Assert.That(reader.GetDouble(0),                Is.EqualTo(8.0d));
            Assert.That(reader.GetDecimal(0),               Is.EqualTo(8.0m));
            Assert.That(reader.GetValue(0),                 Is.EqualTo(8));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(8));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadInt32([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 8::INTEGER", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt64(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt16(0),                 Is.EqualTo(8));
            Assert.That(reader.GetByte(0),                  Is.EqualTo(8));
            Assert.That(reader.GetFloat(0),                 Is.EqualTo(8.0f));
            Assert.That(reader.GetDouble(0),                Is.EqualTo(8.0d));
            Assert.That(reader.GetDecimal(0),               Is.EqualTo(8.0m));
            Assert.That(reader.GetValue(0),                 Is.EqualTo(8));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(8));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadInt64([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 8::BIGINT", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt64(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt16(0),                 Is.EqualTo(8));
            Assert.That(reader.GetByte(0),                  Is.EqualTo(8));
            Assert.That(reader.GetFloat(0),                 Is.EqualTo(8.0f));
            Assert.That(reader.GetDouble(0),                Is.EqualTo(8.0d));
            Assert.That(reader.GetDecimal(0),               Is.EqualTo(8.0m));
            Assert.That(reader.GetValue(0),                 Is.EqualTo(8));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(8));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadDouble([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            const double expected = 4.123456789012345;
            var cmd = new NpgsqlCommand("SELECT 4.123456789012345::DOUBLE PRECISION", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetDouble(0), Is.EqualTo(expected).Within(10E-07));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void GetFloat([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            const float expected = .123456F;
            var cmd = new NpgsqlCommand("SELECT .123456::REAL", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFloat(0), Is.EqualTo(expected).Within(10E-07));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadDecimal([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 8::NUMERIC", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt64(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt16(0),                 Is.EqualTo(8));
            Assert.That(reader.GetByte(0),                  Is.EqualTo(8));
            Assert.That(reader.GetFloat(0),                 Is.EqualTo(8.0f));
            Assert.That(reader.GetDouble(0),                Is.EqualTo(8.0d));
            Assert.That(reader.GetDecimal(0),               Is.EqualTo(8.0m));
            Assert.That(reader.GetValue(0),                 Is.EqualTo(8));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(8));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadBool([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT TRUE::BOOLEAN, FALSE::BOOLEAN", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetBoolean(0), Is.True);
            Assert.That(reader.GetBoolean(1), Is.False);
            Assert.That(reader.GetValue(0), Is.True);
            Assert.That(reader.GetProviderSpecificValue(0), Is.True);
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void ReadMoney([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 12345.12::MONEY, '-10.5'::MONEY", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetDecimal(0), Is.EqualTo(12345.12m));
            Assert.That(reader.GetValue(0), Is.EqualTo(12345.12m));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(12345.12m));
            Assert.That(reader.GetDecimal(1), Is.EqualTo(-10.5m));
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void ReadInternalChar([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT typdelim FROM pg_type WHERE typname='int4'", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetChar(0), Is.EqualTo(','));
            Assert.That(reader.GetValue(0), Is.EqualTo(','));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(','));
            reader.Dispose();
            cmd.Dispose();
        }
    }
}
