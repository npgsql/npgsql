using Npgsql.Internal;
using System;
using System.Diagnostics;

namespace Npgsql
{
    static class NpgsqlActivitySource
    {
#if NET5_0_OR_GREATER
        static readonly ActivitySource Source = new("Npgsql");

        public static bool IsEnabled => Source.HasListeners();

        public static IDisposable? CommandStart(NpgsqlConnector connector, string sql)
        {
            var activity = Source.StartActivity(connector.Settings.Database!, ActivityKind.Client);
            if (activity is not null)
            {
                activity.SetTag("db.system", "postgresql");
                activity.SetTag("db.connection_string", connector.UserFacingConnectionString);
                activity.SetTag("db.user", connector.Settings.Username);
                activity.SetTag("db.name", connector.Settings.Database);
                activity.SetTag("db.statement", sql);
                activity.SetTag("db.connection.id", connector.Id);
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
