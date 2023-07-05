using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.Properties;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests.Types;

/// <summary>
/// Tests on PostgreSQL types which don't fit elsewhere
/// </summary>
class MiscTypeTests : MultiplexingTestBase
{
    [Test]
    public async Task Boolean()
    {
        await AssertType(true, "true", "boolean", NpgsqlDbType.Boolean, DbType.Boolean);
        await AssertType(false, "false", "boolean", NpgsqlDbType.Boolean, DbType.Boolean);
    }

    [Test]
    public Task Uuid()
        => AssertType(
            new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11"),
            "a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11",
            "uuid", NpgsqlDbType.Uuid, DbType.Guid);

    [Test, Description("Makes sure that the PostgreSQL 'unknown' type (OID 705) is read properly")]
    public async Task Read_unknown()
    {
        const string expected = "some_text";
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT '{expected}'", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetString(0), Is.EqualTo(expected));
        Assert.That(reader.GetValue(0), Is.EqualTo(expected));
        Assert.That(reader.GetFieldValue<char[]>(0), Is.EqualTo(expected.ToCharArray()));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
    }

    [Test]
    public async Task Null()
    {
        await using var conn = await OpenConnectionAsync();
        await using (var cmd = new NpgsqlCommand("SELECT @p1::TEXT, @p2::TEXT, @p3::TEXT", conn))
        {
            cmd.Parameters.AddWithValue("p1", DBNull.Value);
            cmd.Parameters.Add(new NpgsqlParameter<string?>("p2", null));
            cmd.Parameters.Add(new NpgsqlParameter<object>("p3", DBNull.Value));

            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.IsDBNull(i));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(string)));
            }
        }

        // Setting non-generic NpgsqlParameter.Value to null is not allowed, only DBNull.Value
        await using (var cmd = new NpgsqlCommand("SELECT @p::TEXT", conn))
        {
            cmd.Parameters.AddWithValue("p4", NpgsqlDbType.Text, null!);
            Assert.That(async () => await cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        // Setting generic NpgsqlParameter<object>.Value to null is not allowed, only DBNull.Value
        await using (var cmd = new NpgsqlCommand("SELECT @p::TEXT", conn))
        {
            cmd.Parameters.Add(new NpgsqlParameter<object>("p4", NpgsqlDbType.Text) { Value = null! });
            Assert.That(async () => await cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<InvalidOperationException>());
        }
    }

    [Test, Description("Makes sure that setting DbType.Object makes Npgsql infer the type")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/694")]
    public async Task DbType_causes_inference()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter { ParameterName="p", DbType = DbType.Object, Value = 3 });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(3));
    }

    #region Unrecognized types

    [Test, Description("Retrieves a type as an unknown type, i.e. untreated string")]
    public async Task AllResultTypesAreUnknown()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT TRUE", conn);
        cmd.AllResultTypesAreUnknown = true;
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
        Assert.That(reader.GetString(0), Is.EqualTo("t"));
    }

    [Test, Description("Mixes and matches an unknown type with a known type")]
    public async Task UnknownResultTypeList()
    {
        if (IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT TRUE, 8", conn);
        cmd.UnknownResultTypeList = new[] { true, false };
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
        Assert.That(reader.GetString(0), Is.EqualTo("t"));
        Assert.That(reader.GetInt32(1), Is.EqualTo(8));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/711")]
    public async Task Known_type_as_unknown()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 8", conn);
        cmd.AllResultTypesAreUnknown = true;
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("8"));
    }

    [Test, Description("Sends a null value parameter with no NpgsqlDbType or DbType, but with context for the backend to handle it")]
    public async Task Unrecognized_null()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p::TEXT", conn);
        var p = new NpgsqlParameter("p", DBNull.Value);
        cmd.Parameters.Add(p);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.IsDBNull(0));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
    }

    [Test, Description("Sends a value parameter with an explicit NpgsqlDbType.Unknown, but with context for the backend to handle it")]
    public async Task Send_unknown()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p::INT4", conn);
        var p = new NpgsqlParameter("p", "8");
        cmd.Parameters.Add(p);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(int)));
        Assert.That(reader.GetInt32(0), Is.EqualTo(8));
    }

    #endregion

    [Test]
    public Task Int2Vector()
        => AssertType(new short[] { 4, 5, 6 }, "4 5 6", "int2vector", NpgsqlDbType.Int2Vector, isDefault: false);

    [Test]
    public Task Oidvector()
        => AssertType(new uint[] { 4, 5, 6 }, "4 5 6", "oidvector", NpgsqlDbType.Oidvector, isDefault: false);

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1138")]
    public async Task Void()
    {
        await using var conn = await OpenConnectionAsync();
        Assert.That(await conn.ExecuteScalarAsync("SELECT pg_sleep(0)"), Is.SameAs(null));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1364")]
    public async Task Unsupported_DbType()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        Assert.That(() => cmd.Parameters.Add(new NpgsqlParameter("p", DbType.UInt32) { Value = 8u }),
            Throws.Exception.TypeOf<NotSupportedException>());
    }

    public MiscTypeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
