#if NET45
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    public class AsyncTests : TestBase
    {
        public AsyncTests(string backendVersion) : base(backendVersion) {}

        [Test]
        public async void NonQuery()
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_int4) VALUES (4)", Conn))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            Assert.That(ExecuteScalar("SELECT field_int4 FROM data"), Is.EqualTo(4));
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

        [Test, Description("Cancels an async query with the cancellation token")]
        [ExpectedException(typeof(OperationCanceledException))]
        [Timeout(5000)]
        public async void Cancel()
        {
            var cancellationSource = new CancellationTokenSource();
            using (var cmd = new NpgsqlCommand("SELECT pg_sleep(5)", Conn))
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(300);
                    cancellationSource.Cancel();
                });
                await cmd.ExecuteNonQueryAsync(cancellationSource.Token);
            }
        }
    }
}
#endif