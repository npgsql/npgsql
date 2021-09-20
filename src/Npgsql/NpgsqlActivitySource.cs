﻿using Npgsql.Internal;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace Npgsql
{
    static class NpgsqlActivitySource
    {
#if NET5_0_OR_GREATER
        static readonly ActivitySource Source;

        static NpgsqlActivitySource()
        {
            var assembly = typeof(NpgsqlActivitySource).Assembly;
            var version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version;
            Source = new("Npgsql", version);
        }

        public static bool IsEnabled => Source.HasListeners();

        public static IDisposable? CommandStart(NpgsqlConnector connector, string sql)
        {
            var settings = connector.Settings;
            var activity = Source.StartActivity(settings.Database!, ActivityKind.Client);
            if (activity is not Activity { IsAllDataRequested: true })
                return activity;

            activity.SetTag("db.system", "postgresql");
            activity.SetTag("db.connection_string", connector.UserFacingConnectionString);
            activity.SetTag("db.user", settings.Username);
            activity.SetTag("db.name", settings.Database);
            activity.SetTag("db.statement", sql);
            activity.SetTag("db.connection_id", connector.Id);

            var endPoint = connector.ConnectedEndPoint;
            Debug.Assert(endPoint is not null);
            if (endPoint is IPEndPoint ipEndPoint)
            {
                activity.SetTag("net.transport", "ip_tcp");
                activity.SetTag("net.peer.ip", ipEndPoint.Address.ToString());
                if (ipEndPoint.Port != 5432)
                    activity.SetTag("net.peer.port", ipEndPoint.Port);
                activity.SetTag("net.peer.name", settings.Host);
            }
            else
            {
                activity.SetTag("net.transport", "unix");
                activity.SetTag("net.peer.name", settings.Host);
            }

            return activity;
        }

        public static void CommandStop(IDisposable currentActivity)
        {
            if (currentActivity is not Activity activity)
                return;

            activity.SetTag("otel.status_code", "OK");
            activity.Dispose();
        }

        public static void SetException(IDisposable? currentActivity, Exception ex, bool escaped = true)
        {
            if (currentActivity is not Activity activity)
                return;

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
        }
#else
        public static bool IsEnabled => false;
        public static IDisposable? CommandStart(NpgsqlConnector connector, string sql) => null;
        public static void CommandStop(IDisposable currentActivity) { }
        public static void SetException(IDisposable? currentActivity, Exception ex, bool escaped = true) { }
#endif
    }
}
