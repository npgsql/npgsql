using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;

namespace Npgsql;

// ReSharper disable InconsistentNaming
#pragma warning disable SYSLIB1015 // Argument is not referenced from the logging message
#pragma warning disable SYSLIB1006 // Multiple logging methods are using event id

static partial class LogMessages
{
    #region Connection

    [LoggerMessage(
        EventId = NpgsqlEventId.OpeningConnection,
        Level = LogLevel.Trace,
        Message = "Opening connection to {Host}:{Port}/{Database}...")]
    internal static partial void OpeningConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString);

    [LoggerMessage(
        EventId = NpgsqlEventId.OpenedConnection,
        Level = LogLevel.Debug,
        Message = "Opened connection to {Host}:{Port}/{Database}")]
    internal static partial void OpenedConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.OpenedConnection,
        Level = LogLevel.Debug,
        Message = "Opened multiplexing connection to {Host}:{Port}/{Database}")]
    internal static partial void OpenedMultiplexingConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString);

    [LoggerMessage(
        EventId = NpgsqlEventId.ClosingConnection,
        Level = LogLevel.Trace,
        Message = "Closing connection to {Host}:{Port}/{Database}...")]
    internal static partial void ClosingConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ClosedConnection,
        Level = LogLevel.Debug,
        Message = "Closed connection to {Host}:{Port}/{Database}")]
    internal static partial void ClosedConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ClosedConnection,
        Level = LogLevel.Debug,
        Message = "Closed multiplexing connection to {Host}:{Port}/{Database}")]
    internal static partial void ClosedMultiplexingConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString);

    [LoggerMessage(
        EventId = NpgsqlEventId.OpeningPhysicalConnection,
        Level = LogLevel.Trace,
        Message = "Opening physical connection to {Host}:{Port}/{Database}...")]
    internal static partial void OpeningPhysicalConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString);

    [LoggerMessage(
        EventId = NpgsqlEventId.OpenedPhysicalConnection,
        Level = LogLevel.Debug,
        Message = "Opened physical connection to {Host}:{Port}/{Database} (in {DurationMs}ms)")]
    internal static partial void OpenedPhysicalConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString, long DurationMs, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ClosingPhysicalConnection,
        Level = LogLevel.Trace,
        Message = "Closing physical connection to {Host}:{Port}/{Database}...")]
    internal static partial void ClosingPhysicalConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ClosedPhysicalConnection,
        Level = LogLevel.Debug,
        Message = "Closed physical connection to {Host}:{Port}/{Database}")]
    internal static partial void ClosedPhysicalConnection(ILogger logger, string Host, int Port, string Database, string ConnectionString, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.StartingWait,
        Level = LogLevel.Information,
        Message = "Starting to wait (timeout={TimeoutMs}ms)...")]
    internal static partial void StartingWait(ILogger logger, int TimeoutMs, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ReceivedNotice,
        Level = LogLevel.Debug,
        Message = "Received notice: {NoticeText}")]
    internal static partial void ReceivedNotice(ILogger logger, string NoticeText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ConnectionExceededMaximumLifetime,
        Level = LogLevel.Debug,
        Message = "Connection has exceeded its maximum lifetime ('{ConnectionMaximumLifeTime}') and will be closed.")]
    internal static partial void ConnectionExceededMaximumLifetime(ILogger logger, TimeSpan ConnectionMaximumLifeTime, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.SendingKeepalive,
        Level = LogLevel.Trace,
        Message = "Sending keepalive...")]
    internal static partial void SendingKeepalive(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CompletedKeepalive,
        Level = LogLevel.Trace,
        Message = "Completed keepalive")]
    internal static partial void CompletedKeepalive(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.KeepaliveFailed,
        Level = LogLevel.Trace,
        Message = "Keepalive failed")]
    internal static partial void KeepaliveFailed(ILogger logger, int ConnectorId, Exception exception);

    [LoggerMessage(
        EventId = NpgsqlEventId.BreakingConnection,
        Level = LogLevel.Trace,
        Message = "Breaking connection")]
    internal static partial void BreakingConnection(ILogger logger, int ConnectorId, Exception exception);

    [LoggerMessage(
        EventId = NpgsqlEventId.CaughtUserExceptionInNoticeEventHandler,
        Level = LogLevel.Error,
        Message = "User exception caught when emitting notice event")]
    internal static partial void CaughtUserExceptionInNoticeEventHandler(ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = NpgsqlEventId.CaughtUserExceptionInNotificationEventHandler,
        Level = LogLevel.Error,
        Message = "User exception caught when emitting notification event")]
    internal static partial void CaughtUserExceptionInNotificationEventHandler(ILogger logger, Exception exception);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExceptionWhenClosingPhysicalConnection,
        Level = LogLevel.Warning,
        Message = "Exception while closing connector")]
    internal static partial void ExceptionWhenClosingPhysicalConnection(ILogger logger, int ConnectorId, Exception exception);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExceptionWhenOpeningConnectionForMultiplexing,
        Level = LogLevel.Error,
        Message = "Exception opening a connection for multiplexing")]
    internal static partial void ExceptionWhenOpeningConnectionForMultiplexing(ILogger logger, Exception exception);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Start user action")]
    internal static partial void StartUserAction(ILogger logger, int ConnectorId);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "End user action")]
    internal static partial void EndUserAction(ILogger logger, int ConnectorId);

    #endregion Connection

    #region Command

    [LoggerMessage(
        EventId = NpgsqlEventId.ExecutingCommand,
        Level = LogLevel.Debug,
        Message = "Executing command: {CommandText}",
        SkipEnabledCheck = true)]
    internal static partial void ExecutingCommand(ILogger logger, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExecutingCommand,
        Level = LogLevel.Debug,
        Message = "Executing command: {CommandText}\n  Parameters: {Parameters}",
        SkipEnabledCheck = true)]
    internal static partial void ExecutingCommandWithParameters(ILogger logger, string CommandText, IEnumerable<object> Parameters, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExecutingCommand,
        Level = LogLevel.Debug,
        Message = "Executing batch: {BatchCommands}",
        SkipEnabledCheck = true)]
    internal static partial void ExecutingBatch(ILogger logger, string[] BatchCommands, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExecutingCommand,
        Level = LogLevel.Debug,
        Message = "Executing batch: {BatchCommands}",
        SkipEnabledCheck = true)]
    internal static partial void ExecutingBatchWithParameters(ILogger logger, (string CommandText, object[] Parameters)[] BatchCommands, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CommandExecutionCompleted,
        Level = LogLevel.Information,
        Message = "Command execution completed (duration={DurationMs}ms): {CommandText}",
        SkipEnabledCheck = true)]
    internal static partial void CommandExecutionCompleted(ILogger logger, string CommandText, long DurationMs, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CommandExecutionCompleted,
        Level = LogLevel.Information,
        Message = "Command execution completed (duration={DurationMs}ms): {CommandText}\n  Parameters: {Parameters}",
        SkipEnabledCheck = true)]
    internal static partial void CommandExecutionCompletedWithParameters(ILogger logger, string CommandText, IEnumerable<object> Parameters, long DurationMs, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CommandExecutionCompleted,
        Level = LogLevel.Information,
        Message = "Batch execution completed (duration={DurationMs}ms): {BatchCommands}",
        SkipEnabledCheck = true)]
    internal static partial void BatchExecutionCompleted(ILogger logger, string[] BatchCommands, long DurationMs, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CommandExecutionCompleted,
        Level = LogLevel.Information,
        Message = "Batch execution completed (duration={DurationMs}ms): {BatchCommands}",
        SkipEnabledCheck = true)]
    internal static partial void BatchExecutionCompletedWithParameters(
        ILogger logger, (string CommandText, object[] Parameters)[] BatchCommands, long DurationMs, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CancellingCommand,
        Level = LogLevel.Debug,
        Message = "Sending PostgreSQL cancellation...")]
    internal static partial void CancellingCommand(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExecutingInternalCommand,
        Level = LogLevel.Debug,
        Message = "Executing internal command: {CommandText}")]
    internal static partial void ExecutingInternalCommand(ILogger logger, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.PreparingCommandExplicitly,
        Level = LogLevel.Debug,
        Message = "Preparing command explicitly: {CommandText}",
        SkipEnabledCheck = true)]
    internal static partial void PreparingCommandExplicitly(ILogger logger, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CommandPreparedExplicitly,
        Level = LogLevel.Information,
        Message = "Prepared command explicitly")]
    internal static partial void CommandPreparedExplicitly(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.AutoPreparingStatement,
        Level = LogLevel.Debug,
        Message = "Auto-preparing statement: {CommandText}")]
    internal static partial void AutoPreparingStatement(ILogger logger, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.UnpreparingCommand,
        Level = LogLevel.Debug,
        Message = "Prepared command explicitly")]
    internal static partial void UnpreparingCommand(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.DerivingParameters,
        Level = LogLevel.Debug,
        Message = "Deriving Parameters for query: {CommandText}")]
    internal static partial void DerivingParameters(ILogger logger, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExceptionWhenWritingMultiplexedCommands,
        Level = LogLevel.Error,
        Message = "Exception while writing multiplexed commands")]
    internal static partial void ExceptionWhenWritingMultiplexedCommands(ILogger logger, int ConnectorId, Exception exception);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Cleaning up reader")]
    internal static partial void ReaderCleanup(ILogger logger, int ConnectorId);

    #endregion Command

    #region Transaction

    [LoggerMessage(
        EventId = NpgsqlEventId.StartedTransaction,
        Level = LogLevel.Debug,
        Message = "Starting transaction")]
    internal static partial void StartedTransaction(ILogger logger, IsolationLevel IsolationLevel, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CommittedTransaction,
        Level = LogLevel.Debug,
        Message = "Committed transaction")]
    internal static partial void CommittedTransaction(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.RolledBackTransaction,
        Level = LogLevel.Debug,
        Message = "Rolled back transaction")]
    internal static partial void RolledBackTransaction(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CreatingSavepoint,
        Level = LogLevel.Debug,
        Message = "Creating savepoint '{SavepointName}'")]
    internal static partial void CreatingSavepoint(ILogger logger, string SavepointName, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.RolledBackToSavepoint,
        Level = LogLevel.Debug,
        Message = "Rolled back to savepoint '{SavepointName}'")]
    internal static partial void RolledBackToSavepoint(ILogger logger, string SavepointName, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ReleasedSavepoint,
        Level = LogLevel.Debug,
        Message = "Released savepoint '{SavepointName}'")]
    internal static partial void ReleasedSavepoint(ILogger logger, string SavepointName, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExceptionDuringTransactionDispose,
        Level = LogLevel.Error,
        Message = "Exception while disposing transaction")]
    internal static partial void ExceptionDuringTransactionDispose(ILogger logger, int ConnectorId, Exception exception);

    [LoggerMessage(
        EventId = NpgsqlEventId.EnlistedVolatileResourceManager,
        Level = LogLevel.Debug,
        Message = "Enlisted volatile resource manager (local transaction ID={LocalTransactionIdentifier})")]
    internal static partial void EnlistedVolatileResourceManager(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CommittingSinglePhaseTransaction,
        Level = LogLevel.Debug,
        Message = "Committing single-phase transaction (local ID={LocalTransactionIdentifier})")]
    internal static partial void CommittingSinglePhaseTransaction(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.RollingBackSinglePhaseTransaction,
        Level = LogLevel.Debug,
        Message = "Rolling back single-phase transaction (local ID={LocalTransactionIdentifier})")]
    internal static partial void RollingBackSinglePhaseTransaction(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.SinglePhaseTransactionRollbackFailed,
        Level = LogLevel.Error,
        Message = "Exception during single-phase transaction rollback (local ID={LocalTransactionIdentifier})")]
    internal static partial void SinglePhaseTransactionRollbackFailed(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.PreparingTwoPhaseTransaction,
        Level = LogLevel.Debug,
        Message = "Preparing two-phase transaction (local ID={LocalTransactionIdentifier})")]
    internal static partial void PreparingTwoPhaseTransaction(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CommittingTwoPhaseTransaction,
        Level = LogLevel.Debug,
        Message = "Committing two-phase transaction (local ID={LocalTransactionIdentifier})")]
    internal static partial void CommittingTwoPhaseTransaction(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.TwoPhaseTransactionCommitFailed,
        Level = LogLevel.Error,
        Message = "Exception during two-phase transaction commit (local ID={LocalTransactionIdentifier})")]
    internal static partial void TwoPhaseTransactionCommitFailed(ILogger logger, string LocalTransactionIdentifier, int ConnectorId, Exception exception);

    [LoggerMessage(
        EventId = NpgsqlEventId.RollingBackTwoPhaseTransaction,
        Level = LogLevel.Debug,
        Message = "Rolling back two-phase transaction (local ID={LocalTransactionIdentifier})")]
    internal static partial void RollingBackTwoPhaseTransaction(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.TwoPhaseTransactionRollbackFailed,
        Level = LogLevel.Debug,
        Message = "Exception during two-phase transaction rollback (local ID={LocalTransactionIdentifier})")]
    internal static partial void TwoPhaseTransactionRollbackFailed(ILogger logger, string LocalTransactionIdentifier, int ConnectorId, Exception exception);

    [LoggerMessage(
        EventId = NpgsqlEventId.TwoPhaseTransactionInDoubt,
        Level = LogLevel.Warning,
        Message = "Two-phase transaction in doubt (local ID={LocalTransactionIdentifier})")]
    internal static partial void TwoPhaseTransactionInDoubt(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ConnectionInUseWhenRollingBack,
        Level = LogLevel.Warning,
        Message = "Connection in use while trying to rollback, will cancel and retry (local ID={LocalTransactionIdentifier}")]
    internal static partial void ConnectionInUseWhenRollingBack(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CleaningUpResourceManager,
        Level = LogLevel.Trace,
        Message = "Cleaning up resource manager (local ID={LocalTransactionIdentifier})")]
    internal static partial void CleaningUpResourceManager(ILogger logger, string LocalTransactionIdentifier, int ConnectorId);

    #endregion Transaction

    #region Copy

    [LoggerMessage(
        EventId = NpgsqlEventId.StartingBinaryExport,
        Level = LogLevel.Information,
        Message = "Starting binary export")]
    internal static partial void StartingBinaryExport(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.StartingBinaryImport,
        Level = LogLevel.Information,
        Message = "Starting binary import")]
    internal static partial void StartingBinaryImport(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.StartingTextExport,
        Level = LogLevel.Information,
        Message = "Starting text export")]
    internal static partial void StartingTextExport(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.StartingTextImport,
        Level = LogLevel.Information,
        Message = "Starting text import")]
    internal static partial void StartingTextImport(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.StartingRawCopy,
        Level = LogLevel.Information,
        Message = "Starting raw COPY operation")]
    internal static partial void StartingRawCopy(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CopyOperationCompleted,
        Level = LogLevel.Information,
        Message = "Binary COPY operation completed ({Rows} rows transferred)")]
    internal static partial void BinaryCopyOperationCompleted(ILogger logger, ulong Rows, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CopyOperationCompleted,
        Level = LogLevel.Information,
        Message = "COPY operation completed")]
    internal static partial void CopyOperationCompleted(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.CopyOperationCancelled,
        Level = LogLevel.Information,
        Message = "COPY operation was cancelled")]
    internal static partial void CopyOperationCancelled(ILogger logger, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExceptionWhenDisposingCopyOperation,
        Level = LogLevel.Debug,
        Message = "Exception when disposing a COPY operation")]
    internal static partial void ExceptionWhenDisposingCopyOperation(ILogger logger, int ConnectorId, Exception exception);

    #endregion Copy

    #region Replication

    [LoggerMessage(
        EventId = NpgsqlEventId.CreatingReplicationSlot,
        Level = LogLevel.Information,
        Message = "Creating replication slot '{SlotName}'")]
    internal static partial void CreatingReplicationSlot(ILogger logger, string SlotName, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.DroppingReplicationSlot,
        Level = LogLevel.Information,
        Message = "Dropping replication slot '{SlotName}'")]
    internal static partial void DroppingReplicationSlot(ILogger logger, string SlotName, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.StartingLogicalReplication,
        Level = LogLevel.Information,
        Message = "Starting logical replication on slot '{SlotName}'")]
    internal static partial void StartingLogicalReplication(ILogger logger, string SlotName, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.StartingPhysicalReplication,
        Level = LogLevel.Information,
        Message = "Starting physical replication on slot: '{SlotName}'")]
    internal static partial void StartingPhysicalReplication(ILogger logger, string? SlotName, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ExecutingReplicationCommand,
        Level = LogLevel.Debug,
        Message = "Executing replication command: {CommandText}")]
    internal static partial void ExecutingReplicationCommand(ILogger logger, string CommandText, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ReceivedReplicationPrimaryKeepalive,
        Level = LogLevel.Trace,
        Message = "Received replication primary keepalive message from the server with current end of WAL of {EndLsn} and timestamp of {Timestamp}")]
    internal static partial void ReceivedReplicationPrimaryKeepalive(ILogger logger, NpgsqlLogSequenceNumber EndLsn, DateTime Timestamp, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.SendingReplicationStandbyStatusUpdate,
        Level = LogLevel.Trace,
        Message = "Sending a replication standby status update because {Reason}")]
    internal static partial void SendingReplicationStandbyStatusUpdate(ILogger logger, string Reason, int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.SentReplicationFeedbackMessage,
        Level = LogLevel.Trace,
        Message = "Feedback message sent with LastReceivedLsn={LastReceivedLsn}, LastFlushedLsn={LastFlushedLsn}, LastAppliedLsn={LastAppliedLsn}, Timestamp={Timestamp}",
        SkipEnabledCheck = true)]
    internal static partial void SentReplicationFeedbackMessage(
        ILogger logger,
        NpgsqlLogSequenceNumber LastReceivedLsn,
        NpgsqlLogSequenceNumber LastFlushedLsn,
        NpgsqlLogSequenceNumber LastAppliedLsn,
        DateTime Timestamp,
        int ConnectorId);

    [LoggerMessage(
        EventId = NpgsqlEventId.ReplicationFeedbackMessageSendingFailed,
        Level = LogLevel.Error,
        Message = "An exception occurred while sending a feedback message")]
    internal static partial void ReplicationFeedbackMessageSendingFailed(ILogger logger, int? ConnectorId, Exception exception);

    #endregion Replication
}
