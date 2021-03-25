using System;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Tests.Support;
using static Npgsql.Tests.TestUtil;
using static Npgsql.Tests.Support.MockState;

namespace Npgsql.Tests
{
    public class MultipleHostsTests : TestBase
    {
        static readonly object[] MyCases =
        {
            new object[] { TargetSessionAttributes.Standby,       new[] { Primary,         Standby         }, 1 },
            new object[] { TargetSessionAttributes.Standby,       new[] { PrimaryReadOnly, Standby         }, 1 },
            new object[] { TargetSessionAttributes.PreferStandby, new[] { Primary,         Standby         }, 1 },
            new object[] { TargetSessionAttributes.PreferStandby, new[] { PrimaryReadOnly, Standby         }, 1 },
            new object[] { TargetSessionAttributes.PreferStandby, new[] { Primary,         Primary         }, 0 },
            new object[] { TargetSessionAttributes.Primary,       new[] { Standby,         Primary         }, 1 },
            new object[] { TargetSessionAttributes.Primary,       new[] { Standby,         PrimaryReadOnly }, 1 },
            new object[] { TargetSessionAttributes.PreferPrimary, new[] { Standby,         Primary         }, 1 },
            new object[] { TargetSessionAttributes.PreferPrimary, new[] { Standby,         PrimaryReadOnly }, 1 },
            new object[] { TargetSessionAttributes.PreferPrimary, new[] { Standby,         Standby         }, 0 },
            new object[] { TargetSessionAttributes.Any,           new[] { Standby,         Primary         }, 0 },
            new object[] { TargetSessionAttributes.Any,           new[] { Primary,         Standby         }, 0 },
            new object[] { TargetSessionAttributes.Any,           new[] { PrimaryReadOnly, Standby         }, 0 },
            new object[] { TargetSessionAttributes.ReadWrite,     new[] { Standby,         Primary         }, 1 },
            new object[] { TargetSessionAttributes.ReadWrite,     new[] { PrimaryReadOnly, Primary         }, 1 },
            new object[] { TargetSessionAttributes.ReadOnly,      new[] { Primary,         Standby         }, 1 },
            new object[] { TargetSessionAttributes.ReadOnly,      new[] { PrimaryReadOnly, Standby         }, 0 }
        };

        [Test]
        [TestCaseSource(nameof(MyCases))]
        public async Task Connect_to_correct_host(TargetSessionAttributes targetSessionAttributes, MockState[] servers, int expectedServer)
        {
            var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(postmasters),
                TargetSessionAttributes = targetSessionAttributes,
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            };

            using var pool = CreateTempPool(connectionStringBuilder, out var connectionString);
            await using var conn = await OpenConnectionAsync(connectionString);

            Assert.That(conn.Port, Is.EqualTo(postmasters[expectedServer].Port));

            for (var i = 0; i <= expectedServer; i++)
                _ = await postmasters[i].WaitForServerConnection();
        }

        [Test]
        [TestCaseSource(nameof(MyCases))]
        public async Task Connect_to_correct_host_with_available_idle(
            TargetSessionAttributes targetSessionAttributes, MockState[] servers, int expectedServer)
        {
            var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();

            // First, open and close a connection with the TargetSessionAttributes matching the first server.
            // This ensures wew have an idle connection in the pool.
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(postmasters),
                TargetSessionAttributes = servers[0] switch
                {
                    Primary => TargetSessionAttributes.ReadWrite,
                    PrimaryReadOnly => TargetSessionAttributes.ReadOnly,
                    Standby => TargetSessionAttributes.Standby,
                    _ => throw new ArgumentOutOfRangeException()
                },
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            };

            using var pool = CreateTempPool(connectionStringBuilder, out var connectionString);
            await using (_ = await OpenConnectionAsync(connectionString))
            {
                // Do nothing, close to have an idle connection in the pool.
            }

            // Now connect with the test TargetSessionAttributes
            connectionString = new NpgsqlConnectionStringBuilder(connectionString)
            {
                TargetSessionAttributes = targetSessionAttributes
            }.ConnectionString;

            await using var conn = await OpenConnectionAsync(connectionString);

            Assert.That(conn.Port, Is.EqualTo(postmasters[expectedServer].Port));

