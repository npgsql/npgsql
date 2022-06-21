using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Npgsql;

/// <summary>
/// Configures Npgsql logging
/// </summary>
public class NpgsqlLoggingConfiguration
{
    internal static readonly NpgsqlLoggingConfiguration NullConfiguration
        = new(NullLoggerFactory.Instance, isParameterLoggingEnabled: false);

    internal static ILoggerFactory GlobalLoggerFactory = NullLoggerFactory.Instance;
    internal static bool GlobalIsParameterLoggingEnabled;

    internal NpgsqlLoggingConfiguration(ILoggerFactory loggerFactory, bool isParameterLoggingEnabled)
    {
        ConnectionLogger = loggerFactory.CreateLogger("Npgsql.Connection");
        CommandLogger = loggerFactory.CreateLogger("Npgsql.Command");
        TransactionLogger = loggerFactory.CreateLogger("Npgsql.Transaction");
        CopyLogger = loggerFactory.CreateLogger("Npgsql.Copy");
        ReplicationLogger = loggerFactory.CreateLogger("Npgsql.Replication");
        ExceptionLogger = loggerFactory.CreateLogger("Npgsql.Exception");

        IsParameterLoggingEnabled = isParameterLoggingEnabled;
    }

    internal ILogger ConnectionLogger { get; }
    internal ILogger CommandLogger { get; }
    internal ILogger TransactionLogger { get; }
    internal ILogger CopyLogger { get; }
    internal ILogger ReplicationLogger { get; }
    internal ILogger ExceptionLogger { get; }

    /// <summary>
    /// Determines whether parameter contents will be logged alongside SQL statements - this may reveal sensitive information.
    /// Defaults to false.
    /// </summary>
    internal bool IsParameterLoggingEnabled { get; }

    /// <summary>
    /// <para>
    /// Globally initializes Npgsql logging to use the provided <paramref name="loggerFactory" />.
    /// Must be called before any Npgsql APIs are used.
    /// </para>
    /// <para>
    /// This is a legacy-only, backwards compatibility API. New applications should set the logger factory on
    /// <see cref="NpgsqlDataSourceBuilder" /> and use the resulting <see cref="NpgsqlDataSource "/> instead.
    /// </para>
    /// </summary>
    /// <param name="loggerFactory">The logging factory to use when logging from Npgsql.</param>
    /// <param name="parameterLoggingEnabled">
    /// Determines whether parameter contents will be logged alongside SQL statements - this may reveal sensitive information.
    /// Defaults to <see langword="false" />.
    /// </param>
    public static void InitializeLogging(ILoggerFactory loggerFactory, bool parameterLoggingEnabled = false)
        => (GlobalLoggerFactory, GlobalIsParameterLoggingEnabled) = (loggerFactory, parameterLoggingEnabled);
}