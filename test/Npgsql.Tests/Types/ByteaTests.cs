using System;
using System.Data;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

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
        [Test]
        [TestCase(new byte[] { 1, 2, 3, 4, 5 }, "\\x0102030405", TestName = "Bytea")]
        [TestCase(new byte[] { }, "\\x", TestName = "Bytea_empty")]
        public Task Bytea(byte[] byteArray, string sqlLiteral)
            => AssertType(byteArray, sqlLiteral, "bytea", NpgsqlDbType.Bytea, DbType.Binary);

        [Test]
        public async Task Bytea_long()
        {
            await using var conn = await OpenConnectionAsync();
            var array = new byte[conn.Settings.WriteBufferSize + 100];
            var sqlLiteral = "\\x" + new string('1', (conn.Settings.WriteBufferSize + 100) * 2);
            for (var i = 0; i < array.Length; i++)
                array[i] = 17;

            await Bytea(array, sqlLiteral);
        }

#if !NETSTANDARD2_0
        [Test]
        public Task Write_as_Memory()
            => AssertTypeWrite(
                new Memory<byte>(new byte[] { 1, 2, 3 }), "\\x010203", "bytea", NpgsqlDbType.Bytea, DbType.Binary, isDefault: false);

        [Test]
        public Task Read_as_Memory_not_supported()
            => AssertTypeUnsupportedRead<Memory<byte>, NotSupportedException>("\\x010203", "bytea");

        [Test]
        public Task Write_as_ReadOnlyMemory()
            => AssertTypeWrite(
                new ReadOnlyMemory<byte>(new byte[] { 1, 2, 3 }), "\\x010203", "bytea", NpgsqlDbType.Bytea, DbType.Binary, isDefault: false);

        [Test]
        public Task Read_as_ReadOnlyMemory_not_supported()
            => AssertTypeUnsupportedRead<ReadOnlyMemory<byte>, NotSupportedException>("\\x010203", "bytea");

#endif

        [Test]
        public Task Write_as_ArraySegment()
            => AssertTypeWrite(
                new ArraySegment<byte>(new byte[] { 1, 2, 3 }), "\\x010203", "bytea", NpgsqlDbType.Bytea, DbType.Binary, isDefault: false);

        [Test]
        public Task Read_as_ArraySegment_not_supported()
            => AssertTypeUnsupportedRead<ArraySegment<byte>, NotSupportedException>("\\x010203", "bytea");

        [Test, Description("Tests that bytea values are truncated when the NpgsqlParameter's Size is set")]
        public async Task Truncate()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT @p", conn);
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

        public ByteaTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
