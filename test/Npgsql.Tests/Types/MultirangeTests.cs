using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

public class MultirangeTests : TestBase
{
    [Test]
    public async Task Read()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT '{[3,7), (8,]}'::int4multirange", conn);
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
        await using var cmd = new NpgsqlCommandOrig("SELECT $1::text", conn);

        await WriteInternal(multirangeArray);
        await WriteInternal(multirangeList);

        async Task WriteInternal(IList<NpgsqlRange<int>> multirange)
        {
            await conn.ReloadTypesAsync();
            cmd.Parameters.Add(new() { Value = multirange });
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[3,7),[9,)}"));

            await conn.ReloadTypesAsync();
            cmd.Parameters[0] = new() { Value = multirange, NpgsqlDbType = NpgsqlDbType.IntegerMultirange };
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[3,7),[9,)}"));

            await conn.ReloadTypesAsync();
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
        await using var cmd = new NpgsqlCommandOrig("SELECT $1::text", conn);

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

    [Test]
    public async Task Read_Datemultirange()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT '{[2020-01-01,2020-01-05), (2020-01-10,]}'::datemultirange", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("datemultirange"));

        var multirangeDateTimeArray = (NpgsqlRange<DateTime>[])reader[0];
        Assert.That(multirangeDateTimeArray.Length, Is.EqualTo(2));
        Assert.That(multirangeDateTimeArray[0], Is.EqualTo(new NpgsqlRange<DateTime>(new(2020, 1, 1), true, false, new(2020, 1, 5), false, false)));
        Assert.That(multirangeDateTimeArray[1], Is.EqualTo(new NpgsqlRange<DateTime>(new(2020, 1, 11), true, false, default, false, true)));

        var multirangeDateTimeList = reader.GetFieldValue<List<NpgsqlRange<DateTime>>>(0);
        Assert.That(multirangeDateTimeList.Count, Is.EqualTo(2));
        Assert.That(multirangeDateTimeList[0], Is.EqualTo(new NpgsqlRange<DateTime>(new(2020, 1, 1), true, false, new(2020, 1, 5), false, false)));
        Assert.That(multirangeDateTimeList[1], Is.EqualTo(new NpgsqlRange<DateTime>(new(2020, 1, 11), true, false, default, false, true)));

#if NET6_0_OR_GREATER
        var multirangeDateOnlyArray = reader.GetFieldValue<NpgsqlRange<DateOnly>[]>(0);
        Assert.That(multirangeDateOnlyArray.Length, Is.EqualTo(2));
        Assert.That(multirangeDateOnlyArray[0], Is.EqualTo(new NpgsqlRange<DateOnly>(new(2020, 1, 1), true, false, new(2020, 1, 5), false, false)));
        Assert.That(multirangeDateOnlyArray[1], Is.EqualTo(new NpgsqlRange<DateOnly>(new(2020, 1, 11), true, false, default, false, true)));

        var multirangeDateOnlyList = reader.GetFieldValue<List<NpgsqlRange<DateOnly>>>(0);
        Assert.That(multirangeDateOnlyList.Count, Is.EqualTo(2));
        Assert.That(multirangeDateOnlyList[0], Is.EqualTo(new NpgsqlRange<DateOnly>(new(2020, 1, 1), true, false, new(2020, 1, 5), false, false)));
        Assert.That(multirangeDateOnlyList[1], Is.EqualTo(new NpgsqlRange<DateOnly>(new(2020, 1, 11), true, false, default, false, true)));
#endif
    }

#if NET6_0_OR_GREATER
    [Test]
    public async Task Write_Datemultirange_DateOnly()
    {
        var multirangeArray = new NpgsqlRange<DateOnly>[]
        {
            new(new(2020, 1, 1), true, false, new(2020, 1, 5), false, false),
            new(new(2020, 1, 10), false, false, default, false, true)
        };

        var multirangeList = new List<NpgsqlRange<DateOnly>>(multirangeArray);

        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT $1::text", conn);

        await WriteInternal(multirangeArray);
        await WriteInternal(multirangeList);

        async Task WriteInternal(IList<NpgsqlRange<DateOnly>> multirange)
        {
            conn.ReloadTypes();
            cmd.Parameters.Add(new() { Value = multirange });
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[2020-01-01,2020-01-05),[2020-01-11,)}"));

            conn.ReloadTypes();
            cmd.Parameters[0] = new() { Value = multirange, NpgsqlDbType = NpgsqlDbType.DateMultirange };
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[2020-01-01,2020-01-05),[2020-01-11,)}"));

            conn.ReloadTypes();
            cmd.Parameters[0] = new() { Value = multirange, DataTypeName = "datemultirange" };
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[2020-01-01,2020-01-05),[2020-01-11,)}"));
        }
    }
#endif

    [Test]
    public async Task Write_Datemultirange_DateTime()
    {
        var multirangeArray = new NpgsqlRange<DateTime>[]
        {
            new(new(2020, 1, 1), true, false, new(2020, 1, 5), false, false),
            new(new(2020, 1, 10), false, false, default, false, true)
        };

        var multirangeList = new List<NpgsqlRange<DateTime>>(multirangeArray);

        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT $1::text", conn);

        await WriteInternal(multirangeArray);
        await WriteInternal(multirangeList);

        async Task WriteInternal(IList<NpgsqlRange<DateTime>> multirange)
        {
            conn.ReloadTypes();
            cmd.Parameters.Add(new() { Value = multirange, NpgsqlDbType = NpgsqlDbType.DateMultirange });
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[2020-01-01,2020-01-05),[2020-01-11,)}"));

            conn.ReloadTypes();
            cmd.Parameters[0] = new() { Value = multirange, DataTypeName = "datemultirange" };
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("{[2020-01-01,2020-01-05),[2020-01-11,)}"));
        }
    }

    [OneTimeSetUp]
    public async Task Setup()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");
    }
}
