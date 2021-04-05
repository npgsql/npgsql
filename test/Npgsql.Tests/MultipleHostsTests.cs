﻿using Npgsql.Tests.Support;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using static Npgsql.Tests.Support.MockState;
using static Npgsql.Tests.TestUtil;

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
                TargetSessionAttributes = servers[0] switch
                {
                    Primary => "ReadWrite",
                    PrimaryReadOnly => "ReadOnly",
                    Standby => "Standby",
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
                TargetSessionAttributes = "Primary",
            };

            var standbyCsb = new NpgsqlConnectionStringBuilder(defaultConnectionString)
            {
                TargetSessionAttributes = "Standby",
            };

            var preferPrimaryCsb = new NpgsqlConnectionStringBuilder(defaultConnectionString)
            {
                TargetSessionAttributes = "PreferPrimary",
            };

            var preferStandbyCsb = new NpgsqlConnectionStringBuilder(defaultConnectionString)
            {
                TargetSessionAttributes = "PreferStandby",
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
        public void TargetSessionAttributes_default_is_null()
            => Assert.That(new NpgsqlConnectionStringBuilder().TargetSessionAttributes, Is.Null);

        [Test, NonParallelizable]
        public async Task TargetSessionAttributes_uses_environment_variable()
        {
            using var envVarResetter = SetEnvironmentVariable("PGTARGETSESSIONATTRS", "PreferStandby");

            await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
            await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading
            };

            Assert.That(builder.TargetSessionAttributes, Is.Null);

            using var _ = CreateTempPool(builder.ConnectionString, out var connectionString);

            await using var conn = await OpenConnectionAsync(connectionString);
            Assert.That(conn.Port, Is.EqualTo(standbyPostmaster.Port));
        }

        [Test]
        public void TargetSessionAttributes_invalid_throws()
            => Assert.Throws<ArgumentException>(() =>
                new NpgsqlConnectionStringBuilder
                {
                    TargetSessionAttributes = nameof(TargetSessionAttributes_invalid_throws)
                });

        [Test]
        public void HostRecheckSeconds_default_value()
        {
            var builder = new NpgsqlConnectionStringBuilder();
            Assert.That(builder.HostRecheckSeconds, Is.EqualTo(10));
            Assert.That(builder.HostRecheckSecondsTranslated, Is.EqualTo(TimeSpan.FromSeconds(10)));
        }

        [Test]
        public void HostRecheckSeconds_zero_value()
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                HostRecheckSeconds = 0,
            };
            Assert.That(builder.HostRecheckSeconds, Is.EqualTo(0));
            Assert.That(builder.HostRecheckSecondsTranslated, Is.EqualTo(TimeSpan.FromSeconds(-1)));
        }

        [Test]
        public void HostRecheckSeconds_invalid_throws()
            => Assert.Throws<ArgumentException>(() =>
                new NpgsqlConnectionStringBuilder
                {
                    HostRecheckSeconds = -1
                });

        [Test]
        public async Task Connect_with_load_balancing()
        {
            await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
            await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

            var defaultCsb = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                MaxPoolSize = 1,
                LoadBalanceHosts = true,
            };

            using var _ = CreateTempPool(defaultCsb.ConnectionString, out var defaultConnectionString);

            NpgsqlConnector firstConnector;
            NpgsqlConnector secondConnector;

            await using (var firstConnection = await OpenConnectionAsync(defaultConnectionString))
            await using (var secondConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                firstConnector = firstConnection.Connector!;
                secondConnector = secondConnection.Connector!;
            }

            Assert.AreNotSame(firstConnector, secondConnector);

            await using (var firstBalancedConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                Assert.AreSame(firstConnector, firstBalancedConnection.Connector);
            }

            await using (var secondBalancedConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                Assert.AreSame(secondConnector, secondBalancedConnection.Connector);
            }

            await using (var thirdBalancedConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                Assert.AreSame(firstConnector, thirdBalancedConnection.Connector);
            }
        }

        [Test]
        public async Task Connect_without_load_balancing()
        {
            await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
            await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

            var defaultCsb = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                MaxPoolSize = 1,
                LoadBalanceHosts = false,
            };

            using var _ = CreateTempPool(defaultCsb.ConnectionString, out var defaultConnectionString);

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
        }

        [Test]
        public async Task Connect_state_changing_hosts([Values] bool alwaysCheckHostState)
        {
            await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
            await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

            var defaultCsb = new NpgsqlConnectionStringBuilder
            {
                Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                MaxPoolSize = 1,
                HostRecheckSeconds = alwaysCheckHostState ? 0 : int.MaxValue,
                TargetSessionAttributes = "PreferPrimary",
                NoResetOnClose = true,
            };

            using var _ = CreateTempPool(defaultCsb.ConnectionString, out var defaultConnectionString);

            NpgsqlConnector firstConnector;
            NpgsqlConnector secondConnector;
            var firstServerTask = Task.Run(async () =>
            {
                var server = await primaryPostmaster.WaitForServerConnection();
                if (!alwaysCheckHostState)
                    return;

                // Update the state after a 'failover'
                await server.SendMockState(Standby);
            });
            var secondServerTask = Task.Run(async () =>
            {
                var server = await standbyPostmaster.WaitForServerConnection();
                if (!alwaysCheckHostState)
                    return;

                // As TargetSessionAttributes is 'prefer', it does another cycle for the 'unpreferred'
                await server.SendMockState(Standby);
                // Update the state after a 'failover'
                await server.SendMockState(Primary);
            });

            await using (var firstConnection = await OpenConnectionAsync(defaultConnectionString))
            await using (var secondConnection = await OpenConnectionAsync(defaultConnectionString))
            {
                firstConnector = firstConnection.Connector!;
                secondConnector = secondConnection.Connector!;
            }

            await using var thirdConnection = await OpenConnectionAsync(defaultConnectionString);
            Assert.AreSame(alwaysCheckHostState ? secondConnector : firstConnector, thirdConnection.Connector);

            await firstServerTask;
            await secondServerTask;
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
        public void IntegrationTest([Values] bool loadBalancing, [Values] bool alwaysCheckHostState)
        {
            PoolManager.Reset();

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = "localhost,127.0.0.1",
                Pooling = true,
                MaxPoolSize = 2,
                LoadBalanceHosts = loadBalancing,
                HostRecheckSeconds = alwaysCheckHostState ? 0 : 10,
            };

            var queriesDone = 0;

            var clientsTask = Task.WhenAll(
                Client(csb, "Any"),
                Client(csb, "Primary"),
                Client(csb, "PreferPrimary"),
                Client(csb, "PreferStandby"),
                Client(csb, "ReadWrite"));

            var onlyStandbyClient = Client(csb, "Standby");
            var readOnlyClient = Client(csb, "ReadOnly");

            Assert.DoesNotThrowAsync(() => clientsTask);
            Assert.ThrowsAsync<NpgsqlException>(() => onlyStandbyClient);
            Assert.ThrowsAsync<NpgsqlException>(() => readOnlyClient);
            Assert.AreEqual(125, queriesDone);

            Assert.AreEqual(8, PoolManager.Pools.Where(x => x.Key is not null).Count());

            PoolManager.Reset();

            Task Client(NpgsqlConnectionStringBuilder csb, string targetSessionAttributes)
            {
                csb = csb.Clone();
                csb.TargetSessionAttributes = targetSessionAttributes;
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
