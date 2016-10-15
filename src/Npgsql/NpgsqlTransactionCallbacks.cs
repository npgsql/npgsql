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
using System.Threading;
using Npgsql.Logging;
using Npgsql.FrontendMessages;

namespace Npgsql
{
    interface INpgsqlTransactionCallbacks : IDisposable
    {
        string GetName();
        void PrepareTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }

    sealed class NpgsqlTransactionCallbacks : MarshalByRefObject, INpgsqlTransactionCallbacks
    {
        NpgsqlConnection _connection;
        readonly string _connectionString;
        bool _closeConnectionRequired;
        bool _prepared;
        readonly string _txName = Guid.NewGuid().ToString();
        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        public NpgsqlTransactionCallbacks(NpgsqlConnection connection)
        {
            _connection = connection;
            _connectionString = _connection.ConnectionString;
            _connection.Disposed += _connection_Disposed;
        }

        void _connection_Disposed(object sender, EventArgs e)
        {
            // TODO: what happens if this is called from another thread?
            // connections should not be shared across threads while in a transaction
            _connection.Disposed -= _connection_Disposed;
            _connection = null;
        }

        NpgsqlConnection GetConnection()
        {
            if (_connection == null || (_connection.FullState & ConnectionState.Open) != ConnectionState.Open)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
                _closeConnectionRequired = true;
                return _connection;
            }
            return _connection;
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

            if (_prepared)
                connection.Connector.ExecuteInternalCommand($"COMMIT PREPARED '{_txName}'");
            else
                connection.Connector.ExecuteInternalCommand(PregeneratedMessage.CommitTransaction);
        }

        public void PrepareTransaction()
        {
            if (_prepared)
                return;
            Log.Debug("Prepare transaction");
            var connection = GetConnection();
            connection.Connector.ExecuteInternalCommand($"PREPARE TRANSACTION '{_txName}'");
            _prepared = true;
        }

        public void RollbackTransaction()
        {
            Log.Debug("Rollback transaction");
            var connection = GetConnection();

            if (_prepared)
                connection.Connector.ExecuteInternalCommand($"ROLLBACK PREPARED '{_txName}'");
            else
                connection.Connector.ExecuteInternalCommand(PregeneratedMessage.RollbackTransaction);
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
