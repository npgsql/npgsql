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
        private readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;
        protected readonly static ResourceManager resman = new ResourceManager(MethodBase.GetCurrentMethod().DeclaringType);

        public virtual void Open(NpgsqlConnector context, Int32 timeout)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void Startup(NpgsqlConnector context,NpgsqlConnectionStringBuilder settings)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void Authenticate(NpgsqlConnector context, byte[] password)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void Query(NpgsqlConnector context, NpgsqlQuery query)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void FunctionCall(NpgsqlConnector context, NpgsqlCommand command)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void Parse(NpgsqlConnector context, NpgsqlParse parse)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public void TestNotify(NpgsqlConnector context)
        {
            //ZA  Hnotifytest CNOTIFY Z
            //Qlisten notifytest;notify notifytest;
            Stream stm = context.Stream;
//            string uuidString = "uuid" + Guid.NewGuid().ToString("N");
            string uuidString = string.Format("uuid{0:N}", Guid.NewGuid());
            Queue<byte> buffer = new Queue<byte>();
            byte[] convertBuffer = new byte[36];

            PGUtil.WriteStringNullTerminated(stm, "Qlisten {0};notify {0};", uuidString);

            for (;;)
            {
                int newByte = stm.ReadByte();
                if (newByte == -1)
                {
                    throw new EndOfStreamException();
                }
                buffer.Enqueue((byte) newByte);
                if (buffer.Count > 35)
                {
                    buffer.CopyTo(convertBuffer, 0);
                    if (BackendEncoding.UTF8Encoding.GetString(convertBuffer) == uuidString)
                    {
                        for (;;)
                        {
                            switch (stm.ReadByte())
                            {
                                case -1:
                                    throw new EndOfStreamException();
                                case 'Z':
                                    //context.Query(new NpgsqlCommand("UNLISTEN *", context));
                                    using(NpgsqlCommand cmd = new NpgsqlCommand("UNLISTEN *", context))
                                    {
                                        cmd.ExecuteBlind();
                                    }
                                    return;
                            }
                        }
                    }
                    else
                    {
                        buffer.Dequeue();
                    }
                }
            }
        }

        public void TestConnector(NpgsqlConnector context)
        {
            switch (context.BackendProtocolVersion)
            {
                case ProtocolVersion.Version2:
                    TestNotify(context);
                    break;
                case ProtocolVersion.Version3:
                    EmptySync(context);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void EmptySync(NpgsqlConnector context)
        {
            Stream stm = context.Stream;
            NpgsqlSync.Default.WriteToStream(stm);
            stm.Flush();
            Queue<int> buffer = new Queue<int>();
            //byte[] compareBuffer = new byte[6];
            int[] messageSought = new int[] {'Z', 0, 0, 0, 5};
            int newByte;
            for (;;)
            {
                switch (newByte = stm.ReadByte())
                {
                    case -1:
                        throw new EndOfStreamException();
                    case 'E':
                    case 'I':
                    case 'T':
                        if (buffer.Count > 4)
                        {
                            bool match = true;
                            int i = 0;
                            foreach (byte cmp in buffer)
                            {
                                if (cmp != messageSought[i++])
                                {
                                    match = false;
                                    break;
                                }
                            }
                            if (match)
                            {
                                return;
                            }
                        }
                        break;
                    default:
                        buffer.Enqueue(newByte);
                        if (buffer.Count > 5)
                        {
                            buffer.Dequeue();
                        }
                        break;
                }
            }
        }

        public virtual void Sync(NpgsqlConnector context)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void Bind(NpgsqlConnector context, NpgsqlBind bind)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void Execute(NpgsqlConnector context, NpgsqlExecute execute)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void Describe(NpgsqlConnector context, NpgsqlDescribe describe)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void CancelRequest(NpgsqlConnector context)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        // COPY methods

        protected virtual void StartCopy(NpgsqlConnector context, NpgsqlCopyFormat copyFormat)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual byte[] GetCopyData(NpgsqlConnector context)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void SendCopyData(NpgsqlConnector context, byte[] buf, int off, int len)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void SendCopyDone(NpgsqlConnector context)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual void SendCopyFail(NpgsqlConnector context, String message)
        {
            throw new InvalidOperationException("Internal Error! " + this);
        }

        public virtual NpgsqlCopyFormat CopyFormat
        {
            get { throw new InvalidOperationException("Internal Error! " + this); }
        }

        public virtual void Close(NpgsqlConnector context)
        {
            try
            {
                context.Stream.Close();
            }
            catch
            {
            }
            context.Stream = null;
            ChangeState(context, NpgsqlClosedState.Instance);
        }

        ///<summary>
        ///This method is used by the states to change the state of the context.
        /// </summary>
        protected static void ChangeState(NpgsqlConnector context, NpgsqlState newState)
        {
            context.CurrentState = newState;
        }

        private class ContextResetter : IDisposable
        {
            private readonly NpgsqlConnector _connector;

            public ContextResetter(NpgsqlConnector connector)
            {
                _connector = connector;
            }

            public void Dispose()
            {
                _connector.RequireReadyForQuery = true;
            }
        }

        /// <summary>
        /// Call ProcessBackendResponsesEnum(), and scan and discard all results.
        /// </summary>
        public void ProcessAndDiscardBackendResponses(NpgsqlConnector context)
        {
            IEnumerable<IServerResponseObject> responseEnum;

            // Flush and wait for responses.
            responseEnum = ProcessBackendResponsesEnum(context);

            // Discard each response.
            foreach (IServerResponseObject response in responseEnum)
            {
                if (response is IDisposable)
                {
                    (response as IDisposable).Dispose();
                }
            }
        }

        ///<summary>
        /// This method is responsible to handle all protocol messages sent from the backend.
        /// It holds all the logic to do it.
        /// To exchange data, it uses a Mediator object from which it reads/writes information
        /// to handle backend requests.
        /// </summary>
        ///
        internal IEnumerable<IServerResponseObject> ProcessBackendResponsesEnum(NpgsqlConnector context)
        {
            try
            {
                // Flush buffers to the wire.
                context.Stream.Flush();

                // Process commandTimeout behavior.

                if ((context.Mediator.CommandTimeout > 0) &&
                        (!CheckForContextSocketAvailability(context, SelectMode.SelectRead)))
                {
                    // If timeout occurs when establishing the session with server then
                    // throw an exception instead of trying to cancel query. This helps to prevent loop as CancelRequest will also try to stablish a connection and sends commands.
                    if (!((this is NpgsqlStartupState || this is NpgsqlConnectedState)))
                    {
                        try
                        {
                            context.CancelRequest();

                            ProcessAndDiscardBackendResponses(context);
                        }
                        catch(Exception)
                        {
                        }
                        //We should have gotten an error from CancelRequest(). Whether we did or not, what we
                        //really have is a timeout exception, and that will be less confusing to the user than
                        //"operation cancelled by user" or similar, so whatever the case, that is what we'll throw.
                        // Changed message again to report about the two possible timeouts: connection or command as the establishment timeout only was confusing users when the timeout was a command timeout.
                    }

                    throw new NpgsqlException(resman.GetString("Exception_ConnectionOrCommandTimeout"));
                }

                switch (context.BackendProtocolVersion)
                {
                    case ProtocolVersion.Version2:
                        return ProcessBackendResponses_Ver_2(context);
                    case ProtocolVersion.Version3:
                        return ProcessBackendResponses_Ver_3(context);
                    default:
                        throw new NpgsqlException(resman.GetString("Exception_UnknownProtocol"));
                }

            }
            catch(ThreadAbortException)
            {
                try
                {
                    context.CancelRequest();
                    context.Close();
                }
                catch {}

                throw;
            }

        }

        /// <summary>
        /// Checks for context socket availability.
        /// Socket.Poll supports integer as microseconds parameter.
        /// This limits the usable command timeout value
        /// to 2,147 seconds: (2,147 x 1,000,000 less than  max_int).
        /// In order to bypass this limit, the availability of
        /// the socket is checked in 2,147 seconds cycles
        /// </summary>
        /// <returns><c>true</c>, if for context socket availability was checked, <c>false</c> otherwise.</returns>
        /// <param name="context">Context.</param>
        /// <param name="selectMode">Select mode.</param>
        internal bool CheckForContextSocketAvailability (NpgsqlConnector context, SelectMode selectMode)
        {
            /* Socket.Poll supports integer as microseconds parameter.
             * This limits the usable command timeout value
             * to 2,147 seconds: (2,147 x 1,000,000 < max_int).
             */
            const int limitOfSeconds = 2147;

            bool socketPoolResponse = false;

            // Because the backend's statement_timeout parameter has been set to context.Mediator.CommandTimeout,
            // we will give an extra 5 seconds because we'd prefer to receive a timeout error from PG
            // than to be forced to start a new connection and send a cancel request.
            // The result is that a timeout could take 5 seconds too long to occur, but if everything
            // is healthy, that shouldn't happen.
            int secondsToWait = context.Mediator.CommandTimeout + 5;

            /* In order to bypass this limit, the availability of
             * the socket is checked in 2,147 seconds cycles
             */
            while ((secondsToWait > limitOfSeconds) && (!socketPoolResponse))
            {
                socketPoolResponse = context.Socket.Poll (1000000 * limitOfSeconds, selectMode);
                secondsToWait -= limitOfSeconds;
            }

            return socketPoolResponse || context.Socket.Poll (1000000 * secondsToWait, selectMode);
        }

        private enum BackEndMessageCode
        {
            IO_ERROR = -1, // Connection broken. Mono returns -1 instead of throwing an exception as ms.net does.

            CopyData = 'd',
            CopyDone = 'c',
            DataRow = 'D',
            BinaryRow = 'B', //Version 2 only

            BackendKeyData = 'K',
            CancelRequest = 'F',
            CompletedResponse = 'C',
            CopyDataRows = ' ',
            CopyInResponse = 'G',
            CopyOutResponse = 'H',
            CursorResponse = 'P', //Version 2 only
            EmptyQueryResponse = 'I',
            ErrorResponse = 'E',
            FunctionCall = 'F',
            FunctionCallResponse = 'V',

            AuthenticationRequest = 'R',

            NoticeResponse = 'N',
            NotificationResponse = 'A',
            ParameterStatus = 'S',
            PasswordPacket = ' ',
            ReadyForQuery = 'Z',
            RowDescription = 'T',
            SSLRequest = ' ',

            // extended query backend messages
            ParseComplete = '1',
            BindComplete = '2',
            PortalSuspended = 's',
            ParameterDescription = 't',
            NoData = 'n',
            CloseComplete = '3'
        }

        private enum AuthenticationRequestType
        {
            AuthenticationOk = 0,
            AuthenticationKerberosV4 = 1,
            AuthenticationKerberosV5 = 2,
            AuthenticationClearTextPassword = 3,
            AuthenticationCryptPassword = 4,
            AuthenticationMD5Password = 5,
            AuthenticationSCMCredential = 6,
            AuthenticationGSS = 7,
            AuthenticationGSSContinue = 8,
            AuthenticationSSPI = 9
        }

        private static NpgsqlCopyFormat ReadCopyHeader(Stream stream)
        {
            byte copyFormat = (byte) stream.ReadByte();
            Int16 numCopyFields = PGUtil.ReadInt16(stream);
            Int16[] copyFieldFormats = new Int16[numCopyFields];
            for (Int16 i = 0; i < numCopyFields; i++)
            {
                copyFieldFormats[i] = PGUtil.ReadInt16(stream);
            }
            return new NpgsqlCopyFormat(copyFormat, copyFieldFormats);
        }
    }

    /// <summary>
    /// Represents a completed response message.
    /// </summary>
    internal class CompletedResponse : IServerResponseObject
    {
        private readonly int? _rowsAffected;
        private readonly long? _lastInsertedOID;

        public CompletedResponse(Stream stream)
        {
            string[] tokens = PGUtil.ReadString(stream).Split();
            if (tokens.Length > 1)
            {
                int rowsAffected;
                if (int.TryParse(tokens[tokens.Length - 1], out rowsAffected))
                    _rowsAffected = rowsAffected;
                else
                    _rowsAffected = null;

            }
            _lastInsertedOID = (tokens.Length > 2 && tokens[0].Trim().ToUpperInvariant() == "INSERT")
                                   ? long.Parse(tokens[1])
                                   : (long?) null;
        }

        public long? LastInsertedOID
        {
            get { return _lastInsertedOID; }
        }

        public int? RowsAffected
        {
            get { return _rowsAffected; }
        }
    }

    /// <summary>
    /// For classes representing messages sent from the client to the server.
    /// </summary>
    internal abstract class ClientMessage
    {
        public abstract void WriteToStream(Stream outputStream);
    }

    /// <summary>
    /// For classes representing simple messages,
    /// consisting only of a message code and length identifier,
    /// sent from the client to the server.
    /// </summary>
    internal abstract class SimpleClientMessage : ClientMessage
    {
        private readonly byte[] _messageData;

        protected SimpleClientMessage(FrontEndMessageCode MessageCode)
        {
            _messageData = new byte[5];
            MemoryStream messageBuilder = new MemoryStream(_messageData);

            messageBuilder
                .WriteBytes((byte)MessageCode)
                .WriteInt32(4);
        }

        public override void WriteToStream(Stream outputStream)
        {
            outputStream.WriteBytes(_messageData);
        }
    }


    /// <summary>
    /// Marker interface which identifies a class which represents part of
    /// a response from the server.
    /// </summary>
    internal interface IServerResponseObject
    {
    }

    /// <summary>
    /// Marker interface which identifies a class which may take possession of a stream for the duration of
    /// it's lifetime (possibly temporarily giving that possession to another class for part of that time.
    ///
    /// It inherits from IDisposable, since any such class must make sure it leaves the stream in a valid state.
    ///
    /// The most important such class is that compiler-generated from ProcessBackendResponsesEnum. Of course
    /// we can't make that inherit from this interface, alas.
    /// </summary>
    internal interface IStreamOwner : IServerResponseObject, IDisposable
    {
    }
}
