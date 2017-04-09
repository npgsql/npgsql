#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests
{
    public class CopyTests : TestBase
    {
        #region Raw

        [Test, Description("Exports data in binary format (raw mode) and then loads it back in")]
        public void RawBinaryRoundtrip()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");

                //var iterations = Conn.BufferSize / 10 + 100;
                //var iterations = Conn.BufferSize / 10 - 100;
                var iterations = 500;

                // Preload some data into the table
                using (var cmd = new NpgsqlCommand("INSERT INTO data (field_text, field_int4) VALUES (@p1, @p2)", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Text, "HELLO");
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Integer, 8);
                    cmd.Prepare();
                    for (var i = 0; i < iterations; i++)
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                var data = new byte[10000];
                var len = 0;
                using (var outStream = conn.BeginRawBinaryCopy("COPY data (field_text, field_int4) TO STDIN BINARY"))
                {
                    StateAssertions(conn);

                    while (true)
                    {
                        var read = outStream.Read(data, len, data.Length - len);
                        if (read == 0)
                            break;
                        len += read;
                    }

                    Assert.That(len, Is.GreaterThan(conn.Settings.ReadBufferSize) & Is.LessThan(data.Length));
                }

                conn.ExecuteNonQuery("TRUNCATE data");

                using (var inStream = conn.BeginRawBinaryCopy("COPY data (field_text, field_int4) FROM STDIN BINARY"))
                {
                    StateAssertions(conn);

                    inStream.Write(data, 0, len);
                }

                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM DATA"), Is.EqualTo(iterations));
            }
        }

        [Test, Description("Disposes a raw binary stream in the middle of an export")]
        public void DisposeInMiddleOfRawBinaryExport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                conn.ExecuteNonQuery("INSERT INTO data (field_text, field_int4) VALUES ('HELLO', 8)");

                var data = new byte[3];
                using (var inStream = conn.BeginRawBinaryCopy("COPY data (field_text, field_int4) TO STDIN BINARY"))
                {
                    // Read some bytes
                    var len = inStream.Read(data, 0, data.Length);
                    Assert.That(len, Is.EqualTo(data.Length));
                }
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Disposes a raw binary stream in the middle of an import")]
        public void DisposeInMiddleOfRawBinaryImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                var inStream = conn.BeginRawBinaryCopy("COPY data (field_text, field_int4) FROM STDIN BINARY");
                inStream.Write(NpgsqlRawCopyStream.BinarySignature, 0, NpgsqlRawCopyStream.BinarySignature.Length);
                Assert.That(() => inStream.Dispose(), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("22P04")
                );
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Cancels a binary write")]
        public void CancelRawBinaryImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                var garbage = new byte[] {1, 2, 3, 4};
                using (var s = conn.BeginRawBinaryCopy("COPY data (field_text, field_int4) FROM STDIN BINARY"))
                {
                    s.Write(garbage, 0, garbage.Length);
                    s.Cancel();
                }

                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test]
        public void ImportLargeValueRaw()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (blob BYTEA)");

                var data = new byte[conn.Settings.WriteBufferSize + 10];
                var dump = new byte[conn.Settings.WriteBufferSize + 200];
                var len = 0;

                // Insert a blob with a regular insert
                using (var cmd = new NpgsqlCommand("INSERT INTO data (blob) VALUES (@p)", conn))
                {
                    cmd.Parameters.AddWithValue("p", data);
                    cmd.ExecuteNonQuery();
                }

                // Raw dump out
                using (var outStream = conn.BeginRawBinaryCopy("COPY data (blob) TO STDIN BINARY"))
                {
                    while (true)
                    {
                        var read = outStream.Read(dump, len, dump.Length - len);
                        if (read == 0)
                            break;
                        len += read;
                    }
                    Assert.That(len < dump.Length);
                }

                conn.ExecuteNonQuery("TRUNCATE data");

                // And raw dump back in
                using (var inStream = conn.BeginRawBinaryCopy("COPY data (blob) FROM STDIN BINARY"))
                {
                    inStream.Write(dump, 0, len);
                }
            }
        }

        #endregion

        #region Binary

        [Test, Description("Roundtrips some data")]
        public void BinaryRoundtrip()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

                using (var writer = conn.BeginBinaryImport("COPY data (field_text, field_int2) FROM STDIN BINARY"))
                {
                    StateAssertions(conn);

                    writer.StartRow();
                    writer.Write("Hello");
                    writer.Write(8, NpgsqlDbType.Smallint);

                    writer.WriteRow("Something", (short) 9);

                    writer.StartRow();
                    writer.Write(longString);
                    writer.WriteNull();
                }

                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));

                using (var reader = conn.BeginBinaryExport("COPY data (field_text, field_int2) TO STDIN BINARY"))
                {
                    StateAssertions(conn);

                    Assert.That(reader.StartRow(), Is.EqualTo(2));
                    Assert.That(reader.Read<string>(), Is.EqualTo("Hello"));
                    Assert.That(reader.Read<int>(NpgsqlDbType.Smallint), Is.EqualTo(8));

                    Assert.That(reader.StartRow(), Is.EqualTo(2));
                    Assert.That(reader.IsNull, Is.False);
                    Assert.That(reader.Read<string>(), Is.EqualTo("Something"));
                    reader.Skip();

                    Assert.That(reader.StartRow(), Is.EqualTo(2));
                    Assert.That(reader.Read<string>(), Is.EqualTo(longString));
                    Assert.That(reader.IsNull, Is.True);
                    reader.Skip();

                    Assert.That(reader.StartRow(), Is.EqualTo(-1));
                }

                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public void CancelBinaryImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                using (var writer = conn.BeginBinaryImport("COPY data (field_text, field_int4) FROM STDIN BINARY"))
                {
                    writer.StartRow();
                    writer.Write("Hello");
                    writer.Write(8);

                    writer.Cancel();
                }
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/657")]
        public void ImportBytea()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field BYTEA)");

                var data = new byte[] {1, 5, 8};

                using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
                {
                    writer.StartRow();
                    writer.Write(data, NpgsqlDbType.Bytea);
                }

                Assert.That(conn.ExecuteScalar("SELECT field FROM data"), Is.EqualTo(data));
            }
        }

        [Test]
        public void ImportStringArray()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field TEXT[])");

                var data = new[] {"foo", "a", "bar"};
                using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
                {
                    writer.StartRow();
                    writer.Write(data, NpgsqlDbType.Array | NpgsqlDbType.Text);
                }

                Assert.That(conn.ExecuteScalar("SELECT field FROM data"), Is.EqualTo(data));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/816")]
        public void ImportStringWithBufferLength()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field TEXT)");

                var data = new string('a', conn.Settings.WriteBufferSize);
                using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
                {
                    writer.StartRow();
                    writer.Write(data, NpgsqlDbType.Text);
                }
                Assert.That(conn.ExecuteScalar("SELECT field FROM data"), Is.EqualTo(data));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/662")]
        public void ImportDirectBuffer()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (blob BYTEA)");

                using (var writer = conn.BeginBinaryImport("COPY data (blob) FROM STDIN BINARY"))
                {
                    // Big value - triggers use of the direct write optimization
                    var data = new byte[conn.Settings.WriteBufferSize + 10];

                    writer.StartRow();
                    writer.Write(data);
                    writer.StartRow();
                    writer.Write(data);
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/661")]
        [Ignore("Unreliable")]
        public void UnexpectedExceptionBinaryImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (blob BYTEA)");

                var data = new byte[conn.Settings.WriteBufferSize + 10];

                var writer = conn.BeginBinaryImport("COPY data (blob) FROM STDIN BINARY");

                using (var conn2 = OpenConnection())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");

                Thread.Sleep(50);
                Assert.That(() =>
                {
                    writer.StartRow();
                    writer.Write(data);
                    writer.Dispose();
                }, Throws.Exception.TypeOf<IOException>());
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/657")]
        [Explicit]
        public void ImportByteaMassive()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field BYTEA)");

                const int iterations = 10000;
                var data = new byte[1024*1024];

                using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
                {
                    for (var i = 0; i < iterations; i++)
                    {
                        if (i%100 == 0)
                            Console.WriteLine("Iteration " + i);
                        writer.StartRow();
                        writer.Write(data, NpgsqlDbType.Bytea);
                    }
                }

                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(iterations));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1134")]
        public void ReadBitString()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (bits BIT(3), bitarray BIT(3)[])");
                conn.ExecuteNonQuery("INSERT INTO data (bits, bitarray) VALUES (B'101', ARRAY[B'101', B'111'])");

                using (var reader = conn.BeginBinaryExport("COPY data (bits, bitarray) TO STDIN BINARY"))
                {
                    reader.StartRow();
                    Assert.That(reader.Read<BitArray>(), Is.EqualTo(new BitArray(new[] { true, false, true })));
                    Assert.That(reader.Read<BitArray[]>(), Is.EqualTo(new[]
                    {
                        new BitArray(new[] { true, false, true }),
                        new BitArray(new[] { true, true, true })
                    }));
                }
            }
        }

        [Test]
        public void Array()
        {
            var expected = new[] { 8 };

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (arr INTEGER[])");

                using (var writer = conn.BeginBinaryImport("COPY data (arr) FROM STDIN BINARY"))
                {
                    writer.StartRow();
                    writer.Write(expected);
                }

                using (var reader = conn.BeginBinaryExport("COPY data (arr) TO STDIN BINARY"))
                {
                    reader.StartRow();
                    Assert.That(reader.Read<int[]>(), Is.EqualTo(expected));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1440")]
        public void ErrorDuringImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INT, CONSTRAINT uq UNIQUE(foo))");
                var writer = conn.BeginBinaryImport("COPY DATA (foo) FROM STDIN BINARY");
                writer.StartRow();
                writer.Write(8);
                writer.StartRow();
                writer.Write(8);
                Assert.That(() => writer.Dispose(), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("23505"));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        #endregion

        #region Text

        [Test]
        public void TextImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                const string line = "HELLO\t1\n";

                // Short write
                var writer = conn.BeginTextImport("COPY data (field_text, field_int4) FROM STDIN");
                StateAssertions(conn);
                writer.Write(line);
                writer.Dispose();
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data WHERE field_int4=1"), Is.EqualTo(1));
                Assert.That(() => writer.Write(line), Throws.Exception.TypeOf<ObjectDisposedException>());
                conn.ExecuteNonQuery("TRUNCATE data");

                // Long (multi-buffer) write
                var iterations = WriteBuffer.MinimumSize/line.Length + 100;
                writer = conn.BeginTextImport("COPY data (field_text, field_int4) FROM STDIN");
                for (var i = 0; i < iterations; i++)
                    writer.Write(line);
                writer.Dispose();
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data WHERE field_int4=1"), Is.EqualTo(iterations));
            }
        }

        [Test]
        public void CancelTextImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");

                var writer = (NpgsqlCopyTextWriter)conn.BeginTextImport("COPY data (field_text, field_int4) FROM STDIN");
                writer.Write("HELLO\t1\n");
                writer.Cancel();
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test]
        public void TextImportEmpty()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                using (conn.BeginTextImport("COPY data (field_text, field_int4) FROM STDIN"))
                {
                }
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test]
        public void TextExport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                var chars = new char[30];

                // Short read
                conn.ExecuteNonQuery("INSERT INTO data (field_text, field_int4) VALUES ('HELLO', 1)");
                var reader = conn.BeginTextExport("COPY data (field_text, field_int4) TO STDIN");
                StateAssertions(conn);
                Assert.That(reader.Read(chars, 0, chars.Length), Is.EqualTo(8));
                Assert.That(new string(chars, 0, 8), Is.EqualTo("HELLO\t1\n"));
                Assert.That(reader.Read(chars, 0, chars.Length), Is.EqualTo(0));
                Assert.That(reader.Read(chars, 0, chars.Length), Is.EqualTo(0));
                reader.Dispose();
                Assert.That(() => reader.Read(chars, 0, chars.Length), Throws.Exception.TypeOf<ObjectDisposedException>());
                conn.ExecuteNonQuery("TRUNCATE data");
            }
        }

        [Test]
        public void DisposeInMiddleOfTextExport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                conn.ExecuteNonQuery("INSERT INTO data (field_text, field_int4) VALUES ('HELLO', 1)");
                var reader = conn.BeginTextExport("COPY data (field_text, field_int4) TO STDIN");
                reader.Dispose();
                // Make sure the connection is stil OK
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        #endregion

        #region Other

        [Test, Description("Starts a transaction before a COPY, testing that prepended messages are handled well")]
        public void PrependedMessages()
        {
            using (var conn = OpenConnection())
            {
                conn.BeginTransaction();
                TextImport();
            }
        }

        [Test]
        public void UndefinedTable()
        {
            using (var conn = OpenConnection())
                Assert.That(() => conn.BeginBinaryImport("COPY undefined_table (field_text, field_int2) FROM STDIN BINARY"),
                    Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("42P01")
                );
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/621")]
        public void CloseDuringCopy()
        {
            // TODO: Check no broken connections were returned to the pool
            using (var conn = OpenConnection()) {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                conn.BeginBinaryImport("COPY data (field_text, field_int4) FROM STDIN BINARY");
            }

            using (var conn = OpenConnection()) {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                conn.BeginBinaryExport("COPY data (field_text, field_int2) TO STDIN BINARY");
            }

            using (var conn = OpenConnection()) {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                conn.BeginRawBinaryCopy("COPY data (field_text, field_int4) FROM STDIN BINARY");
            }

            using (var conn = OpenConnection()) {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                conn.BeginRawBinaryCopy("COPY data (field_text, field_int4) TO STDIN BINARY");
            }

            using (var conn = OpenConnection()) {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                conn.BeginTextImport("COPY data (field_text, field_int4) FROM STDIN");
            }

            using (var conn = OpenConnection()) {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                conn.BeginTextExport("COPY data (field_text, field_int4) TO STDIN");
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/994")]
        public void NonAsciiColumnName()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (non_ascii_éè TEXT)");
                using (conn.BeginBinaryImport("COPY data (non_ascii_éè) FROM STDIN BINARY")) { }
            }
        }

        [Test, IssueLink("http://stackoverflow.com/questions/37431054/08p01-insufficient-data-left-in-message-for-nullable-datetime/37431464")]
        public void WriteNullValues()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo1 INT, foo2 UUID, foo3 INT, foo4 UUID)");

                using (var writer = conn.BeginBinaryImport("COPY data (foo1, foo2, foo3, foo4) FROM STDIN BINARY"))
                {
                    writer.StartRow();
                    writer.Write(DBNull.Value, NpgsqlDbType.Integer);
                    writer.Write((string)null, NpgsqlDbType.Uuid);
                    writer.Write(DBNull.Value);
                    writer.Write((string)null);
                }
                using (var cmd = new NpgsqlCommand("SELECT foo1,foo2,foo3,foo4 FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    Assert.That(reader.Read(), Is.True);
                    for (var i = 0; i < reader.FieldCount; i++)
                        Assert.That(reader.IsDBNull(i), Is.True);
                }
            }
        }

        #endregion

        #region Utils

        /// <summary>
        /// Checks that the connector state is properly managed for COPY operations
        /// </summary>
        void StateAssertions(NpgsqlConnection conn)
        {
            Assert.That(conn.Connector.State, Is.EqualTo(ConnectorState.Copy));
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open | ConnectionState.Fetching));
            Assert.That(() => conn.ExecuteScalar("SELECT 1"), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());
        }

        #endregion
    }
}
