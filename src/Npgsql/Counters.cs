using System;
using Npgsql.Logging;

#if NET461
using System.Diagnostics;
using System.Reflection;
#endif

namespace Npgsql
{
    static class Counters
    {
        /// <summary>
        /// The number of connections per second that are being made to a database server.
        /// </summary>
        internal static readonly Counter HardConnectsPerSecond = new Counter(nameof(HardConnectsPerSecond));
        /// <summary>
        /// The number of disconnects per second that are being made to a database server.
        /// </summary>
        internal static readonly Counter HardDisconnectsPerSecond = new Counter(nameof(HardDisconnectsPerSecond));
        /// <summary>
        /// The total number of connection pools.
        /// </summary>
        internal static readonly Counter NumberOfActiveConnectionPools = new Counter(nameof(NumberOfActiveConnectionPools));
        /// <summary>
        /// The number of (pooled) active connections that are currently in use.
        /// </summary>
        internal static readonly Counter NumberOfActiveConnections = new Counter(nameof(NumberOfActiveConnections));
        /// <summary>
        /// The number of connections available for use in the connection pools.
        /// </summary>
        internal static readonly Counter NumberOfFreeConnections = new Counter(nameof(NumberOfFreeConnections));
        /// <summary>
        /// The number of active connections that are not pooled.
        /// </summary>
        internal static readonly Counter NumberOfNonPooledConnections = new Counter(nameof(NumberOfNonPooledConnections));
        /// <summary>
        /// The number of active connections that are being managed by the connection pooling infrastructure.
        /// </summary>
        internal static readonly Counter NumberOfPooledConnections = new Counter(nameof(NumberOfPooledConnections));
        /// <summary>
        /// The number of active connections being pulled from the connection pool.
        /// </summary>
        internal static readonly Counter SoftConnectsPerSecond = new Counter(nameof(SoftConnectsPerSecond));
        /// <summary>
        /// The number of active connections that are being returned to the connection pool.
        /// </summary>
        internal static readonly Counter SoftDisconnectsPerSecond = new Counter(nameof(SoftDisconnectsPerSecond));

        static bool _initialized;
        static readonly object InitLock = new object();

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(Counters));

#pragma warning disable CA1801 // Review unused parameters
        internal static void Initialize(bool usePerfCounters)
        {
            lock (InitLock)
            {
                if (_initialized)
                    return;
                _initialized = true;
                var enabled = false;
                var expensiveEnabled = false;

                if (usePerfCounters)
                {
#if NET461
                    try
                    {
                        enabled = PerformanceCounterCategory.Exists(Counter.DiagnosticsCounterCategory);
                        if (!enabled)
                            Log.Warn($"{nameof(NpgsqlConnectionStringBuilder.UsePerfCounters)} was specified but the Performance Counter category wasn't found. You probably need to install the Npgsql MSI.");
                        var perfCtrSwitch = new TraceSwitch("ConnectionPoolPerformanceCounterDetail",
                            "level of detail to track with connection pool performance counters");
                        expensiveEnabled = enabled && perfCtrSwitch.Level == TraceLevel.Verbose;
                    }
                    catch (Exception e)
                    {
                        Log.Debug("Exception while checking for performance counter category (counters will be disabled)", e);
                    }
#else
                    throw new NotSupportedException("The legacy Windows Performance Counters are only supported on .NET Framework. " +
                                                    "The new .NET Core performance counter feature (EventSource) doesn't require any connection string parameters.");
#endif
                }

                try
                {
                    HardConnectsPerSecond.Initialize(enabled);
                    HardDisconnectsPerSecond.Initialize(enabled);
                    NumberOfActiveConnectionPools.Initialize(enabled);
                    NumberOfNonPooledConnections.Initialize(enabled);
                    NumberOfPooledConnections.Initialize(enabled);
                    SoftConnectsPerSecond.Initialize(expensiveEnabled);
                    SoftDisconnectsPerSecond.Initialize(expensiveEnabled);
                    NumberOfActiveConnections.Initialize(expensiveEnabled);
                    NumberOfFreeConnections.Initialize(expensiveEnabled);
                }
                catch (Exception e)
                {
                    Log.Debug("Exception while setting up performance counter (counters will be disabled)", e);
                }
            }
        }
    }
