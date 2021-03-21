using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.Tests
{
    public class NestedDataReaderTests : TestBase
    {
        [Test]
        public async Task BasicFunctionality()
        {
            await using var conn = await OpenConnectionAsync();
            await using var command = new NpgsqlCommand(@"SELECT ARRAY[ROW(1, 2, 3), ROW(4, 5, 6)]
                                                    UNION ALL
                                                    SELECT ARRAY[ROW(7, 8, 9), ROW(10, 11, 12)]", conn);
            await using var reader = await command.ExecuteReaderAsync();
            for (var i = 0; i < 2; i++)
            {
                await reader.ReadAsync();
                using var nestedReader = reader.GetData(0);
                Assert.That(nestedReader.HasRows, Is.True);

                for (var j = 0; j < 2; j++)
                {
                    Assert.That(nestedReader.Read(), Is.True);
                    Assert.That(nestedReader.FieldCount, Is.EqualTo(3));
                    Assert.That(nestedReader.GetFieldType(0), Is.EqualTo(typeof(int)));
                    Assert.That(nestedReader.GetDataTypeName(0), Is.EqualTo("integer"));
                    for (var k = 0; k < 3; k++)
                    {
                        Assert.That(nestedReader.GetInt32(k), Is.EqualTo(1 + 6 * i + j * 3 + k));
                        Assert.That(nestedReader.GetValue(k), Is.EqualTo(1 + 6 * i + j * 3 + k));
                    }
                }
                if (i == 0)
                    Assert.That(nestedReader.Read(), Is.False);

                Assert.That(nestedReader.NextResult(), Is.False);
                Assert.That(nestedReader.HasRows, Is.False);
            }
        }

        [Test]
        public async Task DifferentFieldCount()
        {
            await using var conn = await OpenConnectionAsync();
            await using var command = new NpgsqlCommand(@"SELECT ARRAY[ROW(1), ROW(), ROW('2'::TEXT, 3), ROW(4)]", conn);
            await using var reader = await command.ExecuteReaderAsync();
            Assert.That(await reader.ReadAsync(), Is.True);
            using var nestedReader = reader.GetData(0);
            Assert.That(nestedReader.Read(), Is.True);
            Assert.That(nestedReader.FieldCount, Is.EqualTo(1));
            Assert.That(nestedReader.GetFieldType(0), Is.EqualTo(typeof(int)));
            Assert.That(nestedReader.GetInt32(0), Is.EqualTo(1));
            Assert.That(nestedReader.Read(), Is.True);
            Assert.That(nestedReader.FieldCount, Is.EqualTo(0));
            Assert.That(nestedReader.Read(), Is.True);
            Assert.That(nestedReader.FieldCount, Is.EqualTo(2));
            Assert.That(nestedReader.GetFieldType(0), Is.EqualTo(typeof(string)));
            Assert.That(nestedReader.GetFieldType(1), Is.EqualTo(typeof(int)));
            Assert.That(nestedReader.GetString(0), Is.EqualTo("2"));
            Assert.That(nestedReader.GetInt32(1), Is.EqualTo(3));
            Assert.That(nestedReader.Read(), Is.True);
            Assert.That(nestedReader.GetFieldType(0), Is.EqualTo(typeof(int)));
            Assert.That(nestedReader.GetInt32(0), Is.EqualTo(4));
            Assert.That(nestedReader.Read(), Is.False);
        }

        [Test]
        public async Task Nested()
        {
            await using var conn = await OpenConnectionAsync();
            await using var command = new NpgsqlCommand(@"SELECT
                ARRAY[
                    ROW(
                        ARRAY[
                            ROW('row000'::TEXT, NULL::TEXT),
                            ROW('row010'::TEXT, 'row011'::TEXT)
                        ]
                    ),
                    ROW(
                        ARRAY[
                            ROW('row100'::TEXT, NULL::TEXT),
                            ROW('row110'::TEXT, 'row111'::TEXT)
                        ]
                    )
                ], 2", conn);
            await using var reader = await command.ExecuteReaderAsync();

            for (var i = 0; i < 1; i++)
            {
                await reader.ReadAsync();
                using var nestedReader = reader.GetData(0);
                for (var j = 0; j < 2; j++)
                {
                    Assert.That(nestedReader.Read(), Is.True);
                    var nestedReader2 = nestedReader.GetData(0);
                    for (var k = 0; k < 2; k++)
                    {
                        Assert.That(nestedReader2.Read(), Is.True);
                        for (var l = 0; l < 2; l++)
                        {
                            if (k == 0 && l == 1)
                            {
                                Assert.That(nestedReader2.IsDBNull(l), Is.True);
                                Assert.That(nestedReader2.GetValue(l), Is.EqualTo(DBNull.Value));
                                Assert.That(nestedReader2.GetProviderSpecificValue(l), Is.EqualTo(DBNull.Value));
                            }
                            else
                            {
                                Assert.That(nestedReader2.GetString(l), Is.EqualTo("row" + j + k + l));
                            }
                        }
                    }
                }
                Assert.That(reader.GetInt32(1), Is.EqualTo(2));
            }
        }

        [Test]
        public async Task SingleRow()
        {
            await using var conn = await OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT ROW(1, ARRAY[ROW(2), ROW(3)])", conn);
            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            using var nestedReader = reader.GetData(0);
            Assert.That(nestedReader.Read(), Is.True);
            Assert.That(nestedReader.FieldCount, Is.EqualTo(2));
            Assert.That(nestedReader.GetInt32(0), Is.EqualTo(1));
            using var nestedReader2 = nestedReader.GetData(1);
            for (var i = 0; i < 2; i++)
            {
                Assert.That(nestedReader2.Read(), Is.True);
                Assert.That(nestedReader2.FieldCount, Is.EqualTo(1));
                Assert.That(nestedReader2.GetInt32(0), Is.EqualTo(2 + i));
            }
            Assert.That(nestedReader2.Read(), Is.False);
        }

        [Test]
        public async Task EmptyArray()
        {
            await using var conn = await OpenConnectionAsync();
            await using var command = new NpgsqlCommand("SELECT ARRAY[]::RECORD[]", conn);
            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            using var nestedReader = reader.GetData(0);
            Assert.That(nestedReader.Read(), Is.False);
            Assert.That(nestedReader.NextResult(), Is.False);
        }

        [Test]
        public async Task Composite()
        {
            await using var conn = await OpenConnectionAsync();
            await conn.ExecuteNonQueryAsync("DROP TYPE IF EXISTS nested_db_reader_composite");
            await conn.ExecuteNonQueryAsync("CREATE TYPE nested_db_reader_composite AS (c0 integer, c1 text)");
            await Task.Run(() => conn.ReloadTypes());
            var sqls = new string[] { "SELECT ROW('1', '2')::nested_db_reader_composite",
                                      "SELECT ARRAY[ROW('1', '2')::nested_db_reader_composite]"};
            foreach (var sql in sqls)
            {
                using (var command = new NpgsqlCommand(sql, conn))
                {
                    await using var reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    using var nestedReader = reader.GetData(0);
                    nestedReader.Read();
                    Assert.That(nestedReader.GetDataTypeName(0), Is.EqualTo("integer"));
                    Assert.That(nestedReader.GetDataTypeName(1), Is.EqualTo("text"));
                    Assert.That(nestedReader.GetInt32(0), Is.EqualTo(1));
                    Assert.That(nestedReader.GetString(1), Is.EqualTo("2"));
                }
            }
        }
    }
}
