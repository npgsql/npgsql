using System;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types
{
    public class DomainTests : MultiplexingTestBase
    {
        [Test, Description("Resolves a domain type handler via the different pathways")]
        public async Task Domain_resolution()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, ReloadTypes");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(Domain_resolution),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using var conn = await OpenConnectionAsync(csb);
            await using var _ = await GetTempTypeName(conn, out var type);
            await conn.ExecuteNonQueryAsync($"CREATE DOMAIN {type} AS text");

            // Resolve type by DataTypeName
            conn.ReloadTypes();
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName="p", DataTypeName = type, Value = DBNull.Value });
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo("text"));
                }
            }

            // When sending back domain types, PG sends back the type OID of their base type. So we never need to resolve domains from
            // a type OID.
            conn.ReloadTypes();
            using (var cmd = new NpgsqlCommand($"SELECT 'foo'::{type}", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                Assert.That(reader.GetDataTypeName(0), Is.EqualTo("text"));
                Assert.That(reader.GetString(0), Is.EqualTo("foo"));
            }
        }

        [Test]
        public async Task Domain()
        {
            using var conn = await OpenConnectionAsync();
            await using var _ = await GetTempTypeName(conn, out var type);
            await conn.ExecuteNonQueryAsync($"CREATE DOMAIN {type} AS text");
            Assert.That(await conn.ExecuteScalarAsync($"SELECT 'foo'::{type}"), Is.EqualTo("foo"));
        }

        [Test]
        public async Task Domain_in_composite()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, ReloadTypes");

            using var conn = await OpenConnectionAsync();
            await using var t1 = await GetTempTypeName(conn, out var domainType);
            await using var t2 = await GetTempTypeName(conn, out var compositeType);
            await conn.ExecuteNonQueryAsync($@"
CREATE DOMAIN {domainType} AS text;
CREATE TYPE {compositeType} AS (value {domainType});");

            conn.ReloadTypes();
            conn.TypeMapper.MapComposite<SomeComposite>(compositeType);

            var result = (SomeComposite)(await conn.ExecuteScalarAsync($"SELECT ROW('foo')::{compositeType}"))!;
            Assert.That(result.Value, Is.EqualTo("foo"));
        }

        class SomeComposite
        {
            public string? Value { get; set; }
        }

        public DomainTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
