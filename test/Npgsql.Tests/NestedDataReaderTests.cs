using NUnit.Framework;
using System;
using System.Threading.Tasks;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class NestedDataReaderTests : TestBase
{
    [Test]
    public async Task Basic()
    {
        await using var conn = await OpenConnectionAsync();
        await using var command = new NpgsqlCommandOrig(@"SELECT ARRAY[ROW(1, 2, 3), ROW(4, 5, 6)]
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
                Assert.That(nestedReader.GetName(0), Is.EqualTo("?column?"));
                Assert.Throws<NotSupportedException>(() => nestedReader.GetOrdinal("c0"));
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
    public async Task Different_field_count()
    {
        await using var conn = await OpenConnectionAsync();
        await using var command = new NpgsqlCommandOrig(@"SELECT ARRAY[ROW(1), ROW(), ROW('2'::TEXT, 3), ROW(4)]", conn);
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
        await using var command = new NpgsqlCommandOrig(@"SELECT
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
    public async Task Single_row()
    {
        await using var conn = await OpenConnectionAsync();
        await using var command = new NpgsqlCommandOrig("SELECT ROW(1, ARRAY[ROW(2), ROW(3)])", conn);
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
    public async Task Empty_array()
    {
        await using var conn = await OpenConnectionAsync();
        await using var command = new NpgsqlCommandOrig("SELECT ARRAY[]::RECORD[]", conn);
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
        var typeName = await GetTempTypeName(conn);
        await conn.ExecuteNonQueryAsync($"CREATE TYPE {typeName} AS (c0 integer, c1 text)");
        conn.ReloadTypes();
        var sqls = new string[]
        {
            $"SELECT ROW('1', '2')::{typeName}",
            $"SELECT ARRAY[ROW('1', '2')::{typeName}]"
        };
        foreach (var sql in sqls)
        {
            await using var command = new NpgsqlCommandOrig(sql, conn);
            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            using var nestedReader = reader.GetData(0);
            nestedReader.Read();
            Assert.That(nestedReader.GetDataTypeName(0), Is.EqualTo("integer"));
            Assert.That(nestedReader.GetDataTypeName(1), Is.EqualTo("text"));
            Assert.That(nestedReader.GetInt32(0), Is.EqualTo(1));
            Assert.That(nestedReader.GetString(1), Is.EqualTo("2"));
            Assert.That(nestedReader.GetName(0), Is.EqualTo("c0"));
            Assert.That(nestedReader.GetName(1), Is.EqualTo("c1"));
            Assert.That(nestedReader.GetOrdinal("C1"), Is.EqualTo(1));
            Assert.That(nestedReader["C1"], Is.EqualTo("2"));
            Assert.Throws<IndexOutOfRangeException>(() => nestedReader.GetOrdinal("ABC"));
        }
    }

    [Test]
    public void GetBytes()
    {
        using var conn = OpenConnection();
        using var command = new NpgsqlCommandOrig(@"SELECT ROW('\x010203'::BYTEA, NULL::BYTEA)", conn);
        using var reader = command.ExecuteReader();
        reader.Read();
        using var nestedReader = reader.GetData(0);
        nestedReader.Read();
        Assert.That(nestedReader.GetFieldType(0), Is.EqualTo(typeof(byte[])));
        var buf = new byte[4];
        Assert.That(nestedReader.GetBytes(0, 0, null, 0, 3), Is.EqualTo(3));
        Assert.That(nestedReader.GetBytes(0, 0, null, 0, 4), Is.EqualTo(3));
        Assert.That(nestedReader.GetBytes(0, 0, buf, 0, 3), Is.EqualTo(3));
        Assert.That(nestedReader.GetBytes(0, 0, buf, 0, 4), Is.EqualTo(3));
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 0 }, buf);
        buf = new byte[2];
        Assert.That(nestedReader.GetBytes(0, 0, buf, 0, 2), Is.EqualTo(2));
        CollectionAssert.AreEqual(new byte[] { 1, 2 }, buf);
        buf = new byte[2];
        Assert.That(nestedReader.GetBytes(0, 1, buf, 1, 1), Is.EqualTo(1));
        CollectionAssert.AreEqual(new byte[] { 0, 2 }, buf);
        Assert.That(nestedReader.GetBytes(0, 2, buf, 1, 1), Is.EqualTo(1));
        CollectionAssert.AreEqual(new byte[] { 0, 3 }, buf);
        Assert.Throws<InvalidCastException>(() => nestedReader.GetBytes(1, 0, buf, 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => nestedReader.GetBytes(0, 4, buf, 0, 1));
    }

    [Test]
    public async Task Throw_after_next_row()
    {
        await using var conn = await OpenConnectionAsync();
        await using var command = new NpgsqlCommandOrig(@"SELECT ROW(1) UNION ALL SELECT ROW(2) UNION ALL SELECT ROW(3)", conn);
        await using var reader = await command.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync(), Is.True);
        var nestedReader = reader.GetData(0);
        nestedReader.Read();
        await reader.ReadAsync();
        Assert.Throws<InvalidOperationException>(() => nestedReader.IsDBNull(0));
        nestedReader = reader.GetData(0);
        reader.Read();
        Assert.Throws<InvalidOperationException>(() => nestedReader.Read());
        nestedReader = reader.GetData(0);
        nestedReader.Read();
        Assert.That(nestedReader.IsDBNull(0), Is.False);
    }
}
