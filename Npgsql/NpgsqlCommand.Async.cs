using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    public sealed partial class NpgsqlCommand
    {
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior cb, CancellationToken cancellationToken)
        {
            _log.Debug("ExecuteReader with CommandBehavior=" + cb);

            // Close connection if requested even when there is an error.
            try
            {
                return await GetReaderAsync(cb);
            }
            catch (Exception)
            {
                if ((cb & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
                {
                    _connection.Close();
                }

                throw;
            }
        }

        internal async Task<NpgsqlDataReader> GetReaderAsync(CommandBehavior cb, CancellationToken ct)
        {
            CheckConnectionState();

            // Block the notification thread before writing anything to the wire.
            using (_connector.BlockNotificationThread())
            {
                NpgsqlDataReader reader;

                _connector.SetBackendCommandTimeout(CommandTimeout);

                if (_prepared == PrepareStatus.NeedsPrepare)
                {
                    PrepareInternal();
                }

                if (_prepared == PrepareStatus.NotPrepared)
                {
                    byte[] commandText = GetCommandText();

                    var query = new NpgsqlQuery(commandText);

                    // Write the Query message to the wire.
                    _connector.Query(query);

                    // Tell to mediator what command is being sent.
                    if (_prepared == PrepareStatus.NotPrepared)
                    {
                        _connector.Mediator.SetSqlSent(commandText, NpgsqlMediator.SQLSentType.Simple);
                    }
                    else
                    {
                        _connector.Mediator.SetSqlSent(_preparedCommandText, NpgsqlMediator.SQLSentType.Execute);
                    }

                    reader = new NpgsqlDataReader(this, cb, _connector.BlockNotificationThread());

                    if (
                        CommandType == CommandType.StoredProcedure
                        && reader.FieldCount == 1
                        && reader.GetDataTypeName(0) == "refcursor"
                    )
                    {
                        // When a function returns a sole column of refcursor, transparently
                        // FETCH ALL from every such cursor and return those results.
                        var sw = new StringWriter();

                        while (reader.Read())
                        {
                            sw.WriteLine("FETCH ALL FROM \"{0}\";", reader.GetString(0));
                        }

                        reader.Dispose();

                        var queryText = sw.ToString();

                        if (queryText == "")
                        {
                            queryText = ";";
                        }

                        // Passthrough the commandtimeout to the inner command, so user can also control its timeout.
                        // TODO: Check if there is a better way to handle that.

                        query = new NpgsqlQuery(queryText);
                        _connector.Query(query);
                        reader = new NpgsqlDataReader(this, cb, _connector.BlockNotificationThread());
                    }
                }
                else
                {
                    // Update the Bind object with current parameter data as needed.
                    BindParameters();

                    // Write the Bind, Execute, and Sync message to the wire.
                    _connector.Bind(_bind);
                    _connector.Execute(_execute);
                    _connector.Sync();

                    // Tell to mediator what command is being sent.
                    _connector.Mediator.SetSqlSent(_preparedCommandText, NpgsqlMediator.SQLSentType.Execute);

                    // Construct the return reader, possibly with a saved row description from Prepare().
                    reader = new NpgsqlDataReader(this, cb, _connector.BlockNotificationThread(), true, _currentRowDescription);
                }

                return reader;
            }
        }
    }
}
