using Npgsql.BackendMessages;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Npgsql
{
    class NpgsqlServerStatus
    {
        internal enum ServerType
        {
            Unknown,
            Down,
            Primary,
            Secondary
        }
    }
}
