using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Data;

namespace Npgsql
{
    internal static class NpgsqlDiagnosticListenerExtensions
    {
        public const string DiagnosticListenerName = "NpgsqlDiagnosticListener";

        const string NpgsqlClientPrefix = "Npgsql.";

        public const string NpgsqlBeforeExecuteCommand = NpgsqlClientPrefix + nameof(WriteCommandBefore);
        public const string NpgsqlAfterExecuteCommand = NpgsqlClientPrefix + nameof(WriteCommandAfter);
        public const string NpgsqlErrorExecuteCommand = NpgsqlClientPrefix + nameof(WriteCommandError);

        public const string NpgsqlBeforeOpenConnection = NpgsqlClientPrefix + nameof(WriteConnectionOpenBefore);
        public const string NpgsqlAfterOpenConnection = NpgsqlClientPrefix + nameof(WriteConnectionOpenAfter);
        public const string NpgsqlErrorOpenConnection = NpgsqlClientPrefix + nameof(WriteConnectionOpenError);

        public const string NpgsqlBeforeCloseConnection = NpgsqlClientPrefix + nameof(WriteConnectionCloseBefore);
        public const string NpgsqlAfterCloseConnection = NpgsqlClientPrefix + nameof(WriteConnectionCloseAfter);
        public const string NpgsqlErrorCloseConnection = NpgsqlClientPrefix + nameof(WriteConnectionCloseError);

        public const string NpgsqlBeforeCommitTransaction = NpgsqlClientPrefix + nameof(WriteTransactionCommitBefore);
        public const string NpgsqlAfterCommitTransaction = NpgsqlClientPrefix + nameof(WriteTransactionCommitAfter);
        public const string NpgsqlErrorCommitTransaction = NpgsqlClientPrefix + nameof(WriteTransactionCommitError);

        public const string NpgsqlBeforeRollbackTransaction = NpgsqlClientPrefix + nameof(WriteTransactionRollbackBefore);
        public const string NpgsqlAfterRollbackTransaction = NpgsqlClientPrefix + nameof(WriteTransactionRollbackAfter);
        public const string NpgsqlErrorRollbackTransaction = NpgsqlClientPrefix + nameof(WriteTransactionRollbackError);

        public static Guid WriteCommandBefore(this DiagnosticListener @this, NpgsqlCommand command, [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(NpgsqlBeforeExecuteCommand))
            {
                return Guid.Empty;
            }

            var operationId = Guid.NewGuid();

            @this.Write(
                NpgsqlBeforeExecuteCommand,
                new
                {
                    OperationId = operationId,
                    Operation = operation,
                    ConnectionId = command.Connection?.Connector?.Id,
                    Command = command
                });

            return operationId;
        }

        public static void WriteCommandAfter(this DiagnosticListener @this, Guid operationId, NpgsqlCommand command, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlAfterExecuteCommand))
            {
                @this.Write(
                    NpgsqlAfterExecuteCommand,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        ConnectionId = command.Connection?.Connector?.Id,
                        Command = command,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static void WriteCommandError(this DiagnosticListener @this, Guid operationId, NpgsqlCommand command, Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlErrorExecuteCommand))
            {
                @this.Write(
                    NpgsqlErrorExecuteCommand,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        ConnectionId = command.Connection?.Connector?.Id,
                        Command = command,
                        Exception = ex,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static Guid WriteConnectionOpenBefore(this DiagnosticListener @this, NpgsqlConnection connection, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlBeforeOpenConnection))
            {
                Guid operationId = Guid.NewGuid();

                @this.Write(
                    NpgsqlBeforeOpenConnection,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        Connection = connection,
                        Timestamp = Stopwatch.GetTimestamp()
                    });

                return operationId;
            }
            else
                return Guid.Empty;
        }

