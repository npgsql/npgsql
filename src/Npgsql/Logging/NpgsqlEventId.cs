#pragma warning disable 1591

namespace Npgsql.Logging
{
    /// <summary>
    /// Event IDs emitted by Npgsql's logging facilities.
    /// </summary>
    /// <remarks>
    /// Although these IDs should remain relatively stable, IDs may be removed or renamed between version.
    /// </remarks>
    public static class NpgsqlEventId
    {
        // Connection open/close
        public const int OpeningConnection = 1;
        public const int UsingPgpassFile = 2;
        public const int ConnectionOpened = 3;
        public const int ClosingConnection = 4;
        public const int ConnectionClosed = 5;
        public const int OpenedConnection = 6;
        public const int SslNegotiationSuccessful = 7;
        public const int SocketConnected = 8;
        public const int AttemptingToConnectTo = 9;
        public const int TimeoutConnecting = 10;
        public const int FailedToConnect = 11;
        public const int Authenticating = 12;
        public const int ConnectorClosing = 13;
        public const int Breaking = 14;
        public const int Keepalive = 15;
        public const int KeepaliveFailure = 16;

        // Command processing
        public const int ExecuteCommand = 100;
        public const int Preparing = 101;
        public const int StartUserAction = 102;
        public const int EndUserAction = 103;
        public const int Cancel = 104;
        public const int ExceptionWhileCancellingConnector = 105;
        public const int ResponseAfterCancel = 106;
        public const int ExceptionWhileClosing = 107;
        public const int Cleanup = 108;
        public const int ExecutingInternalCommand = 109;
        public const int ReaderCleanup = 110;

        // Pool
        public const int ExceptionClosingOutdatedConnector = 200;
        public const int ExceptionEnsuringMinPoolSize = 201;
        public const int ExceptionClosingPrunedConnector = 202;
        public const int ExceptionClearingConnector = 203;

        // Transactions
        public const int BeginningTransaction = 300;
        public const int Committing = 301;
        public const int RollingBack = 302;
        public const int CreatingSavepoint = 303;
        public const int RollingBackSavepoint = 304;
        public const int ReleasingSavepoint = 305;

        // System.Transactions
        public const int Enlisted = 400;
        public const int SinglePhaseCommit = 401;
        public const int TwoPhasePrepare = 402;
        public const int TwoPhaseCommit = 403;
        public const int TwoPhaseCommitException = 404;
        public const int TwoPhaseRollback = 405;
        public const int SinglePhaseRollback = 406;
        public const int RollbackException = 407;
        public const int TransactionInDoubt = 408;
        public const int ConnectionInUseDuringRollback = 409;
        public const int CleaningUpResourceManager = 410;

        // COPY
        public const int StartingBinaryImport = 500;
        public const int StartingBinaryExport = 501;
        public const int StartingTextImport = 502;
        public const int StartingTextExport = 503;
        public const int StartingRawCopy = 504;
        public const int EndCopy = 505;

        // Wait
        public const int StartingSyncWait = 600;
        public const int StartingAsyncWait = 601;
    }
}
