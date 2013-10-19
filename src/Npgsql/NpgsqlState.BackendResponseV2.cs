// created on 6/14/2002 at 7:56 PM

// Npgsql.NpgsqlState.cs
//
// Author:
//     Dave Joyner <d4ljoyn@yahoo.com>
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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;

namespace Npgsql
{
    ///<summary> This class represents the base class for the state pattern design pattern
    /// implementation.
    /// </summary>
    ///
    internal abstract partial class NpgsqlState
    {
        protected IEnumerable<IServerResponseObject> ProcessBackendResponses_Ver_2(NpgsqlConnector context)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ProcessBackendResponses");

            using (new ContextResetter(context))
            {
                Stream stream = context.Stream;
                NpgsqlMediator mediator = context.Mediator;
                List<NpgsqlError> errors = new List<NpgsqlError>();

                for (;;)
                {
                    // Check the first Byte of response.
                    switch ((BackEndMessageCode) stream.ReadByte())
                    {
                        case BackEndMessageCode.ErrorResponse:

                            {
                                NpgsqlError error = new NpgsqlError(context.BackendProtocolVersion, stream);
                                error.ErrorSql = mediator.SqlSent;

                                errors.Add(error);

                                NpgsqlEventLog.LogMsg(resman, "Log_ErrorResponse", LogLevel.Debug, error.Message);
                            }

                            // Return imediately if it is in the startup state or connected state as
                            // there is no more messages to consume.
                            // Possible error in the NpgsqlStartupState:
                            //        Invalid password.
                            // Possible error in the NpgsqlConnectedState:
                            //        No pg_hba.conf configured.

                            if (!context.RequireReadyForQuery)
                            {
                                throw new NpgsqlException(errors);
                            }

                            break;

                        case BackEndMessageCode.AuthenticationRequest:
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "AuthenticationRequest");
                            AuthenticationRequestType authType = (AuthenticationRequestType) PGUtil.ReadInt32(stream);
                            switch (authType)
                            {
                                case AuthenticationRequestType.AuthenticationOk:
                                    NpgsqlEventLog.LogMsg(resman, "Log_AuthenticationOK", LogLevel.Debug);
                                    break;
                                case AuthenticationRequestType.AuthenticationClearTextPassword:
                                    NpgsqlEventLog.LogMsg(resman, "Log_AuthenticationClearTextRequest", LogLevel.Debug);
                                    // Send the PasswordPacket.
                                    ChangeState(context, NpgsqlStartupState.Instance);
                                    context.Authenticate(context.Password);
                                    context.Stream.Flush();

                                    break;
                                case AuthenticationRequestType.AuthenticationMD5Password:
                                    NpgsqlEventLog.LogMsg(resman, "Log_AuthenticationMD5Request", LogLevel.Debug);
                                    // Now do the "MD5-Thing"
                                    // for this the Password has to be:
                                    // 1. md5-hashed with the username as salt
                                    // 2. md5-hashed again with the salt we get from the backend

                                    MD5 md5 = MD5.Create();

                                    // 1.
                                    byte[] passwd = context.Password;
                                    byte[] saltUserName = BackendEncoding.UTF8Encoding.GetBytes(context.UserName);

                                    byte[] crypt_buf = new byte[passwd.Length + saltUserName.Length];

                                    passwd.CopyTo(crypt_buf, 0);
                                    saltUserName.CopyTo(crypt_buf, passwd.Length);

                                    StringBuilder sb = new StringBuilder();
                                    byte[] hashResult = md5.ComputeHash(crypt_buf);
                                    foreach (byte b in hashResult)
                                    {
                                        sb.Append(b.ToString("x2"));
                                    }

                                    String prehash = sb.ToString();

                                    byte[] prehashbytes = BackendEncoding.UTF8Encoding.GetBytes(prehash);

                                    byte[] saltServer = new byte[4];
                                    stream.Read(saltServer, 0, 4);
                                    // Send the PasswordPacket.
                                    ChangeState(context, NpgsqlStartupState.Instance);

                                    // 2.

                                    crypt_buf = new byte[prehashbytes.Length + saltServer.Length];
                                    prehashbytes.CopyTo(crypt_buf, 0);
                                    saltServer.CopyTo(crypt_buf, prehashbytes.Length);

                                    sb = new StringBuilder("md5"); // This is needed as the backend expects md5 result starts with "md5"
                                    hashResult = md5.ComputeHash(crypt_buf);
                                    foreach (byte b in hashResult)
                                    {
                                        sb.Append(b.ToString("x2"));
                                    }

                                    context.Authenticate(BackendEncoding.UTF8Encoding.GetBytes(sb.ToString()));
                                    context.Stream.Flush();

                                    break;
                                default:
                                    // Only AuthenticationClearTextPassword and AuthenticationMD5Password supported for now.
                                    errors.Add(
                                        new NpgsqlError(context.BackendProtocolVersion,
                                                        String.Format(resman.GetString("Exception_AuthenticationMethodNotSupported"), authType)));
                                    throw new NpgsqlException(errors);
                            }
                            break;
                        case BackEndMessageCode.RowDescription:
                            yield return new NpgsqlRowDescriptionV2(stream, context.OidToNameMapping, context.CompatVersion);
                            break;

                        case BackEndMessageCode.DataRow:
                            yield return new StringRowReaderV2(stream);
                            break;

                        case BackEndMessageCode.BinaryRow:
                            throw new NotSupportedException();
                        case BackEndMessageCode.ReadyForQuery:
                            ChangeState(context, NpgsqlReadyState.Instance);
                            if (errors.Count != 0)
                            {
                                throw new NpgsqlException(errors);
                            }
                            yield break;
                        case BackEndMessageCode.BackendKeyData:
                            context.BackEndKeyData = new NpgsqlBackEndKeyData(context.BackendProtocolVersion, stream);
                            break;
                        case BackEndMessageCode.NoticeResponse:
                            context.FireNotice(new NpgsqlError(context.BackendProtocolVersion, stream));
                            break;
                        case BackEndMessageCode.CompletedResponse:
                            yield return new CompletedResponse(stream);
                            break;

                        case BackEndMessageCode.CursorResponse:
                            // This is the cursor response message.
                            // It is followed by a C NULL terminated string with the name of
                            // the cursor in a FETCH case or 'blank' otherwise.
                            // In this case it should be always 'blank'.
                            // [FIXME] Get another name for this function.
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "CursorResponse");

                            String cursorName = PGUtil.ReadString(stream);
                            // Continue waiting for ReadyForQuery message.
                            break;

                        case BackEndMessageCode.EmptyQueryResponse:
                            NpgsqlEventLog.LogMsg(resman, "Log_ProtocolMessage", LogLevel.Debug, "EmptyQueryResponse");
                            PGUtil.ReadString(stream);
                            break;

                        case BackEndMessageCode.NotificationResponse:
                            context.FireNotification(new NpgsqlNotificationEventArgs(stream, false));
                            if (context.IsNotificationThreadRunning)
                            {
                                yield break;
                            }
                            break;
                        case BackEndMessageCode.IO_ERROR:
                            // Connection broken. Mono returns -1 instead of throw an exception as ms.net does.
                            throw new IOException();

                        default:
                            // This could mean a number of things
                            //   We've gotten out of sync with the backend?
                            //   We need to implement this type?
                            //   Backend has gone insane?
                            throw new DataException("Backend sent unrecognized response type");
                    }
                }
            }
        }
    }
}
