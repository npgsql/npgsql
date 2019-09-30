using System;
using System.Globalization;
using System.Threading;

// This type has been moved to the Npgsql.Util namespace for 5.0, but is kept here for backwards compatibility
// with 4.0.

namespace Npgsql
{
    /// <summary>
    /// Represents a timeout that will expire at some point.
    /// </summary>
    public readonly struct NpgsqlTimeout
    {
        readonly DateTime _expiration;
        internal DateTime Expiration => _expiration;

        internal static NpgsqlTimeout Infinite = new NpgsqlTimeout(TimeSpan.Zero);

        internal NpgsqlTimeout(TimeSpan expiration)
        {
            _expiration = expiration == TimeSpan.Zero
                ? DateTime.MaxValue
                : DateTime.UtcNow + expiration;
        }

        internal void Check()
        {
            if (HasExpired)
                throw new TimeoutException();
        }

        internal bool IsSet => _expiration != DateTime.MaxValue;

        internal bool HasExpired => DateTime.UtcNow >= Expiration;

        internal TimeSpan TimeLeft => IsSet ? Expiration - DateTime.UtcNow : Timeout.InfiniteTimeSpan;
    }

    sealed class CultureSetter : IDisposable
    {
        readonly CultureInfo _oldCulture;

        internal CultureSetter(CultureInfo newCulture)
        {
            _oldCulture = CultureInfo.CurrentCulture;
#if NET461
            Thread.CurrentThread.CurrentCulture = newCulture;
#else
            CultureInfo.CurrentCulture = newCulture;
#endif
        }

        public void Dispose()
        {
#if NET461
            Thread.CurrentThread.CurrentCulture = _oldCulture;
#else
            CultureInfo.CurrentCulture = _oldCulture;
#endif
        }
    }
}
