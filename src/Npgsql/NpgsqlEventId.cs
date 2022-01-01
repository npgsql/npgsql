namespace Npgsql;

#pragma warning disable CS1591
#pragma warning disable RS0016

public static class NpgsqlEventId
{
    #region Connection

    public const int OpeningConnection    = 1000;
    public const int OpenedConnection     = 1001;
    public const int ClosingConnection    = 1003;
    public const int ClosedConnection     = 1004;

    public const int OpeningPhysicalConnection = 1110;
    public const int OpenedPhysicalConnection  = 1111;
    public const int ClosingPhysicalConnection = 1112;
    public const int ClosedPhysicalConnection  = 1113;

    public const int StartingWait   = 1300;
    public const int ReceivedNotice = 1301;

    public const int ConnectionExceededMaximumLifetime = 1500;

    public const int SendingKeepalive   = 1600;
    public const int CompletedKeepalive = 1601;
    public const int KeepaliveFailed    = 1602;

    public const int BreakingConnection                            = 1900;
    public const int CaughtUserExceptionInNoticeEventHandler       = 1901;
    public const int CaughtUserExceptionInNotificationEventHandler = 1902;
    public const int ExceptionWhenClosingPhysicalConnection        = 1903;
    public const int ExceptionWhenOpeningConnectionForMultiplexing = 1904;

    #endregion Connection

    #region Command

    public const int ExecutingCommand          = 2000;
    public const int CommandExecutionCompleted = 2001;
    public const int CancellingCommand         = 2002;
    public const int ExecutingInternalCommand  = 2003;

    public const int PreparingCommandExplicitly = 2100;
    public const int CommandPreparedExplicitly  = 2101;
    public const int AutoPreparingStatement     = 2102;
    public const int UnpreparingCommand         = 2103;

    public const int DerivingParameters = 2500;

    public const int ExceptionWhenWritingMultiplexedCommands = 2600;

    #endregion Command

    #region Transaction

    public const int StartedTransaction    = 30000;
    public const int CommittedTransaction  = 30001;
    public const int RolledBackTransaction = 30002;

    public const int CreatingSavepoint      = 30100;
    public const int RolledBackToSavepoint  = 30101;
    public const int ReleasedSavepoint      = 30102;

    public const int ExceptionDuringTransactionDispose = 30200;

    public const int EnlistedVolatileResourceManager      = 31000;
    public const int CommittingSinglePhaseTransaction     = 31001;
    public const int RollingBackSinglePhaseTransaction    = 31002;
    public const int SinglePhaseTransactionRollbackFailed = 31003;
    public const int PreparingTwoPhaseTransaction         = 31004;
    public const int CommittingTwoPhaseTransaction        = 31005;
    public const int TwoPhaseTransactionCommitFailed      = 31006;
    public const int RollingBackTwoPhaseTransaction       = 31007;
    public const int TwoPhaseTransactionRollbackFailed    = 31008;
    public const int TwoPhaseTransactionInDoubt           = 31009;
    public const int ConnectionInUseWhenRollingBack       = 31010;
    public const int CleaningUpResourceManager            = 31011;

    #endregion Transaction

    #region Copy

    public const int StartingBinaryExport = 40000;
    public const int StartingBinaryImport = 40001;
    public const int StartingTextExport   = 40002;
    public const int StartingTextImport   = 40003;
    public const int StartingRawCopy      = 40004;

    public const int CopyOperationCompleted              = 40100;
    public const int CopyOperationCancelled              = 40101;
    public const int ExceptionWhenDisposingCopyOperation = 40102;

    #endregion Copy

    #region Replication

    public const int CreatingReplicationSlot     = 50000;
    public const int DroppingReplicationSlot     = 50001;
    public const int StartingLogicalReplication  = 50002;
    public const int StartingPhysicalReplication = 50003;
    public const int ExecutingReplicationCommand = 50004;

    public const int ReceivedReplicationPrimaryKeepalive     = 50100;
    public const int SendingReplicationStandbyStatusUpdate   = 50101;
    public const int SentReplicationFeedbackMessage          = 50102;
    public const int ReplicationFeedbackMessageSendingFailed = 50103;

    #endregion Replication
}
