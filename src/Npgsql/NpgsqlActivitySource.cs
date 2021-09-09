using Npgsql.Internal;
using System;
using System.Diagnostics;
using System.Net;

namespace Npgsql
{
    static class NpgsqlActivitySource
    {
#if NET5_0_OR_GREATER
        static readonly ActivitySource Source = new("Npgsql");

        public static bool IsEnabled => Source.HasListeners();

        public static IDisposable? CommandStart(NpgsqlConnector connector, string sql)
        {
            var settings = connector.Settings;
            var activity = Source.StartActivity(settings.Database!, ActivityKind.Client);
            if (activity is not null)
            {
                activity.SetTag("db.system", "postgresql");
                activity.SetTag("db.connection_string", connector.UserFacingConnectionString);
                activity.SetTag("db.user", settings.Username);
                activity.SetTag("db.name", settings.Database);
                activity.SetTag("db.statement", sql);
                activity.SetTag("db.connection.id", connector.Id);

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
            }

            return activity;
        }

        public static void SetException(IDisposable? currentActivity, Exception ex, bool escaped = true)
        {
            if (currentActivity is Activity activity)
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
        }
#else
        public static bool IsEnabled => false;
        public static IDisposable? CommandStart(NpgsqlConnector connector, string sql) => null;
        public static void SetException(IDisposable? activity, Exception ex, bool escaped = true) { }
#endif
    }
}
