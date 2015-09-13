#if !DNXCORE50
#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Data;
using System.Reflection;
using Npgsql.Logging;
using Npgsql.FrontendMessages;
using System.IO;

namespace Npgsql
{
    internal interface INpgsqlTransactionCallbacks : IDisposable
    {
        string GetName();
        void PrepareTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }

    internal class NpgsqlTransactionCallbacks : MarshalByRefObject, INpgsqlTransactionCallbacks
    {
        private NpgsqlConnection _connection;
        private readonly string _connectionString;
        private bool _closeConnectionRequired;
        private bool _prepared;
        private readonly string _txName = Guid.NewGuid().ToString();
        private static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        public NpgsqlTransactionCallbacks(NpgsqlConnection connection)
        {
            _connection = connection;
            _connectionString = _connection.ConnectionString;
            _connection.Disposed += new EventHandler(_connection_Disposed);
        }

        private void _connection_Disposed(object sender, EventArgs e)
        {
            // TODO: what happens if this is called from another thread?
            // connections should not be shared across threads while in a transaction
            _connection.Disposed -= new EventHandler(_connection_Disposed);
            // There is no need to set the _connection reference to null here.
            // It will be replaced by the conection that will be used to commit or rollback the prepared transaction. 
            //_connection = null;
        }

        private NpgsqlConnection GetConnection()
        {

            // 04/11/2013 Race Condition detected in old code.
            // Do not mess with the _connection reference.
            // if the connection is already prepared, just grap a new one and return.
            if (_prepared)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
                _closeConnectionRequired = true;
                return _connection;
            }
            // if not, return the existing connection which should'nt be null and must be valid !
            else
            {
                return _connection;
            }
        }

        #region INpgsqlTransactionCallbacks Members

        public string GetName()
        {
            return _txName;
        }

        public void CommitTransaction()
        {
            Log.Debug("Commit transaction");
            var connection = GetConnection();

            try
            {

                if (_prepared)
                {
                    connection.Connector.ExecuteInternalCommand(string.Format("COMMIT PREPARED '{0}'", _txName));
                }
                else
                {
                    connection.Connector.ExecuteInternalCommand(PregeneratedMessage.CommitTransaction);
                }
            }
            // If a NotSupportedException is thrown, then there may be one connection used by multiple threads
            // and this is unsupported ! Check the consumer log to confirm.
            catch (System.NotSupportedException nse)
            {
                Log.Error(@"A System.NotSupportedException was caught. Details will follow.", nse);
            }
            catch (NpgsqlException backendFault)
            {
                Log.Error(@"A NpgsqlException was caught. Details will follow.");
                Log.Error(string.Format(@" Code is [{0}]", backendFault.Code));
                Log.Error(string.Format(@" ErrorCode is [{0}]", backendFault.ErrorCode));
                Log.Error(string.Format(@" Message is [{0}]", backendFault.Message));
                Log.Error(string.Format(@" Detail is [{0}]", backendFault.Detail));
                Log.Error(string.Format(@" Hint is [{0}]", backendFault.Hint));
            }
        }

        public void PrepareTransaction()
        {
            if (!_prepared)
            {
                Log.Debug("Prepare transaction");
                NpgsqlConnection connection = GetConnection();
                connection.Connector.ExecuteInternalCommand(string.Format("PREPARE TRANSACTION '{0}'", _txName));
                _prepared = true;
                // 14/10/2013 In case of error Rollback will be done in the lower level.
                connection.DistributedTransactionPreparePhaseEnded();
            }
        }

        public void RollbackTransaction()
        {
            Log.Debug("Rollback transaction");
            NpgsqlConnection connection = GetConnection();

            // If the transaction is aborted because of a timeout, the connection may still be busy (ConnectionState.Fetching)
            // Therefore, we could not send a ROLLBACK, this will throw in InvalidOperationException (see below).
            // The fix is to cancel the current request if the connection is busy.
            // Failing to do so will leak the connection.
            bool couldExecuteCommand = true;
            if (!_prepared)
            {

                if (connection.Connector.State.HasFlag(ConnectionState.Executing) || connection.Connector.State.HasFlag(ConnectionState.Fetching))
                {
                    bool success = false;
                    try
                    {
                        connection.Connector.CancelRequest();
                        success = true;
                    }
                    catch (IOException)
                    {
                        NpgsqlConnection.ClearPool(connection);
                    }
                    catch (NpgsqlException)
                    {
                        // Cancel documentation says the Cancel doesn't throw on failure
                    }
                    if (success)
                    {
                        Log.Warn(@" Current request successfully canceled.");
                        couldExecuteCommand = false;
                    }
                    else
                        Log.Error(@" Failed to cancel the current request. The connection will leak :-(");
                }

            }
            if (couldExecuteCommand)
            {
                try
                {
                    if (_prepared)
                    {
                        connection.Connector.ExecuteInternalCommand(string.Format("ROLLBACK PREPARED '{0}'", _txName));
                    }
                    else
                    {
                        connection.Connector.ExecuteInternalCommand(PregeneratedMessage.RollbackTransaction);
                    }

                }
                catch (System.ObjectDisposedException ode)
                {
                    Log.Error(@"A System.ObjectDisposedException was caught. Details will follow.", ode);

                }
                //An unhandled exception occurred and the process was terminated.
                //Application ID: NpgsqlResourceManager
                //Process ID: 6300
                //Exception: System.InvalidOperationException
                //Message: There is already an open DataReader associated with this Command which must be closed first.
                catch (System.InvalidOperationException ioe)
                {
                    Log.Error(@"A System.InvalidOperationException was caught. Details will follow.", ioe);

                    // If we end up here this could be because the state of the connection is incorrectly reporting Open while a data reader is fetching data
                }
                // If a NotSupportedException is thrown, then there may be one connection used by multiple threads
                // and this is unsupported ! Check the consumer log to confirm.
                catch (System.NotSupportedException nse)
                {
                    Log.Error(@"A System.NotSupportedException was caught. Details will follow.", nse);
                }
                catch (NpgsqlException backendFault)
                {
                    Log.Error(@"A NpgsqlException was caught. Details will follow.");
                    Log.Error(string.Format(@" Code is [{0}]", backendFault.Code));
                    Log.Error(string.Format(@" ErrorCode is [{0}]", backendFault.ErrorCode));
                    Log.Error(string.Format(@" Message is [{0}]", backendFault.Message));
                    Log.Error(string.Format(@" Hint is [{0}]", backendFault.Hint));
                }
                finally
                {
                    // The rollback may change the value of statement_value, set to unknown
                    connection.Connector.SetBackendTimeoutToUnknown();
                }
            }
            // If we're enlisted in a distributed transaction but not prepared already, we should close it anyway !
            // Otherwise it will leak !!!
            if (!_prepared && !_closeConnectionRequired)
            {
                connection.DistributedTransactionAbortedBeforeBeeingPrepared();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_closeConnectionRequired)
            {
                _connection.Close();
            }
            _closeConnectionRequired = false;
        }

        #endregion
    }
}
#endif
