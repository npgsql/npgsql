using System;

namespace Npgsql
{
    /// <summary>
    /// EventArgs class to send Notification parameters.
    /// </summary>
    public sealed class NpgsqlNotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Process ID of the PostgreSQL backend that sent this notification.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PID { get; }

        /// <summary>
        /// Condition that triggered that notification.
        /// </summary>
        public string Condition { get; }

        /// <summary>
        /// Additional information.
        /// </summary>
        public string AdditionalInformation { get; }

        internal NpgsqlNotificationEventArgs(NpgsqlReadBuffer buf)
        {
            PID = buf.ReadInt32();
            Condition = buf.ReadNullTerminatedString();
            AdditionalInformation = buf.ReadNullTerminatedString();
        }
    }
}
