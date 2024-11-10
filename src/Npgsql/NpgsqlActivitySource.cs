using Npgsql.Internal;
using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Npgsql;

// Semantic conventions for database client spans: https://opentelemetry.io/docs/specs/semconv/database/database-spans/
// Semantic conventions for PostgreSQL client operations: https://opentelemetry.io/docs/specs/semconv/database/postgresql/
static class NpgsqlActivitySource
{
    static readonly ActivitySource Source = new("Npgsql", GetLibraryVersion());

    internal static bool IsEnabled => Source.HasListeners();

    internal static Activity? CommandStart(NpgsqlConnectionStringBuilder settings, string commandText, CommandType commandType, string? spanName)
    {
        string? operationName = null;

        switch (commandType)
        {
        case CommandType.StoredProcedure:
            // We follow the {db.operation.name} {target} pattern of the spec, with the operation being SELECT/CALL and
            // the target being the stored procedure name.
            operationName = NpgsqlCommand.EnableStoredProcedureCompatMode ? "SELECT" : "CALL";
            spanName ??= $"{operationName} {commandText}";
            break;
        case CommandType.TableDirect:
            // We follow the {db.operation.name} {target} pattern of the spec, with the operation being SELECT and
            // the target being the table (collection) name.
            operationName = "SELECT";
            spanName ??= $"{operationName} {commandText}";
            break;
        case CommandType.Text:
            // We don't have db.query.summary, db.operation.name or target (without parsing SQL),
            // so we fall back to db.system.name as per the specs.
            spanName ??= "postgresql";
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
        }

        var activity = Source.StartActivity(spanName, ActivityKind.Client);
        if (activity is not { IsAllDataRequested: true })
            return activity;

        activity.SetTag("db.query.text", commandText);

        switch (commandType)
        {
            case CommandType.StoredProcedure:
                Debug.Assert(operationName is not null);
                activity.SetTag("db.operation.name", operationName);
                activity.SetTag("db.stored_procedure.name", commandText);
                break;
            case CommandType.TableDirect:
                Debug.Assert(operationName is not null);
                activity.SetTag("db.operation.name", operationName);
                activity.SetTag("db.collection.name", commandText);
                break;
        }

        return activity;
    }

    internal static Activity? PhysicalConnectionOpen(NpgsqlConnector connector)
    {
        if (!connector.DataSource.Configuration.TracingOptions.EnablePhysicalOpenTracing)
            return null;

        // Note that physical connection open is not part of the OpenTelemetry spec.
        // We emit it if enabled, following the general name/tags guidelines.
        var dbName = connector.Settings.Database ?? connector.InferredUserName;
        var activity = Source.StartActivity("CONNECT " + dbName, ActivityKind.Client);
        if (activity is not { IsAllDataRequested: true })
            return activity;

        // We set these basic tags on the activity so that they're populated even when the physical open fails.
        activity.SetTag("db.system.name", "postgresql");
        activity.SetTag("db.npgsql.data_source", connector.DataSource.Name);

        return activity;
    }

    internal static void Enrich(Activity activity, NpgsqlConnector connector)
    {
        if (!activity.IsAllDataRequested)
            return;

        activity.SetTag("db.system.name", "postgresql");

        // TODO: For now, we only set the database name, without adding the first schema in the search_path
        // as per the PG tracing specs (https://opentelemetry.io/docs/specs/semconv/database/postgresql/).
        // See #6336
        activity.SetTag("db.namespace", connector.Settings.Database ?? connector.InferredUserName);

        var endPoint = connector.ConnectedEndPoint;
        Debug.Assert(endPoint is not null);
        activity.SetTag("server.address", connector.Host);
        switch (endPoint)
        {
        case IPEndPoint ipEndPoint:
            if (ipEndPoint.Port != 5432)
                activity.SetTag("server.port", ipEndPoint.Port);
            break;

        case UnixDomainSocketEndPoint:
            break;

        default:
            throw new UnreachableException("Invalid endpoint type: " + endPoint.GetType());
        }

        // Npgsql-specific tags
        activity.SetTag("db.npgsql.data_source", connector.DataSource.Name);
        activity.SetTag("db.npgsql.connection_id", connector.Id);
    }

    internal static void ReceivedFirstResponse(Activity activity, NpgsqlTracingOptions tracingOptions)
    {
        if (!activity.IsAllDataRequested || !tracingOptions.EnableFirstResponseEvent)
            return;

        var activityEvent = new ActivityEvent("received-first-response");
        activity.AddEvent(activityEvent);
    }

    internal static void SetException(Activity activity, Exception exception, bool escaped = true)
    {
        activity.AddException(exception);

        if (exception is PostgresException { SqlState: var sqlState })
        {
            activity.SetTag("db.response.status_code", sqlState);

            // error.type SHOULD match the db.response.status_code returned by the database or the client library, or the canonical name of exception that occurred.
            // Since we don't have a table to map the error code to a textual description, the SQL state is the best we can do.
            activity.SetTag("error.type", sqlState);
        }
        else
        {
            if (exception is NpgsqlException { InnerException: Exception innerException })
                exception = innerException;

            activity.SetTag("error.type", exception.GetType().FullName);
        }

        var statusDescription = exception is PostgresException pgEx ? pgEx.SqlState : exception.Message;
        activity.SetStatus(ActivityStatusCode.Error, statusDescription);
        activity.Dispose();
    }

    internal static Activity? CopyStart(string command, NpgsqlConnector connector, string? spanName, string? operation = null)
    {
        var dbName = connector.Settings.Database ?? "UNKNOWN";
        var activity = Source.StartActivity(spanName ?? dbName, ActivityKind.Client);
        if (activity is not { IsAllDataRequested: true })
            return activity;
        activity.SetTag("db.statement", command);
        if (operation is not null)
            activity.SetTag("db.operation", operation);
        Enrich(activity, connector);
        return activity;
    }

    internal static Activity? ImportStart(string copyFromCommand, NpgsqlConnector connector, string? spanName)
        => CopyStart(copyFromCommand, connector, spanName, "COPY FROM");

    internal static Activity? ExportStart(string copyToCommand, NpgsqlConnector connector, string? spanName)
        => CopyStart(copyToCommand, connector, spanName, "COPY TO");

    internal static void SetImport(Activity activity)
        => SetOperation(activity, "COPY FROM");

    internal static void SetExport(Activity activity)
        => SetOperation(activity, "COPY TO");

    static void SetOperation(Activity activity, string operation)
    {
        if (!activity.IsAllDataRequested)
            return;
        activity.SetTag("db.operation", operation);
    }

    private static void CopyStop(Activity activity, ulong? rows = null)
    {
        activity.SetStatus(ActivityStatusCode.Ok);
        if (rows.HasValue)
            activity.SetTag("db.rows", rows.Value);
        activity.Dispose();
    }

    internal static void ImportStop(Activity activity, ulong? rows = null)
        => CopyStop(activity, rows);

    internal static void ExportStop(Activity activity, ulong? rows = null)
        => CopyStop(activity, rows);

    internal static void SetCancelled(Activity activity)
    {
        activity.SetStatus(ActivityStatusCode.Error, "Cancelled");
        activity.Dispose();
    }

    static string GetLibraryVersion()
        => typeof(NpgsqlDataSource).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "UNKNOWN";
}
