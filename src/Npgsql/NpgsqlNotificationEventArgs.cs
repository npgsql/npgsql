using System;

namespace Npgsql
{
    /// <summary>
    /// Provides information on a PostgreSQL notification. Notifications are sent when your connection has registered for
    /// notifications on a specific channel via the LISTEN command. NOTIFY can be used to generate such notifications,
    /// allowing for an inter-connection communication channel.
    /// </summary>
    public sealed class NpgsqlNotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Process ID of the PostgreSQL backend that sent this notification.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PID { get; }

        /// <summary>
        /// The channel on which the notification was sent.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// An optional payload string that was sent with this notification.
        /// </summary>
        public string Payload { get; }

        /// <summary>
        /// The channel on which the notification was sent.
        /// </summary>
        [Obsolete("Use Channel instead")]
        public string Condition => Channel;

        /// <summary>
        /// An optional payload string that was sent with this notification.
        /// </summary>
        [Obsolete("Use Payload instead")]
        public string AdditionalInformation => Payload;

        internal NpgsqlNotificationEventArgs(NpgsqlReadBuffer buf)
        {
            PID = buf.ReadInt32();
            Channel = buf.ReadNullTerminatedString();
            Payload = buf.ReadNullTerminatedString();
        }
    }
}
