using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NpgsqlTests.Types
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

        [Test, Description("Roundtrips a bytea")]
        public void Roundtrip()
        {
            byte[] expected = { 1, 2, 3, 4, 5 };
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Bytea);
            var p2 = new NpgsqlParameter("p2", DbType.Binary);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expected };
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea));
            Assert.That(p3.DbType, Is.EqualTo(DbType.Binary));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = p2.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i),          Is.EqualTo(typeof (byte[])));
                Assert.That(reader.GetFieldValue<byte[]>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i),              Is.EqualTo(expected));
            }

            reader.Close();
            cmd.Dispose();            
        }

        [Test]
        public void RoundtripLarge()
        {
            var expected = new byte[Conn.BufferSize + 100];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = 8;
            var cmd = new NpgsqlCommand("SELECT @p::BYTEA", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Bytea) { Value = expected });
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(byte[])));
            Assert.That(reader.GetFieldValue<byte[]>(0), Is.EqualTo(expected));
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void Read([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            byte[] expected = { 1, 2, 3, 4, 5 };
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_bytea) VALUES ({0})", EncodeHex(expected)));

            const string queryText = @"SELECT field_bytea, 'foo', field_bytea, field_bytea, field_bytea FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
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
        public void GetBytes([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            byte[] expected = { 1, 2, 3, 4, 5 };
            var actual = new byte[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_bytea) VALUES ({0})", EncodeHex(expected)));

            const string queryText = @"SELECT field_bytea, 'foo', field_bytea, 'bar', field_bytea, field_bytea FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
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
        public void GetStream([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            // TODO: This is too small to actually test any interesting sequential behavior
            byte[] expected = { 1, 2, 3, 4, 5 };
            var actual = new byte[expected.Length];
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_bytea) VALUES ({0})", EncodeHex(expected)));

            const string queryText = @"SELECT field_bytea, 'foo' FROM data";
            var cmd = new NpgsqlCommand(queryText, Conn);
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();

            var stream = reader.GetStream(0);
            Assert.That(stream.CanSeek, Is.EqualTo(behavior == CommandBehavior.Default));
            Assert.That(stream.Length, Is.EqualTo(expected.Length));
            stream.Read(actual, 0, 2);
            Assert.That(actual[0], Is.EqualTo(expected[0]));
            Assert.That(actual[1], Is.EqualTo(expected[1]));
            if (behavior == CommandBehavior.Default)
            {
                var stream2 = reader.GetStream(0);
                var actual2 = new byte[2];
                stream2.Read(actual2, 0, 2);
                Assert.That(actual2[0], Is.EqualTo(expected[0]));
                Assert.That(actual2[1], Is.EqualTo(expected[1]));
            }
            else
            {
                Assert.That(() => reader.GetStream(0), Throws.Exception.TypeOf<InvalidOperationException>(), "Sequential stream twice on same column");
            }
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
        public void GetNull([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            var buf = new byte[8];
            ExecuteNonQuery(@"INSERT INTO data (field_bytea) VALUES (NULL)");
            var cmd = new NpgsqlCommand("SELECT field_bytea FROM data", Conn);
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
        public void EmptyRoundtrip()
        {
            var expected = new byte[0];
            var cmd = new NpgsqlCommand("SELECT :val::BYTEA", Conn);
            cmd.Parameters.Add("val", NpgsqlDbType.Bytea);
            cmd.Parameters["val"].Value = expected;
            var result = (byte[])cmd.ExecuteScalar();
            Assert.That(result, Is.EqualTo(expected));
            cmd.Dispose();
        }

        [Test, Description("In sequential mode, checks that moving to the next column disposes a currently open stream")]
        public void StreamDisposeOnSequentialColumn()
        {
            var data = new byte[] { 1, 2, 3 };
            var cmd = new NpgsqlCommand(@"SELECT @p, @p", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("p", data));
            var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            reader.Read();
            var stream = reader.GetStream(0);
            // ReSharper disable once UnusedVariable
            var v = reader.GetValue(1);
            Assert.That(() => stream.ReadByte(), Throws.Exception.TypeOf<ObjectDisposedException>());
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("In non-sequential mode, checks that moving to the next row disposes all currently open streams")]
        public void StreamDisposeOnNonSequentialRow()
        {
            var data = new byte[] { 1, 2, 3 };
            var cmd = new NpgsqlCommand(@"SELECT @p", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("p", data));
            var reader = cmd.ExecuteReader();
            reader.Read();
            var s1 = reader.GetStream(0);
            var s2 = reader.GetStream(0);
            reader.Read();
            Assert.That(() => s1.ReadByte(), Throws.Exception.TypeOf<ObjectDisposedException>());
            Assert.That(() => s2.ReadByte(), Throws.Exception.TypeOf<ObjectDisposedException>());
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("Tests that bytea values are truncated when the NpgsqlParameter's Size is set")]
        public void Truncate()
        {
            byte[] data = { 1, 2, 3, 4 , 5, 6 };
            var cmd = new NpgsqlCommand("SELECT @p", Conn);
            var p = new NpgsqlParameter("p", data) { Size = 4 };
            cmd.Parameters.Add(p);
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));

            // NpgsqlParameter.Size needs to persist when value is changed
            byte[] data2 = { 11, 12, 13, 14, 15, 16 };
            p.Value = data2;
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(new byte[] { 11, 12, 13, 14} ));

            // NpgsqlParameter.Size larger than the value size should mean the value size
            p.Size = data2.Length + 10;
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2));
        }

        // Older tests from here

        [Test]
        public void MultidimensionalRoundtrip()
        {
            using (var cmd = new NpgsqlCommand("SELECT :p1", Conn))
            {
                var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
                var inVal = new[] { bytes, bytes };
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Bytea | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                var retVal = (byte[][])cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void Prepared()
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

        [Test]
        public void ParameterWithPrepare()
        {
            var command = new NpgsqlCommand("select field_bytea from data where field_bytea = :bytesData", Conn);

            var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
            command.Parameters[":bytesData"].Value = bytes;
            command.Prepare();
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(-1, result);
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
