#if !NET40
// Unsurprisingly, NUnit doesn't properly run async tests under .NET 4.0
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class AsyncTests : TestBase
    {
        public AsyncTests(string backendVersion) : base(backendVersion) {}

        [Test]
        public async void NonQuery()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER)");
            using (var cmd = new NpgsqlCommand("INSERT INTO data (int) VALUES (4)", Conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            Assert.That(ExecuteScalar("SELECT int FROM data"), Is.EqualTo(4));
        }

        [Test]
        public async void Scalar()
        {
            using (var cmd = new NpgsqlCommand("SELECT 1", Conn)) {
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
            }
        }

        [Test]
        public async void Reader()
        {
            using (var cmd = new NpgsqlCommand("SELECT 1", Conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                await reader.ReadAsync();
                Assert.That(reader[0], Is.EqualTo(1));
            }
        }

        [Test]
        public async void Columnar()
        {
            using (var cmd = new NpgsqlCommand("SELECT NULL, 2, 'Some Text'", Conn))
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
            {
                await reader.ReadAsync();
                Assert.That(await reader.IsDBNullAsync(0), Is.True);
                Assert.That(await reader.GetFieldValueAsync<string>(2), Is.EqualTo("Some Text"));
            }
        }

        [Test, Description("Cancels an async query with the cancellation token")]
        [Timeout(5000)]
        [Ignore("Not reliable...")]
        public void Cancel()
        {
            var cancellationSource = new CancellationTokenSource();
            using (var cmd = CreateSleepCommand(Conn, 5))
            {
                Task.Factory.StartNew(() =>
                                        {
                                            Thread.Sleep(300);
                                            cancellationSource.Cancel();
                                        });
                var t = cmd.ExecuteNonQueryAsync(cancellationSource.Token);
                Task.WaitAny(t);
                Assert.That(t.IsCanceled);
            }
        }
    }
}
#endif
