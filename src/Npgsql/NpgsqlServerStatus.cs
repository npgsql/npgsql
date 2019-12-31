using Npgsql.BackendMessages;
using System.Collections.Concurrent;
using System.Net.Sockets;

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
                npgsqlConnector.StartUserAction();
                npgsqlConnector.WriteQuery("SELECT pg_is_in_recovery()");
                npgsqlConnector.Flush();
                
                var columnsMsg = npgsqlConnector.ReadMessage();
                var rowMsg = (DataRowMessage)(npgsqlConnector.ReadMessage());
                
                var columnCount = npgsqlConnector.ReadBuffer.ReadInt16();
                var lengthOfColumnValue = npgsqlConnector.ReadBuffer.ReadInt32();
                var buffer = new byte[lengthOfColumnValue];
                npgsqlConnector.ReadBuffer.ReadBytes(buffer, 0, lengthOfColumnValue);

                returnStatus = buffer[0] == 'f' ? ServerType.Primary : ServerType.Secondary;
                
                npgsqlConnector.SkipUntil(BackendMessageCode.ReadyForQuery);
                npgsqlConnector.EndUserAction();

                return returnStatus;
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
