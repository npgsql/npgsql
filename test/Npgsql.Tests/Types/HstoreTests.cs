using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

public class HstoreTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    public Task Hstore()
        => AssertType(
            new Dictionary<string, string?>
            {
                {"a", "3"},
                {"b", null},
                {"cd", "hello"}
            },
            @"""a""=>""3"", ""b""=>NULL, ""cd""=>""hello""",
            "hstore",
            NpgsqlDbType.Hstore, isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Hstore_empty()
        => AssertType(new Dictionary<string, string?>(), @"", "hstore", NpgsqlDbType.Hstore, isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Hstore_as_ImmutableDictionary()
    {
        var builder = ImmutableDictionary<string, string?>.Empty.ToBuilder();
        builder.Add("a", "3");
        builder.Add("b", null);
        builder.Add("cd", "hello");
        var immutableDictionary = builder.ToImmutableDictionary();

        return AssertType(
            immutableDictionary,
            @"""a""=>""3"", ""b""=>NULL, ""cd""=>""hello""",
            "hstore",
            NpgsqlDbType.Hstore,
            isDefaultForReading: false, isNpgsqlDbTypeInferredFromClrType: false);
    }

    [Test]
    public Task Hstore_as_IDictionary()
        => AssertType<IDictionary<string, string?>>(
            new Dictionary<string, string?>
            {
                { "a", "3" },
                { "b", null },
                { "cd", "hello" }
            },
            @"""a""=>""3"", ""b""=>NULL, ""cd""=>""hello""",
            "hstore",
            NpgsqlDbType.Hstore,
            isDefaultForReading: false, isNpgsqlDbTypeInferredFromClrType: false);

    [OneTimeSetUp]
    public async Task SetUp()
    {
        using var conn = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(conn, "9.1", "Hstore introduced in PostgreSQL 9.1");
        await TestUtil.EnsureExtensionAsync(conn, "hstore", "9.1");
    }
}
