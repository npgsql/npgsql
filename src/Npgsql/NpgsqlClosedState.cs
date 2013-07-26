// Npgsql.NpgsqlClosedState.cs
//
// Authors:
// 	Dave Joyner			<d4ljoyn@yahoo.com>
//	Daniel Nauck		<dna(at)mono-project.de>
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
using System.Diagnostics;

namespace Npgsql
{

    internal class NpgsqlNetworkStream : NetworkStream
    {
        NpgsqlConnector mContext = null;

        
        public NpgsqlNetworkStream(NpgsqlConnector context, Socket socket, Boolean owner)
            : base(socket, owner)
        {
            mContext = context;
        }

        

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                mContext.Close();
                mContext = null;
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

				/*TcpClient tcpc = new TcpClient();
								tcpc.Connect(new IPEndPoint(ResolveIPHost(context.Host), context.Port));
								Stream stream = tcpc.GetStream();*/

				/*socket.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.SendTimeout, context.ConnectionTimeout*1000);*/

				//socket.Connect(new IPEndPoint(ResolveIPHost(context.Host), context.Port));


				/*Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                
								IAsyncResult result = socket.BeginConnect(new IPEndPoint(ResolveIPHost(context.Host), context.Port), null, null);

								if (!result.AsyncWaitHandle.WaitOne(context.ConnectionTimeout*1000, false))
								{
										socket.Close();
										throw new Exception(resman.GetString("Exception_ConnectionTimeout"));
								}

								try
								{
										socket.EndConnect(result);
								}
								catch (Exception)
								{
										socket.Close();
										throw;
								}
								*/

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
				for (int I = 0 ; I < ips.Length ; I++)
				{
					NpgsqlEventLog.LogMsg(resman, "Log_ConnectingTo", LogLevel.Debug, ips[I]);

					IPEndPoint ep = new IPEndPoint(ips[I], context.Port);
					socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

					attemptStart = DateTime.Now;

					try
					{
						result = socket.BeginConnect(ep, null, null);

						if (!result.AsyncWaitHandle.WaitOne(timeout / (ips.Length - I), true))
						{
							throw new TimeoutException(resman.GetString("Exception_ConnectionTimeout"));
						}

						socket.EndConnect(result);

						// connect was successful, leave the loop
						break;
					}
					catch (Exception E)
					{
						NpgsqlEventLog.LogMsg(resman, "Log_FailedConnection", LogLevel.Normal, ips[I]);

						timeout -= Convert.ToInt32((DateTime.Now - attemptStart).TotalMilliseconds);
						lastSocketException = E;

						socket.Close();
						socket = null;
					}
				}

				if (socket == null)
				{
					throw lastSocketException;
				}

				//Stream stream = new NetworkStream(socket, true);
								Stream stream = new NpgsqlNetworkStream(context, socket, true);

				// If the PostgreSQL server has SSL connectors enabled Open SslClientStream if (response == 'S') {
				if (context.SSL || (context.SslMode == SslMode.Require) || (context.SslMode == SslMode.Prefer))
				{
					PGUtil.WriteInt32(stream, 8);
					PGUtil.WriteInt32(stream, 80877103);
					// Receive response

					Char response = (Char) stream.ReadByte();
					if (response == 'S')
					{
												//create empty collection
												X509CertificateCollection clientCertificates = new X509CertificateCollection();
                            
												//trigger the callback to fetch some certificates
												context.DefaultProvideClientCertificatesCallback(clientCertificates);

												if (context.UseMonoSsl)
												{
														stream = new SslClientStream(
																stream,
																context.Host,
																true,
																SecurityProtocolType.Default,
																clientCertificates);

														((SslClientStream)stream).ClientCertSelectionDelegate =
																new CertificateSelectionCallback(context.DefaultCertificateSelectionCallback);
														((SslClientStream)stream).ServerCertValidationDelegate =
																new CertificateValidationCallback(context.DefaultCertificateValidationCallback);
														((SslClientStream)stream).PrivateKeyCertSelectionDelegate =
																new PrivateKeySelectionCallback(context.DefaultPrivateKeySelectionCallback);
												}
												else
												{
														SslStream sstream = new SslStream(stream, true, delegate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
														{
																return context.DefaultValidateRemoteCertificateCallback(cert, chain, errors);
														});
														sstream.AuthenticateAsClient(context.Host, clientCertificates, System.Security.Authentication.SslProtocols.Default, false);
														stream = sstream;
												}
					}
					else if (context.SslMode == SslMode.Require)
					{
						throw new InvalidOperationException(resman.GetString("Exception_Ssl_RequestError"));
					}
				}

				context.Stream = new BufferedStream(stream);
				context.Socket = socket;


				NpgsqlEventLog.LogMsg(resman, "Log_ConnectedTo", LogLevel.Normal, context.Host, context.Port);
				ChangeState(context, NpgsqlConnectedState.Instance);
			}
			catch (Exception E)
			{
				throw new NpgsqlException(string.Format(resman.GetString("Exception_FailedConnection"), context.Host), E);
			}
		}

		public override void Close(NpgsqlConnector context)
		{
			//DO NOTHING.
		}
	}
}
