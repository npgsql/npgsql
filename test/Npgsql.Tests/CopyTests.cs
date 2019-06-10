using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class CopyTests : TestBase
    {
        #region issue 2257

        [Test, Description("Reproduce #2257")]
        public async Task Issue2257([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection(new NpgsqlConnectionStringBuilder(ConnectionString) { CommandTimeout = 3 }))
            {
                const int rowCount = 1000000;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"CREATE TEMP TABLE test_2257_master AS SELECT * FROM generate_series(1, {rowCount}) id";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "ALTER TABLE test_2257_master ADD CONSTRAINT master_pk PRIMARY KEY (id)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TEMP TABLE test_2257_detail (master_id integer NOT NULL REFERENCES test_2257_master (id))";
                    cmd.ExecuteNonQuery();
                }

                using (var writer = conn.BeginBinaryImport("COPY test_2257_detail FROM STDIN BINARY"))
                {
                    for (var i = 1; i <= rowCount; ++i)
                    {
                        await writer.StartRow(isAsync);
                        await writer.Write(i, isAsync);
                    }

                    var e = Assert.Throws<NpgsqlException>(() => writer.Complete());
                    Assert.That(e.InnerException, Is.TypeOf<IOException>());
                }
            }
        }

        #endregion

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
        public async Task CancelRawBinaryImport([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                var garbage = new byte[] {1, 2, 3, 4};
                using (var s = conn.BeginRawBinaryCopy("COPY data (field_text, field_int4) FROM STDIN BINARY"))
                {
                    s.Write(garbage, 0, garbage.Length);
                    await s.Cancel(isAsync);
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

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongTableDefinitionRawBinaryCopy()
        {
            using (var conn = OpenConnection())
            {
                Assert.Throws<PostgresException>(() => conn.BeginRawBinaryCopy("COPY table_is_not_exist (blob) TO STDOUT BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));

                Assert.Throws<PostgresException>(() => conn.BeginRawBinaryCopy("COPY table_is_not_exist (blob) FROM STDIN BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongFormatRawBinaryCopy()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("create temp table temp_table(blob bytea)");
                Assert.Throws<ArgumentException>(() => conn.BeginRawBinaryCopy("COPY temp_table (blob) TO STDOUT"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("create temp table temp_table(blob bytea)");
                Assert.Throws<ArgumentException>(() => conn.BeginRawBinaryCopy("COPY temp_table (blob) FROM STDIN"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        #endregion

        #region Binary

        [Test, Description("Roundtrips some data")]
        public async Task BinaryRoundtrip([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT)");
                var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

                using (var writer = conn.BeginBinaryImport("COPY data (field_text, field_int2) FROM STDIN BINARY"))
                {
                    StateAssertions(conn);

                    await writer.StartRow(isAsync);
                    await writer.Write("Hello", isAsync);
                    await writer.Write((short)8, NpgsqlDbType.Smallint, isAsync);

                    await writer.WriteRow(isAsync, "Something", (short)9);

                    await writer.StartRow(isAsync);
                    await writer.Write(longString, "text", isAsync);
                    await writer.WriteNull(isAsync);

                    var rowsWritten = await writer.Complete(isAsync);
                    Assert.That(rowsWritten, Is.EqualTo(3));
                }

                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));

                using (var reader = conn.BeginBinaryExport("COPY data (field_text, field_int2) TO STDIN BINARY"))
                {
                    StateAssertions(conn);

                    Assert.That(await reader.StartRow(isAsync), Is.EqualTo(2));
                    Assert.That(await reader.Read<string>(isAsync), Is.EqualTo("Hello"));
                    Assert.That(await reader.Read<int>(NpgsqlDbType.Smallint, isAsync), Is.EqualTo(8));

                    Assert.That(await reader.StartRow(isAsync), Is.EqualTo(2));
                    Assert.That(reader.IsNull, Is.False);
                    Assert.That(await reader.Read<string>(isAsync), Is.EqualTo("Something"));
                    await reader.Skip(isAsync);

                    Assert.That(await reader.StartRow(isAsync), Is.EqualTo(2));
                    Assert.That(await reader.Read<string>(isAsync), Is.EqualTo(longString));
                    Assert.That(reader.IsNull, Is.True);
                    await reader.Skip(isAsync);

                    Assert.That(await reader.StartRow(isAsync), Is.EqualTo(-1));
                }

                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task CancelBinaryImport([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                using (var writer = conn.BeginBinaryImport("COPY data (field_text, field_int4) FROM STDIN BINARY"))
                {
                    await writer.StartRow(isAsync);
                    await writer.Write("Hello", isAsync);
                    await writer.Write(8, isAsync);
                    // No commit should rollback
                }
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/657")]
        public async Task ImportBytea([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field BYTEA)");

                var data = new byte[] {1, 5, 8};

                using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
                {
                    await writer.StartRow(isAsync);
                    await writer.Write(data, NpgsqlDbType.Bytea, isAsync);
                    var rowsWritten = await writer.Complete(isAsync);
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }

                Assert.That(conn.ExecuteScalar("SELECT field FROM data"), Is.EqualTo(data));
            }
        }

        [Test]
        public async Task ImportStringArray([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field TEXT[])");

                var data = new[] {"foo", "a", "bar"};
                using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
                {
                    await writer.StartRow(isAsync);
                    await writer.Write(data, NpgsqlDbType.Array | NpgsqlDbType.Text, isAsync);
                    var rowsWritten = await writer.Complete(isAsync);
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }

                Assert.That(conn.ExecuteScalar("SELECT field FROM data"), Is.EqualTo(data));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/816")]
        public async Task ImportStringWithBufferLength([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field TEXT)");

                var data = new string('a', conn.Settings.WriteBufferSize);
                using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
                {
                    await writer.StartRow(isAsync);
                    await writer.Write(data, NpgsqlDbType.Text, isAsync);
                    var rowsWritten = await writer.Complete(isAsync);
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }
                Assert.That(conn.ExecuteScalar("SELECT field FROM data"), Is.EqualTo(data));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/662")]
        public async Task ImportDirectBuffer([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (blob BYTEA)");

                using (var writer = conn.BeginBinaryImport("COPY data (blob) FROM STDIN BINARY"))
                {
                    // Big value - triggers use of the direct write optimization
                    var data = new byte[conn.Settings.WriteBufferSize + 10];

                    await writer.StartRow(isAsync);
                    await writer.Write(data, isAsync);
                    await writer.StartRow(isAsync);
                    await writer.Write(data, isAsync);
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongTableDefinitionBinaryImport()
        {
            using (var conn = OpenConnection())
            {
                // Connection should be kept alive after PostgresException was triggered
                Assert.Throws<PostgresException>(() => conn.BeginBinaryImport("COPY table_is_not_exist (blob) FROM STDIN BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongFormatBinaryImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("create temp table temp_table(blob bytea)");
                Assert.Throws<ArgumentException>(() => conn.BeginBinaryImport("COPY temp_table (blob) FROM STDIN"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongTableDefinitionBinaryExport()
        {
            using (var conn = OpenConnection())
            {
                // Connection should be kept alive after PostgresException was triggered
                Assert.Throws<PostgresException>(() => conn.BeginBinaryExport("COPY table_is_not_exist (blob) TO STDOUT BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongFormatBinaryExport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("create temp table temp_table(blob bytea)");
                Assert.Throws<ArgumentException>(() => conn.BeginBinaryExport("COPY temp_table (blob) TO STDOUT"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/661")]
        [Ignore("Unreliable")]
        public void UnexpectedExceptionBinaryImport([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (blob BYTEA)");

                var data = new byte[conn.Settings.WriteBufferSize + 10];

                var writer = conn.BeginBinaryImport("COPY data (blob) FROM STDIN BINARY");

                using (var conn2 = OpenConnection())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");

                Thread.Sleep(50);
                Assert.That(async () =>
                {
                    await writer.StartRow(isAsync);
                    await writer.Write(data, isAsync);
                    writer.Dispose();
                }, Throws.Exception.TypeOf<IOException>());
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/657")]
        [Explicit]
        public async Task ImportByteaMassive([Values(true, false)] bool isAsync)
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
                        await writer.StartRow(isAsync);
                        await writer.Write(data, NpgsqlDbType.Bytea, isAsync);
                    }
                }

                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(iterations));
            }
        }

        [Test]
        public async Task ExportLongString([Values(true, false)] bool isAsync)
        {
            const int iterations = 100;
            using (var conn = OpenConnection())
            {
                var len = conn.Settings.WriteBufferSize;
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo1 TEXT, foo2 TEXT, foo3 TEXT, foo4 TEXT, foo5 TEXT)");
                using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p, @p, @p, @p, @p)", conn))
                {
                    cmd.Parameters.AddWithValue("p", new string('x', len));
                    for (var i = 0; i < iterations; i++)
                        cmd.ExecuteNonQuery();
                }

                using (var reader = conn.BeginBinaryExport("COPY data (foo1, foo2, foo3, foo4, foo5) TO STDIN BINARY"))
                {
                    for (var row = 0; row < iterations; row++)
                    {
                        Assert.That(await reader.StartRow(isAsync), Is.EqualTo(5));
                        for (var col = 0; col < 5; col++)
                            Assert.That((await reader.Read<string>(isAsync)).Length, Is.EqualTo(len));
                    }
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1134")]
        public async Task ReadBitString([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (bits BIT(3), bitarray BIT(3)[])");
                conn.ExecuteNonQuery("INSERT INTO data (bits, bitarray) VALUES (B'101', ARRAY[B'101', B'111'])");

                using (var reader = conn.BeginBinaryExport("COPY data (bits, bitarray) TO STDIN BINARY"))
                {
                    await reader.StartRow(isAsync);
                    Assert.That(await reader.Read<BitArray>(isAsync), Is.EqualTo(new BitArray(new[] { true, false, true })));
                    Assert.That(await reader.Read<BitArray[]>(isAsync), Is.EqualTo(new[]
                    {
                        new BitArray(new[] { true, false, true }),
                        new BitArray(new[] { true, true, true })
                    }));
                }
            }
        }

        [Test]
        public async Task Array([Values(true, false)] bool isAsync)
        {
            var expected = new[] { 8 };

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (arr INTEGER[])");

                using (var writer = conn.BeginBinaryImport("COPY data (arr) FROM STDIN BINARY"))
                {
                    await writer.StartRow(isAsync);
                    await writer.Write(expected, isAsync);
                    var rowsWritten = await writer.Complete(isAsync);
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }

                using (var reader = conn.BeginBinaryExport("COPY data (arr) TO STDIN BINARY"))
                {
                    await reader.StartRow(isAsync);
                    Assert.That(await reader.Read<int[]>(isAsync), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void Enum()
        {
            var expected = Mood.Happy;

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('sad', 'ok', 'happy')");
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<Mood>();

                conn.ExecuteNonQuery("CREATE TEMP TABLE data (mymood mood)");

                using (var writer = conn.BeginBinaryImport("COPY data (mymood) FROM STDIN BINARY"))
                {
                    writer.StartRow();
                    writer.Write(expected);
                    var rowsWritten = writer.Complete();
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }

                using (var reader = conn.BeginBinaryExport("COPY data (mymood) TO STDIN BINARY"))
                {
                    reader.StartRow();
                    Assert.That(reader.Read<Mood>(), Is.EqualTo(expected));
                }
            }
        }

        enum Mood { Sad, Ok, Happy };

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1440")]
        public async Task ErrorDuringImport([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INT, CONSTRAINT uq UNIQUE(foo))");
                var writer = conn.BeginBinaryImport("COPY DATA (foo) FROM STDIN BINARY");
                await writer.StartRow(isAsync);
                await writer.Write(8, isAsync);
                await writer.StartRow(isAsync);
                await writer.Write(8, isAsync);
                Assert.That(() => writer.Complete(isAsync), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("23505"));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task ImportCannotWriteAfterCommit([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INT)");
                try
                {
                    using (var writer = conn.BeginBinaryImport("COPY DATA (foo) FROM STDIN BINARY"))
                    {
                        await writer.StartRow(isAsync);
                        await writer.Write(8, isAsync);
                        var rowsWritten = await writer.Complete(isAsync);
                        Assert.That(rowsWritten, Is.EqualTo(1));
                        await writer.StartRow(isAsync);
                        Assert.Fail("StartRow should have thrown");
                    }
                }
                catch (InvalidOperationException)
                {
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
                }
            }
        }

        [Test]
        public async Task ImportCommitInMiddleOfRow([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INT, bar TEXT)");
                try
                {
                    using (var writer = conn.BeginBinaryImport("COPY DATA (foo, bar) FROM STDIN BINARY"))
                    {
                        await writer.StartRow(isAsync);
                        await writer.Write(8, isAsync);
                        await writer.Write("hello", isAsync);
                        await writer.StartRow(isAsync);
                        await writer.Write(9, isAsync);
                        await writer.Complete(isAsync);
                        Assert.Fail("Commit should have thrown");
                    }
                }
                catch (InvalidOperationException)
                {
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
                }
            }
        }

        [Test]
        public async Task ImportExceptionDoesNotCommit([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INT)");
                try
                {
                    using (var writer = conn.BeginBinaryImport("COPY DATA (foo) FROM STDIN BINARY"))
                    {
                        await writer.StartRow(isAsync);
                        await writer.Write(8, isAsync);
                        throw new Exception("FOO");
                    }
                }
                catch (Exception e) when (e.Message == "FOO")
                {
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.Zero);
                }
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
                var iterations = NpgsqlWriteBuffer.MinimumSize/line.Length + 100;
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

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongTableDefinitionTextImport()
        {
            using (var conn = OpenConnection())
            {
                Assert.Throws<PostgresException>(() => conn.BeginTextImport("COPY table_is_not_exist (blob) FROM STDIN"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongFormatTextImport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("create temp table temp_table(blob bytea)");
                Assert.Throws<Exception>(() => conn.BeginTextImport("COPY temp_table (blob) FROM STDIN BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongTableDefinitionTextExport()
        {
            using (var conn = OpenConnection())
            {
                Assert.Throws<PostgresException>(() => conn.BeginTextExport("COPY table_is_not_exist (blob) TO STDOUT"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public void WrongFormatTextExport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("create temp table temp_table(blob bytea)");
                Assert.Throws<Exception>(() => conn.BeginTextExport("COPY temp_table (blob) TO STDOUT BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
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
        public async Task WriteNullValues([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo1 INT, foo2 UUID, foo3 INT, foo4 UUID)");

                using (var writer = conn.BeginBinaryImport("COPY data (foo1, foo2, foo3, foo4) FROM STDIN BINARY"))
                {
                    await writer.StartRow(isAsync);
                    await writer.Write(DBNull.Value, NpgsqlDbType.Integer, isAsync);
                    await writer.Write((string?)null, NpgsqlDbType.Uuid, isAsync);
                    await writer.Write(DBNull.Value, isAsync);
                    await writer.Write((string?)null, isAsync);
                    var rowsWritten = await writer.Complete(isAsync);
                    Assert.That(rowsWritten, Is.EqualTo(1));
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

        [Test]
        public async Task WriteDifferentTypes([Values(true, false)] bool isAsync)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo INT, bar INT[])");

                using (var writer = conn.BeginBinaryImport("COPY data (foo, bar) FROM STDIN BINARY"))
                {
                    await writer.StartRow(isAsync);
                    await writer.Write(3.0, NpgsqlDbType.Integer, isAsync);
                    await writer.Write((object)new[] { 1, 2, 3 }, isAsync);
                    await writer.StartRow(isAsync);
                    await writer.Write(3, NpgsqlDbType.Integer, isAsync);
                    await writer.Write((object)new List<int> { 4, 5, 6 }, isAsync);
                    var rowsWritten = await writer.Complete(isAsync);
                    Assert.That(rowsWritten, Is.EqualTo(2));
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(2));
            }
        }

        #endregion

        #region Utils

        /// <summary>
        /// Checks that the connector state is properly managed for COPY operations
        /// </summary>
        void StateAssertions(NpgsqlConnection conn)
        {
            Assert.That(conn.Connector!.State, Is.EqualTo(ConnectorState.Copy));
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open | ConnectionState.Fetching));
            Assert.That(() => conn.ExecuteScalar("SELECT 1"), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());
        }

        #endregion
    }
}
