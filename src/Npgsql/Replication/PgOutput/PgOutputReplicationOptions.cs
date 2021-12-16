using System;
using System.Collections.Generic;
using System.Globalization;

namespace Npgsql.Replication.PgOutput
{
    /// <summary>
    /// Options to be passed to the pgoutput plugin
    /// </summary>
    public class PgOutputReplicationOptions : IEquatable<PgOutputReplicationOptions>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PgOutputReplicationOptions"/>.
        /// </summary>
        /// <param name="publicationName">The publication names to include into the stream</param>
        /// <param name="protocolVersion">The version of the logical streaming replication protocol</param>
        /// <param name="binary">Send values in binary representation</param>
        /// <param name="streaming">Enable streaming of in-progress transactions</param>
        /// <param name="messages">Write logical decoding messages into the replication stream</param>
        public PgOutputReplicationOptions(string publicationName, ulong protocolVersion, bool? binary = null, bool? streaming = null, bool? messages = null)
            : this(new List<string> { publicationName ?? throw new ArgumentNullException(nameof(publicationName)) }, protocolVersion, binary, streaming, messages)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="PgOutputReplicationOptions"/>.
        /// </summary>
        /// <param name="publicationNames">The publication names to include into the stream</param>
        /// <param name="protocolVersion">The version of the logical streaming replication protocol</param>
        /// <param name="binary">Send values in binary representation</param>
        /// <param name="streaming">Enable streaming of in-progress transactions</param>
        /// <param name="messages">Write logical decoding messages into the replication stream</param>
        public PgOutputReplicationOptions(IEnumerable<string> publicationNames, ulong protocolVersion, bool? binary = null, bool? streaming = null, bool? messages = null)
        {
            var publicationNamesList = new List<string>(publicationNames);
            if (publicationNamesList.Count < 1)
                throw new ArgumentException("You have to pass at least one publication name.", nameof(publicationNames));

            foreach (var publicationName in publicationNamesList)
                if (string.IsNullOrWhiteSpace(publicationName))
                    throw publicationName is null
                        ? new ArgumentNullException(nameof(publicationName))
                        : new ArgumentException("Invalid publication name", nameof(publicationName));

            PublicationNames = publicationNamesList;
            ProtocolVersion = protocolVersion;
            Binary = binary;
            Streaming = streaming;
            Messages = messages;
        }

        /// <summary>
        /// The version of the Logical Streaming Replication Protocol
        /// </summary>
        public ulong ProtocolVersion { get; }

        /// <summary>
        /// The publication names to stream
        /// </summary>
        public List<string> PublicationNames { get; }

        /// <summary>
        /// Send values in binary representation
        /// </summary>
        /// <remarks>
        /// This works in PostgreSQL versions 14+
        /// </remarks>
        // See: https://github.com/postgres/postgres/commit/9de77b5453130242654ff0b30a551c9c862ed661
        public bool? Binary { get; }

        /// <summary>
        /// Enable streaming of in-progress transactions
        /// </summary>
        /// <remarks>
        /// This works as of logical streaming replication protocol version 2 (PostgreSQL 14+)
        /// </remarks>
        // See: https://github.com/postgres/postgres/commit/464824323e57dc4b397e8b05854d779908b55304
        public bool? Streaming { get; }

        /// <summary>
        /// Write logical decoding messages into the replication stream
        /// </summary>
        /// <remarks>
        /// This works in PostgreSQL versions 14+
        /// </remarks>
        // See: https://github.com/postgres/postgres/commit/ac4645c0157fc5fcef0af8ff571512aa284a2cec
        public bool? Messages { get; }

        internal IEnumerable<KeyValuePair<string, string?>> GetOptionPairs()
        {
            yield return new KeyValuePair<string, string?>("proto_version", ProtocolVersion.ToString(CultureInfo.InvariantCulture));
            yield return new KeyValuePair<string, string?>("publication_names", "\"" + string.Join("\",\"", PublicationNames) + "\"");

            if (Binary != null)
                yield return new KeyValuePair<string, string?>("binary", Binary.Value ? "on" : "off");
            if (Streaming != null)
                yield return new KeyValuePair<string, string?>("streaming", Streaming.Value ? "on" : "off");
            if (Messages != null)
                yield return new KeyValuePair<string, string?>("messages", Messages.Value ? "on" : "off");
        }

        /// <inheritdoc />
        public bool Equals(PgOutputReplicationOptions? other)
            => other != null && (
                ReferenceEquals(this, other) ||
                ProtocolVersion == other.ProtocolVersion && PublicationNames.Equals(other.PublicationNames) && Binary == other.Binary &&
                Streaming == other.Streaming && Messages == other.Messages);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is PgOutputReplicationOptions other && other.Equals(this);

        /// <inheritdoc />
        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            var hashCode = ProtocolVersion.GetHashCode();
            hashCode = (hashCode * 397) ^ PublicationNames.GetHashCode();
            hashCode = (hashCode * 397) ^ Binary.GetHashCode();
            hashCode = (hashCode * 397) ^ Streaming.GetHashCode();
            hashCode = (hashCode * 397) ^ Messages.GetHashCode();
            return hashCode;
#else
            return HashCode.Combine(ProtocolVersion, PublicationNames, Binary, Streaming, Messages);
#endif
        }
    }
}
