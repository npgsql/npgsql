using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Npgsql.Logging
{
    /// <summary>
    /// Manages logging for Npgsql, used to set the loggging provider.
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

                _provider = value;
            }
        }

        static INpgsqlLoggingProvider _provider;
        static bool _providerRetrieved;

        static internal NpgsqlLogger CreateLogger(string name)
        {
            return Provider.CreateLogger(name);
        }

        static internal NpgsqlLogger GetCurrentClassLogger()
        {
            return CreateLogger(GetClassFullName());
        }

        // Copied from NLog
        static string GetClassFullName()
        {
            string className;
            Type declaringType;
            int framesToSkip = 2;

            do {
#if SILVERLIGHT
                StackFrame frame = new StackTrace().GetFrame(framesToSkip);
#else
                StackFrame frame = new StackFrame(framesToSkip, false);
#endif
                MethodBase method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null) {
                    className = method.Name;
                    break;
                }

                framesToSkip++;
                className = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return className;
        }

        static NpgsqlLogManager()
        {
            Provider = new NoOpLoggingProvider();
        }
    }
}
