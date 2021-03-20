using Npgsql.Util;
using System;
using System.Collections.Concurrent;

namespace Npgsql
{
    enum HostState
    {
        Unknown = 0,
        Offline = 1,
        Primary = 2,
        Secondary = 3
    }

    internal static class HostsCache
    {
        static readonly ConcurrentDictionary<string, HostStateCache> hosts = new();

        static readonly TimeSpan hostStateExpiration = TimeSpan.FromSeconds(10); // TODO: add a param?

        internal static HostState GetHostState(string host)
        {
            if (hosts.TryGetValue(host, out var hsc) && !hsc.timeout.HasExpired)
                return hsc.state;

            return HostState.Unknown;
        }

        internal static void UpdateHostState(string host, HostState state)
        {
            var hostState = new HostStateCache
            {
                state = state,
                timeout = new NpgsqlTimeout(hostStateExpiration)
            };
            hosts.AddOrUpdate(host, _ => hostState, (_, __) => hostState);
        }

        class HostStateCache
        {
            internal HostState state;
            internal NpgsqlTimeout timeout;
        }
    }
}
