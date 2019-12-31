using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Npgsql.PluginTests
{
    class RawPostgisTests : TestBase
    {
        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1121")]
        public void Roundtrip()
        {
            using (var conn = OpenConnection())
            {
                byte[] bytes;
                using (var cmd = new NpgsqlCommand("SELECT st_makepoint(1,2500)", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    bytes = reader.GetFieldValue<byte[]>(0);

                    var length = (int)reader.GetBytes(0, 0, null, 0, 0);
                    var buffer = new byte[length];
                    reader.GetBytes(0, 0, buffer, 0, length);

                    Assert.That(buffer, Is.EqualTo(bytes));
                }

                using (var cmd = new NpgsqlCommand("SELECT @p::TEXT", conn))
                {
                    cmd.Parameters.AddWithValue("p", NpgsqlDbType.Geometry, bytes);
                    var asString = cmd.ExecuteScalar();
                    Assert.That(asString, Is.EqualTo("0101000000000000000000F03F000000000088A340"));
                }
            }
        }

        protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        {
            var conn = base.OpenConnection(connectionString);
            TestUtil.EnsureExtension(conn, "postgis");
            conn.TypeMapper.UseRawPostgis();
            return conn;
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            using var conn = await base.OpenConnectionAsync();
            await TestUtil.EnsureExtensionAsync(conn, "postgis");
        }
    }
}
