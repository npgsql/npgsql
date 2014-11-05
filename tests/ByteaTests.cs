using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests
{
    class ByteaTests : TestBase
    {
        public ByteaTests(string backendVersion) : base(backendVersion) {}

        [Test]
        //[Timeout(3000)]
        //[TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        //[TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential"   )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential"  )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential"     )]
        public void Read(PrepareOrNot prepare, CommandBehavior behavior)
        {
            byte[] expected = { 1, 2, 3, 4, 5 };
            var actual = new byte[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_bytea) VALUES ({0})", EncodeHex(expected)));

            const string queryText = @"SELECT field_bytea, 'foo', field_bytea, 'bar', field_bytea FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) cmd.Prepare();
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            Assert.That(reader.GetBytes(0, 0, actual, 0, 2), Is.EqualTo(2));
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            Assert.That(reader.GetBytes(0, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");
            Assert.That(() => reader.GetBytes(0, 10, actual, 0, 1), Throws.Exception, "GetBytes from after column ends");
            if (IsSequential(behavior))
                Assert.That(() => reader.GetBytes(0, 0, actual, 4, 1), Throws.Exception, "Seek back sequential");
            else
            {
                Assert.That(reader.GetBytes(0, 0, actual, 4, 1), Is.EqualTo(1));
                Assert.That(actual[4], Is.EqualTo(expected[0]));
            }
            Assert.That(reader.GetBytes(0, 2, actual, 2, 3), Is.EqualTo(3));
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(reader.GetBytes(0, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");

            Assert.That(() => reader.GetBytes(1, 0, null, 0, 0), Throws.Exception.TypeOf<InvalidCastException>(), "GetBytes on non-bytea");
            Assert.That(() => reader.GetBytes(1, 0, actual, 0, 1), Throws.Exception.TypeOf<InvalidCastException>(), "GetBytes on non-bytea");
            Assert.That(reader.GetString(1), Is.EqualTo("foo"));
            reader.GetBytes(2, 0, actual, 0, 2);
            // Jump to another column from the middle of the column
            reader.GetBytes(4, 0, actual, 0, 2);
            // Close in the middle of a column
            reader.Close();
            cmd.Dispose();

            // Redo the query to test GetStream()
            cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            reader = cmd.ExecuteReader(behavior);
            reader.Read();
            var stream = reader.GetStream(2);
            Assert.That(stream.Length, Is.EqualTo(expected.Length));
            stream.Read(actual, 0, expected.Length);
            Assert.That(actual, Is.EqualTo(expected));
            reader.Close();
            cmd.Dispose();

            // Test null column
            ExecuteNonQuery(@"TRUNCATE data");
            ExecuteNonQuery(@"INSERT INTO data (field_bytea) VALUES (NULL)");
            cmd = new NpgsqlCommand("SELECT field_bytea FROM data", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            reader = cmd.ExecuteReader(behavior);
            reader.Read();
            Assert.That(() => reader.GetBytes(0, 0, null, 0, 0), Throws.Exception, "GetBytes with empty buffer on null");
            Assert.That(() => reader.GetBytes(0, 0, actual, 0, 1), Throws.Exception, "GetBytes on null");
            reader.Close();
            cmd.Dispose();

            //var result = (byte[]) cmd.ExecuteScalar();
            //Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void MultidimensionalRoundtrip([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var cmd = new NpgsqlCommand("SELECT :p1", Conn))
            {
                var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
                var inVal = new[] { bytes, bytes };
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Bytea | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                if (prepare == PrepareOrNot.Prepared) {
                    cmd.Prepare();
                }
                var retVal = (byte[][])cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void Prepared()
        {
            PreparedInternal();
        }

        [Test]
        public void Prepared_SuppressBinary()
        {
            using (this.SuppressBackendBinary())
            {
                PreparedInternal();
            }
        }

        private void PreparedInternal()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
                var inVal = new[] { bytes, bytes };
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Bytea | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                cmd.Prepare();

                var retVal = (byte[][])cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void Empty()
        {
            var buff = new byte[0];
            var command = new NpgsqlCommand("select :val", Conn);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            var result = (Byte[])command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        private void EmptyWithPrepare_Internal()
        {
            var buff = new byte[0];
            new Random().NextBytes(buff);
            var command = new NpgsqlCommand("select :val", Conn);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            command.Prepare();
            var result = (Byte[])command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        [Test]
        public void EmptyWithPrepare()
        {
            EmptyWithPrepare_Internal();
        }

        [Test]
        public void EmptyWithPrepare_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                EmptyWithPrepare_Internal();
            }
        }

        [Test]
        public void Large()
        {
            var buff = new byte[100000];
            new Random().NextBytes(buff);
            var command = new NpgsqlCommand("select :val", Conn);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            var result = (Byte[])command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        private void LargeWithPrepare_Internal()
        {
            var buff = new byte[100000];
            new Random().NextBytes(buff);
            var command = new NpgsqlCommand("select :val", Conn);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            command.Prepare();
            var result = (Byte[])command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        [Test]
        public void LargeWithPrepare()
        {
            LargeWithPrepare_Internal();
        }

        [Test]
        public void ByteaLargeWithPrepareSupport_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                LargeWithPrepare_Internal();
            }
        }

        [Test]
        public void Insert1()
        {
            Byte[] toStore = { 0, 1, 255, 254 };

            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);
            var result = (Byte[])cmd.ExecuteScalar();
            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void Insert2()
        {
            Byte[] toStore = { 1, 2, 127, 126 };

            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);
            var result = (Byte[])cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void InsertWithPrepare1()
        {
            Byte[] toStore = { 0 };

            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);

            cmd.Prepare();
            var result = (Byte[])cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void InsertWithPrepare2()
        {
            Byte[] toStore = { 1 };

            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);

            cmd.Prepare();
            var result = (Byte[])cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void Parameter()
        {
            var command = new NpgsqlCommand("select field_bytea from data where field_bytea = :bytesData", Conn);
            var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
            command.Parameters[":bytesData"].Value = bytes;
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(-1, result);
        }

        private void ParameterWithPrepare_Internal()
        {
            var command = new NpgsqlCommand("select field_bytea from data where field_bytea = :bytesData", Conn);

            var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
            command.Parameters[":bytesData"].Value = bytes;
            command.Prepare();
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void ParameterWithPrepare()
        {
            ParameterWithPrepare_Internal();
        }

        [Test]
        public void ParameterWithPrepare_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                ParameterWithPrepare_Internal();
            }
        }

        #region Utilities

        /// <summary>
        /// Utility to encode a byte array in Postgresql hex format
        /// See http://www.postgresql.org/docs/current/static/datatype-binary.html
        /// </summary>
        static string EncodeHex(ICollection<byte> buf)
        {
            var hex = new StringBuilder(@"E'\\x", buf.Count * 2);
            foreach (byte b in buf) {
                hex.Append(String.Format("{0:x2}", b));
            }
            hex.Append("'");
            return hex.ToString();
        }

        static bool IsSequential(CommandBehavior behavior)
        {
            return (behavior & CommandBehavior.SequentialAccess) != 0;
        }

        #endregion
    }
}
