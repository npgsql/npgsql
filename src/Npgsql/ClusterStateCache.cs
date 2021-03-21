using Npgsql.Util;
using System;
using System.Collections.Concurrent;

namespace Npgsql
{
    static class ClusterStateCache
    {
        static readonly ConcurrentDictionary<ClusterIdentifier, ClusterInfo> Clusters = new();

        static readonly TimeSpan ClusterStateExpiration = TimeSpan.FromSeconds(10); // TODO: add a param?

        internal static ClusterState GetClusterState(string host, int port)
            => Clusters.TryGetValue(new(host, port), out var cs) && !cs.Timeout.HasExpired
                ? cs.State
                : ClusterState.Unknown;

#if NETSTANDARD2_0
        internal static void UpdateClusterState(string host, int port, ClusterState state, DateTime timeStamp)
            => Clusters.AddOrUpdate(
                new ClusterIdentifier(host, port),
                new ClusterInfo(state, new NpgsqlTimeout(ClusterStateExpiration), timeStamp),
                (_, oldInfo) => oldInfo.TimeStamp >= timeStamp ? oldInfo : new ClusterInfo(state, new NpgsqlTimeout(ClusterStateExpiration), timeStamp));
#else
        internal static void UpdateClusterState(string host, int port, ClusterState state, DateTime timeStamp)
            => Clusters.AddOrUpdate(
                new ClusterIdentifier(host, port),
                (_, newInfo) => newInfo,
                (_, oldInfo, newInfo) => oldInfo.TimeStamp >= newInfo.TimeStamp ? oldInfo : newInfo,
                new ClusterInfo(state, new NpgsqlTimeout(ClusterStateExpiration), timeStamp));
#endif

        readonly struct ClusterIdentifier : IEquatable<ClusterIdentifier>
        {
            readonly string _host;
            readonly int _port;

            public ClusterIdentifier(string host, int port) => (_host, _port) = (host, port);
            public override bool Equals(object? obj) => obj is ClusterIdentifier other && Equals(other);
            public bool Equals(ClusterIdentifier other) => _port == other._port && _host == other._host;
            public override int GetHashCode() => HashCode.Combine(_host, _port);
        }

        readonly struct ClusterInfo
        {
            internal readonly ClusterState State;
            internal readonly NpgsqlTimeout Timeout;
            internal readonly DateTime TimeStamp;

            public ClusterInfo(ClusterState state, NpgsqlTimeout timeout, DateTime timeStamp) => (State, Timeout, TimeStamp) = (state, timeout, timeStamp);
        }
    }

    enum ClusterState : byte
    {
        Unknown = 0,
        Offline = 1,
        Primary = 2,
        Secondary = 3
    }
}
