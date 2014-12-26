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
    /// <summary>
    /// Tests on the PostgreSQL bytea type
    /// </summary>
    /// <summary>
    /// http://www.postgresql.org/docs/9.4/static/datatype-binary.html
    /// </summary>
    class ByteaTests : TestBase
    {
        public ByteaTests(string backendVersion) : base(backendVersion) {}

        [Test]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential"   )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential"  )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential"     )]
        public void Read(PrepareOrNot prepare, CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            byte[] expected = { 1, 2, 3, 4, 5 };
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_bytea) VALUES ({0})", EncodeHex(expected)));

            const string queryText = @"SELECT field_bytea, 'foo', field_bytea, field_bytea, field_bytea FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            var actual = reader.GetFieldValue<byte[]>(0);
            Assert.That(actual, Is.EqualTo(expected));

            if (IsSequential(behavior))
                Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
            else
                Assert.That(reader.GetFieldValue<byte[]>(0), Is.EqualTo(expected));

            Assert.That(reader.GetString(1), Is.EqualTo("foo"));

            Assert.That(reader[2], Is.EqualTo(expected));
            Assert.That(reader.GetValue(3), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<byte[]>(4), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential"   )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential"  )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential"     )]
        public void GetBytes(PrepareOrNot prepare, CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            byte[] expected = { 1, 2, 3, 4, 5 };
            var actual = new byte[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_bytea) VALUES ({0})", EncodeHex(expected)));

            const string queryText = @"SELECT field_bytea, 'foo', field_bytea, 'bar', field_bytea, field_bytea FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            Assert.That(reader.GetBytes(0, 0, actual, 0, 2), Is.EqualTo(2));
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            Assert.That(reader.GetBytes(0, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");
            if (IsSequential(behavior))
                Assert.That(() => reader.GetBytes(0, 0, actual, 4, 1), Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
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
            Assert.That(reader.GetBytes(4, expected.Length - 1, actual, 0, 2), Is.EqualTo(1), "Length greater than data length");
            Assert.That(actual[0], Is.EqualTo(expected[expected.Length - 1]), "Length greater than data length");
            Assert.That(() => reader.GetBytes(4, 0, actual, 0, actual.Length + 1), Throws.Exception.TypeOf<IndexOutOfRangeException>(), "Length great than output buffer length");
            // Close in the middle of a column
            reader.GetBytes(5, 0, actual, 0, 2);
            reader.Close();
            cmd.Dispose();

            //var result = (byte[]) cmd.ExecuteScalar();
            //Assert.AreEqual(2, result.Length);
        }

        [Test]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential"   )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential"  )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential"     )]
        public void GetStream(PrepareOrNot prepare, CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            byte[] expected = { 1, 2, 3, 4, 5 };
            var actual = new byte[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_bytea) VALUES ({0})", EncodeHex(expected)));

            const string queryText = @"SELECT field_bytea, 'foo' FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            var stream = reader.GetStream(0);
            Assert.That(stream.Length, Is.EqualTo(expected.Length));
            stream.Read(actual, 0, 2);
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            Assert.That(() => reader.GetString(1), Throws.Exception.TypeOf<InvalidOperationException>(), "Access row while streaming");
            stream.Read(actual, 2, 1);
            Assert.That(actual[2], Is.EqualTo(expected[2]));
            stream.Close();
            if (IsSequential(behavior))
                Assert.That(() => reader.GetBytes(0, 0, actual, 4, 1), Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
            else
            {
                Assert.That(reader.GetBytes(0, 0, actual, 4, 1), Is.EqualTo(1));
                Assert.That(actual[4], Is.EqualTo(expected[0]));
            }
            Assert.That(reader.GetString(1), Is.EqualTo("foo"));
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default, TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential")]
        [TestCase(PrepareOrNot.Prepared, CommandBehavior.Default, TestName = "PreparedNonSequential")]
        [TestCase(PrepareOrNot.Prepared, CommandBehavior.SequentialAccess, TestName = "PreparedSequential")]
        public void GetNull(PrepareOrNot prepare, CommandBehavior behavior)
        {
            var buf = new byte[8];
            ExecuteNonQuery(@"INSERT INTO data (field_bytea) VALUES (NULL)");
            var cmd = new NpgsqlCommand("SELECT field_bytea FROM data", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();
            Assert.That(reader.IsDBNull(0), Is.True);
            Assert.That(() => reader.GetBytes(0, 0, buf, 0, 1), Throws.Exception, "GetBytes");
            Assert.That(() => reader.GetStream(0), Throws.Exception, "GetStream");
            Assert.That(() => reader.GetBytes(0, 0, null, 0, 0), Throws.Exception, "GetBytes with null buffer");
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void EmptyRoundtrip([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new byte[0];
            var cmd = new NpgsqlCommand("SELECT :val::BYTEA", Conn);
            cmd.Parameters.Add("val", NpgsqlDbType.Bytea);
            cmd.Parameters["val"].Value = expected;
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var result = (byte[])cmd.ExecuteScalar();
            Assert.That(result, Is.EqualTo(expected));
            cmd.Dispose();
        }

        // Older tests from here

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

        // Older tests

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
            var hex = new StringBuilder(@"E'\\x", buf.Count * 2 + 3);
            foreach (byte b in buf) {
                hex.Append(String.Format("{0:x2}", b));
            }
            hex.Append("'");
            return hex.ToString();
        }

        #endregion
    }
}
