// NpgsqlTransactionCallbacks.cs
//
// Author:
//  Josh Cooley <jbnpgsql@tuxinthebox.net>
//
// Copyright (C) 2007, The Npgsql Development Team
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Npgsql
{
    internal interface INpgsqlTransactionCallbacks
    {
        string GetName();
        void PrepareTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }

    internal class NpgsqlTransactionCallbacks : MarshalByRefObject, INpgsqlTransactionCallbacks
    {
        private NpgsqlConnection _connection;
        private string _connectionString;
        private bool _prepared;
        private string _txName = Guid.NewGuid().ToString();

        private static readonly String CLASSNAME = "NpgsqlTransactionCallbacks";

        public NpgsqlTransactionCallbacks(NpgsqlConnection connection)
        {
            _connection = connection;
            _connectionString = _connection.ConnectionString;
            _connection.Disposed += new EventHandler(_connection_Disposed);
        }

        void _connection_Disposed(object sender, EventArgs e)
        {
            // TODO: what happens if this is called from another thread?
            _connection.Disposed -= new EventHandler(_connection_Disposed);
            _connection = null;
        }

        private ConnectionScope GetConnectionScope()
        {
            if (_connection == null ||
                (_connection.State & ConnectionState.Open) != ConnectionState.Open)
                return new ConnectionScope(_connectionString);
            else
                return new ConnectionScope(_connection);
        }

        #region INpgsqlTransactionCallbacks Members

        public string GetName()
        {
            return _txName;
        }

        public void CommitTransaction()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "CommitTransaction");
            using (ConnectionScope scope = GetConnectionScope())
            {
                NpgsqlCommand command = null;
                if (_prepared)
                {
                    command = new NpgsqlCommand(string.Format("COMMIT PREPARED '{0}'", _txName), scope.Connection);
                }
                else
                {
                    command = new NpgsqlCommand("COMMIT", scope.Connection);
                }
                command.ExecuteNonQuery();
            }
        }

        public void PrepareTransaction()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "PrepareTransaction");
            // TODO: this isn't going to work well in non-pooled connections
            // since prepare will usually preceed commit
            using (ConnectionScope scope = GetConnectionScope())
            {
                NpgsqlCommand command = new NpgsqlCommand(string.Format("PREPARE TRANSACTION '{0}'", _txName), scope.Connection);
                command.ExecuteNonQuery();
                _prepared = true;
            }
        }

        public void RollbackTransaction()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "RollbackTransaction");
            using (ConnectionScope scope = GetConnectionScope())
            {
                NpgsqlCommand command = null;
                if (_prepared)
                {
                    command = new NpgsqlCommand(string.Format("ROLLBACK PREPARED '{0}'", _txName), scope.Connection);
                }
                else
                {
                    command = new NpgsqlCommand("ROLLBACK", scope.Connection);
                }
                command.ExecuteNonQuery();
            }
        }

        #endregion

        private class ConnectionScope : IDisposable
        {
            private NpgsqlConnection _connection;
            private bool _disposeConnection;

            public ConnectionScope(NpgsqlConnection connection)
            {
                _connection = connection;
                _disposeConnection = false;
            }

            public ConnectionScope(string connectionString)
            {
                _connection = new NpgsqlConnection(connectionString);
                _connection.Open();
                _disposeConnection = true;
            }

            public NpgsqlConnection Connection
            {
                get { return _connection; }
            }

            #region IDisposable Members

            public void Dispose()
            {
                if (_disposeConnection)
                    _connection.Dispose();
            }

            #endregion
        }
    }
}
