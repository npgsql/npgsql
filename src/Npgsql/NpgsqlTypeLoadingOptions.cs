namespace Npgsql;

/// <summary>
/// Options for configuring Npgsql type loading.
/// </summary>
class NpgsqlTypeLoadingOptions
{
    /// <summary>
    /// Load table composite type definitions, and not just free-standing composite types.
    /// </summary>
    public bool LoadTableComposites { get; init; }

    /// <summary>
    /// A compatibility mode for special PostgreSQL server types.
    /// </summary>
    public ServerCompatibilityMode ServerCompatibilityMode { get; init; }
}

/// <summary>
/// Options builder for configuring Npgsql type loading.
/// </summary>
public sealed class NpgsqlTypeLoadingOptionsBuilder
{
    bool _loadTableComposites;
    ServerCompatibilityMode _serverCompatibilityMode;

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
    public NpgsqlTypeLoadingOptionsBuilder SetServerCompatibilityMode(
        ServerCompatibilityMode serverCompatibilityMode = ServerCompatibilityMode.None)
    {
        _serverCompatibilityMode = serverCompatibilityMode;
        return this;
    }

    internal NpgsqlTypeLoadingOptions Build() => new()
    {
        LoadTableComposites = _loadTableComposites,
        ServerCompatibilityMode = _serverCompatibilityMode
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
    Redshift,
    /// <summary>
    /// The server is doesn't support full type loading from the PostgreSQL catalogs, support the basic set
    /// of types via information hardcoded inside Npgsql.
    /// </summary>
    NoTypeLoading,
}
