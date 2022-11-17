using Npgsql.Internal;
using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Npgsql;

static class NpgsqlActivitySource
{
    static readonly ActivitySource Source;

    static NpgsqlActivitySource()
    {
        var assembly = typeof(NpgsqlActivitySource).Assembly;
        var version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "0.0.0";
        Source = new("Npgsql", version);
    }

    internal static bool IsEnabled => Source.HasListeners();

    internal static Activity? CommandStart(NpgsqlConnector connector, string commandText, CommandType commandType)
    {
        var settings = connector.Settings;

        var dbName = settings.Database ?? connector.InferredUserName;
        string? dbOperation = null;
        string? dbSqlTable = null;
        string activityName;
        switch (commandType)
        {
        case CommandType.StoredProcedure:
            dbOperation = NpgsqlCommand.EnableStoredProcedureCompatMode ? "SELECT" : "CALL";
            // In this case our activity name follows the concept of the CommandType.TableDirect case
            // ("<db.operation> <db.name>.<db.sql.table>") but replaces db.sql.table with the procedure name
            // which seems to match the spec's intent without being explicitly specified that way (it suggests
            // using the procedure name but doesn't mention using db.operation or db.name in that case).
            activityName = $"{dbOperation} {dbName}.{commandText}";
            break;
        case CommandType.TableDirect:
            dbOperation = "SELECT";
            // The OpenTelemetry spec actually asks to include the database name into db.sql.table
            // but then again mixes the concept of database and schema.
            // As I interpret it, it actually wants db.sql.table to include the schema name and not the
            // database name if the concept of schemas exists in the database system.
            // This also makes sense in the context of the activity name which otherwise would include the
            // database name twice.
            dbSqlTable = commandText;
            activityName = $"{dbOperation} {dbName}.{dbSqlTable}";
            break;
        case CommandType.Text:
            activityName = dbName;
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
        }

        var activity = Source.StartActivity(activityName, ActivityKind.Client);
        if (activity is not { IsAllDataRequested: true })
            return activity;

        activity.SetTag("db.system", "postgresql");
        activity.SetTag("db.connection_string", connector.UserFacingConnectionString);
        activity.SetTag("db.user", connector.InferredUserName);
        // We trace the actual (maybe inferred) database name we're connected to, even if it
        // wasn't specified in the connection string
        activity.SetTag("db.name", dbName);
        activity.SetTag("db.statement", commandText);
        activity.SetTag("db.connection_id", connector.Id);
        if (dbOperation != null)
            activity.SetTag("db.operation", dbOperation);
        if (dbSqlTable != null)
            activity.SetTag("db.sql.table", dbSqlTable);

        var endPoint = connector.ConnectedEndPoint;
        Debug.Assert(endPoint is not null);
        switch (endPoint)
        {
        case IPEndPoint ipEndPoint:
            activity.SetTag("net.transport", "ip_tcp");
            activity.SetTag("net.peer.ip", ipEndPoint.Address.ToString());
            if (ipEndPoint.Port != 5432)
                activity.SetTag("net.peer.port", ipEndPoint.Port);
            activity.SetTag("net.peer.name", settings.Host);
            break;

        case UnixDomainSocketEndPoint:
            activity.SetTag("net.transport", "unix");
            activity.SetTag("net.peer.name", settings.Host);
            break;

        default:
            throw new ArgumentOutOfRangeException("Invalid endpoint type: " + endPoint.GetType());
        }

        return activity;
    }

    internal static void ReceivedFirstResponse(Activity activity)
    {
        var activityEvent = new ActivityEvent("received-first-response");
        activity.AddEvent(activityEvent);
    }

    internal static void CommandStop(Activity activity)
    {
        activity.SetTag("otel.status_code", "OK");
        activity.Dispose();
    }

    internal static void SetException(Activity activity, Exception ex, bool escaped = true)
    {
        var tags = new ActivityTagsCollection
        {
            { "exception.type", ex.GetType().FullName },
            { "exception.message", ex.Message },
            { "exception.stacktrace", ex.ToString() },
            { "exception.escaped", escaped }
        };
        var activityEvent = new ActivityEvent("exception", tags: tags);
        activity.AddEvent(activityEvent);
        activity.SetTag("otel.status_code", "ERROR");
        activity.SetTag("otel.status_description", ex is PostgresException pgEx ? pgEx.SqlState : ex.Message);
        activity.Dispose();
    }
}