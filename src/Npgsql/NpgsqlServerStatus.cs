using Npgsql.BackendMessages;
using Npgsql.Util;
using System;
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

        internal static ServerType Load(NpgsqlConnector npgsqlConnector)
        {
            var returnStatus = ServerType.Unknown;
            try
            {
                npgsqlConnector.WriteQuery("SELECT pg_is_in_recovery()");
                npgsqlConnector.Flush();
                npgsqlConnector.SkipUntil(BackendMessageCode.ReadyForQuery);
                //var columnMessage = npgsqlConnector.ReadMessage();
                //var rowMessage = npgsqlConnector.ReadMessage() as DataRowMessage;

                /*
                var reader = new NpgsqlDataReader(npgsqlConnector);
                var x = reader.NextResult();
                var recoveryStatus = reader.GetString(0);
                */

                //var recoveryStatus = (string?)(command.ExecuteScalar());

                //
                //returnStatus = "t" == "f" ? ServerType.Primary : ServerType.Secondary;
                return ServerType.Primary;
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
