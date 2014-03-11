// Npgsql.NpgsqlClosedState.cs
//
// Authors:
//     Dave Joyner            <d4ljoyn@yahoo.com>
//    Daniel Nauck        <dna(at)mono-project.de>
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
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Mono.Security.Protocol.Tls;
using SecurityProtocolType=Mono.Security.Protocol.Tls.SecurityProtocolType;
using System.Security.Cryptography.X509Certificates;

namespace Npgsql
{

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

    internal sealed class NpgsqlClosedState : NpgsqlState
    {
        private static readonly NpgsqlClosedState _instance = new NpgsqlClosedState();
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        private NpgsqlClosedState()
            : base()
        {
        }

        public static NpgsqlClosedState Instance
        {
            get
            {
                NpgsqlEventLog.LogPropertyGet(LogLevel.Debug, CLASSNAME, "Instance");
                return _instance;
            }
        }

        public override void Open(NpgsqlConnector context, Int32 timeout)
        {
            try
            {
                NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Open");

                IAsyncResult result;
                // Keep track of time remaining; Even though there may be multiple timeout-able calls,
                // this allows us to still respect the caller's timeout expectation.
                DateTime attemptStart;

                attemptStart = DateTime.Now;

                result = Dns.BeginGetHostAddresses(context.Host, null, null);

                if (!result.AsyncWaitHandle.WaitOne(timeout, true))
                {
                    // Timeout was used up attempting the Dns lookup
                    throw new TimeoutException(resman.GetString("Exception_DnsLookupTimeout"));
                }

                timeout -= Convert.ToInt32((DateTime.Now - attemptStart).TotalMilliseconds);

                IPAddress[] ips = Dns.EndGetHostAddresses(result);
                Socket socket = null;
                Exception lastSocketException = null;

                // try every ip address of the given hostname, use the first reachable one
                // make sure not to exceed the caller's timeout expectation by splitting the
                // time we have left between all the remaining ip's in the list.
                for (int i = 0 ; i < ips.Length ; i++)
                {
                    NpgsqlEventLog.LogMsg(resman, "Log_ConnectingTo", LogLevel.Debug, ips[i]);

                    IPEndPoint ep = new IPEndPoint(ips[i], context.Port);
                    socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    attemptStart = DateTime.Now;

                    try
                    {
                        result = socket.BeginConnect(ep, null, null);

                        if (!result.AsyncWaitHandle.WaitOne(timeout / (ips.Length - i), true))
                        {
                            throw new TimeoutException(resman.GetString("Exception_ConnectionTimeout"));
                        }

                        socket.EndConnect(result);

                        // connect was successful, leave the loop
                        break;
                    }
                    catch (Exception e)
                    {
                        NpgsqlEventLog.LogMsg(resman, "Log_FailedConnection", LogLevel.Normal, ips[i]);

                        timeout -= Convert.ToInt32((DateTime.Now - attemptStart).TotalMilliseconds);
                        lastSocketException = e;

                        socket.Close();
                        socket = null;
                    }
                }

                if (socket == null)
                {
                    throw lastSocketException;
                }

                NpgsqlNetworkStream baseStream = new NpgsqlNetworkStream(socket, true);
                Stream sslStream = null;

                // If the PostgreSQL server has SSL connectors enabled Open SslClientStream if (response == 'S') {
                if (context.SSL || (context.SslMode == SslMode.Require) || (context.SslMode == SslMode.Prefer))
                {
                    baseStream
                        .WriteInt32(8)
                        .WriteInt32(80877103);

                    // Receive response
                    Char response = (Char) baseStream.ReadByte();

                    if (response == 'S')
                    {
                        //create empty collection
                        X509CertificateCollection clientCertificates = new X509CertificateCollection();

                        //trigger the callback to fetch some certificates
                        context.DefaultProvideClientCertificatesCallback(clientCertificates);

                        //if (context.UseMonoSsl)
                        if (!NpgsqlConnector.UseSslStream)
                        {
                            SslClientStream sslStreamPriv;

                            sslStreamPriv = new SslClientStream(
                                    baseStream,
                                    context.Host,
                                    true,
                                    SecurityProtocolType.Default,
                                    clientCertificates);

                            sslStreamPriv.ClientCertSelectionDelegate =
                                    new CertificateSelectionCallback(context.DefaultCertificateSelectionCallback);
                            sslStreamPriv.ServerCertValidationDelegate =
                                    new CertificateValidationCallback(context.DefaultCertificateValidationCallback);
                            sslStreamPriv.PrivateKeyCertSelectionDelegate =
                                    new PrivateKeySelectionCallback(context.DefaultPrivateKeySelectionCallback);
                            sslStream = sslStreamPriv;
                        }
                        else
                        {
                            SslStream sslStreamPriv;

                            sslStreamPriv = new SslStream(baseStream, true, context.DefaultValidateRemoteCertificateCallback);

                            sslStreamPriv.AuthenticateAsClient(context.Host, clientCertificates, System.Security.Authentication.SslProtocols.Default, false);
                            sslStream = sslStreamPriv;
                        }
                    }
                    else if (context.SslMode == SslMode.Require)
                    {
                        throw new InvalidOperationException(resman.GetString("Exception_Ssl_RequestError"));
                    }
                }

                context.Socket = socket;
                context.BaseStream = baseStream;
                context.Stream = new BufferedStream(sslStream == null ? baseStream : sslStream, 8192);

                NpgsqlEventLog.LogMsg(resman, "Log_ConnectedTo", LogLevel.Normal, context.Host, context.Port);
                ChangeState(context, NpgsqlConnectedState.Instance);
            }
            catch (Exception e)
            {
                throw new NpgsqlException(string.Format(resman.GetString("Exception_FailedConnection"), context.Host), e);
            }
        }

        public override void Close(NpgsqlConnector context)
        {
            //DO NOTHING.
        }
    }
}
