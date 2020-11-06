﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace Npgsql.Replication.PgOutput
{
    /// <summary>
    /// Options to be passed to the pgoutput plugin
    /// </summary>
    public class NpgsqlPgOutputReplicationOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlPgOutputReplicationOptions"/>.
        /// </summary>
        /// <param name="publicationName">The publication names to include into the stream</param>
        /// <param name="protocolVersion">The version of the logical streaming replication protocol</param>
        /// <param name="binary">Send values in binary representation</param>
        /// <param name="streaming">Enable streaming output</param>
        public NpgsqlPgOutputReplicationOptions(string publicationName, ulong protocolVersion = 1UL, bool? binary = null, bool? streaming = null)
            : this(new List<string> { publicationName ?? throw new ArgumentNullException(nameof(publicationName)) }, protocolVersion, binary, streaming)
        { }

        /// <summary>
        /// Creates a new instance of <see cref="NpgsqlPgOutputReplicationOptions"/>.
        /// </summary>
        /// <param name="publicationNames">The publication names to include into the stream</param>
        /// <param name="protocolVersion">The version of the logical streaming replication protocol</param>
        /// <param name="binary">Send values in binary representation</param>
        /// <param name="streaming">Enable streaming output</param>
        public NpgsqlPgOutputReplicationOptions(IEnumerable<string> publicationNames, ulong protocolVersion = 1UL, bool? binary = null, bool? streaming = null)
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
        }

        /// <summary>
        /// The version of the logical streaming replication protocol
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
        public bool? Binary { get; }

        /// <summary>
        /// Enable streaming output
        /// </summary>
        /// <remarks>
        /// This works in PostgreSQL versions 14+
        /// </remarks>
        public bool? Streaming { get; }

        internal IEnumerable<KeyValuePair<string, string?>> GetOptionPairs()
        {
            yield return new KeyValuePair<string, string?>("proto_version", ProtocolVersion.ToString(CultureInfo.InvariantCulture));
            yield return new KeyValuePair<string, string?>("publication_names", "\"" + string.Join("\",\"", PublicationNames) + "\"");

            if (Binary != null)
                yield return new KeyValuePair<string, string?>("binary", Binary.Value ? "t" : "f");
            if (Streaming != null)
                yield return new KeyValuePair<string, string?>("streaming", Streaming.Value ? "t" : "f");
        }
    }
}
