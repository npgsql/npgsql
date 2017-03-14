using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Npgsql.Logging
{
    #region Logging

    static class Log
    {
        internal static readonly ILogger Logger = NpgsqlLogManager.Logger;

        internal static bool IsEnabled(LogLevel level) => Logger.IsEnabled(level);

        // ReSharper disable InconsistentNaming

        // Connection
        static readonly Action<ILogger, Exception> _openingConnection = LoggerMessage.Define(LogLevel.Trace, NpgsqlEventId.OpeningConnection, "Opening connection...");
        static readonly Action<ILogger, Exception> _usingPgpassFile = LoggerMessage.Define(LogLevel.Trace, NpgsqlEventId.UsingPgpassFile, "Taking password from pgpass file");
        static readonly Action<ILogger, int, Exception> _connectionOpened = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.ConnectionOpened, "[{ConnectorId}] Connection opened");
        static readonly Action<ILogger, int, Exception> _closingConnection = LoggerMessage.Define<int>(LogLevel.Trace, NpgsqlEventId.ClosingConnection, "[{ConnectorId}] Closing connection...");
        static readonly Action<ILogger, int, Exception> _connectionClosed = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.ConnectionClosed, "[{ConnectorId}] Connection closed");
        static readonly Action<ILogger, int, Exception> _connectorClosing = LoggerMessage.Define<int>(LogLevel.Trace, NpgsqlEventId.ConnectorClosing, "[{ConnectorId}] Closing connector");
        static readonly Action<ILogger, int, string, int, Exception> _openedConnection = LoggerMessage.Define<int, string, int>(LogLevel.Trace, NpgsqlEventId.OpenedConnection, "[{ConnectorId}] Opened connection to {Host}:{Port}");
        static readonly Action<ILogger, Exception> _sslNegotiationSuccessful = LoggerMessage.Define(LogLevel.Trace, NpgsqlEventId.SslNegotiationSuccessful, "SSL negotiation successful");
        static readonly Action<ILogger, string, int, Exception> _socketConnected = LoggerMessage.Define<string, int>(LogLevel.Trace, NpgsqlEventId.SocketConnected, "Socket connected to {Host}:{Port}");
        static readonly Action<ILogger, EndPoint, Exception> _attemptingToConnectTo = LoggerMessage.Define<EndPoint>(LogLevel.Trace, NpgsqlEventId.AttemptingToConnectTo, "Attempting to connect to {Endpoint}");
        static readonly Action<ILogger, double, EndPoint, Exception> _timeoutConnecting = LoggerMessage.Define<double, EndPoint>(LogLevel.Trace, NpgsqlEventId.TimeoutConnecting, "Timeout after {Seconds} seconds when connecting to {Endpoint}");
        static readonly Action<ILogger, EndPoint, Exception> _failedToConnect = LoggerMessage.Define<EndPoint>(LogLevel.Trace, NpgsqlEventId.FailedToConnect, "Failed to connect to {Endpoint}");
        static readonly Action<ILogger, int, Exception> _authenticating = LoggerMessage.Define<int>(LogLevel.Trace, NpgsqlEventId.Authenticating, "[{ConnectorId}] Authenticating...");
        static readonly Action<ILogger, int, Exception> _keepalive = LoggerMessage.Define<int>(LogLevel.Trace, NpgsqlEventId.Keepalive, "[{ConnectorId}] Performed keepalive");
        static readonly Action<ILogger, int, string, Exception> _notice = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.Notice, "[{ConnectorId}] Received notice: {Notice}");

        // Command processing
        static readonly Action<ILogger, int, Exception> _cancel = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.Cancel, "[{ConnectorId}] Sending cancellation...");
        static readonly Action<ILogger, int, Exception> _cleanup = LoggerMessage.Define<int>(LogLevel.Trace, NpgsqlEventId.Cleanup, "[{ConnectorId}] Cleaning up connector");
        static readonly Action<ILogger, int, Exception> _startUserAction = LoggerMessage.Define<int>(LogLevel.Trace, NpgsqlEventId.StartUserAction, "[{ConnectorId}] Start user action");
        static readonly Action<ILogger, int, Exception> _endUserAction = LoggerMessage.Define<int>(LogLevel.Trace, NpgsqlEventId.EndUserAction, "[{ConnectorId}] End user action");
        static readonly Action<ILogger, int, string, Exception> _executingInternalCommand = LoggerMessage.Define<int, string>(LogLevel.Trace, NpgsqlEventId.ExecutingInternalCommand, "[{ConnectorId}] Executing internal command: {Message}");
        static readonly Action<ILogger, int, string, Exception> _preparing = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.Preparing, "[{ConnectorId}] Preparing: {Sql}");
        static readonly Action<ILogger, int, Exception> _readerCleanup = LoggerMessage.Define<int>(LogLevel.Trace, NpgsqlEventId.ReaderCleanup, "[{ConnectorId}] Cleaning up reader");
        static readonly Action<ILogger, int, string, Exception> _autoPrepareing = LoggerMessage.Define<int, string>(LogLevel.Trace, NpgsqlEventId.AutoPreparing, "[{ConnectorId}] Automatically preparing statement: {Sql}");
        static readonly Action<ILogger, int, Exception> _closingCommandPreparedStatements = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.ClosingCommandPreparedStatements, "[{ConnectorId}] Closing command's prepared statements");

        // Transactions
        static readonly Action<ILogger, int, IsolationLevel, Exception> _beginningTransaction = LoggerMessage.Define<int, IsolationLevel>(LogLevel.Debug, NpgsqlEventId.BeginningTransaction, "[{ConnectorId}] Beginning transaction with isolation level {IsolationLevel}");
        static readonly Action<ILogger, int, Exception> _committing = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.Committing, "[{ConnectorId}] Committing transaction");
        static readonly Action<ILogger, int, Exception> _rollingBack = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.RollingBack, "[{ConnectorId}] Rolling back transaction");
        static readonly Action<ILogger, int, string, Exception> _creatingSavepoint = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.CreatingSavepoint, "[{ConnectorId}] Creating savepoint {Savepoint}");
        static readonly Action<ILogger, int, string, Exception> _rollingBackSavepoint = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.RollingBackSavepoint, "[{ConnectorId}] Rolling back savepoint {Savepoint}");
        static readonly Action<ILogger, int, string, Exception> _releasingSavepoint = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.ReleasingSavepoint, "[{ConnectorId}] Releasing savepoint {Savepoint}");

        // System.Transactions
        static readonly Action<ILogger, int, string, Exception> _enlisted = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.Enlisted, "[{ConnectorId}] Enlisted volatile resource manager (localid={TransactionId})");
        static readonly Action<ILogger, int, string, Exception> _singlePhaseCommit = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.SinglePhaseCommit, "[{ConnectorId}] Single Phase Commit (localid={TransactionId})");
        static readonly Action<ILogger, int, string, Exception> _twoPhasePrepare = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.Preparing, "[{ConnectorId}] Two-phase transaction prepare (localid={TransactionId}");
        static readonly Action<ILogger, int, string, Exception> _twoPhaseCommit = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.TwoPhaseCommit, "[{ConnectorId}] Two-phase transaction commit (localid={TransactionId}");
        static readonly Action<ILogger, int, string, Exception> _twoPhaseRollback = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.TwoPhaseRollback, "[{ConnectorId}] Two-phase transaction rollback (localid={TransactionId}");
        static readonly Action<ILogger, int, string, Exception> _singlePhaseRollback = LoggerMessage.Define<int, string>(LogLevel.Debug, NpgsqlEventId.SinglePhaseRollback, "[{ConnectorId}] Single-phase transaction rollback (localid={TransactionId}");
        static readonly Action<ILogger, int, string, Exception> _transactionInDoubt = LoggerMessage.Define<int, string>(LogLevel.Warning, NpgsqlEventId.TransactionInDoubt, "[{ConnectorId}] Two-phase transaction in doubt (localid={TransactionId}");
        static readonly Action<ILogger, int, string, Exception> _cleaningUpResourceManager = LoggerMessage.Define<int, string>(LogLevel.Trace, NpgsqlEventId.CleaningUpResourceManager, "[{ConnectorId}] Cleaning up resource manager (localid={TransactionId}");

        // Wait
        static readonly Action<ILogger, int, int, Exception> _startingSyncWait = LoggerMessage.Define<int, int>(LogLevel.Debug, NpgsqlEventId.StartingSyncWait, "[{ConnectorId}] Starting to wait (timeout={timeout})...");
        static readonly Action<ILogger, int, Exception> _startingAsyncWait = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.StartingAsyncWait, "[{ConnectorId}] Starting to wait asynchronously...");

        // COPY
        static readonly Action<ILogger, int, Exception> _startingBinaryImport = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.StartingBinaryImport, "[{ConnectorId}] Starting binary import");
        static readonly Action<ILogger, int, Exception> _startingBinaryExport = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.StartingBinaryExport, "[{ConnectorId}] Starting binary export");
        static readonly Action<ILogger, int, Exception> _startingTextImport = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.StartingTextImport, "[{ConnectorId}] Starting text import");
        static readonly Action<ILogger, int, Exception> _startingTextExport = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.StartingTextExport, "[{ConnectorId}] Starting text export");
        static readonly Action<ILogger, int, Exception> _startingRawCopy = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.StartingRawCopy, "[{ConnectorId}] Starting raw COPY operation");
        static readonly Action<ILogger, int, Exception> _endCopy = LoggerMessage.Define<int>(LogLevel.Debug, NpgsqlEventId.EndCopy, "[{ConnectorId}] COPY operation ended");

        // ReSharper restore InconsistentNaming

        // Connection
        internal static void OpeningConnection() => _openingConnection(Logger, null);
        internal static void UsingPgpassFile() => _usingPgpassFile(Logger, null);
        internal static void ConnectionOpened(int connectorId) => _connectionOpened(Logger, connectorId, null);
        internal static void ClosingConnection(int connectorId) => _closingConnection(Logger, connectorId, null);
        internal static void ConnectionClosed(int connectorId) => _connectionClosed(Logger, connectorId, null);
        internal static void ConnectorClosing(int connectorId) => _connectorClosing(Logger, connectorId, null);
        internal static void OpenedConnection(int connectorId, string host, int port) => _openedConnection(Logger, connectorId, host, port, null);
        internal static void SslNegotiationSuccessful() => _sslNegotiationSuccessful(Logger, null);
        internal static void SocketConnected(string host, int port) => _socketConnected(Logger, host, port, null);
        internal static void AttemptingToConnectTo(EndPoint ip) => _attemptingToConnectTo(Logger, ip, null);
        internal static void TimeoutConnecting(double seconds, EndPoint ep) => _timeoutConnecting(Logger, seconds, ep, null);
        internal static void FailedToConnect(EndPoint ep) => _failedToConnect(Logger, ep, null);
        internal static void Authenticating(int connectorId) => _authenticating(Logger, connectorId, null);
        internal static void Keepalive(int connectorId) => _keepalive(Logger, connectorId, null);
        internal static void Notice(int connectorId, PostgresNotice notice) => _notice(Logger, connectorId, notice.MessageText, null);

        // Command processing
        internal static void Cancel(int connectorId) => _cancel(Logger, connectorId, null);
        internal static void Cleanup(int connectorId) => _cleanup(Logger, connectorId, null);
        internal static void StartUserAction(int connectorId) => _startUserAction(Logger, connectorId, null);
        internal static void EndUserAction(int connectorId) => _endUserAction(Logger, connectorId, null);
        internal static void ExecutingInternalCommand(int connectorId, FrontendMessage message) => _executingInternalCommand(Logger, connectorId, message.ToString(), null);
        internal static void Preparing(int connectorId, string sql) => _preparing(Logger, connectorId, sql, null);
        internal static void ReaderCleanup(int connectorId) => _readerCleanup(Logger, connectorId, null);
        internal static void AutoPreparing(int connectorId, string sql) => _autoPrepareing(Logger, connectorId, sql, null);
        internal static void ClosingCommandPreparedStatements(int connectorId) => _closingCommandPreparedStatements(Logger, connectorId, null);

        // Transactions
        internal static void BeginningTransaction(int connectorId, IsolationLevel isolationLevel) => _beginningTransaction(Logger, connectorId, isolationLevel, null);
        internal static void Committing(int connectorId) => _committing(Logger, connectorId, null);
        internal static void RollingBack(int connectorId) => _rollingBack(Logger, connectorId, null);
        internal static void CreatingSavepoint(int connectorId, string savepoint) => _creatingSavepoint(Logger, connectorId, savepoint, null);
        internal static void RollingBackSavepoint(int connectorId, string savepoint) => _rollingBackSavepoint(Logger, connectorId, savepoint, null);
        internal static void ReleasingSavepoint(int connectorId, string savepoint) => _releasingSavepoint(Logger, connectorId, savepoint, null);

        // System.Transactions
        internal static void Enlisted(int connectorId, string transactionId) => _enlisted(Logger, connectorId, transactionId, null);
        internal static void SinglePhaseCommit(int connectorId, string transactionId) => _singlePhaseCommit(Logger, connectorId, transactionId, null);
        internal static void TwoPhasePrepare(int connectorId, string transactionId) => _twoPhasePrepare(Logger, connectorId, transactionId, null);
        internal static void TwoPhaseCommit(int connectorId, string transactionId) => _twoPhaseCommit(Logger, connectorId, transactionId, null);
        internal static void TwoPhaseRollback(int connectorId, string transactionId) => _twoPhaseRollback(Logger, connectorId, transactionId, null);
        internal static void SinglePhaseRollback(int connectorId, string transactionId) => _singlePhaseRollback(Logger, connectorId, transactionId, null);
        internal static void TransactionInDoubt(int connectorId, string transactionId) => _transactionInDoubt(Logger, connectorId, transactionId, null);
        internal static void CleaningUpResourceManager(int connectorId, string transactionId) => _cleaningUpResourceManager(Logger, connectorId, transactionId, null);

        // Wait
        internal static void StartingSyncWait(int connectorId, int timeout) => _startingSyncWait(Logger, connectorId, timeout, null);
        internal static void StartingAsyncWait(int connectorId) => _startingAsyncWait(Logger, connectorId, null);

        // COPY
        internal static void StartingBinaryImport(int connectorId) => _startingBinaryImport(Logger, connectorId, null);
        internal static void StartingBinaryExport(int connectorId) => _startingBinaryExport(Logger, connectorId, null);
        internal static void StartingTextImport(int connectorId) => _startingTextImport(Logger, connectorId, null);
        internal static void StartingTextExport(int connectorId) => _startingTextExport(Logger, connectorId, null);
        internal static void StartingRawCopy(int connectorId) => _startingRawCopy(Logger, connectorId, null);
        internal static void EndCopy(int connectorId) => _endCopy(Logger, connectorId, null);

        internal static void ExecuteCommand(int connectorId, NpgsqlCommand command)
        {
            if (!Logger.IsEnabled(LogLevel.Debug))
                return;

            Logger.Log(LogLevel.Debug, NpgsqlEventId.ExecuteCommand, new CommandLogWrapper(connectorId, command), null, CommandLogWrapper.ToLogMessage);
        }

        // We use this to emit a specially-crafted textual log message while retaining the structured data we want,
        // without the two corresponding.
        struct CommandLogWrapper : IEnumerable<KeyValuePair<string,object>>
        {
            readonly int _connectorId;
            readonly NpgsqlCommand _command;

            internal CommandLogWrapper(int connectorId, NpgsqlCommand command)
            {
                _connectorId = connectorId;
                _command = command;
            }

            internal static string ToLogMessage(CommandLogWrapper w, Exception e)
            {
                var sb = new StringBuilder();
                sb
                    .Append('[')
                    .Append(w._connectorId)
                    .Append("] Executing statement(s):");
                foreach (var s in w._command.Statements)
                    sb.AppendLine().Append("\t").Append(s.SQL);

                var parameters = w._command.Parameters;
                if (NpgsqlLogManager.IsParameterLoggingEnabled && parameters.Count > 0)
                {
                    sb.AppendLine().AppendLine("Parameters:");
                    for (var i = 0; i < parameters.Count; i++)
                        sb.Append("\t$").Append(i + 1).Append(": ").Append(Convert.ToString(parameters[i].Value, CultureInfo.InvariantCulture));
                }
                return sb.ToString();
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                yield return new KeyValuePair<string, object>("ConnectorId", _connectorId);
                yield return new KeyValuePair<string, object>("CommandText", _command.Statements.Select(s => s.SQL).Join("; "));
                yield return new KeyValuePair<string, object>("CommandTimeout", _command.CommandTimeout);
                yield return new KeyValuePair<string, object>("Parameters", _command.Parameters);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }

    #endregion
}
