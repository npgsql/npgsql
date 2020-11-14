using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Util
{
    static class Statics
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T Expect<T>(IBackendMessage msg, NpgsqlConnector connector)
        {
            if (msg is T asT)
                return asT;

            throw connector.Break(
                new NpgsqlException($"Received backend message {msg.Code} while expecting {typeof(T).Name}. " +
                                    "Please file a bug."));
        }

        internal static DeferDisposable Defer(Action action) => new DeferDisposable(action);
        internal static DeferDisposable<T> Defer<T>(Action<T> action, T arg) => new DeferDisposable<T>(action, arg);
        internal static DeferDisposable<T1, T2> Defer<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2) => new DeferDisposable<T1, T2>(action, arg1, arg2);
        // internal static AsyncDeferDisposable DeferAsync(Func<ValueTask> func) => new AsyncDeferDisposable(func);
        internal static AsyncDeferDisposable DeferAsync(Func<Task> func) => new AsyncDeferDisposable(func);

        internal readonly struct DeferDisposable : IDisposable
        {
            readonly Action _action;
            public DeferDisposable(Action action) => _action = action;
            public void Dispose() => _action();
        }

        internal readonly struct DeferDisposable<T> : IDisposable
        {
            readonly Action<T> _action;
            readonly T _arg;
            public DeferDisposable(Action<T> action, T arg)
            {
                _action = action;
                _arg = arg;
            }
            public void Dispose() => _action(_arg);
        }

        internal readonly struct DeferDisposable<T1, T2> : IDisposable
        {
            readonly Action<T1, T2> _action;
            readonly T1 _arg1;
            readonly T2 _arg2;
            public DeferDisposable(Action<T1, T2> action, T1 arg1, T2 arg2)
            {
                _action = action;
                _arg1 = arg1;
                _arg2 = arg2;
            }
            public void Dispose() => _action(_arg1, _arg2);
        }

        internal readonly struct AsyncDeferDisposable : IAsyncDisposable
        {
            readonly Func<Task> _func;
            public AsyncDeferDisposable(Func<Task> func) => _func = func;
            public async ValueTask DisposeAsync() => await _func();
        }
    }

    // ReSharper disable once InconsistentNaming
    static class PGUtil
    {
        internal static readonly UTF8Encoding UTF8Encoding = new UTF8Encoding(false, true);
        internal static readonly UTF8Encoding RelaxedUTF8Encoding = new UTF8Encoding(false, false);

        internal const int BitsInInt = sizeof(int) * 8;

        internal static void ValidateBackendMessageCode(BackendMessageCode code)
        {
            switch (code)
            {
            case BackendMessageCode.AuthenticationRequest:
            case BackendMessageCode.BackendKeyData:
            case BackendMessageCode.BindComplete:
            case BackendMessageCode.CloseComplete:
            case BackendMessageCode.CommandComplete:
            case BackendMessageCode.CopyData:
            case BackendMessageCode.CopyDone:
            case BackendMessageCode.CopyBothResponse:
            case BackendMessageCode.CopyInResponse:
            case BackendMessageCode.CopyOutResponse:
            case BackendMessageCode.DataRow:
            case BackendMessageCode.EmptyQueryResponse:
            case BackendMessageCode.ErrorResponse:
            case BackendMessageCode.FunctionCall:
            case BackendMessageCode.FunctionCallResponse:
            case BackendMessageCode.NoData:
            case BackendMessageCode.NoticeResponse:
            case BackendMessageCode.NotificationResponse:
            case BackendMessageCode.ParameterDescription:
            case BackendMessageCode.ParameterStatus:
            case BackendMessageCode.ParseComplete:
            case BackendMessageCode.PasswordPacket:
            case BackendMessageCode.PortalSuspended:
            case BackendMessageCode.ReadyForQuery:
            case BackendMessageCode.RowDescription:
                return;
            default:
                throw new NpgsqlException("Unknown message code: " + code);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int RotateShift(int val, int shift)
            => (val << shift) | (val >> (BitsInInt - shift));

        internal static readonly Task<bool> TrueTask = Task.FromResult(true);
        internal static readonly Task<bool> FalseTask = Task.FromResult(false);

        internal static StringComparer InvariantCaseIgnoringStringComparer => StringComparer.InvariantCultureIgnoreCase;

        internal static bool IsWindows =>
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
    }

    enum FormatCode : short
    {
        Text = 0,
        Binary = 1
    }

    static class EnumerableExtensions
    {
        internal static string Join(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }
    }

    static class ExceptionExtensions
    {
        internal static Exception UnwrapAggregate(this Exception exception)
            => exception is AggregateException agg ? agg.InnerException! : exception;
    }

    /// <summary>
    /// Represents a timeout that will expire at some point.
    /// </summary>
    public readonly struct NpgsqlTimeout
    {
        readonly DateTime _expiration;
        internal DateTime Expiration => _expiration;

        internal static NpgsqlTimeout Infinite = new NpgsqlTimeout(TimeSpan.Zero);

        internal NpgsqlTimeout(TimeSpan expiration)
            => _expiration = expiration == TimeSpan.Zero ? DateTime.MaxValue : DateTime.UtcNow + expiration;

        internal void Check()
        {
            if (HasExpired)
                throw new TimeoutException();
        }

        internal void CheckAndApply(NpgsqlConnector connector)
        {
            if (!IsSet)
                return;

            var timeLeft = TimeLeft;
            if (timeLeft > TimeSpan.Zero)
            {
                // Set the remaining timeout on the read and write buffers
                connector.ReadBuffer.Timeout = connector.WriteBuffer.Timeout = timeLeft;

                // Note that we set UserTimeout as well, otherwise the read timeout will get overwritten in ReadMessage
                // Note also that we must set the read buffer's timeout directly (above), since the SSL handshake
                // reads data directly from the buffer, without going through ReadMessage.
                connector.UserTimeout = (int)Math.Ceiling(timeLeft.TotalMilliseconds);
            }

            Check();
        }

        internal bool IsSet => _expiration != DateTime.MaxValue;

        internal bool HasExpired => DateTime.UtcNow >= Expiration;

        internal TimeSpan TimeLeft => IsSet ? Expiration - DateTime.UtcNow : Timeout.InfiniteTimeSpan;
    }

    static class MethodInfos
    {
        internal static readonly ConstructorInfo InvalidCastExceptionCtor =
            typeof(InvalidCastException).GetConstructor(new[] { typeof(string) })!;

        internal static readonly MethodInfo StringFormat =
            typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object) })!;

        internal static readonly MethodInfo ObjectGetType =
            typeof(object).GetMethod(nameof(GetType), new Type[0])!;
    }
}
