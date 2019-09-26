using System;
using System.Diagnostics;

namespace Npgsql.Tests
{
    /// <summary>
    /// Keep track of metrics related to performance.
    /// </summary>
    internal sealed class TestMetrics : IDisposable
    {
        private static Process process = Process.GetCurrentProcess();

        private bool running;
        /// <summary>
        /// The number of iterations accumulated.
        /// </summary>
        public int Iterations { get; private set; }
        private TimeSpan systemCPUTime;
        private TimeSpan userCPUTime;
        private Stopwatch stopwatch;
        private TimeSpan allowedTime;

        private bool reportOnStop;

        private TestMetrics(TimeSpan allowedTime, bool reportOnStop)
        {
            Iterations = 0;
            systemCPUTime = process.PrivilegedProcessorTime;
            userCPUTime = process.UserProcessorTime;
            stopwatch = Stopwatch.StartNew();
            this.allowedTime = allowedTime;
            this.reportOnStop = reportOnStop;

            running = true;
        }

        /// <summary>
        /// Construct and start a new TestMetrics object.
        /// </summary>
        /// <param name="allowedTime">Length of time the test should run.</param>
        /// <param name="reportOnStop">Report metrics to stdout when stopped.</param>
        /// <returns>A new running TestMetrics object.</returns>
        public static TestMetrics Start(TimeSpan allowedTime, bool reportOnStop)
        {
            return new TestMetrics(allowedTime, reportOnStop);
        }

        /// <summary>
        /// Incremnent the Iterations value by one.
        /// </summary>
        public void IncrementIterations()
        {
            Iterations++;
        }

        /// <summary>
        /// Stop the internal stop watch and record elapsed CPU times.
        /// </summary>
        public void Stop()
        {
            if (! running)
            {
                return;
            }

            stopwatch.Stop();
            systemCPUTime = process.PrivilegedProcessorTime - systemCPUTime;
            userCPUTime = process.UserProcessorTime - userCPUTime;

            running = false;

            if (reportOnStop)
            {
                Console.WriteLine("Elapsed: {0:mm\\:ss\\.ff}", ElapsedClockTime);
                Console.WriteLine("CPU: {0:mm\\:ss\\.ffffff} (User: {1:mm\\:ss\\.ffffff}, System: {2:mm\\:ss\\.ffffff})", ElapsedTotalCPUTime, ElapsedUserCPUTime, ElapsedSystemCPUTime);
                Console.WriteLine("Iterations: {0}; {1:0.00}/second, {2:0.00}/CPU second", Iterations, IterationsPerSecond(), IterationsPerCPUSecond());
            }
        }

        /// <summary>
        /// Stop the internal stop watch and record elapsed CPU times.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Report whether ElapsedClockTime has met or exceeded the maximum run time.
        /// </summary>
        public bool TimesUp => (stopwatch.Elapsed >= allowedTime);

        /// <summary>
        /// Calculate the number of iterations accumulated per the time span provided.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>The number of iterations accumulated per the time span provided.</returns>
        public double IterationsPer(TimeSpan timeSpan)
        {
            return (double)Iterations / ((double)stopwatch.Elapsed.TotalMilliseconds / (double)timeSpan.TotalMilliseconds);
        }

        /// <summary>
        /// Calculate the number of iterations accumulated per second.
        /// Equivelent to calling IterationsPer(new TimeSpan(0, 0, 1)).
        /// </summary>
        /// <returns>The number of iterations accumulated per second.</returns>
        public double IterationsPerSecond()
        {
            return IterationsPer(new TimeSpan(0, 0, 1));
        }

        /// <summary>
        /// Calculate the number of iterations accumulated per the CPU time span provided.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>The number of iterations accumulated per the CPU time span provided.</returns>
        public double IterationsPerCPU(TimeSpan timeSpan)
        {
            return (double)Iterations / ((double)ElapsedTotalCPUTime.TotalMilliseconds / (double)timeSpan.TotalMilliseconds);
        }

        /// <summary>
        /// Calculate the number of iterations accumulated per CPU second.
        /// Equivelent to calling IterationsPerCPU(new TimeSpan(0, 0, 1)).
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>The number of iterations accumulated per CPU second.</returns>
        public double IterationsPerCPUSecond()
        {
            return IterationsPerCPU(new TimeSpan(0, 0, 1));
        }

        /// <summary>
        /// Elapsed time since start.
        /// </summary>
        public TimeSpan ElapsedClockTime => stopwatch.Elapsed;

        /// <summary>
        /// Elapsed system CPU time since start.
        /// </summary>
        public TimeSpan ElapsedSystemCPUTime
        {
            get
            {
                if (running)
                {
                    return process.PrivilegedProcessorTime - systemCPUTime;
                }
                else
                {
                    return systemCPUTime;
                }
            }
        }

        /// <summary>
        /// Elapsed user CPU time since start.
        /// </summary>
        public TimeSpan ElapsedUserCPUTime
        {
            get
            {
                if (running)
                {
                    return process.UserProcessorTime - userCPUTime;
                }
                else
                {
                    return userCPUTime;
                }
            }
        }

        /// <summary>
        /// Elapsed total (system + user) CPU time since start.
        /// </summary>
        public TimeSpan ElapsedTotalCPUTime => ElapsedSystemCPUTime + ElapsedUserCPUTime;
    }
}
