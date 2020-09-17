using System;

namespace Npgsql.Logging
{
    /// <summary>
    /// Manages logging for Npgsql, used to set the logging provider.
    /// </summary>
    public static class NpgsqlLogManager
    {
        /// <summary>
        /// The logging provider used for logging in Npgsql.
        /// </summary>
        public static INpgsqlLoggingProvider Provider
        {
            get
            {
                _providerRetrieved = true;
                return _provider;
            }
            set
            {
                if (_providerRetrieved)
                    throw new InvalidOperationException("The logging provider must be set before any Npgsql action is taken");

                _provider = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Determines whether parameter contents will be logged alongside SQL statements - this may reveal sensitive information.
        /// Defaults to false.
        /// </summary>
        public static bool IsParameterLoggingEnabled { get; set; }

        static INpgsqlLoggingProvider _provider = new NoOpLoggingProvider();
        static bool _providerRetrieved;

        internal static NpgsqlLogger CreateLogger(string name) => Provider.CreateLogger("Npgsql." + name);
    }
}
