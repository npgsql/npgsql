using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Tests
{
    public class MultipleHostsTests : MultiplexingTestBase
    {
        public MultipleHostsTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) { }

        [Test, Timeout(10000)]
        public void Basic()
        {
            if (IsMultiplexing)
                return;

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = "localhost,127.0.0.1",
                Pooling = true,
                MaxPoolSize = 2,
            };

            var queriesDone = 0;

            var clientsTask = Task.WhenAll(
                Client(csb, TargetSessionAttributes.Any),
                Client(csb, TargetSessionAttributes.Primary),
                Client(csb, TargetSessionAttributes.PreferPrimary),
                Client(csb, TargetSessionAttributes.PreferSecondary));

            var onlySecondaryClient = Client(csb, TargetSessionAttributes.Secondary);

            Assert.DoesNotThrowAsync(async () => await clientsTask);
            Assert.ThrowsAsync<NpgsqlException>(async () => await onlySecondaryClient);
            Assert.AreEqual(100, queriesDone);

            Task Client(NpgsqlConnectionStringBuilder csb, TargetSessionAttributes targetServerType)
            {
                csb = csb.Clone();
                csb.TargetSessionAttributes = targetServerType;
                var tasks = new List<Task>(5);

                for (var i = 0; i < 5; i++)
                {
                    tasks.Add(Task.Run(() => Query(csb.ToString())));
                }

                return Task.WhenAll(tasks);
            }

            async Task Query(string connectionString)
            {
                await using var conn = new NpgsqlConnection(connectionString);
                for (var i = 0; i < 5; i++)
                {
                    await conn.OpenAsync();
                    await conn.ExecuteNonQueryAsync("SELECT 1");
                    await conn.CloseAsync();
                    Interlocked.Increment(ref queriesDone);
                }
            }
        }
    }
}
