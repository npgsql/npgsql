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
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests
{
    public class CopyTests : MultiplexingTestBase
    {
        #region issue 2257

        [Test, Description("Reproduce #2257")]
        public async Task Issue2257()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");

            using (var conn = await OpenConnectionAsync(new NpgsqlConnectionStringBuilder(ConnectionString) { CommandTimeout = 3 }))
            {
                await using var _ = await GetTempTableName(conn, out var table1);
                await using var __ = await GetTempTableName(conn, out var table2);

                const int rowCount = 1000000;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"CREATE TABLE {table1} AS SELECT * FROM generate_series(1, {rowCount}) id";
                    // Creating table can take some time, so we set quite large timeout
                    cmd.CommandTimeout = 30;
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = $"ALTER TABLE {table1} ADD CONSTRAINT {table1}_pk PRIMARY KEY (id)";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.CommandText = $"CREATE TABLE {table2} (master_id integer NOT NULL REFERENCES {table1} (id))";
                    // We need to fail with timeout while calling writer.Complete() and conn.BeginBinaryImport reuses timeout from previous command
                    // so we set default timeout here
                    cmd.CommandTimeout = 3;
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var writer = conn.BeginBinaryImport($"COPY {table2} FROM STDIN BINARY"))
                {
                    for (var i = 1; i <= rowCount; ++i)
                    {
                        await writer.StartRowAsync();
                        await writer.WriteAsync(i);
                    }

                    var e = Assert.Throws<NpgsqlException>(() => writer.Complete());
                    Assert.That(e.InnerException, Is.TypeOf<TimeoutException>());
                }
            }
        }

        #endregion

        #region Raw

        [Test, Description("Exports data in binary format (raw mode) and then loads it back in")]
        public async Task RawBinaryRoundtrip()
        {
            using (var conn = await OpenConnectionAsync())
            {
                //var iterations = Conn.BufferSize / 10 + 100;
                //var iterations = Conn.BufferSize / 10 - 100;
                const int iterations = 500;

                await using var _ = await GetTempTableName(conn, out var table);

                using (var tx = await conn.BeginTransactionAsync())
                {
                    await conn.ExecuteNonQueryAsync($@"CREATE TABLE {table} (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");

                    // Preload some data into the table
                    using (var cmd =
                        new NpgsqlCommand($"INSERT INTO {table} (field_text, field_int4) VALUES (@p1, @p2)", conn))
                    {
                        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Text, "HELLO");
                        cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Integer, 8);
                        for (var i = 0; i < iterations; i++)
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    await tx.CommitAsync();
                }

                var data = new byte[10000];
                var len = 0;
                using (var outStream = conn.BeginRawBinaryCopy($"COPY {table} (field_text, field_int4) TO STDIN BINARY"))
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

                await conn.ExecuteNonQueryAsync($"TRUNCATE {table}");

                using (var inStream = conn.BeginRawBinaryCopy($"COPY {table} (field_text, field_int4) FROM STDIN BINARY"))
                {
                    StateAssertions(conn);

                    inStream.Write(data, 0, len);
                }

                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(iterations));
            }
        }

        [Test, Description("Disposes a raw binary stream in the middle of an export")]
        public async Task DisposeInMiddleOfRawBinaryExport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await GetTempTableName(conn, out var table);
                await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER);
