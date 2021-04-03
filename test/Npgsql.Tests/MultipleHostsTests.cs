using System;
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
using System.Transactions;

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
        public async Task Connect_to_correct_host(TestTargetSessionAttributes targetSessionAttributes, MockState[] servers, int expectedServer)
        {
            var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
            await using var __ = new DisposableWrapper(postmasters);

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(postmasters),
                TargetSessionAttributes = targetSessionAttributes.ToString(),
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
            TestTargetSessionAttributes targetSessionAttributes, MockState[] servers, int expectedServer)
        {
            var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
            await using var __ = new DisposableWrapper(postmasters);

            // First, open and close a connection with the TargetSessionAttributes matching the first server.
            // This ensures wew have an idle connection in the pool.
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(postmasters),
                TargetSessionAttributesParsed = servers[0] switch
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
                TargetSessionAttributes = targetSessionAttributes.ToString()
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
        public async Task Valid_host_not_found(TestTargetSessionAttributes targetSessionAttributes, MockState[] servers)
        {
            var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
            await using var __ = new DisposableWrapper(postmasters);

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(postmasters),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                TargetSessionAttributes = targetSessionAttributes.ToString()
            };

            using var pool = CreateTempPool(connectionStringBuilder.ConnectionString, out var connectionString);

            var exception = Assert.ThrowsAsync<NpgsqlException>(async () => await OpenConnectionAsync(connectionString))!;
            Assert.That(exception.Message, Is.EqualTo("No suitable host was found."));
            Assert.That(exception.InnerException, Is.Null);

            for (var i = 0; i < servers.Length; i++)
                _ = await postmasters[i].WaitForServerConnection();
        }

        [Test]
        [Description("Test that enlist returns a new connector if a previous connector is for an incompatible server type")]
        public async Task Enlist_depends_on_session_attributes()
        {
            await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
            await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

            var defaultCsb = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                Enlist = true,
            };

            using var _ = CreateTempPool(defaultCsb.ConnectionString, out var defaultConnectionString);

            var primaryCsb = new NpgsqlConnectionStringBuilder(defaultConnectionString)
            {
                TargetSessionAttributesParsed = TargetSessionAttributes.Primary,
            };

            var standbyCsb = new NpgsqlConnectionStringBuilder(defaultConnectionString)
            {
                TargetSessionAttributesParsed = TargetSessionAttributes.Standby,
            };

            var preferPrimaryCsb = new NpgsqlConnectionStringBuilder(defaultConnectionString)
            {
                TargetSessionAttributesParsed = TargetSessionAttributes.PreferPrimary,
            };

            var preferStandbyCsb = new NpgsqlConnectionStringBuilder(defaultConnectionString)
            {
                TargetSessionAttributesParsed = TargetSessionAttributes.PreferStandby,
            };

            // Note that the transaction scope is not disposed due to a rollback (which isn't something a mock expects)
            var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            NpgsqlConnector primaryConnector;
            NpgsqlConnector standbyConnector;

            using (var primaryConnection = await OpenConnectionAsync(primaryCsb.ConnectionString))
            {
                primaryConnector = primaryConnection.Connector!;
            }

            using (var preferPrimaryConnection = await OpenConnectionAsync(preferPrimaryCsb.ConnectionString))
            {
                Assert.AreSame(primaryConnector, preferPrimaryConnection.Connector);
            }

            using (var preferStandbyConnection = await OpenConnectionAsync(preferStandbyCsb.ConnectionString))
            {
                Assert.AreSame(primaryConnector, preferStandbyConnection.Connector);
            }

            using (var standbyConnection = await OpenConnectionAsync(standbyCsb.ConnectionString))
            {
                standbyConnector = standbyConnection.Connector!;
            }

            using (var preferPrimaryConnection = await OpenConnectionAsync(preferPrimaryCsb.ConnectionString))
            {
                Assert.AreSame(standbyConnector, preferPrimaryConnection.Connector);
            }

            using (var preferStandbyConnection = await OpenConnectionAsync(preferStandbyCsb.ConnectionString))
            {
                Assert.AreSame(standbyConnector, preferStandbyConnection.Connector);
            }

            Assert.AreNotSame(primaryConnector, standbyConnector);

            await primaryPostmaster.WaitForServerConnection();
            await standbyPostmaster.WaitForServerConnection();
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
        public async Task TargetSessionAttributes_with_single_host([Values] TestTargetSessionAttributes targetSessionAttributes)
        {
            var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                TargetSessionAttributes = targetSessionAttributes.ToString()
            }.ConnectionString;

            if (targetSessionAttributes == TestTargetSessionAttributes.Any)
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
            Assert.That(builder.TargetSessionAttributes, Is.EqualTo(TargetSessionAttributes.Any.ToString()));
            Assert.That(builder.TargetSessionAttributesParsed, Is.EqualTo(TargetSessionAttributes.Any));
        }

        [Test]
        public void TargetSessionAttributes_invalid_is_Any()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                TargetSessionAttributes = nameof(TargetSessionAttributes_invalid_is_Any)
            };
            Assert.That(builder.TargetSessionAttributes, Is.EqualTo(nameof(TargetSessionAttributes_invalid_is_Any)));
            Assert.That(builder.TargetSessionAttributesParsed, Is.EqualTo(TargetSessionAttributes.Any));
        }

        [Test]
        public async Task Load_balancing_is_working()
        {
            await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
            await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

            var defaultCsb = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                MaxPoolSize = 1,
            };

            using var _ = CreateTempPool(defaultCsb.ConnectionString, out var defaultConnectionString);

            var balancingCsb = new NpgsqlConnectionStringBuilder(defaultConnectionString)
            {
                LoadBalanceHosts = true
            };

            NpgsqlConnector firstConnector;
            NpgsqlConnector secondConnector;

            await using (var firstConnection = await OpenConnectionAsync(defaultConnectionString))
            await using (var secondConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                firstConnector = firstConnection.Connector!;
                secondConnector = secondConnection.Connector!;
            }

            Assert.AreNotSame(firstConnector, secondConnector);

            await using (var firstUnbalancedConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                Assert.AreSame(firstConnector, firstUnbalancedConnection.Connector);
            }

            await using (var secondUnbalancedConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                Assert.AreSame(firstConnector, secondUnbalancedConnection.Connector);
            }

            await using (var firstBalancedConnection = await OpenConnectionAsync(balancingCsb.ConnectionString))
            {
                Assert.AreSame(firstConnector, firstBalancedConnection.Connector);
            }

            await using (var thirdUnbalancedConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                Assert.AreSame(firstConnector, thirdUnbalancedConnection.Connector);
            }

            await using (var secondBalancedConnection = await OpenConnectionAsync(balancingCsb.ConnectionString))
            {
                Assert.AreSame(secondConnector, secondBalancedConnection.Connector);
            }

            await using (var thirdBalancedConnection = await OpenConnectionAsync(balancingCsb.ConnectionString))
            {
                Assert.AreSame(firstConnector, thirdBalancedConnection.Connector);
            }
        }

        [Test]
        public void Cluster_state_cache_basic()
        {
            const string host = nameof(Cluster_state_cache_basic);
            var timeStamp = DateTime.UtcNow;

            ClusterStateCache.UpdateClusterState(host, 5432, ClusterState.PrimaryReadWrite, timeStamp, TimeSpan.Zero);
            Assert.AreEqual(ClusterState.PrimaryReadWrite, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: false));

            // Update with the same timestamp - shouldn't change anything
            ClusterStateCache.UpdateClusterState(host, 5432, ClusterState.Standby, timeStamp, TimeSpan.Zero);
            Assert.AreEqual(ClusterState.PrimaryReadWrite, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: false));

            // Update with a new timestamp
            timeStamp = timeStamp.AddSeconds(1);
            ClusterStateCache.UpdateClusterState(host, 5432, ClusterState.PrimaryReadOnly, timeStamp, TimeSpan.Zero);
            Assert.AreEqual(ClusterState.PrimaryReadOnly, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: false));

            // Expired state returns as Unknown (depending on ignoreExpiration)
            timeStamp = timeStamp.AddSeconds(1);
            ClusterStateCache.UpdateClusterState(host, 5432, ClusterState.PrimaryReadWrite, timeStamp, TimeSpan.FromSeconds(-1));
            Assert.AreEqual(ClusterState.Unknown, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: false));
            Assert.AreEqual(ClusterState.PrimaryReadWrite, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: true));
        }

        // This is the only test in this class which actually connects to PostgreSQL (the others use the PostgreSQL mock)
        [Test, Timeout(10000), NonParallelizable]
        public void IntegrationTest([Values(true, false)] bool withLoadBalancing)
        {
            PoolManager.Reset();

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = "localhost,127.0.0.1",
                Pooling = true,
                MaxPoolSize = 2,
                LoadBalanceHosts = withLoadBalancing
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
                csb.TargetSessionAttributesParsed = targetServerType;
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

        public enum TestTargetSessionAttributes : byte
        {
            Any = TargetSessionAttributes.Any,
            ReadWrite = TargetSessionAttributes.ReadWrite,
            ReadOnly = TargetSessionAttributes.ReadOnly,
            Primary = TargetSessionAttributes.Primary,
            Standby = TargetSessionAttributes.Standby,
            PreferPrimary = TargetSessionAttributes.PreferPrimary,
            PreferStandby = TargetSessionAttributes.PreferStandby,
        }

        class DisposableWrapper : IAsyncDisposable
        {
            private readonly IEnumerable<IAsyncDisposable> disposables;

            public DisposableWrapper(IEnumerable<IAsyncDisposable> disposables) => this.disposables = disposables;

            public async ValueTask DisposeAsync()
            {
                foreach (var disposable in disposables)
                    await disposable.DisposeAsync();
            }
        }
    }
}
