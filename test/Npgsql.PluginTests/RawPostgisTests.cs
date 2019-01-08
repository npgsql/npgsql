using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

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

        protected override NpgsqlConnection OpenConnection(string connectionString = null)
        {
            var conn = base.OpenConnection(connectionString);
            conn.TypeMapper.UseRawPostgis();
            return conn;
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT postgis_version()", conn))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (PostgresException)
                {
                    cmd.CommandText = "SELECT version()";
                    var versionString = (string)cmd.ExecuteScalar();
                    Debug.Assert(versionString != null);
                    var m = Regex.Match(versionString, @"^PostgreSQL ([0-9.]+(\w*)?)");
                    if (!m.Success)
                        throw new Exception("Couldn't parse PostgreSQL version string: " + versionString);
                    var version = m.Groups[1].Value;
                    var prerelease = m.Groups[2].Value;
                    if (!string.IsNullOrWhiteSpace(prerelease))
                        Assert.Ignore($"PostGIS not installed, ignoring because we're on a prerelease version of PostgreSQL ({version})");
                    TestUtil.IgnoreExceptOnBuildServer("PostGIS extension not installed.");
                }
            }
        }
    }
}