INSERT INTO {table} (field_text, field_int4) VALUES ('HELLO', 8)");

                var data = new byte[3];
                using (var inStream = conn.BeginRawBinaryCopy($"COPY {table} (field_text, field_int4) TO STDIN BINARY"))
                {
                    // Read some bytes
                    var len = inStream.Read(data, 0, data.Length);
                    Assert.That(len, Is.EqualTo(data.Length));
                }
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Disposes a raw binary stream in the middle of an import")]
        public async Task DisposeInMiddleOfRawBinaryImport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await GetTempTableName(conn, out var table);
                await conn.ExecuteNonQueryAsync($@"CREATE TABLE {table} (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");

                var inStream = conn.BeginRawBinaryCopy($"COPY {table} (field_text, field_int4) FROM STDIN BINARY");
                inStream.Write(NpgsqlRawCopyStream.BinarySignature, 0, NpgsqlRawCopyStream.BinarySignature.Length);
                Assert.That(() => inStream.Dispose(), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("22P04")
                );
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Cancels a binary write")]
        public async Task CancelRawBinaryImport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await GetTempTableName(conn, out var table);
                await conn.ExecuteNonQueryAsync($@"CREATE TABLE {table} (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");

                var garbage = new byte[] {1, 2, 3, 4};
                using (var s = conn.BeginRawBinaryCopy($"COPY {table} (field_text, field_int4) FROM STDIN BINARY"))
                {
                    s.Write(garbage, 0, garbage.Length);
                    await s.CancelAsync();
                }

                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
            }
        }

        [Test]
        public async Task ImportLargeValueRaw()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);

                var data = new byte[conn.Settings.WriteBufferSize + 10];
                var dump = new byte[conn.Settings.WriteBufferSize + 200];
                var len = 0;

                // Insert a blob with a regular insert
                using (var cmd = new NpgsqlCommand($"INSERT INTO {table} (blob) VALUES (@p)", conn))
                {
                    cmd.Parameters.AddWithValue("p", data);
                    await cmd.ExecuteNonQueryAsync();
                }

                // Raw dump out
                using (var outStream = conn.BeginRawBinaryCopy($"COPY {table} (blob) TO STDIN BINARY"))
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

                await conn.ExecuteNonQueryAsync($"TRUNCATE {table}");

                // And raw dump back in
                using (var inStream = conn.BeginRawBinaryCopy($"COPY {table} (blob) FROM STDIN BINARY"))
                {
                    inStream.Write(dump, 0, len);
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongTableDefinitionRawBinaryCopy()
        {
            using (var conn = await OpenConnectionAsync())
            {
                Assert.Throws<PostgresException>(() => conn.BeginRawBinaryCopy("COPY table_is_not_exist (blob) TO STDOUT BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));

                Assert.Throws<PostgresException>(() => conn.BeginRawBinaryCopy("COPY table_is_not_exist (blob) FROM STDIN BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongFormatRawBinaryCopy()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);
                Assert.Throws<ArgumentException>(() => conn.BeginRawBinaryCopy($"COPY {table} (blob) TO STDOUT"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }

            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);
                Assert.Throws<ArgumentException>(() => conn.BeginRawBinaryCopy($"COPY {table} (blob) FROM STDIN"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        #endregion

        #region Binary

        [Test, Description("Roundtrips some data")]
        public async Task BinaryRoundtrip()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT", out var table);

                var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

                using (var writer = conn.BeginBinaryImport($"COPY {table} (field_text, field_int2) FROM STDIN BINARY"))
                {
                    StateAssertions(conn);

                    await writer.StartRowAsync();
                    await writer.WriteAsync("Hello");
                    await writer.WriteAsync((short)8, NpgsqlDbType.Smallint);

                    writer.WriteRow("Something", (short)9);

                    await writer.StartRowAsync();
                    await writer.WriteAsync(longString, "text");
                    await writer.WriteNullAsync();

                    var rowsWritten = await writer.CompleteAsync();
                    Assert.That(rowsWritten, Is.EqualTo(3));
                }

                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));

                using (var reader = conn.BeginBinaryExport($"COPY {table} (field_text, field_int2) TO STDIN BINARY"))
                {
                    StateAssertions(conn);

                    Assert.That(await reader.StartRowAsync(), Is.EqualTo(2));
                    Assert.That(reader.Read<string>(), Is.EqualTo("Hello"));
                    Assert.That(reader.Read<int>(NpgsqlDbType.Smallint), Is.EqualTo(8));

                    Assert.That(await reader.StartRowAsync(), Is.EqualTo(2));
                    Assert.That(reader.IsNull, Is.False);
                    Assert.That(reader.Read<string>(), Is.EqualTo("Something"));
                    await reader.SkipAsync();

                    Assert.That(await reader.StartRowAsync(), Is.EqualTo(2));
                    Assert.That(reader.Read<string>(), Is.EqualTo(longString));
                    Assert.That(reader.IsNull, Is.True);
                    await reader.SkipAsync();

                    Assert.That(await reader.StartRowAsync(), Is.EqualTo(-1));
                }

                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task CancelBinaryImport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);

                using (var writer = conn.BeginBinaryImport($"COPY {table} (field_text, field_int4) FROM STDIN BINARY"))
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync("Hello");
                    await writer.WriteAsync(8);
                    // No commit should rollback
                }
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/657")]
        public async Task ImportBytea()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field BYTEA", out var table);

                var data = new byte[] {1, 5, 8};

                using (var writer = conn.BeginBinaryImport($"COPY {table} (field) FROM STDIN BINARY"))
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(data, NpgsqlDbType.Bytea);
                    var rowsWritten = await writer.CompleteAsync();
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }

                Assert.That(await conn.ExecuteScalarAsync($"SELECT field FROM {table}"), Is.EqualTo(data));
            }
        }

        [Test]
        public async Task ImportStringArray()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field TEXT[]", out var table);

                var data = new[] {"foo", "a", "bar"};
                using (var writer = conn.BeginBinaryImport($"COPY {table} (field) FROM STDIN BINARY"))
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(data, NpgsqlDbType.Array | NpgsqlDbType.Text);
                    var rowsWritten = await writer.CompleteAsync();
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }

                Assert.That(await conn.ExecuteScalarAsync($"SELECT field FROM {table}"), Is.EqualTo(data));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/816")]
        public async Task ImportStringWithBufferLength()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field TEXT", out var table);

                var data = new string('a', conn.Settings.WriteBufferSize);
                using (var writer = conn.BeginBinaryImport($"COPY {table} (field) FROM STDIN BINARY"))
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(data, NpgsqlDbType.Text);
                    var rowsWritten = await writer.CompleteAsync();
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }
                Assert.That(await conn.ExecuteScalarAsync($"SELECT field FROM {table}"), Is.EqualTo(data));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/662")]
        public async Task ImportDirectBuffer()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);

                using (var writer = conn.BeginBinaryImport($"COPY {table} (blob) FROM STDIN BINARY"))
                {
                    // Big value - triggers use of the direct write optimization
                    var data = new byte[conn.Settings.WriteBufferSize + 10];

                    await writer.StartRowAsync();
                    await writer.WriteAsync(data);
                    await writer.StartRowAsync();
                    await writer.WriteAsync(data);
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongTableDefinitionBinaryImport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                // Connection should be kept alive after PostgresException was triggered
                Assert.Throws<PostgresException>(() => conn.BeginBinaryImport("COPY table_is_not_exist (blob) FROM STDIN BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongFormatBinaryImport()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);
                Assert.Throws<ArgumentException>(() => conn.BeginBinaryImport($"COPY {table} (blob) FROM STDIN"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongTableDefinitionBinaryExport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                // Connection should be kept alive after PostgresException was triggered
                Assert.Throws<PostgresException>(() => conn.BeginBinaryExport("COPY table_is_not_exist (blob) TO STDOUT BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongFormatBinaryExport()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);
                Assert.Throws<ArgumentException>(() => conn.BeginBinaryExport($"COPY {table} (blob) TO STDOUT"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/661")]
        [Ignore("Unreliable")]
        public async Task UnexpectedExceptionBinaryImport()
        {
            if (IsMultiplexing)
                return;

            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);

                var data = new byte[conn.Settings.WriteBufferSize + 10];

                var writer = conn.BeginBinaryImport($"COPY {table} (blob) FROM STDIN BINARY");

                using (var conn2 = await OpenConnectionAsync())
                    await conn2.ExecuteNonQueryAsync($"SELECT pg_terminate_backend({conn.ProcessID})");

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
        public async Task ImportByteaMassive()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field BYTEA", out var table);

                const int iterations = 10000;
                var data = new byte[1024*1024];

                using (var writer = conn.BeginBinaryImport($"COPY {table} (field) FROM STDIN BINARY"))
                {
                    for (var i = 0; i < iterations; i++)
                    {
                        if (i%100 == 0)
                            Console.WriteLine("Iteration " + i);
                        await writer.StartRowAsync();
                        await writer.WriteAsync(data, NpgsqlDbType.Bytea);
                    }
                }

                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(iterations));
            }
        }

        [Test]
        public async Task ExportLongString()
        {
            const int iterations = 100;
            using (var conn = await OpenConnectionAsync())
            {
                var len = conn.Settings.WriteBufferSize;
                await using var _ = await CreateTempTable(conn, "foo1 TEXT, foo2 TEXT, foo3 TEXT, foo4 TEXT, foo5 TEXT", out var table);
                using (var cmd = new NpgsqlCommand($"INSERT INTO {table} VALUES (@p, @p, @p, @p, @p)", conn))
                {
                    cmd.Parameters.AddWithValue("p", new string('x', len));
                    for (var i = 0; i < iterations; i++)
                        await cmd.ExecuteNonQueryAsync();
                }

                using (var reader = conn.BeginBinaryExport($"COPY {table} (foo1, foo2, foo3, foo4, foo5) TO STDIN BINARY"))
                {
                    for (var row = 0; row < iterations; row++)
                    {
                        Assert.That(await reader.StartRowAsync(), Is.EqualTo(5));
                        for (var col = 0; col < 5; col++)
                            Assert.That(reader.Read<string>().Length, Is.EqualTo(len));
                    }
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1134")]
        public async Task ReadBitString()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await GetTempTableName(conn, out var table);

                await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (bits BIT(3), bitarray BIT(3)[]);
INSERT INTO {table} (bits, bitarray) VALUES (B'101', ARRAY[B'101', B'111'])");

                using (var reader = conn.BeginBinaryExport($"COPY {table} (bits, bitarray) TO STDIN BINARY"))
                {
                    await reader.StartRowAsync();
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
        public async Task Array()
        {
            var expected = new[] { 8 };

            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "arr INTEGER[]", out var table);

                using (var writer = conn.BeginBinaryImport($"COPY {table} (arr) FROM STDIN BINARY"))
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(expected);
                    var rowsWritten = await writer.CompleteAsync();
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }

                using (var reader = conn.BeginBinaryExport($"COPY {table} (arr) TO STDIN BINARY"))
                {
                    await reader.StartRowAsync();
                    Assert.That(reader.Read<int[]>(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public async Task Enum()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: connection-specific mapping");

            using var conn = await OpenConnectionAsync();
            await conn.ExecuteNonQueryAsync("CREATE TYPE pg_temp.mood AS ENUM ('sad', 'ok', 'happy')");
            conn.ReloadTypes();
            conn.TypeMapper.MapEnum<Mood>();

            await conn.ExecuteNonQueryAsync("CREATE TEMP TABLE data (mymood mood, mymoodarr mood[])");

            using (var writer = conn.BeginBinaryImport("COPY data (mymood, mymoodarr) FROM STDIN BINARY"))
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(Mood.Happy);
                await writer.WriteAsync(new[] { Mood.Happy });
                var rowsWritten = await writer.CompleteAsync();
                Assert.That(rowsWritten, Is.EqualTo(1));
            }

            using (var reader = conn.BeginBinaryExport("COPY data (mymood, mymoodarr) TO STDIN BINARY"))
            {
                await reader.StartRowAsync();
                Assert.That(reader.Read<Mood>(), Is.EqualTo(Mood.Happy));
                Assert.That(reader.Read<Mood[]>(), Is.EqualTo(new[] { Mood.Happy }));
            }
        }

        enum Mood { Sad, Ok, Happy };

        [Test]
        public async Task Read_NullAsNullable_Succeeds()
        {
            using var connection = await OpenConnectionAsync();
            using var exporter = connection.BeginBinaryExport("COPY (SELECT NULL::int) TO STDOUT BINARY");

            await exporter.StartRowAsync();

            Assert.That(exporter.Read<int?>(), Is.Null);
        }

        [Test]
        public async Task Read_NullAsValue_ThrowsInvalidCastException()
        {
            using var connection = await OpenConnectionAsync();
            using var exporter = connection.BeginBinaryExport("COPY (SELECT NULL::int) TO STDOUT BINARY");

            await exporter.StartRowAsync();

            Assert.Throws<InvalidCastException>(() => exporter.Read<int>());
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1440")]
        public async Task ErrorDuringImport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INT, CONSTRAINT uq UNIQUE(foo)", out var table);

                var writer = conn.BeginBinaryImport($"COPY {table} (foo) FROM STDIN BINARY");
                await writer.StartRowAsync();
                await writer.WriteAsync(8);
                await writer.StartRowAsync();
                await writer.WriteAsync(8);
                Assert.That(() => writer.Complete(), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("23505"));
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task ImportCannotWriteAfterCommit()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INT", out var table);
                try
                {
                    using (var writer = conn.BeginBinaryImport($"COPY {table} (foo) FROM STDIN BINARY"))
                    {
                        await writer.StartRowAsync();
                        await writer.WriteAsync(8);
                        var rowsWritten = await writer.CompleteAsync();
                        Assert.That(rowsWritten, Is.EqualTo(1));
                        await writer.StartRowAsync();
                        Assert.Fail("StartRow should have thrown");
                    }
                }
                catch (InvalidOperationException)
                {
                    Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
                }
            }
        }

        [Test]
        public async Task ImportCommitInMiddleOfRow()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INT, bar TEXT", out var table);

                try
                {
                    using (var writer = conn.BeginBinaryImport($"COPY {table} (foo, bar) FROM STDIN BINARY"))
                    {
                        await writer.StartRowAsync();
                        await writer.WriteAsync(8);
                        await writer.WriteAsync("hello");
                        await writer.StartRowAsync();
                        await writer.WriteAsync(9);
                        await writer.CompleteAsync();
                        Assert.Fail("Commit should have thrown");
                    }
                }
                catch (InvalidOperationException)
                {
                    Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
                }
            }
        }

        [Test]
        public async Task ImportExceptionDoesNotCommit()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INT", out var table);

                try
                {
                    using (var writer = conn.BeginBinaryImport($"COPY {table} (foo) FROM STDIN BINARY"))
                    {
                        await writer.StartRowAsync();
                        await writer.WriteAsync(8);
                        throw new Exception("FOO");
                    }
                }
                catch (Exception e) when (e.Message == "FOO")
                {
                    Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.Zero);
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2347")]
        public async Task Write_ColumnOutOfBounds_ThrowsInvalidOperationException()
        {
            using var conn = await OpenConnectionAsync();
            await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 INTEGER", out var table);

            using var writer = conn.BeginBinaryImport($"COPY {table} (field_text, field_int2) FROM STDIN BINARY");
            StateAssertions(conn);

            await writer.StartRowAsync();
            await writer.WriteAsync("Hello");
            await writer.WriteAsync(8, NpgsqlDbType.Smallint);

            Assert.Throws<InvalidOperationException>(() => writer.Write("I should not be here"));

            await writer.StartRowAsync();
            await writer.WriteAsync("Hello");
            await writer.WriteAsync(8, NpgsqlDbType.Smallint);

            Assert.Throws<InvalidOperationException>(() => writer.Write("I should not be here", NpgsqlDbType.Text));

            await writer.StartRowAsync();
            await writer.WriteAsync("Hello");
            await writer.WriteAsync(8, NpgsqlDbType.Smallint);

            Assert.Throws<InvalidOperationException>(() => writer.Write("I should not be here", "text"));
            Assert.Throws<InvalidOperationException>(() => writer.WriteRow("Hello", 8, "I should not be here"));
        }

        #endregion

        #region Text

        [Test]
        public async Task TextImport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);
                const string line = "HELLO\t1\n";

                // Short write
                var writer = conn.BeginTextImport($"COPY {table} (field_text, field_int4) FROM STDIN");
                StateAssertions(conn);
                await writer.WriteAsync(line);
                await writer.DisposeAsync();
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table} WHERE field_int4=1"), Is.EqualTo(1));
                Assert.That(() => writer.Write(line), Throws.Exception.TypeOf<ObjectDisposedException>());
                await conn.ExecuteNonQueryAsync($"TRUNCATE {table}");

                // Long (multi-buffer) write
                var iterations = NpgsqlWriteBuffer.MinimumSize/line.Length + 100;
                writer = conn.BeginTextImport($"COPY {table} (field_text, field_int4) FROM STDIN");
                for (var i = 0; i < iterations; i++)
                    await writer.WriteAsync(line);
                await writer.DisposeAsync();
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table} WHERE field_int4=1"), Is.EqualTo(iterations));
            }
        }

        [Test]
        public async Task CancelTextImport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);

                var writer = (NpgsqlCopyTextWriter)conn.BeginTextImport($"COPY {table} (field_text, field_int4) FROM STDIN");
                await writer.WriteAsync("HELLO\t1\n");
                await writer.CancelAsync();
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
            }
        }

        [Test]
        public async Task TextImportEmpty()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);

                using (conn.BeginTextImport($"COPY {table} (field_text, field_int4) FROM STDIN"))
                {
                }
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
            }
        }

        [Test]
        public async Task TextExport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await GetTempTableName(conn, out var table);

                await conn.ExecuteNonQueryAsync($@"
CREATE  TABLE {table} (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER);
INSERT INTO {table} (field_text, field_int4) VALUES ('HELLO', 1)");

                var chars = new char[30];

                // Short read
                var reader = conn.BeginTextExport($"COPY {table} (field_text, field_int4) TO STDIN");
                StateAssertions(conn);
                Assert.That(await reader.ReadAsync(chars, 0, chars.Length), Is.EqualTo(8));
                Assert.That(new string(chars, 0, 8), Is.EqualTo("HELLO\t1\n"));
                Assert.That(await reader.ReadAsync(chars, 0, chars.Length), Is.EqualTo(0));
                Assert.That(await reader.ReadAsync(chars, 0, chars.Length), Is.EqualTo(0));
                reader.Dispose();
                Assert.That(() => reader.Read(chars, 0, chars.Length), Throws.Exception.TypeOf<ObjectDisposedException>());
                await conn.ExecuteNonQueryAsync($"TRUNCATE {table}");
            }
        }

        [Test]
        public async Task DisposeInMiddleOfTextExport()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await GetTempTableName(conn, out var table);

                await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER);