        public static void WriteConnectionOpenAfter(this DiagnosticListener @this, Guid operationId, NpgsqlConnection connection, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlAfterOpenConnection))
            {
                @this.Write(
                    NpgsqlAfterOpenConnection,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        ConnectionId = connection.Connector.Id,
                        Connection = connection,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static void WriteConnectionOpenError(this DiagnosticListener @this, Guid operationId, NpgsqlConnection sqlConnection, Exception ex,
            [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlErrorOpenConnection))
            {
                @this.Write(
                    NpgsqlErrorOpenConnection,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        ConnectionId = sqlConnection.Connector.Id,
                        Connection = sqlConnection,
                        Exception = ex,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static Guid WriteConnectionCloseBefore(this DiagnosticListener @this, NpgsqlConnection connection, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlBeforeCloseConnection))
            {
                Guid operationId = Guid.NewGuid();

                @this.Write(
                    NpgsqlBeforeCloseConnection,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        ConnectionId = connection.Connector.Id,
                        Connection = connection,
                        Timestamp = Stopwatch.GetTimestamp()
                    });

                return operationId;
            }
            else
                return Guid.Empty;
        }

        public static void WriteConnectionCloseAfter(this DiagnosticListener @this, Guid operationId, NpgsqlConnection connection, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlAfterCloseConnection))
            {
                @this.Write(
                    NpgsqlAfterCloseConnection,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        Connection = connection,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static void WriteConnectionCloseError(this DiagnosticListener @this, Guid operationId, NpgsqlConnection connection, Exception ex,
            [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlErrorCloseConnection))
            {
                @this.Write(
                    NpgsqlErrorCloseConnection,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        Connection = connection,
                        Exception = ex,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static Guid WriteTransactionCommitBefore(this DiagnosticListener @this, IsolationLevel isolationLevel, NpgsqlConnection connection,
            [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlBeforeCommitTransaction))
            {
                Guid operationId = Guid.NewGuid();

                @this.Write(
                    NpgsqlBeforeCommitTransaction,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        IsolationLevel = isolationLevel,
                        Connection = connection,
                        Timestamp = Stopwatch.GetTimestamp()
                    });

                return operationId;
            }
            else
                return Guid.Empty;
        }

        public static void WriteTransactionCommitAfter(this DiagnosticListener @this, Guid operationId, IsolationLevel isolationLevel, NpgsqlConnection connection,
            [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlAfterCommitTransaction))
            {
                @this.Write(
                    NpgsqlAfterCommitTransaction,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        IsolationLevel = isolationLevel,
                        Connection = connection,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static void WriteTransactionCommitError(this DiagnosticListener @this, Guid operationId, IsolationLevel isolationLevel, NpgsqlConnection connection, Exception ex,
            [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlErrorCommitTransaction))
            {
                @this.Write(
                    NpgsqlErrorCommitTransaction,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        IsolationLevel = isolationLevel,
                        Connection = connection,
                        Exception = ex,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static Guid WriteTransactionRollbackBefore(this DiagnosticListener @this, IsolationLevel isolationLevel, NpgsqlConnection connection, string transactionName,
            [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlBeforeRollbackTransaction))
            {
                Guid operationId = Guid.NewGuid();

                @this.Write(
                    NpgsqlBeforeRollbackTransaction,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        IsolationLevel = isolationLevel,
                        Connection = connection,
                        TransactionName = transactionName,
                        Timestamp = Stopwatch.GetTimestamp()
                    });

                return operationId;
            }
            else
                return Guid.Empty;
        }

        public static void WriteTransactionRollbackAfter(this DiagnosticListener @this, Guid operationId, IsolationLevel isolationLevel, NpgsqlConnection connection,
            string transactionName, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlAfterRollbackTransaction))
            {
                @this.Write(
                    NpgsqlAfterRollbackTransaction,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        IsolationLevel = isolationLevel,
                        Connection = connection,
                        TransactionName = transactionName,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }

        public static void WriteTransactionRollbackError(this DiagnosticListener @this, Guid operationId, IsolationLevel isolationLevel, NpgsqlConnection connection,
            string transactionName, Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(NpgsqlErrorRollbackTransaction))
            {
                @this.Write(
                    NpgsqlErrorRollbackTransaction,
                    new
                    {
                        OperationId = operationId,
                        Operation = operation,
                        IsolationLevel = isolationLevel,
                        Connection = connection,
                        TransactionName = transactionName,
                        Exception = ex,
                        Timestamp = Stopwatch.GetTimestamp()
                    });
            }
        }
    }
}

