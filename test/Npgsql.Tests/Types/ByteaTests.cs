using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on the PostgreSQL bytea type
    /// </summary>
    /// <summary>
    /// https://www.postgresql.org/docs/current/static/datatype-binary.html
    /// </summary>
    public class ByteaTests : MultiplexingTestBase
    {
        [Test, Description("Roundtrips a bytea")]
        public async Task Roundtrip()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);
            byte[] expected = { 1, 2, 3, 4, 5 };
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Bytea);
            var p2 = new NpgsqlParameter("p2", DbType.Binary);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expected };
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea));
            Assert.That(p3.DbType, Is.EqualTo(DbType.Binary));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = p2.Value = expected;
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(byte[])));
                Assert.That(reader.GetFieldValue<byte[]>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i), Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task Roundtrip_large()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p::BYTEA", conn);
            var expected = new byte[conn.Settings.WriteBufferSize + 100];
            for (var i = 0; i < expected.Length; i++)
                expected[i] = 8;
            cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Bytea) { Value = expected });
            var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(byte[])));
            Assert.That(reader.GetFieldValue<byte[]>(0), Is.EqualTo(expected));
        }

        [Test]
        public async Task Read([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            using var conn = await OpenConnectionAsync();
            await using (await CreateTempTable(conn, "bytes BYTEA", out var table))
            {
                // TODO: This is too small to actually test any interesting sequential behavior
                byte[] expected = {1, 2, 3, 4, 5};
                await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (bytes) VALUES ({EncodeByteaHex(expected)})");

                string queryText = $"SELECT bytes, 'foo', bytes, bytes, bytes FROM {table}";
                using (var cmd = new NpgsqlCommand(queryText, conn))
                using (var reader = await cmd.ExecuteReaderAsync(behavior))
                {
                    reader.Read();

                    var actual = reader.GetFieldValue<byte[]>(0);
                    Assert.That(actual, Is.EqualTo(expected));

                    if (behavior.IsSequential())
                        Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
                    else
                        Assert.That(reader.GetFieldValue<byte[]>(0), Is.EqualTo(expected));

                    Assert.That(reader.GetString(1), Is.EqualTo("foo"));

                    Assert.That(reader[2], Is.EqualTo(expected));
                    Assert.That(reader.GetValue(3), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldValue<byte[]>(4), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public async Task Empty_roundtrip()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT :val::BYTEA", conn);
            var expected = new byte[0];
            cmd.Parameters.Add("val", NpgsqlDbType.Bytea);
            cmd.Parameters["val"].Value = expected;
            var result = (byte[]?)await cmd.ExecuteScalarAsync();
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, Description("Tests that bytea values are truncated when the NpgsqlParameter's Size is set")]
        public async Task Truncate()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p", conn);
            byte[] data = { 1, 2, 3, 4, 5, 6 };
            var p = new NpgsqlParameter("p", data) { Size = 4 };
            cmd.Parameters.Add(p);
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));

            // NpgsqlParameter.Size needs to persist when value is changed
            byte[] data2 = { 11, 12, 13, 14, 15, 16 };
            p.Value = data2;
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(new byte[] { 11, 12, 13, 14 }));

            // NpgsqlParameter.Size larger than the value size should mean the value size, as well as 0 and -1
            p.Size = data2.Length + 10;
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(data2));
            p.Size = 0;
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(data2));
            p.Size = -1;
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(data2));

            Assert.That(() => p.Size = -2, Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public async Task Bytea_over_array_of_bytes()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p", conn);
            cmd.Parameters.AddWithValue("p", new byte[3]);
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("bytea"));
        }

        [Test]
        public async Task Array_of_bytea()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT :p1", conn);
            var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
            var inVal = new[] { bytes, bytes };
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bytea | NpgsqlDbType.Array, inVal);
            var retVal = (byte[][]?)await cmd.ExecuteScalarAsync();
            Assert.AreEqual(inVal.Length, retVal!.Length);
            Assert.AreEqual(inVal[0], retVal[0]);
            Assert.AreEqual(inVal[1], retVal[1]);
        }

