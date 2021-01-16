namespace Npgsql
{
    /// <summary>
    /// Base class for all classes which represent a message sent by the PostgreSQL backend.
    /// </summary>
    interface IBackendMessage
    {
        BackendMessageCode Code { get; }
    }

    enum BackendMessageCode : byte
    {
        AuthenticationRequest = (byte)'R',
        BackendKeyData        = (byte)'K',
        BindComplete          = (byte)'2',
        CloseComplete         = (byte)'3',
        CommandComplete       = (byte)'C',
        CopyData              = (byte)'d',
        CopyDone              = (byte)'c',
        CopyBothResponse      = (byte)'W',
        CopyInResponse        = (byte)'G',
        CopyOutResponse       = (byte)'H',
        DataRow               = (byte)'D',
        EmptyQueryResponse    = (byte)'I',
        ErrorResponse         = (byte)'E',
        FunctionCall          = (byte)'F',
        FunctionCallResponse  = (byte)'V',
        NoData                = (byte)'n',
        NoticeResponse        = (byte)'N',
        NotificationResponse  = (byte)'A',
        ParameterDescription  = (byte)'t',
        ParameterStatus       = (byte)'S',
        ParseComplete         = (byte)'1',
        PasswordPacket        = (byte)' ',
        PortalSuspended       = (byte)'s',
        ReadyForQuery         = (byte)'Z',
        RowDescription        = (byte)'T',
    }

    static class FrontendMessageCode
    {
        internal const byte Describe =  (byte)'D';
        internal const byte Sync =      (byte)'S';
        internal const byte Execute =   (byte)'E';
        internal const byte Parse =     (byte)'P';
        internal const byte Bind =      (byte)'B';
        internal const byte Close =     (byte)'C';
        internal const byte Query =     (byte)'Q';
        internal const byte CopyData =  (byte)'d';
        internal const byte CopyDone =  (byte)'c';
        internal const byte CopyFail =  (byte)'f';
        internal const byte Terminate = (byte)'X';
        internal const byte Password =  (byte)'p';
    }

    enum StatementOrPortal : byte
    {
        Statement = (byte)'S',
        Portal = (byte)'P'
    }

    /// <summary>
    /// Specifies the type of SQL statement, e.g. SELECT
    /// </summary>
    public enum StatementType
    {
#pragma warning disable 1591
        Unknown,
        Select,
        Insert,
        Delete,
        Update,
        CreateTableAs,
        Move,
        Fetch,
        Copy,
        Other
#pragma warning restore 1591
    }
}
