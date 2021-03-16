using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.Tests
{
    public class RecordDbDataReaderTests : MultiplexingTestBase
    {
        [Test]
        public async Task Test1()
        {
            using var conn = await OpenConnectionAsync();
            using var command = new NpgsqlCommand(@"SELECT ARRAY[ROW(1, 2, 3), ROW(4, 5, 6)]
                                                    UNION ALL
                                                    SELECT ARRAY[ROW(7, 8, 9), ROW(10, 11, 12)]", conn);
            using var reader = await command.ExecuteReaderAsync();
            for (var i = 0; i < 2; i++)
            {
                await reader.ReadAsync();
                using var innerReader = reader.GetData(0);
                Assert.That(innerReader.HasRows, Is.True);

                for (var j = 0; j < 2; j++)
                {
                    Assert.That(innerReader.Read(), Is.True);
                    Assert.That(innerReader.FieldCount, Is.EqualTo(3));
                    Assert.That(innerReader.GetFieldType(0), Is.EqualTo(typeof(int)));
                    Assert.That(innerReader.GetDataTypeName(0), Is.EqualTo("integer"));
                    for (var k = 0; k < 3; k++)
                    {
                        Assert.That(innerReader.GetInt32(k), Is.EqualTo(1 + 6 * i + j * 3 + k));
                        Assert.That(innerReader.GetValue(k), Is.EqualTo(1 + 6 * i + j * 3 + k));
                    }
                }
                if (i == 0)
                    Assert.That(innerReader.Read(), Is.False);

                Assert.That(innerReader.NextResult(), Is.False);
                Assert.That(innerReader.HasRows, Is.False);
            }
        }

        [Test]
        public async Task TestNested()
        {
            using var conn = await OpenConnectionAsync();
            using var command = new NpgsqlCommand(@"SELECT
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
            using var reader = await command.ExecuteReaderAsync();

            for (var i = 0; i < 1; i++)
            {
                await reader.ReadAsync();
                using var innerReader = reader.GetData(0);
                for (var j = 0; j < 2; j++)
                {
                    Assert.That(innerReader.Read(), Is.True);
                    var innerReader2 = innerReader.GetData(0);
                    for (var k = 0; k < 2; k++)
                    {
                        Assert.That(innerReader2.Read(), Is.True);
                        for (var l = 0; l < 2; l++)
                        {
                            if (k == 0 && l == 1)
                            {
                                Assert.That(innerReader2.IsDBNull(l), Is.True);
                                Assert.That(innerReader2.GetValue(l), Is.EqualTo(DBNull.Value));
                                Assert.That(innerReader2.GetProviderSpecificValue(l), Is.EqualTo(DBNull.Value));
                            }
                            else
                            {
                                Assert.That(innerReader2.GetString(l), Is.EqualTo("row" + j + k + l));
                            }
                        }
                    }
                }
                Assert.That(reader.GetInt32(1), Is.EqualTo(2));
            }
        }

        [Test]
        public async Task TestSingleRow()
        {
            using var conn = await OpenConnectionAsync();
            using var command = new NpgsqlCommand("SELECT ROW(1, ARRAY[ROW(2), ROW(3)])", conn);
            using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            using var innerReader = reader.GetData(0);
            Assert.That(innerReader.Read(), Is.True);
            Assert.That(innerReader.FieldCount, Is.EqualTo(2));
            Assert.That(innerReader.GetInt32(0), Is.EqualTo(1));
            using var innerReader2 = innerReader.GetData(1);
            for (var i = 0; i < 2; i++)
            {
                Assert.That(innerReader2.Read(), Is.True);
                Assert.That(innerReader2.FieldCount, Is.EqualTo(1));
                Assert.That(innerReader2.GetInt32(0), Is.EqualTo(2 + i));
            }
            Assert.That(innerReader2.Read(), Is.False);
        }

        public RecordDbDataReaderTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) { }
    }
}
