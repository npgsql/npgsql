using System;
using System.Collections.Generic;

namespace Npgsql.Replication.TestDecoding;

/// <summary>
/// Options to be passed to the test_decoding plugin
/// </summary>
public class TestDecodingOptions : IEquatable<TestDecodingOptions>
{
    /// <summary>
    /// Creates a new instance of <see cref="TestDecodingOptions"/>.
    /// </summary>
    /// <param name="includeXids">Include the transaction number for BEGIN and COMMIT command output</param>
    /// <param name="includeTimestamp">Include the timestamp for COMMIT command output</param>
    /// <param name="forceBinary">Set the <a href="https://www.postgresql.org/docs/current/logicaldecoding-output-plugin.html#LOGICALDECODING-OUTPUT-MODE">output mode</a> to binary</param>
    /// <param name="skipEmptyXacts">Skip output for transactions that didn't change the database</param>
    /// <param name="onlyLocal">Only output data that don't have the <a href="https://www.postgresql.org/docs/11/replication-origins.html">replication origin</a> set</param>
    /// <param name="includeRewrites">Include output from table rewrites that were caused by DDL statements</param>
    /// <param name="streamChanges">Enable streaming output</param>
    public TestDecodingOptions(bool? includeXids = null, bool? includeTimestamp = null, bool? forceBinary = null,
        bool? skipEmptyXacts = null, bool? onlyLocal = null, bool? includeRewrites = null, bool? streamChanges = null)
    {
        IncludeXids = includeXids;
        IncludeTimestamp = includeTimestamp;
        ForceBinary = forceBinary;
        SkipEmptyXacts = skipEmptyXacts;
        OnlyLocal = onlyLocal;
        IncludeRewrites = includeRewrites;
        StreamChanges = streamChanges;
    }

    /// <summary>
    /// Include the transaction number for BEGIN and COMMIT command output
    /// </summary>
    public bool? IncludeXids { get; }

    /// <summary>
    /// Include the timestamp for COMMIT command output
    /// </summary>
    public bool? IncludeTimestamp { get; }

    /// <summary>
    /// Set the <a href="https://www.postgresql.org/docs/current/logicaldecoding-output-plugin.html#LOGICALDECODING-OUTPUT-MODE">output mode</a> to binary
    /// </summary>
    public bool? ForceBinary { get; }

    /// <summary>
    /// Skip output for transactions that didn't change the database
    /// </summary>
    public bool? SkipEmptyXacts { get; }

    /// <summary>
    /// Only output data that don't have the <a href="https://www.postgresql.org/docs/11/replication-origins.html">replication origin</a> set
    /// </summary>
    public bool? OnlyLocal { get; }

    /// <summary>
    /// Include output from table rewrites that were caused by DDL statements
    /// </summary>
    public bool? IncludeRewrites { get; }

    /// <summary>
    /// Enable streaming output
    /// </summary>
    public bool? StreamChanges { get; }

    internal IEnumerable<KeyValuePair<string, string?>> GetOptionPairs()
    {
        if (IncludeXids != null)
            yield return new KeyValuePair<string, string?>("include-xids", IncludeXids.Value ? null : "f");
        if (IncludeTimestamp != null)
            yield return new KeyValuePair<string, string?>("include-timestamp", IncludeTimestamp.Value ? null : "f");
        if (ForceBinary != null)
            yield return new KeyValuePair<string, string?>("force-binary", ForceBinary.Value ? "t" : "f");
        if (SkipEmptyXacts != null)
            yield return new KeyValuePair<string, string?>("skip-empty-xacts", SkipEmptyXacts.Value ? null : "f");
        if (OnlyLocal != null)
            yield return new KeyValuePair<string, string?>("only-local", OnlyLocal.Value ? null : "false");
        if (IncludeRewrites != null)
            yield return new KeyValuePair<string, string?>("include-rewrites", IncludeRewrites.Value ? "t" : "f");
        if (StreamChanges != null)
            yield return new KeyValuePair<string, string?>("stream-changes", StreamChanges.Value ? "t" : "f");
    }

    /// <inheritdoc />
    public bool Equals(TestDecodingOptions? other)
        => other != null && (
            ReferenceEquals(this, other) ||
            IncludeXids == other.IncludeXids && IncludeTimestamp == other.IncludeTimestamp && ForceBinary == other.ForceBinary &&
            SkipEmptyXacts == other.SkipEmptyXacts && OnlyLocal == other.OnlyLocal && IncludeRewrites == other.IncludeRewrites &&
            StreamChanges == other.StreamChanges);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is TestDecodingOptions other && other.Equals(this);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(IncludeXids, IncludeTimestamp, ForceBinary, SkipEmptyXacts, OnlyLocal, IncludeRewrites, StreamChanges);
}
