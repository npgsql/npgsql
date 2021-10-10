using System.Collections.Generic;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types
{
    public class MultirangeTests : TestBase
    {
        [Test]
        public async Task Read()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT '{[3,7), (8,]}'::int4multirange", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("int4multirange"));

            var multirangeArray = (NpgsqlRange<int>[])reader[0];
            Assert.That(multirangeArray.Length, Is.EqualTo(2));
            Assert.That(multirangeArray[0], Is.EqualTo(new NpgsqlRange<int>(3, true, false, 7, false, false)));
            Assert.That(multirangeArray[1], Is.EqualTo(new NpgsqlRange<int>(9, true, false, 0, false, true)));

            var multirangeList = reader.GetFieldValue<List<NpgsqlRange<int>>>(0);
            Assert.That(multirangeList.Count, Is.EqualTo(2));
            Assert.That(multirangeList[0], Is.EqualTo(new NpgsqlRange<int>(3, true, false, 7, false, false)));
            Assert.That(multirangeList[1], Is.EqualTo(new NpgsqlRange<int>(9, true, false, 0, false, true)));
        }

        [Test]
        public async Task Write()
        {
            var multirangeArray = new NpgsqlRange<int>[]
            {
                new(3, true, false, 7, false, false),
                new(8, false, false, 0, false, true)
            };

            var multirangeList = new List<NpgsqlRange<int>>(multirangeArray);

            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1::text", conn);

            await WriteInternal(multirangeArray);
            await WriteInternal(multirangeList);

            async Task WriteInternal(IList<NpgsqlRange<int>> multirange)
            {
                conn.ReloadTypes();
                cmd.Parameters.Add(new() { Value = multirange });
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[3,7),[9,)}"));

                conn.ReloadTypes();
                cmd.Parameters[0] = new() { Value = multirange, NpgsqlDbType = NpgsqlDbType.IntegerMultirange };
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[3,7),[9,)}"));

                conn.ReloadTypes();
                cmd.Parameters[0] = new() { Value = multirange, DataTypeName = "int4multirange" };
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[3,7),[9,)}"));
            }
        }

        [Test]
        public async Task Write_nummultirange()
        {
            var multirangeArray = new NpgsqlRange<decimal>[]
            {
                new(3, true, false, 7, false, false),
                new(8, false, false, 0, false, true)
            };

            var multirangeList = new List<NpgsqlRange<decimal>>(multirangeArray);

            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1::text", conn);

            await WriteInternal(multirangeArray);
            await WriteInternal(multirangeList);

            async Task WriteInternal(IList<NpgsqlRange<decimal>> multirange)
            {
                conn.ReloadTypes();
                cmd.Parameters.Add(new() { Value = multirange });
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[3,7),(8,)}"));

                conn.ReloadTypes();
                cmd.Parameters[0] = new() { Value = multirange, NpgsqlDbType = NpgsqlDbType.NumericMultirange };
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[3,7),(8,)}"));

                conn.ReloadTypes();
                cmd.Parameters[0] = new() { Value = multirange, DataTypeName = "nummultirange" };
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[3,7),(8,)}"));
            }
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await using var conn = await OpenConnectionAsync();
            MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");
        }
    }
}
