using Npgsql.Util;
using System.Collections.Concurrent;
using System.Data;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    class NpgsqlServerStatus
    {
        internal static readonly ConcurrentDictionary<string, ServerType> Cache = new ConcurrentDictionary<string, ServerType>();

        internal enum ServerType
        {
            Unknown,
            Down,
            Primary,
            Secondary
        }

        internal static async Task<ServerType> Load(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
        {
            var returnStatus = ServerType.Unknown;
            try
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT pg_is_in_recovery();";
                    command.AllResultTypesAreUnknown = true;
                    var recoveryStatus = (string?)(await command.ExecuteScalarAsync());

                    returnStatus = recoveryStatus == "f" ? ServerType.Primary : ServerType.Secondary;
                }
            }
            catch (SocketException)
            {
                returnStatus = ServerType.Down;
            }
            catch (NpgsqlException)
            {
                returnStatus = ServerType.Down;
            }

            return returnStatus;
        }

    }
}
