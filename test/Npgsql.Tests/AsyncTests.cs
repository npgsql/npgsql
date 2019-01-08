using System.Data;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class AsyncTests : TestBase
    {
        [Test]
        public async Task NonQuery()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER)");
                using (var cmd = new NpgsqlCommand("INSERT INTO data (int) VALUES (4)", conn))
                    await cmd.ExecuteNonQueryAsync();
                Assert.That(conn.ExecuteScalar("SELECT int FROM data"), Is.EqualTo(4));
            }
        }

        [Test]
        public async Task Scalar()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn)) {
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task Reader()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                await reader.ReadAsync();
                Assert.That(reader[0], Is.EqualTo(1));
            }
        }

        [Test]
        public async Task Columnar()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT NULL, 2, 'Some Text'", conn))
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
            {
                await reader.ReadAsync();
                Assert.That(await reader.IsDBNullAsync(0), Is.True);
                Assert.That(await reader.GetFieldValueAsync<string>(2), Is.EqualTo("Some Text"));
            }
        }
    }
}
