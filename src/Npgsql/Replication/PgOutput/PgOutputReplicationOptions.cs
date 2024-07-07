using System;
using System.Collections.Generic;
using System.Globalization;

namespace Npgsql.Replication.PgOutput;

/// <summary>
/// Options to be passed to the pgoutput plugin
/// </summary>
public class PgOutputReplicationOptions : IEquatable<PgOutputReplicationOptions>
{
    /// <summary>
    /// Creates a new instance of <see cref="PgOutputReplicationOptions"/>.
    /// </summary>
    /// <param name="publicationName">The publication names to include into the stream</param>
    /// <param name="protocolVersion">The version of the logical streaming replication protocol.
    /// Passing in unsupported protocol version numbers may lead to runtime errors.</param>
    /// <param name="binary">Send values in binary representation</param>
    /// <param name="streaming">Enable streaming of in-progress transactions.
    /// Setting this to <see langword="true"/> sets <see cref="PgOutputReplicationOptions.StreamingMode"/>
    /// to <see cref="PgOutputStreamingMode.On"/>.</param>
    /// <param name="messages">Write logical decoding messages into the replication stream</param>
    /// <param name="twoPhase">Enable streaming of prepared transactions</param>
    [Obsolete("Please switch to the overloads that take PgOutputProtocolVersion and PgOutputStreamingMode values instead.")]
    public PgOutputReplicationOptions(string publicationName, ulong protocolVersion, bool? binary = null, bool? streaming = null,
        bool? messages = null, bool? twoPhase = null)
        : this([publicationName ?? throw new ArgumentNullException(nameof(publicationName))], (PgOutputProtocolVersion)protocolVersion,
            binary, streaming.HasValue ? streaming.Value ? PgOutputStreamingMode.On : PgOutputStreamingMode.Off : null, messages, twoPhase)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="PgOutputReplicationOptions"/>.
    /// </summary>
    /// <param name="publicationName">The publication names to include into the stream</param>
    /// <param name="protocolVersion">The version of the logical streaming replication protocol</param>
    /// <param name="binary">Send values in binary representation</param>
    /// <param name="streamingMode">Enable streaming of in-progress transactions</param>
    /// <param name="messages">Write logical decoding messages into the replication stream</param>
    /// <param name="twoPhase">Enable streaming of prepared transactions</param>
    public PgOutputReplicationOptions(string publicationName, PgOutputProtocolVersion protocolVersion, bool? binary = null,
        PgOutputStreamingMode? streamingMode = null, bool? messages = null, bool? twoPhase = null)
        : this([publicationName ?? throw new ArgumentNullException(nameof(publicationName))], protocolVersion, binary, streamingMode,
            messages, twoPhase)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="PgOutputReplicationOptions"/>.
    /// </summary>
    /// <param name="publicationNames">The publication names to include into the stream</param>
    /// <param name="protocolVersion">The version of the logical streaming replication protocol.
    /// Passing in unsupported protocol version numbers may lead to runtime errors.</param>
    /// <param name="binary">Send values in binary representation</param>
    /// <param name="streaming">Enable streaming of in-progress transactions.
    /// Setting this to <see langword="true"/> sets <see cref="PgOutputReplicationOptions.StreamingMode"/>
    /// to <see cref="PgOutputStreamingMode.On"/>.</param>
    /// <param name="messages">Write logical decoding messages into the replication stream</param>
    /// <param name="twoPhase">Enable streaming of prepared transactions</param>
    [Obsolete("Please switch to the overloads that take PgOutputProtocolVersion and PgOutputStreamingMode values instead.")]
    public PgOutputReplicationOptions(IEnumerable<string> publicationNames, ulong protocolVersion, bool? binary = null,
        bool? streaming = null, bool? messages = null, bool? twoPhase = null)
        : this(publicationNames, (PgOutputProtocolVersion)protocolVersion, binary,
            streaming.HasValue ? streaming.Value ? PgOutputStreamingMode.On : PgOutputStreamingMode.Off : null, messages, twoPhase)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="PgOutputReplicationOptions"/>.
    /// </summary>
    /// <param name="publicationNames">The publication names to include into the stream</param>
    /// <param name="protocolVersion">The version of the logical streaming replication protocol</param>
    /// <param name="binary">Send values in binary representation</param>
    /// <param name="streamingMode">Enable streaming of in-progress transactions</param>
    /// <param name="messages">Write logical decoding messages into the replication stream</param>
    /// <param name="twoPhase">Enable streaming of prepared transactions</param>
    public PgOutputReplicationOptions(IEnumerable<string> publicationNames, PgOutputProtocolVersion protocolVersion, bool? binary = null,
        PgOutputStreamingMode? streamingMode = null, bool? messages = null, bool? twoPhase = null)
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
        StreamingMode = streamingMode;
        Messages = messages;
        TwoPhase = twoPhase;
    }

    /// <summary>
    /// The version of the Logical Streaming Replication Protocol
    /// </summary>
    public PgOutputProtocolVersion ProtocolVersion { get; }

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
    /// <see cref="PgOutputStreamingMode.On"/> works as of logical streaming replication protocol version 2 (PostgreSQL 14+),
    /// <see cref="PgOutputStreamingMode.Parallel"/> works as of logical streaming replication protocol version 4 (PostgreSQL 16+),
    /// </remarks>
    // See: https://github.com/postgres/postgres/commit/464824323e57dc4b397e8b05854d779908b55304
    // and https://github.com/postgres/postgres/commit/216a784829c2c5f03ab0c43e009126cbb819e9b2
    public PgOutputStreamingMode? StreamingMode { get; }

    /// <summary>
    /// Write logical decoding messages into the replication stream
    /// </summary>
    /// <remarks>
    /// This works in PostgreSQL versions 14+
    /// </remarks>
    // See: https://github.com/postgres/postgres/commit/ac4645c0157fc5fcef0af8ff571512aa284a2cec
    public bool? Messages { get; }

    /// <summary>
    /// Enable streaming of prepared transactions
    /// </summary>
    /// <remarks>
    /// This works in PostgreSQL versions 15+
    /// </remarks>
    // See: https://github.com/postgres/postgres/commit/a8fd13cab0ba815e9925dc9676e6309f699b5f72
    // and https://github.com/postgres/postgres/commit/63cf61cdeb7b0450dcf3b2f719c553177bac85a2
    public bool? TwoPhase { get; }

    internal IEnumerable<KeyValuePair<string, string?>> GetOptionPairs()
    {
        yield return new KeyValuePair<string, string?>("proto_version", ((ulong)ProtocolVersion).ToString(CultureInfo.InvariantCulture));
        yield return new KeyValuePair<string, string?>("publication_names", "\"" + string.Join("\",\"", PublicationNames) + "\"");

        if (Binary != null)
            yield return new KeyValuePair<string, string?>("binary", Binary.Value ? "on" : "off");
        if (StreamingMode != null)
        {
            yield return new KeyValuePair<string, string?>("streaming", StreamingMode.Value switch
            {
                PgOutputStreamingMode.Off => "off",
                PgOutputStreamingMode.On => "on",
                PgOutputStreamingMode.Parallel => "parallel",
                _ => throw new ArgumentOutOfRangeException($"Unknown {nameof(PgOutputStreamingMode)} value: {StreamingMode.Value}")
            });
        }
        if (Messages != null)
            yield return new KeyValuePair<string, string?>("messages", Messages.Value ? "on" : "off");
        if (TwoPhase != null)
            yield return new KeyValuePair<string, string?>("two_phase", TwoPhase.Value ? "on" : "off");
    }

    /// <inheritdoc />
    public bool Equals(PgOutputReplicationOptions? other)
        => other != null && (
            ReferenceEquals(this, other) ||
            ProtocolVersion == other.ProtocolVersion && PublicationNames.Equals(other.PublicationNames) && Binary == other.Binary &&
            StreamingMode == other.StreamingMode && Messages == other.Messages && TwoPhase == other.TwoPhase);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is PgOutputReplicationOptions other && other.Equals(this);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(ProtocolVersion, PublicationNames, Binary, StreamingMode, Messages, TwoPhase);
}
