#if NET45 || NET451
#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
        // Prepare Transaction may fail !
        bool PrepareTransaction();
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


        public NpgsqlTransactionCallbacks(NpgsqlConnection connection, System.Transactions.Transaction tx)
        {
            _connection = connection;
            _connectionString = _connection.ConnectionString;
            _connection.Disposed += new EventHandler(_connection_Disposed);

            // Use the transaction distributed identifier to ease log analysis.
            if (tx.TransactionInformation.DistributedIdentifier.CompareTo(Guid.Empty) != 0)
            {
                // the distributed identifier is postfixed with the process id to support several prepared transaction for a single .NET transaction
                _txName = String.Concat(tx.TransactionInformation.DistributedIdentifier.ToString(), @":", connection.ProcessID);
            }
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
                    connection.Connector.ExecuteInternalCommand($"COMMIT PREPARED '{_txName}'");
                }
                else
                {
                    connection.Connector.ExecuteInternalCommand(PregeneratedMessage.CommitTransaction);
                }
            }
            catch (NpgsqlException backendFault)
            {
                Log.Error(@"A NpgsqlException was caught. Details will follow.");
                Log.Error(String.Format(@" ErrorCode is [{0}]", backendFault.ErrorCode));
                Log.Error(String.Format(@" Message is [{0}]", backendFault.Message));
            }
        }

        public bool PrepareTransaction()
        {
            if (!_prepared)
            {
                Log.Debug("Prepare transaction");
                NpgsqlConnection connection = GetConnection();


                try
                {
                    connection.Connector.ExecuteInternalCommand($"PREPARE TRANSACTION '{_txName}'");
                    _prepared = true;
                    // In case of error Rollback will be done in the lower level.
                    connection.DistributedTransactionPreparePhaseEnded();
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
                    Log.Error(String.Format(@" ErrorCode is [{0}]", backendFault.ErrorCode));
                    Log.Error(String.Format(@" Message is [{0}]", backendFault.Message));
                }
            }
            return _prepared;
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
            string commandText = null;

            if (_prepared)
            {
                commandText = $"ROLLBACK PREPARED '{_txName}'";
            }
            else
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
                        couldExecuteCommand = false;
                    }
                    else
                        Log.Error("Failed to cancel the current request. The connection will leak.");
                }
                else
                    commandText = PregeneratedMessage.RollbackTransaction.ToString();
            }

            if (couldExecuteCommand)
            {
                try
                {
                    connection.Connector.ExecuteInternalCommand(commandText);
                }
                // Due to threading issues, the connection could be already disposed by the caller
                catch (System.ObjectDisposedException ode)
                {
                    Log.Error("A System.ObjectDisposedException was caught", ode);
                }
                catch (System.InvalidOperationException ioe)
                {
                    Log.Error(@"A System.InvalidOperationException was caught", ioe);
                }
                // If a NotSupportedException is thrown, then there may be one connection used by multiple threads
                // and this is unsupported ! Check the consumer log to confirm.
                catch (System.NotSupportedException nse)
                {
                    Log.Error("A System.NotSupportedException was caught", nse);
                }
                catch (NpgsqlException backendFault)
                {
                    Log.Error("A NpgsqlException was caught.");
                    Log.Error($" ErrorCode is [{backendFault.ErrorCode}]");
                    Log.Error($" Message is [{backendFault.Message}]");
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