            for (var i = 0; i <= expectedServer; i++)
                _ = await postmasters[i].WaitForServerConnection();
        }

        [Test]
        [TestCase(TargetSessionAttributes.Standby,   new[] { Primary,         Primary })]
        [TestCase(TargetSessionAttributes.Primary,   new[] { Standby,         Standby })]
        [TestCase(TargetSessionAttributes.ReadWrite, new[] { PrimaryReadOnly, Standby })]
        [TestCase(TargetSessionAttributes.ReadOnly,  new[] { Primary,         Primary })]
        public async Task Valid_host_not_found(TargetSessionAttributes targetSessionAttributes, MockState[] servers)
        {
            var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(postmasters),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                TargetSessionAttributes = targetSessionAttributes
            };

            using var pool = CreateTempPool(connectionStringBuilder.ConnectionString, out var connectionString);

            var exception = Assert.ThrowsAsync<NpgsqlException>(async () => await OpenConnectionAsync(connectionString))!;
            Assert.That(exception.Message, Is.EqualTo("No suitable host was found."));
            Assert.That(exception.InnerException, Is.Null);

            for (var i = 0; i < servers.Length; i++)
                _ = await postmasters[i].WaitForServerConnection();
        }

        [Test]
        public void All_hosts_are_down()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, 0);

            using var socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket1.Bind(endpoint);
            var localEndPoint1 = (IPEndPoint)socket1.LocalEndPoint!;

            using var socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket2.Bind(endpoint);
            var localEndPoint2 = (IPEndPoint)socket2.LocalEndPoint!;

            // Note that we Bind (to reserve the port), but do not Listen - connection attempts will fail.

            var connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = $"{localEndPoint1.Address}:{localEndPoint1.Port},{localEndPoint2.Address}:{localEndPoint2.Port}"
            }.ConnectionString;

            var exception = Assert.ThrowsAsync<NpgsqlException>(async () => await OpenConnectionAsync(connectionString))!;
            var aggregateException = (AggregateException)exception.InnerException!;
            Assert.That(aggregateException.InnerExceptions, Has.Count.EqualTo(2));

            for (var i = 0; i < aggregateException.InnerExceptions.Count; i++)
            {
                Assert.That(aggregateException.InnerExceptions[i], Is.TypeOf<NpgsqlException>()
                    .With.InnerException.TypeOf<SocketException>()
                    .With.InnerException.Property(nameof(SocketException.SocketErrorCode)).EqualTo(SocketError.ConnectionRefused));
            }
        }

        [Test]
        public async Task First_host_is_down()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endpoint = new IPEndPoint(IPAddress.Loopback, 0);
            socket.Bind(endpoint);
            var localEndPoint = (IPEndPoint)socket.LocalEndPoint!;
            // Note that we Bind (to reserve the port), but do not Listen - connection attempts will fail.

            await using var postmaster = PgPostmasterMock.Start(state: Primary);

            var connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = $"{localEndPoint.Address}:{localEndPoint.Port},{postmaster.Host}:{postmaster.Port}",
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading
            }.ConnectionString;

            await using var conn = await OpenConnectionAsync(connectionString);
            Assert.That(conn.Port, Is.EqualTo(postmaster.Port));
        }

        [Test]
        public async Task TargetSessionAttributes_with_single_host([Values] TargetSessionAttributes targetSessionAttributes)
        {
            var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                TargetSessionAttributes = targetSessionAttributes
            }.ConnectionString;

            if (targetSessionAttributes == TargetSessionAttributes.Any)
            {
                await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
                using var pool = CreateTempPool(postmasterMock.ConnectionString, out connectionString);
                await using var conn = await OpenConnectionAsync(connectionString);
                _ = await postmasterMock.WaitForServerConnection();
            }
            else
            {
                Assert.That(() => OpenConnectionAsync(connectionString), Throws.Exception.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void TargetSessionAttributes_default_is_Any()
        {
            var builder = new NpgsqlConnectionStringBuilder();
            Assert.That(builder.TargetSessionAttributes, Is.EqualTo(TargetSessionAttributes.Any));
        }

        // This is the only test in this class which actually connects to PostgreSQL (the others use the PostgreSQL mock)
        [Test, Timeout(10000), NonParallelizable]
        public void IntegrationTest()
        {
            PoolManager.Reset();

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
                Client(csb, TargetSessionAttributes.PreferStandby),
                Client(csb, TargetSessionAttributes.ReadWrite));

            var onlyStandbyClient = Client(csb, TargetSessionAttributes.Standby);
            var readOnlyClient = Client(csb, TargetSessionAttributes.ReadOnly);

            Assert.DoesNotThrowAsync(async () => await clientsTask);
            Assert.ThrowsAsync<NpgsqlException>(async () => await onlyStandbyClient);
            Assert.ThrowsAsync<NpgsqlException>(async () => await readOnlyClient);
            Assert.AreEqual(125, queriesDone);

            Assert.AreEqual(8, PoolManager.Pools.Where(x => x.Key is not null).Count());

            PoolManager.Reset();

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

        static string MultipleHosts(params PgPostmasterMock[] postmasters)
            => string.Join(",", postmasters.Select(p => $"{p.Host}:{p.Port}"));
    }
}
