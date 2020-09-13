using JetBrains.Annotations;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Enum representing the additional options for the TRUNCATE command as flags
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum TruncateOptions : byte
    {
        /// <summary>
        /// No additional option was specified
        /// </summary>
        [PublicAPI]
        None = 0,

        /// <summary>
        /// CASCADE was specified
        /// </summary>
        [PublicAPI]
        Cascade = 1,

        /// <summary>
        /// RESTART IDENTITY was specified
        /// </summary>
        [PublicAPI]
        RestartIdentity = 2
    }
}
