using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

public class InternalTypeTests : MultiplexingTestBase
{
    [Test]
    public async Task Read_internal_char()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT typdelim FROM pg_type WHERE typname='int4'", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetChar(0), Is.EqualTo(','));
        Assert.That(reader.GetValue(0), Is.EqualTo(','));
        Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(','));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(char)));
    }

    [Test]
    [TestCase(NpgsqlDbType.Oid)]
    [TestCase(NpgsqlDbType.Regtype)]
    [TestCase(NpgsqlDbType.Regconfig)]
    public async Task Internal_uint_types(NpgsqlDbType npgsqlDbType)
    {
        var postgresType = npgsqlDbType.ToString().ToLowerInvariant();
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig($"SELECT @max, 4294967295::{postgresType}, @eight, 8::{postgresType}", conn);
        cmd.Parameters.AddWithValue("max", npgsqlDbType, uint.MaxValue);
        cmd.Parameters.AddWithValue("eight", npgsqlDbType, 8u);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        for (var i = 0; i < reader.FieldCount; i++)
            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(uint)));

        Assert.That(reader.GetValue(0), Is.EqualTo(uint.MaxValue));
        Assert.That(reader.GetValue(1), Is.EqualTo(uint.MaxValue));
        Assert.That(reader.GetValue(2), Is.EqualTo(8u));
        Assert.That(reader.GetValue(3), Is.EqualTo(8u));
    }

    [Test]
    public async Task Tid()
    {
        var expected = new NpgsqlTid(3, 5);
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT '(1234,40000)'::tid, @p::tid";
        cmd.Parameters.AddWithValue("p", NpgsqlDbType.Tid, expected);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.AreEqual(1234, reader.GetFieldValue<NpgsqlTid>(0).BlockNumber);
        Assert.AreEqual(40000, reader.GetFieldValue<NpgsqlTid>(0).OffsetNumber);
        Assert.AreEqual(expected.BlockNumber, reader.GetFieldValue<NpgsqlTid>(1).BlockNumber);
        Assert.AreEqual(expected.OffsetNumber, reader.GetFieldValue<NpgsqlTid>(1).OffsetNumber);
    }

    #region NpgsqlLogSequenceNumber / PgLsn

    static readonly TestCaseData[] EqualsObjectCases = {
        new TestCaseData(new NpgsqlLogSequenceNumber(1ul), null).Returns(false),
        new TestCaseData(new NpgsqlLogSequenceNumber(1ul), new object()).Returns(false),
        new TestCaseData(new NpgsqlLogSequenceNumber(1ul), 1ul).Returns(false), // no implicit cast
        new TestCaseData(new NpgsqlLogSequenceNumber(1ul), "0/0").Returns(false), // no implicit cast/parsing
        new TestCaseData(new NpgsqlLogSequenceNumber(1ul), new NpgsqlLogSequenceNumber(1ul)).Returns(true),
    };

    [Test, TestCaseSource(nameof(EqualsObjectCases))]
    public bool NpgsqlLogSequenceNumber_equals(NpgsqlLogSequenceNumber lsn, object? obj)
        => lsn.Equals(obj);


    [Test]
    public async Task NpgsqlLogSequenceNumber()
    {
        var expected1 = new NpgsqlLogSequenceNumber(42949672971ul);
        Assert.AreEqual(expected1, NpgsqlTypes.NpgsqlLogSequenceNumber.Parse("A/B"));
        await using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 'A/B'::pg_lsn, @p::pg_lsn";
        cmd.Parameters.AddWithValue("p", NpgsqlDbType.PgLsn, expected1);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        var result1 = reader.GetFieldValue<NpgsqlLogSequenceNumber>(0);
        var result2 = reader.GetFieldValue<NpgsqlLogSequenceNumber>(1);
        Assert.AreEqual(expected1, result1);
        Assert.AreEqual(42949672971ul, (ulong)result1);
        Assert.AreEqual("A/B", result1.ToString());
        Assert.AreEqual(expected1, result2);
        Assert.AreEqual(42949672971ul, (ulong)result2);
        Assert.AreEqual("A/B", result2.ToString());
    }

    #endregion NpgsqlLogSequenceNumber / PgLsn

    public InternalTypeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}