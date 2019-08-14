﻿using System;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Can be thrown by readers to indicate that interpreting the value failed, but the value was read wholly
    /// and it is safe to continue reading. Any other exception is assumed to leave the buffer in an unknown position,
    /// losing protocol sync and therefore setting the connector to state Broken.
    /// Note that an inner exception is mandatory, and will get thrown to the user instead of the NpgsqlSafeReadException.
    /// </summary>
    public class NpgsqlSafeReadException : Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="NpgsqlSafeReadException"/>.
        /// </summary>
        /// <param name="originalException"></param>
        public NpgsqlSafeReadException(Exception originalException) : base(string.Empty)
            => OriginalException = originalException ?? throw new ArgumentNullException(nameof(originalException));

        /// <summary>
        /// Gets the Exception instance that caused the current exception.
        /// </summary>
        public Exception OriginalException { get; }

        /// <inheritdocs />
        public override Exception GetBaseException()
            => OriginalException;
    }
}
