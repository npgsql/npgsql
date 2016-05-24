#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Npgsql
{
    /// <summary>
    /// A class to handle everything associated with SSPI authentication
    /// </summary>
    internal class SSPIHandler : IDisposable
    {
        #region Constants and Structs

        const int SecbufferVersion = 0;
        const int SecbufferToken = 2;
        const int SecEOk = 0x00000000;
        const int SecIContinueNeeded = 0x00090312;
        const int IscReqAllocateMemory = 0x00000100;
        const int SecurityNetworkDrep = 0x00000000;
        const int SecpkgCredOutbound = 0x00000002;

        [StructLayout(LayoutKind.Sequential)]
        struct SecHandle
        {
            internal IntPtr dwLower;
            internal IntPtr dwUpper;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SecBuffer
        {
            internal int cbBuffer;
            internal int BufferType;
            internal IntPtr pvBuffer;
        }

        /// <summary>
        /// Simplified SecBufferDesc struct with only one SecBuffer
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SecBufferDesc
        {
            internal int ulVersion;
            internal int cBuffers;
            internal IntPtr pBuffer;
        }

        #endregion

        #region P/Invoke methods

        // ReSharper disable InconsistentNaming

        [DllImport("Secur32.dll")]
        static extern int AcquireCredentialsHandle(
            string pszPrincipal,
            string pszPackage,
            int fCredentialUse,
            IntPtr pvLogonID,
            IntPtr pAuthData,
            IntPtr pGetKeyFn,
            IntPtr pvGetKeyArgument,
            ref SecHandle phCredential,
            out SecHandle ptsExpiry
        );

        [DllImport("secur32", SetLastError=true)]
        static extern int InitializeSecurityContext(
            ref SecHandle phCredential,
            ref SecHandle phContext,
            string pszTargetName,
            int fContextReq,
            int Reserved1,
            int TargetDataRep,
            ref SecBufferDesc pInput,
            int Reserved2,
            out SecHandle phNewContext,
            out SecBufferDesc pOutput,
            out int pfContextAttr,
            out SecHandle ptsExpiry);

        [DllImport("secur32", SetLastError=true)]
        static extern int InitializeSecurityContext(
            ref SecHandle phCredential,
            IntPtr phContext,
            string pszTargetName,
            int fContextReq,
            int Reserved1,
            int TargetDataRep,
            IntPtr pInput,
            int Reserved2,
            out SecHandle phNewContext,
            out SecBufferDesc pOutput,
            out int pfContextAttr,
            out SecHandle ptsExpiry);

        [DllImport("Secur32.dll")]
        static extern int FreeContextBuffer(
            IntPtr pvContextBuffer
        );

        [DllImport("Secur32.dll")]
        static extern int FreeCredentialsHandle(
            ref SecHandle phCredential
        );

        [DllImport("Secur32.dll")]
        static extern int DeleteSecurityContext(
            ref SecHandle phContext
        );

        // ReSharper restore InconsistentNaming

        #endregion

        bool _disposed;
        readonly string _sspiTarget;
        SecHandle _sspiCred;
        SecHandle _sspiCtx;
        bool _isSSPICtxSet;

        internal SSPIHandler(string pghost, [CanBeNull] string krbsrvname, bool useGssapi)
        {
            if (pghost == null)
                throw new ArgumentNullException(nameof(pghost));
            if (krbsrvname == null)
                krbsrvname = string.Empty;
            _sspiTarget = $"{krbsrvname}/{pghost}";

            SecHandle expire;
            var status = AcquireCredentialsHandle(
                "",
                useGssapi ? "kerberos" : "negotiate",
                SecpkgCredOutbound,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                ref _sspiCred,
                out expire
            );
            if (status != SecEOk)
            {
                // This will automaticcaly fill in the message of the last Win32 error
                throw new Win32Exception();
            }
        }

        internal byte[] Continue([CanBeNull] byte[] authData)
        {
            if (authData == null && _isSSPICtxSet)
                throw new InvalidOperationException("The authData parameter con only be null at the first call to continue!");

            var outBuffer = new SecBuffer
            {
                pvBuffer = IntPtr.Zero,
                BufferType = SecbufferToken,
                cbBuffer = 0
            };

            var outbuf = new SecBufferDesc
            {
                cBuffers = 1,
                ulVersion = SecbufferVersion,
                pBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(outBuffer))
            };

            try
            {
                int status;
                SecHandle newContext;
                SecHandle expire;
                int contextAttr;

                Marshal.StructureToPtr(outBuffer, outbuf.pBuffer, false);

                if (_isSSPICtxSet)
                {
                    Debug.Assert(authData != null);
                    var inbuf = new SecBufferDesc { pBuffer = IntPtr.Zero };
                    var inBuffer = new SecBuffer { pvBuffer = Marshal.AllocHGlobal(authData.Length) };
                    try
                    {
                        Marshal.Copy(authData, 0, inBuffer.pvBuffer, authData.Length);
                        inBuffer.cbBuffer = authData.Length;
                        inBuffer.BufferType = SecbufferToken;
                        inbuf.ulVersion = SecbufferVersion;
                        inbuf.cBuffers = 1;
                        inbuf.pBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(inBuffer));
                        Marshal.StructureToPtr(inBuffer, inbuf.pBuffer, false);
                        status = InitializeSecurityContext(
                            ref _sspiCred,
                            ref _sspiCtx,
                            _sspiTarget,
                            IscReqAllocateMemory,
                            0,
                            SecurityNetworkDrep,
                            ref inbuf,
                            0,
                            out newContext,
                            out outbuf,
                            out contextAttr,
                            out expire
                        );
                    }
                    finally
                    {
                        if (inBuffer.pvBuffer != IntPtr.Zero)
                            Marshal.FreeHGlobal(inBuffer.pvBuffer);
                        if (inbuf.pBuffer != IntPtr.Zero)
                            Marshal.FreeHGlobal(inbuf.pBuffer);
                    }
                }
                else
                {
                    status = InitializeSecurityContext(
                        ref _sspiCred,
                        IntPtr.Zero,
                        _sspiTarget,
                        IscReqAllocateMemory,
                        0,
                        SecurityNetworkDrep,
                        IntPtr.Zero,
                        0,
                        out newContext,
                        out outbuf,
                        out contextAttr,
                        out expire
                    );
                }

                if (status != SecEOk && status != SecIContinueNeeded)
                {
                    // This will automaticcaly fill in the message of the last Win32 error
                    throw new Win32Exception();
                }
                if (!_isSSPICtxSet)
                {
                    _sspiCtx.dwUpper = newContext.dwUpper;
                    _sspiCtx.dwLower = newContext.dwLower;
                    _isSSPICtxSet = true;
                }

                if (outbuf.cBuffers > 0)
                {
                    if (outbuf.cBuffers != 1)
                        throw new InvalidOperationException("SSPI returned invalid number of output buffers");
                    // attention: OutBuffer is still our initially created struct but outbuf.pBuffer doesn't point to
                    // it but to the copy of it we created on the unmanaged heap and passed to InitializeSecurityContext()
                    // we have to marshal it back to see the content change
#if NET45
                    outBuffer = (SecBuffer)Marshal.PtrToStructure(outbuf.pBuffer, typeof(SecBuffer));
#else
                    outBuffer = Marshal.PtrToStructure<SecBuffer>(outbuf.pBuffer);
#endif
                    if (outBuffer.cbBuffer > 0)
                    {
                        // we need the buffer with a terminating 0 so we
                        // make it one byte bigger
                        var buffer = new byte[outBuffer.cbBuffer];
                        Marshal.Copy(outBuffer.pvBuffer, buffer, 0, buffer.Length);
                        // The SSPI authentication data must be sent as password message

                        return buffer;
                        //stream.WriteByte((byte)'p');
                        //PGUtil.WriteInt32(stream, buffer.Length + 5);
                        //stream.Write(buffer, 0, buffer.Length);
                        //stream.Flush();
                    }
                }
                return PGUtil.EmptyBuffer;
            }
            finally
            {
                if (outBuffer.pvBuffer != IntPtr.Zero)
                    FreeContextBuffer(outBuffer.pvBuffer);
                if (outbuf.pBuffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(outbuf.pBuffer);
            }
        }

        #region Resource Cleanup

        void FreeHandles()
        {
            if (_isSSPICtxSet)
            {
                FreeCredentialsHandle(ref _sspiCred);
                DeleteSecurityContext(ref _sspiCtx);
            }
        }

        ~SSPIHandler()
        {
            FreeHandles();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                FreeHandles();
            _disposed = true;
        }

        #endregion
    }
}
