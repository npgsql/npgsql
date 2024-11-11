namespace Npgsql;

/// <summary>
/// Options for configuring Npgsql type loading.
/// </summary>
public class NpgsqlTypeLoadingOptions
{
    readonly NpgsqlConnectionStringBuilder _csb;
    internal NpgsqlTypeLoadingOptions(NpgsqlConnectionStringBuilder csb) => _csb = csb;

    bool? _loadTableComposites;
    /// <summary>
    /// Load table composite type definitions, and not just free-standing composite types.
    /// </summary>
    public bool LoadTableComposites
    {
        get => _loadTableComposites ?? _csb.LoadTableComposites;
        set => _loadTableComposites = value;
    }

    ServerCompatibilityMode? _serverCompatibilityMode;
    /// <summary>
    /// A compatibility mode for special PostgreSQL server types.
    /// </summary>
    public ServerCompatibilityMode ServerCompatibilityMode
    {
        get => _serverCompatibilityMode ?? _csb.ServerCompatibilityMode;
        set => _serverCompatibilityMode = value;
    }

    internal NpgsqlTypeLoadingOptions Clone()
        => new(_csb)
        {
            LoadTableComposites = LoadTableComposites,
            ServerCompatibilityMode = ServerCompatibilityMode
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