#if !NETSTANDARD2_0
        [Test]
        public async Task Memory()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            var bytes = new byte[] { 1, 2, 3 };
            cmd.Parameters.AddWithValue("p1", new ReadOnlyMemory<byte>(bytes));
            cmd.Parameters.AddWithValue("p2", new Memory<byte>(bytes));
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader[0], Is.EqualTo(bytes));
            Assert.That(reader[1], Is.EqualTo(bytes));
            Assert.That(() => reader.GetFieldValue<ReadOnlyMemory<byte>>(0), Throws.Exception.TypeOf<NotSupportedException>());
            Assert.That(() => reader.GetFieldValue<Memory<byte>>(0), Throws.Exception.TypeOf<NotSupportedException>());
        }
#endif

        // Older tests from here

        [Test]
        public async Task Insert1()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @bytes", conn);
            byte[] toStore = { 0, 1, 255, 254 };
            cmd.Parameters.AddWithValue("@bytes", toStore);
            var result = (byte[]?)await cmd.ExecuteScalarAsync();
            Assert.AreEqual(toStore, result!);
        }

        [Test]
        public async Task ArraySegment()
        {
            using var conn = await OpenConnectionAsync();
            using (var cmd = new NpgsqlCommand("select :bytearr", conn))
            {
                var arr = new byte[20000];
                for (var i = 0; i < arr.Length; i++)
                {
                    arr[i] = (byte) (i & 0xff);
                }

                // Big value, should go through "direct buffer"
                var segment = new ArraySegment<byte>(arr, 17, 18000);
                cmd.Parameters.Add(new NpgsqlParameter("bytearr", DbType.Binary) {Value = segment});
                var returned = (byte[]?)await cmd.ExecuteScalarAsync();
                Assert.That(segment.SequenceEqual(returned!));

                cmd.Parameters[0].Size = 17000;
                returned = (byte[]?)await cmd.ExecuteScalarAsync();
                Assert.That(returned!.SequenceEqual(new ArraySegment<byte>(segment.Array!, segment.Offset, 17000)));

                // Small value, should be written normally through the NpgsqlBuffer
                segment = new ArraySegment<byte>(arr, 6, 10);
                cmd.Parameters[0].Value = segment;
                returned = (byte[]?)await cmd.ExecuteScalarAsync();
                Assert.That(segment.SequenceEqual(returned!));

                cmd.Parameters[0].Size = 2;
                returned = (byte[]?)await cmd.ExecuteScalarAsync();
                Assert.That(returned!.SequenceEqual(new ArraySegment<byte>(segment.Array!, segment.Offset, 2)));
            }

            using (var cmd = new NpgsqlCommand("select :bytearr", conn))
            {
                var segment = new ArraySegment<byte>(new byte[] {1, 2, 3}, 1, 2);
                cmd.Parameters.AddWithValue("bytearr", segment);
                Assert.That(segment.SequenceEqual((byte[])(await cmd.ExecuteScalarAsync())!));
            }
        }

        [Test, Description("Writes a bytea that doesn't fit in a partially-full buffer, but does fit in an empty buffer")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/654")]
        public async Task Write_does_not_fit_initially_but_fits_later()
        {
            using var conn = await OpenConnectionAsync();
            await using (await CreateTempTable(conn, "field BYTEA", out var table))
            {
                var bytea = new byte[8180];
                for (var i = 0; i < bytea.Length; i++)
                {
                    bytea[i] = (byte) (i%256);
                }

                using (var cmd = new NpgsqlCommand($"INSERT INTO {table} (field) VALUES (@p)", conn))
                {
                    cmd.Parameters.AddWithValue("@p", bytea);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public ByteaTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
