#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsyncRewriter;

namespace Npgsql
{
    // ReSharper disable once InconsistentNaming
    static class PGUtil
    {
        internal static readonly byte[] EmptyBuffer = new byte[0];

        internal static readonly UTF8Encoding UTF8Encoding = new UTF8Encoding(false, true);
        internal static readonly UTF8Encoding RelaxedUTF8Encoding = new UTF8Encoding(false, false);

        internal static void ValidateBackendMessageCode(BackendMessageCode code)
        {
            switch (code)
            {
            case BackendMessageCode.AuthenticationRequest:
            case BackendMessageCode.BackendKeyData:
            case BackendMessageCode.BindComplete:
            case BackendMessageCode.CloseComplete:
            case BackendMessageCode.CompletedResponse:
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

        public static int RotateShift(int val, int shift)
        {
            return (val << shift) | (val >> (sizeof (int) - shift));
        }

        /// <summary>
        /// Creates a Task&lt;TResult&gt; that's completed successfully with the specified result.
        /// </summary>
        /// <remarks>
        /// In .NET 4.5 Task provides this. In .NET 4.0 with BCL.Async, TaskEx provides this. This
        /// method wraps the two.
        /// </remarks>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="result">The result to store into the completed task.</param>
        /// <returns>The successfully completed task.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Task<TResult> TaskFromResult<TResult>(TResult result)
        {
            return Task.FromResult(result);
        }

        internal static readonly Task CompletedTask = TaskFromResult(0);

#if NET45 || NET451
        internal static StringComparer InvariantCaseIgnoringStringComparer => StringComparer.InvariantCultureIgnoreCase;
#else
        internal static StringComparer InvariantCaseIgnoringStringComparer => CultureInfo.InvariantCulture.CompareInfo.GetStringComparer(CompareOptions.IgnoreCase);
#endif

        internal static bool IsWindows =>
#if NET45 || NET451
            Environment.OSVersion.Platform == PlatformID.Win32NT;
#else
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
#endif
    }

    enum FormatCode : short
    {
        Text = 0,
        Binary = 1
    }

    internal static class EnumerableExtensions
    {
        internal static string Join(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }
    }

    /// <summary>
    /// Represents a timeout that will expire at some point.
    /// </summary>
    internal struct NpgsqlTimeout
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
#if NET45 || NET451
            Thread.CurrentThread.CurrentCulture = newCulture;
#else
            CultureInfo.CurrentCulture = newCulture;
#endif
        }

        public void Dispose()
        {
#if NET45 || NET451
            Thread.CurrentThread.CurrentCulture = _oldCulture;
#else
            CultureInfo.CurrentCulture = _oldCulture;
#endif
        }
    }

    /// <summary>
    /// This mechanism is used to temporarily set the current synchronization context to null while
    /// executing Npgsql code, making all await continuations execute on the thread pool. This replaces
    /// the need to place ConfigureAwait(false) everywhere, and should be used in all surface async methods,
    /// without exception.
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/a/28307965/640325
    /// </remarks>
    static class NoSynchronizationContextScope
    {
        internal static Disposable Enter()
        {
            var sc = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            return new Disposable(sc);
        }

        internal struct Disposable : IDisposable
        {
            readonly SynchronizationContext _synchronizationContext;

            internal Disposable(SynchronizationContext synchronizationContext)
            {
                _synchronizationContext = synchronizationContext;
            }

            public void Dispose() => SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
        }
    }
}
