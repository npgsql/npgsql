﻿using Npgsql.Internal;
using System;
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

    internal static NpgsqlTracingOptions Options { get; set; } = new();

    internal static Activity? CommandStart(NpgsqlConnector connector, NpgsqlCommand command)
    {
        var settings = connector.Settings;
        var activity = Source.StartActivity(settings.Database!, ActivityKind.Client);
        if (activity is not { IsAllDataRequested: true })
            return activity;

        activity.SetTag("db.system", "postgresql");
        activity.SetTag("db.connection_string", connector.UserFacingConnectionString);
        activity.SetTag("db.user", settings.Username);
        activity.SetTag("db.name", settings.Database);
        activity.SetTag("db.statement", command.CommandText);
        activity.SetTag("db.connection_id", connector.Id);

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

        Options.EnrichCommandExecution?.Invoke(activity, "OnStartActivity", command);

        return activity;
    }

    internal static void ReceivedFirstResponse(Activity activity, NpgsqlCommand command)
    {
        if (activity.IsAllDataRequested)
        {
            if (Options.RecordCommandExecutionFirstResponse)
            {
                var activityEvent = new ActivityEvent("received-first-response");
                activity.AddEvent(activityEvent);
            }

            Options.EnrichCommandExecution?.Invoke(activity, "OnFirstResponse", command);
        }
    }

    internal static void CommandStop(Activity activity, NpgsqlCommand command)
    {
        activity.SetTag("otel.status_code", "OK");
        activity.SetEndTime(DateTime.UtcNow);
        if (activity.IsAllDataRequested)
        {
            Options.EnrichCommandExecution?.Invoke(activity, "OnStopActivity", command);
        }

        activity.Dispose();
    }

    internal static void SetException(Activity activity, NpgsqlCommand command, Exception ex, bool escaped = true)
    {
        if (activity.IsAllDataRequested && Options.RecordCommandExecutionException)
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
        }

        activity.SetTag("otel.status_code", "ERROR");
        activity.SetTag("otel.status_description", ex is PostgresException pgEx ? pgEx.SqlState : ex.Message);
        activity.SetEndTime(DateTime.UtcNow);
        if (activity.IsAllDataRequested)
        {
            Options.EnrichCommandExecution?.Invoke(activity, "OnException", (command, ex));
        }

        activity.Dispose();
    }
}