#pragma warning restore CA1801 // Review unused parameters

    /// <summary>
    /// This class is currently a simple wrapper around System.Diagnostics.PerformanceCounter.
    /// Since these aren't supported in .NET Standard, all the ifdef'ing happens here.
    /// When an alternative performance counter API emerges for netstandard, it can be added here.
    /// </summary>
    sealed class Counter : IDisposable
    {
#if NET461
        internal const string DiagnosticsCounterCategory = ".NET Data Provider for PostgreSQL (Npgsql)";

        internal PerformanceCounter? DiagnosticsCounter { get; private set; }
#endif
        public string Name { get; }

        internal Counter(string diagnosticsCounterName)
            => Name = diagnosticsCounterName;

        internal void Initialize(bool enabled)
        {
#if NET461
            if (!enabled)
                return;

            DiagnosticsCounter = new PerformanceCounter
            {
                CategoryName = DiagnosticsCounterCategory,
                CounterName = Name,
                InstanceName = InstanceName,
                InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                ReadOnly = false,
                RawValue = 0
            };

            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
#endif
        }

        internal void Increment()
        {
#if NET461
            DiagnosticsCounter?.Increment();
#endif
        }

        internal void Decrement()
        {
#if NET461
            DiagnosticsCounter?.Decrement();
#endif
        }

        public void Dispose()
        {
#if NET461
            var diagnosticsCounter = DiagnosticsCounter;
            DiagnosticsCounter = null;
            diagnosticsCounter?.RemoveInstance();
#endif
        }

#if NET461
        void OnProcessExit(object sender, EventArgs e) => Dispose();
        void OnDomainUnload(object sender, EventArgs e) => Dispose();
        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
                Dispose();
        }

        const int CounterInstanceNameMaxLength = 127;

        static readonly string InstanceName = GetInstanceName();

        static string GetInstanceName()
        {
            string? result = null;

            // Code borrowed from the .NET reference sources

            var instanceName = Assembly.GetEntryAssembly()?.GetName().Name;
            if (string.IsNullOrEmpty(instanceName))
                instanceName = AppDomain.CurrentDomain.FriendlyName;

            var pid = Process.GetCurrentProcess().Id;

            result = string.Format(null, "{0}[{1}]", instanceName, pid);
            result = result.Replace('(', '[').Replace(')', ']').Replace('#', '_').Replace('/', '_').Replace('\\', '_');

            if (result.Length > CounterInstanceNameMaxLength)
            {
                // Replacing the middle part with "[...]"
                // For example: if path is c:\long_path\very_(Ax200)_long__path\perftest.exe and process ID is 1234 than the resulted instance name will be:
                // c:\long_path\very_(AxM)[...](AxN)_long__path\perftest.exe[1234]
                // while M and N are adjusted to make each part before and after the [...] = 61 (making the total = 61 + 5 + 61 = 127)
                const string insertString = "[...]";
                var firstPartLength = (CounterInstanceNameMaxLength - insertString.Length) / 2;
                var lastPartLength = CounterInstanceNameMaxLength - firstPartLength - insertString.Length;
                result = string.Format(null, "{0}{1}{2}",
                    result.Substring(0, firstPartLength),
                    insertString,
                    result.Substring(result.Length - lastPartLength, lastPartLength));

                Debug.Assert(result.Length == CounterInstanceNameMaxLength,
                    string.Format(null, "wrong calculation of the instance name: expected {0}, actual: {1}", CounterInstanceNameMaxLength, result.Length));
            }

            return result;
        }
#endif

        public override string ToString() => Name;
    }
}
