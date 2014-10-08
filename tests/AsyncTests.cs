#if NET45
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

        [Test]
        public async void Columnar()
        {
            ExecuteNonQuery("INSERT INTO DATA (field_int4, field_text) VALUES (2, 'Some Text')");
            using (var cmd = new NpgsqlCommand("SELECT field_int2, field_int4, field_text FROM data", Conn))
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
            {
                await reader.ReadAsync();
                Assert.That(await reader.IsDBNullAsync(0), Is.True);
                Assert.That(await reader.GetFieldValueAsync<string>(2), Is.EqualTo("Some Text"));
            }
        }

        [Test, Description("Cancels an async query with the cancellation token")]
        [Timeout(5000)]
        public void Cancel()
        {
            var cancellationSource = new CancellationTokenSource();
            using (var cmd = new NpgsqlCommand("SELECT pg_sleep(5)", Conn))
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