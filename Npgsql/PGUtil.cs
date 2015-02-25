// created on 1/6/2002 at 22:27

// Npgsql.PGUtil.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Npgsql.Localization;

namespace Npgsql
{
    // ReSharper disable once InconsistentNaming
    internal static class PGUtil
    {
        internal static readonly UTF8Encoding UTF8Encoding = new UTF8Encoding(false, true);

        /// <summary>
        /// This method takes a version string as returned by SELECT VERSION() and returns
        /// a valid version string ("7.2.2" for example).
        /// This is only needed when running protocol version 2.
        /// This does not do any validity checks.
        /// </summary>
        public static string ExtractServerVersion(string VersionString)
        {
            Int32 Start = 0, End = 0;

            // find the first digit and assume this is the start of the version number
            for (; Start < VersionString.Length && !Char.IsDigit(VersionString[Start]); Start++)
            {
                ;
            }

            End = Start;

            // read until hitting whitespace, which should terminate the version number
            for (; End < VersionString.Length && !Char.IsWhiteSpace(VersionString[End]); End++)
            {
                ;
            }

            // Deal with this here so that if there are
            // changes in a future backend version, we can handle it here in the
            // protocol handler and leave everybody else put of it.

            VersionString = VersionString.Substring(Start, End - Start + 1);

            for (int idx = 0; idx != VersionString.Length; ++idx)
            {
                char c = VersionString[idx];
                if (!Char.IsDigit(c) && c != '.')
                {
                    VersionString = VersionString.Substring(0, idx);
                    break;
                }
            }

            return VersionString;
        }

        /// <summary>
        /// Write a 32-bit integer to the given stream in the correct byte order.
        /// </summary>
        [GenerateAsync]
        public static Stream WriteInt32(this Stream stream, Int32 value)
        {
            stream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)), 0, 4);

            return stream;
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
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static Task<TResult> TaskFromResult<TResult>(TResult result)
        {
#if NET45
            return Task.FromResult(result);
#else
            return TaskEx.FromResult(result);
#endif
        }

        /// <summary>
        /// Throws an exception with the given string and also invokes a contract failure, allowing the static checker
        /// to detect scenarios leading up to this error.
        ///
        /// See http://blogs.msdn.com/b/francesco/archive/2014/09/12/how-to-use-cccheck-to-prove-no-case-is-forgotten.aspx
        /// </summary>
        /// <param name="message">the exception message</param>
        /// <returns>an exception to be thrown</returns>
        [ContractVerification(false)]
        public static Exception ThrowIfReached(string message = null)
        {
            Contract.Requires(false);
            return message == null ? new Exception() : new Exception(message);
        }
    }

    /// <summary>
    /// Represent the frontend/backend protocol version.
    /// </summary>
    public enum ProtocolVersion
    {
        Unknown  = 0,
        Version3 = 3
    }

    internal enum FormatCode : short
    {
        Text = 0,
        Binary = 1
    }

    internal class NpgsqlNetworkStream : NetworkStream
    {
        NpgsqlConnector mContext = null;

        public NpgsqlNetworkStream(Socket socket, Boolean owner)
            : base(socket, owner)
        {
        }

        public void AttachConnector(NpgsqlConnector context)
        {
            mContext = context;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                if (mContext != null)
                {
                    mContext.Close();
                    mContext = null;
                }
            }

            base.Dispose(disposing);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class GenerateAsync : Attribute
    {
        public GenerateAsync(string transformedName=null, bool withOverride=false) {}
    }
}
