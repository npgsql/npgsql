using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Localization;

namespace Npgsql
{
    internal partial class NpgsqlConnector
    {
        internal async Task<IServerMessage> ReadMessageAsync(CancellationToken ct)
        {
            try
            {
                return await ReadMessageInternalAsync(ct);
            }
            catch (IOException e)
            {
                // TODO: Identify socket timeout? But what to do at this point? Seems
                // impossible to actually recover the connection (sync the protocol), best just
                // transition to Broken like any other IOException...
                throw;
            }
            catch (ThreadAbortException)
            {
                try
                {
                    CancelRequest();
                    Close();
                }
                catch { }
                throw;
            }
        }

        async Task<IServerMessage> ReadMessageInternalAsync(CancellationToken ct)
        {
            for (;;)
            {
                // Check the first Byte of response.
                // TODO: Inefficient, use a connector buffer
                var buf = new byte[1];
                var message = (BackEndMessageCode) await Stream.ReadAsync(buf, 0, 1, ct);
                switch (message)
                {
                    case BackEndMessageCode.ErrorResponse:
                        var error = new NpgsqlError(Stream);
                        _log.Trace("Received backend error: " + error.Message);
                        error.ErrorSql = Mediator.GetSqlSent();

                        // We normally accumulate errors until the query ends (ReadyForQuery). But
                        // during the connection phase errors need to be thrown immediately
                        // Possible error in the NpgsqlStartupState:
                        //        Invalid password.
                        // Possible error in the NpgsqlConnectedState:
                        //        No pg_hba.conf configured.

                        if (State == NpgsqlState.Connecting)
                        {
                            throw new NpgsqlException(error);
                        }

                        _pendingErrors.Add(error);
                        continue;

                    case BackEndMessageCode.AuthenticationRequest:
                        // Get the length in case we're getting AuthenticationGSSContinue
                        var authDataLength = Stream.ReadInt32() - 8;

                        var authType = (AuthenticationRequestType)Stream.ReadInt32();
                        _log.Trace("Received AuthenticationRequest of type " + authType);
                        switch (authType)
                        {
                            case AuthenticationRequestType.AuthenticationOk:
                                continue;
                            case AuthenticationRequestType.AuthenticationClearTextPassword:
                                // Send the PasswordPacket.
                                Authenticate(PGUtil.NullTerminateArray(Password));
                                continue;
                            case AuthenticationRequestType.AuthenticationMD5Password:
                                // Now do the "MD5-Thing"
                                // for this the Password has to be:
                                // 1. md5-hashed with the username as salt
                                // 2. md5-hashed again with the salt we get from the backend

                                var md5 = MD5.Create();

                                // 1.
                                var passwd = Password;
                                var saltUserName = BackendEncoding.UTF8Encoding.GetBytes(UserName);

                                var cryptBuf = new byte[passwd.Length + saltUserName.Length];

                                passwd.CopyTo(cryptBuf, 0);
                                saltUserName.CopyTo(cryptBuf, passwd.Length);

                                var sb = new StringBuilder();
                                var hashResult = md5.ComputeHash(cryptBuf);
                                foreach (byte b in hashResult)
                                {
                                    sb.Append(b.ToString("x2"));
                                }

                                var prehash = sb.ToString();

                                var prehashbytes = BackendEncoding.UTF8Encoding.GetBytes(prehash);
                                cryptBuf = new byte[prehashbytes.Length + 4];

                                Stream.Read(cryptBuf, prehashbytes.Length, 4);
                                // Send the PasswordPacket.

                                // 2.
                                prehashbytes.CopyTo(cryptBuf, 0);

                                sb = new StringBuilder("md5");
                                // This is needed as the backend expects md5 result starts with "md5"
                                hashResult = md5.ComputeHash(cryptBuf);
                                foreach (var b in hashResult)
                                {
                                    sb.Append(b.ToString("x2"));
                                }

                                Authenticate(PGUtil.NullTerminateArray(BackendEncoding.UTF8Encoding.GetBytes(sb.ToString())));
                                continue;

                            case AuthenticationRequestType.AuthenticationGSS:
                                {
                                    if (IntegratedSecurity)
                                    {
                                        // For GSSAPI we have to use the supplied hostname
                                        SSPI = new SSPIHandler(Host, "POSTGRES", true);
                                        Authenticate(SSPI.Continue(null));
                                        continue;
                                    }
                                    else
                                    {
                                        // TODO: correct exception
                                        throw new Exception();
                                    }
                                }

                            case AuthenticationRequestType.AuthenticationSSPI:
                                {
                                    if (IntegratedSecurity)
                                    {
                                        // For SSPI we have to get the IP-Address (hostname doesn't work)
                                        var ipAddressString = ((IPEndPoint)Socket.RemoteEndPoint).Address.ToString();
                                        SSPI = new SSPIHandler(ipAddressString, "POSTGRES", false);
                                        Authenticate(SSPI.Continue(null));
                                        continue;
                                    }
                                    else
                                    {
                                        // TODO: correct exception
                                        throw new Exception();
                                    }
                                }

                            case AuthenticationRequestType.AuthenticationGSSContinue:
                                {
                                    var authData = new byte[authDataLength];
                                    Stream.CheckedStreamRead(authData, 0, authDataLength);
                                    var passwdRead = SSPI.Continue(authData);
                                    if (passwdRead.Length != 0)
                                    {
                                        Authenticate(passwdRead);
                                    }
                                    continue;
                                }

                            default:
                                throw new NotSupportedException(String.Format(L10N.AuthenticationMethodNotSupported, authType));
                        }

                    case BackEndMessageCode.RowDescription:
                        _log.Trace("Received RowDescription");
                        return new NpgsqlRowDescription(Stream, OidToNameMapping);

                    case BackEndMessageCode.ParameterDescription:
                        _log.Trace("Received ParameterDescription");
                        // Do nothing,for instance,  just read...
                        Stream.ReadInt32();
                        var nbParam = Stream.ReadInt16();
                        for (var i = 0; i < nbParam; i++)
                        {
                            Stream.ReadInt32();  // typeoids
                        }
                        continue;

                    case BackEndMessageCode.DataRow:
                        _log.Trace("Received DataRow");
                        State = NpgsqlState.Fetching;
                        return new StringRowReader(Stream);

                    case BackEndMessageCode.ReadyForQuery:
                        _log.Trace("Received ReadyForQuery");

                        // Possible status bytes returned:
                        //   I = Idle (no transaction active).
                        //   T = In transaction, ready for more.
                        //   E = Error in transaction, queries will fail until transaction aborted.
                        // Just eat the status byte, we have no use for it at this time.
                        Stream.ReadInt32();
                        Stream.ReadByte();

                        State = NpgsqlState.Ready;

                        if (_pendingErrors.Any())
                        {
                            var e = new NpgsqlException(_pendingErrors);
                            _pendingErrors.Clear();
                            throw e;
                        }
                        return ReadyForQueryMsg.Instance;

                    case BackEndMessageCode.BackendKeyData:
                        _log.Trace("Received BackendKeyData");
                        BackEndKeyData = new NpgsqlBackEndKeyData(Stream);
                        // Wait for ReadForQuery message
                        continue;

                    case BackEndMessageCode.NoticeResponse:
                        _log.Trace("Received NoticeResponse");
                        // Notices and errors are identical except that we
                        // just throw notices away completely ignored.
                        FireNotice(new NpgsqlError(Stream));
                        continue;

                    case BackEndMessageCode.CompletedResponse:
                        _log.Trace("Received CompletedResponse");
                        Stream.ReadInt32();
                        return new CompletedResponse(Stream);

                    case BackEndMessageCode.ParseComplete:
                        _log.Trace("Received ParseComplete");
                        // Just read up the message length.
                        Stream.ReadInt32();
                        continue;

                    case BackEndMessageCode.BindComplete:
                        _log.Trace("Received BindComplete");
                        // Just read up the message length.
                        Stream.ReadInt32();
                        continue;

                    case BackEndMessageCode.EmptyQueryResponse:
                        _log.Trace("Received EmptyQueryResponse");
                        Stream.ReadInt32();
                        continue;

                    case BackEndMessageCode.NotificationResponse:
                        _log.Trace("Received NotificationResponse");
                        // Eat the length
                        Stream.ReadInt32();
                        FireNotification(new NpgsqlNotificationEventArgs(Stream, true));
                        if (IsNotificationThreadRunning)
                        {
                            throw new Exception("Internal state error, notification thread is running");
                        }
                        continue;

                    case BackEndMessageCode.ParameterStatus:
                        var paramStatus = new NpgsqlParameterStatus(Stream);
                        _log.TraceFormat("Received ParameterStatus {0}={1}", paramStatus.Parameter, paramStatus.ParameterValue);
                        AddParameterStatus(paramStatus);

                        if (paramStatus.Parameter == "server_version")
                        {
                            // Deal with this here so that if there are
                            // changes in a future backend version, we can handle it here in the
                            // protocol handler and leave everybody else put of it.
                            var versionString = paramStatus.ParameterValue.Trim();
                            for (var idx = 0; idx != versionString.Length; ++idx)
                            {
                                var c = paramStatus.ParameterValue[idx];
                                if (!char.IsDigit(c) && c != '.')
                                {
                                    versionString = versionString.Substring(0, idx);
                                    break;
                                }
                            }
                            ServerVersion = new Version(versionString);
                        }
                        continue;

                    case BackEndMessageCode.NoData:
                        // This nodata message may be generated by prepare commands issued with queries which doesn't return rows
                        // for example insert, update or delete.
                        // Just eat the message.
                        _log.Trace("Received NoData");
                        Stream.ReadInt32();
                        continue;

                    case BackEndMessageCode.CopyInResponse:
                        _log.Trace("Received CopyInResponse");
                        throw new NotImplementedException("Copy temporarily out of order");
                    /*
                    // Enter COPY sub protocol and start pushing data to server
                    State = NpgsqlState.CopyIn;
                    Stream.ReadInt32(); // length redundant
                    StartCopyIn(ReadCopyHeader());
                    yield break;
                    // Either StartCopy called us again to finish the operation or control should be passed for user to feed copy data
                     */

                    case BackEndMessageCode.CopyOutResponse:
                        _log.Trace("Received CopyOutResponse");
                        throw new NotImplementedException("Copy temporarily out of order");
                    /*
                    // Enter COPY sub protocol and start pulling data from server
                    State = NpgsqlState.CopyOut;
                    Stream.ReadInt32(); // length redundant
                    StartCopyOut(ReadCopyHeader());
                    yield break;
                    // Either StartCopy called us again to finish the operation or control should be passed for user to feed copy data
                     */

                    case BackEndMessageCode.CopyData:
                        _log.Trace("Received CopyData");
                        throw new NotImplementedException("Copy temporarily out of order");
                    /*
                    var len = Stream.ReadInt32() - 4;
                    var buf = new byte[len];
                    Stream.ReadBytes(buf, 0, len);
                    Mediator.ReceivedCopyData = buf;
                    yield break;
                    // read data from server one chunk at a time while staying in copy operation mode
                     */

                    case BackEndMessageCode.CopyDone:
                        _log.Trace("Received CopyDone");
                        throw new NotImplementedException("Copy temporarily out of order");
                    /*
                    Stream.ReadInt32(); // CopyDone can not have content so this is always 4
                    // This will be followed by normal CommandComplete + ReadyForQuery so no op needed
                    break;
                     */

                    case BackEndMessageCode.IO_ERROR:
                        // Connection broken. Mono returns -1 instead of throwing an exception as ms.net does.
                        throw new IOException();

                    default:
                        // This could mean a number of things
                        //   We've gotten out of sync with the backend?
                        //   We need to implement this type?
                        //   Backend has gone insane?
                        // FIXME
                        // what exception should we really throw here?
                        throw new NotSupportedException(String.Format("Backend sent unrecognized response type: {0}", (Char)message));
                }
            }
        }
    }
}
