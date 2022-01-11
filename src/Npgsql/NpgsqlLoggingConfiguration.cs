using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Npgsql;

/// <summary>
/// Configures Npgsql logging
/// </summary>
public static class NpgsqlLoggingConfiguration
{
    static NpgsqlLoggingConfiguration()
    {
        var nullFactory = new NullLoggerFactory();

        ConnectionLogger = nullFactory.CreateLogger("Npgsql.Connection");
        CommandLogger = nullFactory.CreateLogger("Npgsql.Command");
        TransactionLogger = nullFactory.CreateLogger("Npgsql.Transaction");
        CopyLogger = nullFactory.CreateLogger("Npgsql.Copy");
        ReplicationLogger = nullFactory.CreateLogger("Npgsql.Replication");
        ExceptionLogger = nullFactory.CreateLogger("Npgsql.Exception");
    }

    /// <summary>
    /// Initializes Npgsql logging to use the provided <paramref name="loggerFactory" />.
    /// Must be called before any Npgsql APIs are used.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="parameterLoggingEnabled">
    /// Determines whether parameter contents will be logged alongside SQL statements - this may reveal sensitive information.
    /// Defaults to <see langword="false" />.
    /// </param>
    public static void InitializeLogging(ILoggerFactory loggerFactory, bool parameterLoggingEnabled = false)
    {
        ConnectionLogger = loggerFactory.CreateLogger("Npgsql.Connection");
        CommandLogger = loggerFactory.CreateLogger("Npgsql.Command");
        TransactionLogger = loggerFactory.CreateLogger("Npgsql.Transaction");
        CopyLogger = loggerFactory.CreateLogger("Npgsql.Copy");
        ReplicationLogger = loggerFactory.CreateLogger("Npgsql.Replication");
        ExceptionLogger = loggerFactory.CreateLogger("Npgsql.Exception");

        IsParameterLoggingEnabled = parameterLoggingEnabled;
    }

    internal static ILogger ConnectionLogger { get; private set; }
    internal static ILogger CommandLogger { get; private set; }
    internal static ILogger TransactionLogger { get; private set; }
    internal static ILogger CopyLogger { get; private set; }
    internal static ILogger ReplicationLogger { get; private set; }
    internal static ILogger ExceptionLogger { get; private set; }

    /// <summary>
    /// Determines whether parameter contents will be logged alongside SQL statements - this may reveal sensitive information.
    /// Defaults to false.
    /// </summary>
    internal static bool IsParameterLoggingEnabled { get; set; }
}