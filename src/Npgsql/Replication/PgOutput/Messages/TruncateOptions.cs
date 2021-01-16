using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Enum representing the additional options for the TRUNCATE command as flags
    /// </summary>
    [Flags]
    public enum TruncateOptions : byte
    {
        /// <summary>
        /// No additional option was specified
        /// </summary>
        None = 0,

        /// <summary>
        /// CASCADE was specified
        /// </summary>
        Cascade = 1,

        /// <summary>
        /// RESTART IDENTITY was specified
        /// </summary>
        RestartIdentity = 2
    }
}
