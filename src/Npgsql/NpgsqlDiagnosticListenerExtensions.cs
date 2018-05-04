using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Data;

namespace Npgsql
{
    static class NpgsqlDiagnosticListenerExtensions
    {
        public const string CommandDiagnosticListenerName = "Npgsql.Command";
        public const string ConnectionDiagnosticListenerName = "Npgsql.Connection";
        public const string TransactionDiagnosticListenerName = "Npgsql.Transaction";

        public const string NpgsqlExecuteCommandStart = nameof(ExecuteCommandStart);
        public const string NpgsqlExecuteCommandStop = nameof(ExecuteCommandStop);
        public const string NpgsqlExecuteCommandError = nameof(ExecuteCommandError);

        public const string NpgsqlOpenConnectionStart = nameof(OpenConnectionStart);
        public const string NpgsqlOpenConnectionStop = nameof(OpenConnectionStop);
        public const string NpgsqlOpenConnectionError = nameof(WriteConnectionOpenError);

        public const string NpgsqlCloseConnectionStart = nameof(CloseConnectionStart);
        public const string NpgsqlCloseConnectionStop = nameof(CloseConnectionStop);
        public const string NpgsqlCloseConnectionError = nameof(CloseConnectionError);

        public const string NpgsqlCommitTransactionStart = nameof(CommitTransactionStart);
        public const string NpgsqlCommitTransactionStop = nameof(CommitTransactionStop);
        public const string NpgsqlCommitTransactionError = nameof(CommitTransactionError);

        public const string NpgsqlRollbackTransactionStart = nameof(RollbackTransactionStart);
        public const string NpgsqlRollbackTransactionStop = nameof(RollbackTransactionStop);
        public const string NpgsqlRollbackTransactionError = nameof(RollbackTransactionError);

        public static void ExecuteCommandStart(this DiagnosticListener @this, NpgsqlCommand command)
        {
            if (!@this.IsEnabled(NpgsqlExecuteCommandStart))
            {
                return;
            }

            @this.Write(NpgsqlExecuteCommandStart, command);
        }

        public static void ExecuteCommandStop(this DiagnosticListener @this, NpgsqlCommand command)
        {
            if (@this.IsEnabled(NpgsqlExecuteCommandStop))
            {
                @this.Write(NpgsqlExecuteCommandStop, command);
            }
        }

        public static void ExecuteCommandError(this DiagnosticListener @this, NpgsqlCommand command, Exception ex)
        {
            if (@this.IsEnabled(NpgsqlExecuteCommandError))
            {
                @this.Write(
                    NpgsqlExecuteCommandError,
                    new
                    {
                        Command = command,
                        Exception = ex
                    });
            }
        }

        public static void OpenConnectionStart(this DiagnosticListener @this, NpgsqlConnection connection)
        {
            if (@this.IsEnabled(NpgsqlOpenConnectionStart))
            {
                @this.Write(NpgsqlOpenConnectionStart, connection);
            }
        }

        public static void OpenConnectionStop(this DiagnosticListener @this, NpgsqlConnection connection)
        {
            if (@this.IsEnabled(NpgsqlOpenConnectionStop))
            {
                @this.Write(NpgsqlOpenConnectionStop, connection);
            }
        }

        public static void WriteConnectionOpenError(this DiagnosticListener @this, NpgsqlConnection sqlConnection, Exception ex)
        {
            if (@this.IsEnabled(NpgsqlOpenConnectionError))
            {
                @this.Write(
                    NpgsqlOpenConnectionError,
                    new
                    {
                        Connection = sqlConnection,
                        Exception = ex
                    });
            }
        }

        public static void CloseConnectionStart(this DiagnosticListener @this, NpgsqlConnection connection)
        {
            if (@this.IsEnabled(NpgsqlCloseConnectionStart))
            {

                @this.Write(NpgsqlCloseConnectionStart, connection);
            }
        }

        public static void CloseConnectionStop(this DiagnosticListener @this, NpgsqlConnection connection)
        {
            if (@this.IsEnabled(NpgsqlCloseConnectionStop))
            {
                @this.Write(NpgsqlCloseConnectionStop, connection);
            }
        }

        public static void CloseConnectionError(this DiagnosticListener @this, NpgsqlConnection connection, Exception ex)
        {
            if (@this.IsEnabled(NpgsqlCloseConnectionError))
            {
                @this.Write(
                    NpgsqlCloseConnectionError,
                    new
                    {
                        Connection = connection,
                        Exception = ex
                    });
            }
        }

        public static void CommitTransactionStart(this DiagnosticListener @this, NpgsqlTransaction transaction)
        {
            if (@this.IsEnabled(NpgsqlCommitTransactionStart))
            {
                @this.Write(NpgsqlCommitTransactionStart, transaction);
            }
        }

        public static void CommitTransactionStop(this DiagnosticListener @this, NpgsqlTransaction transaction)
        {
            if (@this.IsEnabled(NpgsqlCommitTransactionStop))
            {
                @this.Write(NpgsqlCommitTransactionStop, transaction);
            }
        }

        public static void CommitTransactionError(this DiagnosticListener @this, NpgsqlTransaction transaction, Exception ex)
        {
            if (@this.IsEnabled(NpgsqlCommitTransactionError))
            {
                @this.Write(
                    NpgsqlCommitTransactionError,
                    new
                    {
                        Transaction = transaction,
                        Exception = ex
                    });
            }
        }

        public static void RollbackTransactionStart(this DiagnosticListener @this, NpgsqlTransaction transaction)
        {
            if (@this.IsEnabled(NpgsqlRollbackTransactionStart))
            {
                @this.Write(NpgsqlRollbackTransactionStart, transaction);
            }
        }

        public static void RollbackTransactionStop(this DiagnosticListener @this, NpgsqlTransaction transaction)
        {
            if (@this.IsEnabled(NpgsqlRollbackTransactionStop))
            {
                @this.Write(NpgsqlRollbackTransactionStop, transaction);
            }
        }

        public static void RollbackTransactionError(this DiagnosticListener @this, NpgsqlTransaction transaction, Exception ex)
        {
            if (@this.IsEnabled(NpgsqlRollbackTransactionError))
            {
                @this.Write(
                    NpgsqlRollbackTransactionError,
                    new
                    {
                        Transaction = transaction,
                        Exception = ex
                    });
            }
        }
    }
}

