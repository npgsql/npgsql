using System;

namespace Npgsql;

/// <summary>
/// Options for configuring Npgsql type loading.
/// </summary>
sealed class NpgsqlTypeLoadingOptions
{
    /// <summary>
    /// Load table composite type definitions, and not just free-standing composite types.
    /// </summary>
    public bool LoadTableComposites { get; init; }

    /// <summary>
    /// The server is doesn't support full type loading from the PostgreSQL catalogs, support the basic set
    /// of types via information hardcoded inside Npgsql.
    /// </summary>
    public bool TypeLoading { get; init; }
}

/// <summary>
/// Options builder for configuring Npgsql type loading.
/// </summary>
public sealed class NpgsqlTypeLoadingOptionsBuilder
{
    bool _loadTableComposites;
    bool _typeLoading;

    internal NpgsqlTypeLoadingOptionsBuilder() {}

    /// <summary>
    /// Enable loading table composite type definitions, and not just free-standing composite types.
    /// </summary>
    public NpgsqlTypeLoadingOptionsBuilder EnableTableCompositesLoading(bool enable = true)
    {
        _loadTableComposites = enable;
        return this;
    }

    /// <summary>
    /// Set a compatibility mode for special PostgreSQL server types.
    /// </summary>
    public NpgsqlTypeLoadingOptionsBuilder EnableTypeLoading(
        bool enable = true)
    {
        _typeLoading = enable;
        return this;
    }

    internal NpgsqlTypeLoadingOptions Build() => new()
    {
        LoadTableComposites = _loadTableComposites,
        TypeLoading = _typeLoading
    };
}

/// <summary>
/// An option specified in the connection string that activates special compatibility features.
/// </summary>
public enum ServerCompatibilityMode
{
    /// <summary>
    /// No special server compatibility mode is active
    /// </summary>
    None,

    /// <summary>
    /// The server is an Amazon Redshift instance.
    /// </summary>
    [Obsolete("ServerCompatibilityMode.Redshift no longer does anything and can be safely removed.")]
    Redshift,

    /// <summary>
    /// The server is doesn't support full type loading from the PostgreSQL catalogs, support the basic set
    /// of types via information hardcoded inside Npgsql.
    /// </summary>
    NoTypeLoading,
}