INSERT INTO {table} (field_text, field_int4) VALUES ('HELLO', 1)");
                var reader = conn.BeginTextExport($"COPY {table} (field_text, field_int4) TO STDIN");
                reader.Dispose();
                // Make sure the connection is still OK
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongTableDefinitionTextImport()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");
            using (var conn = await OpenConnectionAsync())
            {
                Assert.Throws<PostgresException>(() => conn.BeginTextImport("COPY table_is_not_exist (blob) FROM STDIN"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongFormatTextImport()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);
                Assert.Throws<Exception>(() => conn.BeginTextImport($"COPY {table} (blob) FROM STDIN BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongTableDefinitionTextExport()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");
            using (var conn = await OpenConnectionAsync())
            {
                Assert.Throws<PostgresException>(() => conn.BeginTextExport("COPY table_is_not_exist (blob) TO STDOUT"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2330")]
        public async Task WrongFormatTextExport()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "blob BYTEA", out var table);
                Assert.Throws<Exception>(() => conn.BeginTextExport($"COPY {table} (blob) TO STDOUT BINARY"));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        #endregion

        #region Other

        [Test, Description("Starts a transaction before a COPY, testing that prepended messages are handled well")]
        public async Task PrependedMessages()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await conn.BeginTransactionAsync();
                await TextImport();
            }
        }

        [Test]
        public async Task UndefinedTable()
        {
            using (var conn = await OpenConnectionAsync())
                Assert.That(() => conn.BeginBinaryImport("COPY undefined_table (field_text, field_int2) FROM STDIN BINARY"),
                    Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("42P01")
                );
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/621")]
        public async Task CloseDuringCopy()
        {
            // TODO: Check no broken connections were returned to the pool
            using (var conn = await OpenConnectionAsync()) {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);
                conn.BeginBinaryImport($"COPY {table} (field_text, field_int4) FROM STDIN BINARY");
            }

            using (var conn = await OpenConnectionAsync()) {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);
                conn.BeginBinaryExport($"COPY {table} (field_text, field_int2) TO STDIN BINARY");
            }

            using (var conn = await OpenConnectionAsync()) {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);
                conn.BeginRawBinaryCopy($"COPY {table} (field_text, field_int4) FROM STDIN BINARY");
            }

            using (var conn = await OpenConnectionAsync()) {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);
                conn.BeginRawBinaryCopy($"COPY {table} (field_text, field_int4) TO STDIN BINARY");
            }

            using (var conn = await OpenConnectionAsync()) {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);
                conn.BeginTextImport($"COPY {table} (field_text, field_int4) FROM STDIN");
            }

            using (var conn = await OpenConnectionAsync()) {
                await using var _ = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER", out var table);
                conn.BeginTextExport($"COPY {table} (field_text, field_int4) TO STDIN");
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/994")]
        public async Task NonAsciiColumnName()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "non_ascii_éè TEXT", out var table);
                using (conn.BeginBinaryImport($"COPY {table} (non_ascii_éè) FROM STDIN BINARY")) { }
            }
        }

        [Test, IssueLink("http://stackoverflow.com/questions/37431054/08p01-insufficient-data-left-in-message-for-nullable-datetime/37431464")]
        public async Task WriteNullValues()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo1 INT, foo2 UUID, foo3 INT, foo4 UUID", out var table);

                using (var writer = conn.BeginBinaryImport($"COPY {table} (foo1, foo2, foo3, foo4) FROM STDIN BINARY"))
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(DBNull.Value, NpgsqlDbType.Integer);
                    await writer.WriteAsync((string?)null, NpgsqlDbType.Uuid);
                    await writer.WriteAsync(DBNull.Value);
                    await writer.WriteAsync((string?)null);
                    var rowsWritten = await writer.CompleteAsync();
                    Assert.That(rowsWritten, Is.EqualTo(1));
                }
                using (var cmd = new NpgsqlCommand($"SELECT foo1,foo2,foo3,foo4 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    Assert.That(reader.Read(), Is.True);
                    for (var i = 0; i < reader.FieldCount; i++)
                        Assert.That(reader.IsDBNull(i), Is.True);
                }
            }
        }

        [Test]
        public async Task WriteDifferentTypes()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INT, bar INT[]", out var table);

                using (var writer = conn.BeginBinaryImport($"COPY {table} (foo, bar) FROM STDIN BINARY"))
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(3.0, NpgsqlDbType.Integer);
                    await writer.WriteAsync((object)new[] { 1, 2, 3 });
                    await writer.StartRowAsync();
                    await writer.WriteAsync(3, NpgsqlDbType.Integer);
                    await writer.WriteAsync((object)new List<int> { 4, 5, 6 });
                    var rowsWritten = await writer.CompleteAsync();
                    Assert.That(rowsWritten, Is.EqualTo(2));
                }
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(2));
            }
        }

        [Test, Description("Tests nested binding scopes in multiplexing")]
        public async Task WithinTransaction()
        {
            using var conn = await OpenConnectionAsync();
            await using var _ = await CreateTempTable(conn, "foo INT", out var table);

            using (var tx = await conn.BeginTransactionAsync())
            using (var writer = conn.BeginBinaryImport($"COPY {table} (foo) FROM STDIN BINARY"))
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(1);
                await writer.DisposeAsync();
                // Don't complete
                await tx.CommitAsync();
            }

            using (var tx = await conn.BeginTransactionAsync())
            using (var writer = conn.BeginBinaryImport($"COPY {table} (foo) FROM STDIN BINARY"))
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(2);
                await writer.CompleteAsync();
                // Don't commit
            }

            using (var tx = await conn.BeginTransactionAsync())
            {
                using (var writer = conn.BeginBinaryImport($"COPY {table} (foo) FROM STDIN BINARY"))
                {
                    await writer.StartRowAsync();
                    await writer.WriteAsync(3);
                    await writer.CompleteAsync();
                }
                await tx.CommitAsync();
            }

            Assert.That(async () => await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
            Assert.That(async () => await conn.ExecuteScalarAsync($"SELECT foo FROM {table}"), Is.EqualTo(3));
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
            Assert.That(async () => await conn.ExecuteScalarAsync("SELECT 1"), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());
        }

        #endregion

        public CopyTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
