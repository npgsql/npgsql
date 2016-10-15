#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Sockets;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
#if NET45 || NET451
using System.Transactions;
#endif
using Npgsql.Logging;
using NpgsqlTypes;
using IsolationLevel = System.Data.IsolationLevel;
using ThreadState = System.Threading.ThreadState;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.Schema;
using Npgsql.TypeHandlers;
using NpgsqlTypes;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using Npgsql.FrontendMessages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using Npgsql.FrontendMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.Logging;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Diagnostics;
using System.IO;
using Npgsql.BackendMessages;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using NpgsqlTypes;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using Npgsql.TypeHandlers;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
// Resharper disable all

namespace Npgsql
{
    public sealed partial class NpgsqlConnection
    {
        async Task OpenInternalAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(Host))
                throw new ArgumentException("Host can't be null");
            var timeout = new NpgsqlTimeout(TimeSpan.FromSeconds(ConnectionTimeout));
            // If we're postponing a close (see doc on this variable), the connection is already
            // open and can be silently reused
            if (_postponingClose)
                return;
            CheckConnectionClosed();
            Log.Trace("Opening connnection");
            if (Settings.Password == null)
            {
                // no password was provided. Attempt to pull the password from the pgpass file.
                var pgPassFile = PgPassFile.LoadDefaultFile();
                var matchingEntry = pgPassFile?.GetFirstMatchingEntry(Settings.Host, Settings.Port, Settings.Database, Settings.Username);
                if (matchingEntry != null)
                {
                    Log.Trace("Taking password from pgpass file");
                    Settings.Password = matchingEntry.Password;
                }
            }

            _wasBroken = false;
            try
            {
                // Get a Connector, either from the pool or creating one ourselves.
                if (Settings.Pooling)
                {
                    Connector = await (PoolManager.GetOrAdd(Settings).AllocateAsync(this, timeout, cancellationToken));
                    Counters.SoftConnectsPerSecond.Increment();
                    // Since this pooled connector was opened, global enum/composite mappings may have
                    // changed. Bring this up to date if needed.
                    Connector.TypeHandlerRegistry.ActivateGlobalMappings();
                }
                else
                {
                    Connector = new NpgsqlConnector(this);
                    await Connector.OpenAsync(timeout, cancellationToken);
                    Counters.NumberOfNonPooledConnections.Increment();
                }

                Connector.Notice += _noticeDelegate;
                Connector.Notification += _notificationDelegate;
#if NET45 || NET451
                if (Settings.Enlist)
                {
                    Promotable.Enlist(Transaction.Current);
                }
#endif
            }
            catch
            {
                Connector = null;
                throw;
            }

            OpenCounter++;
            _alreadyOpened = true;
            OnStateChange(new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));
        }
    }

    /// <summary>
    /// Represents a connection to a PostgreSQL backend. Unlike NpgsqlConnection objects, which are
    /// exposed to users, connectors are internal to Npgsql and are recycled by the connection pool.
    /// </summary>
    sealed partial class NpgsqlConnector
    {
        internal async Task OpenAsync(NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            Debug.Assert(Connection != null && Connection.Connector == this);
            Debug.Assert(State == ConnectorState.Closed);
            State = ConnectorState.Connecting;
            try
            {
                await RawOpenAsync(timeout, cancellationToken);
                var username = GetUsername();
                WriteStartupMessage(username);
                await WriteBuffer.FlushAsync(cancellationToken);
                timeout.Check();
                await HandleAuthenticationAsync(username, timeout, cancellationToken);
                GenerateResetMessage();
                await TypeHandlerRegistry.SetupAsync(this, timeout, cancellationToken);
                Counters.HardConnectsPerSecond.Increment();
                Log.Debug($"Opened connection to {Host}:{Port}", Id);
            }
            catch
            {
                Break();
                throw;
            }
        }

        async Task RawOpenAsync(NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            try
            {
                await ConnectAsync(timeout, cancellationToken);
                Debug.Assert(_socket != null);
                _baseStream = new NetworkStream(_socket, true);
                _stream = _baseStream;
                TextEncoding = _settings.Encoding == "UTF8" ? PGUtil.UTF8Encoding : Encoding.GetEncoding(_settings.Encoding, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                ReadBuffer = new ReadBuffer(this, _stream, BufferSize, TextEncoding);
                WriteBuffer = new WriteBuffer(this, _stream, BufferSize, TextEncoding);
                ParseMessage = new ParseMessage(TextEncoding);
                QueryMessage = new QueryMessage(TextEncoding);
                if (SslMode == SslMode.Require || SslMode == SslMode.Prefer)
                {
                    Log.Trace("Attempting SSL negotiation");
                    SSLRequestMessage.Instance.WriteFully(WriteBuffer);
                    await WriteBuffer.FlushAsync(cancellationToken);
                    await ReadBuffer.EnsureAsync(1, cancellationToken);
                    var response = (char)ReadBuffer.ReadByte();
                    timeout.Check();
                    switch (response)
                    {
                        default:
                            throw new NpgsqlException($"Received unknown response {response} for SSLRequest (expecting S or N)");
                        case 'N':
                            if (SslMode == SslMode.Require)
                            {
                                throw new NpgsqlException("SSL connection requested. No SSL enabled connection from this host is configured.");
                            }

                            break;
                        case 'S':
                            var clientCertificates = new X509CertificateCollection();
                            Connection.ProvideClientCertificatesCallback?.Invoke(clientCertificates);
                            RemoteCertificateValidationCallback certificateValidationCallback;
                            if (_settings.TrustServerCertificate)
                                certificateValidationCallback = (sender, certificate, chain, errors) => true;
                            else if (Connection.UserCertificateValidationCallback != null)
                                certificateValidationCallback = Connection.UserCertificateValidationCallback;
                            else
                                certificateValidationCallback = DefaultUserCertificateValidationCallback;
                            if (!UseSslStream)
                            {
                                var sslStream = new Tls.TlsClientStream(_stream);
                                sslStream.PerformInitialHandshake(Host, clientCertificates, certificateValidationCallback, false);
                                _stream = sslStream;
                            }
                            else
                            {
                                var sslStream = new SslStream(_stream, false, certificateValidationCallback);
#if NETSTANDARD1_3
                            // CoreCLR removed sync methods from SslStream, see https://github.com/dotnet/corefx/pull/4868.
                            // Consider exactly what to do here.
                            sslStream.AuthenticateAsClientAsync(Host, clientCertificates, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false).Wait();
#else
                                sslStream.AuthenticateAsClient(Host, clientCertificates, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, false);
#endif
                                _stream = sslStream;
                            }

                            timeout.Check();
                            ReadBuffer.Underlying = _stream;
                            WriteBuffer.Underlying = _stream;
                            IsSecure = true;
                            Log.Trace("SSL negotiation successful");
                            break;
                    }
                }

                Log.Trace($"Socket connected to {Host}:{Port}");
            }
            catch
            {
                try
                {
                    _stream?.Dispose();
                }
                catch
                {
                // ignored
                }

                _stream = null;
                try
                {
                    _baseStream?.Dispose();
                }
                catch
                {
                // ignored
                }

                _baseStream = null;
                try
                {
                    _socket?.Dispose();
                }
                catch
                {
                // ignored
                }

                _socket = null;
                throw;
            }
        }

        async Task HandleAuthenticationAsync(string username, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            Log.Trace("Authenticating...", Id);
            while (true)
            {
                var msg = await (ReadMessageAsync(DataRowLoadingMode.NonSequential, cancellationToken));
                timeout.Check();
                switch (msg.Code)
                {
                    case BackendMessageCode.AuthenticationRequest:
                        var passwordMessage = ProcessAuthenticationMessage(username, (AuthenticationRequestMessage)msg);
                        if (passwordMessage != null)
                        {
                            passwordMessage.WriteFully(WriteBuffer);
                            await WriteBuffer.FlushAsync(cancellationToken);
                            timeout.Check();
                        }

                        continue;
                    case BackendMessageCode.BackendKeyData:
                        var backendKeyDataMsg = (BackendKeyDataMessage)msg;
                        BackendProcessId = backendKeyDataMsg.BackendProcessId;
                        _backendSecretKey = backendKeyDataMsg.BackendSecretKey;
                        continue;
                    case BackendMessageCode.ReadyForQuery:
                        State = ConnectorState.Ready;
                        return;
                    default:
                        throw new NpgsqlException("Unexpected message received while authenticating: " + msg.Code);
                }
            }
        }

        async Task<IBackendMessage> ReadMessageWithPrependedAsync(CancellationToken cancellationToken, DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential)
        {
            // First read the responses of any prepended messages.
            ReadPrependedMessages();
            // Now read a non-prepended message
            try
            {
                ReceiveTimeout = UserTimeout;
                return await DoReadMessageAsync(cancellationToken, dataRowLoadingMode);
            }
            catch (PostgresException)
            {
                if (CurrentReader != null)
                {
                    // The reader cleanup will call EndUserAction
                    CurrentReader.Cleanup();
                }
                else
                {
                    EndUserAction();
                }

                throw;
            }
        }

        async Task<IBackendMessage> DoReadMessageAsync(CancellationToken cancellationToken, DataRowLoadingMode dataRowLoadingMode = DataRowLoadingMode.NonSequential, bool isPrependedMessage = false)
        {
            PostgresException error = null;
            while (true)
            {
                var buf = ReadBuffer;
                await ReadBuffer.EnsureAsync(5, cancellationToken);
                var messageCode = (BackendMessageCode)ReadBuffer.ReadByte();
                PGUtil.ValidateBackendMessageCode(messageCode);
                var len = ReadBuffer.ReadInt32() - 4; // Transmitted length includes itself
                if ((messageCode == BackendMessageCode.DataRow && dataRowLoadingMode != DataRowLoadingMode.NonSequential) || messageCode == BackendMessageCode.CopyData)
                {
                    if (dataRowLoadingMode == DataRowLoadingMode.Skip)
                    {
                        await ReadBuffer.SkipAsync(len, cancellationToken);
                        continue;
                    }
                }
                else if (len > ReadBuffer.ReadBytesLeft)
                {
                    buf = await (buf.EnsureOrAllocateTempAsync(len, cancellationToken));
                }

                var msg = ParseServerMessage(buf, messageCode, len, dataRowLoadingMode, isPrependedMessage);
                switch (messageCode)
                {
                    case BackendMessageCode.ErrorResponse:
                        Debug.Assert(msg == null);
                        // An ErrorResponse is (almost) always followed by a ReadyForQuery. Save the error
                        // and throw it as an exception when the ReadyForQuery is received (next).
                        error = new PostgresException(buf);
                        if (State == ConnectorState.Connecting)
                        {
                            // During the startup/authentication phase, an ErrorResponse isn't followed by
                            // an RFQ. Instead, the server closes the connection immediately
                            throw error;
                        }

                        continue;
                    case BackendMessageCode.ReadyForQuery:
                        if (error != null)
                        {
                            throw error;
                        }

                        break;
                    // Asynchronous messages which can come anytime, they have already been handled
                    // in ParseServerMessage. Read the next message.
                    case BackendMessageCode.NoticeResponse:
                    case BackendMessageCode.NotificationResponse:
                    case BackendMessageCode.ParameterStatus:
                        Debug.Assert(msg == null);
                        continue;
                }

                Debug.Assert(msg != null, "Message is null for code: " + messageCode);
                return msg;
            }
        }

        internal async Task<IBackendMessage> SkipUntilAsync(BackendMessageCode stopAt, CancellationToken cancellationToken)
        {
            Debug.Assert(stopAt != BackendMessageCode.DataRow, "Shouldn't be used for rows, doesn't know about sequential");
            while (true)
            {
                var msg = await (ReadMessageAsync(DataRowLoadingMode.Skip, cancellationToken));
                Debug.Assert(!(msg is DataRowMessage));
                if (msg.Code == stopAt)
                {
                    return msg;
                }
            }
        }

        internal async Task<IBackendMessage> SkipUntilAsync(BackendMessageCode stopAt1, BackendMessageCode stopAt2, CancellationToken cancellationToken)
        {
            Debug.Assert(stopAt1 != BackendMessageCode.DataRow, "Shouldn't be used for rows, doesn't know about sequential");
            Debug.Assert(stopAt2 != BackendMessageCode.DataRow, "Shouldn't be used for rows, doesn't know about sequential");
            while (true)
            {
                var msg = await (ReadMessageAsync(DataRowLoadingMode.Skip, cancellationToken));
                Debug.Assert(!(msg is DataRowMessage));
                if (msg.Code == stopAt1 || msg.Code == stopAt2)
                {
                    return msg;
                }
            }
        }

        internal async Task<T> ReadExpectingAsync<T>(CancellationToken cancellationToken)where T : class, IBackendMessage
        {
            var msg = await (ReadMessageAsync(DataRowLoadingMode.NonSequential, cancellationToken));
            var asExpected = msg as T;
            if (asExpected == null)
            {
                Break();
                throw new NpgsqlException($"Received backend message {msg.Code} while expecting {typeof (T).Name}. Please file a bug.");
            }

            return asExpected;
        }

        internal async Task ReadAsyncMessageAsync(CancellationToken cancellationToken)
        {
            ReceiveTimeout = UserTimeout;
            await ReadBuffer.EnsureAsync(5, cancellationToken, true);
            var messageCode = (BackendMessageCode)ReadBuffer.ReadByte();
            Debug.Assert(Enum.IsDefined(typeof (BackendMessageCode), messageCode), "Unknown message code: " + messageCode);
            var len = ReadBuffer.ReadInt32() - 4; // Transmitted length includes itself
            var buf = await (ReadBuffer.EnsureOrAllocateTempAsync(len, cancellationToken));
            var msg = ParseServerMessage(buf, messageCode, len, DataRowLoadingMode.NonSequential, false);
            switch (messageCode)
            {
                case BackendMessageCode.NoticeResponse:
                case BackendMessageCode.NotificationResponse:
                case BackendMessageCode.ParameterStatus:
                    break;
                case BackendMessageCode.ErrorResponse:
                    // We can get certain asynchronous errors if the remote process is terminated, etc.
                    // We assume this is fatal.
                    Break();
                    throw new PostgresException(buf);
                default:
                    Break();
                    throw new NpgsqlException($"Received unexpected message {msg} while waiting for an asynchronous message");
            }
        }
    }

    /// <summary>
    /// Reads a forward-only stream of rows from a data source.
    /// </summary>
    
#pragma warning disable CA1010
    public sealed partial class NpgsqlDataReader
    {
        async Task<bool> ReadInternalAsync(CancellationToken cancellationToken)
        {
            if (_row != null)
            {
                await _row.ConsumeAsync(cancellationToken);
                _row = null;
            }

            switch (_state)
            {
                case ReaderState.InResult:
                    break;
                case ReaderState.BetweenResults:
                case ReaderState.Consumed:
                case ReaderState.Closed:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                if ((_behavior & CommandBehavior.SingleRow) != 0 && _readOneRow)
                {
                    await ConsumeAsync(cancellationToken);
                    return false;
                }

                while (true)
                {
                    var msg = await (ReadMessageAsync(cancellationToken));
                    switch (ProcessMessage(msg))
                    {
                        case ReadResult.RowRead:
                            return true;
                        case ReadResult.RowNotRead:
                            return false;
                        case ReadResult.ReadAgain:
                            continue;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (PostgresException)
            {
                _state = ReaderState.Consumed;
                throw;
            }
        }

        async Task<bool> NextResultInternalAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(!IsSchemaOnly);
            // If we're in the middle of a resultset, consume it
            switch (_state)
            {
                case ReaderState.InResult:
                    if (_row != null)
                    {
                        await _row.ConsumeAsync(cancellationToken);
                        _row = null;
                    }

                    // TODO: Duplication with SingleResult handling above
                    var completedMsg = await (SkipUntilAsync(BackendMessageCode.CompletedResponse, BackendMessageCode.EmptyQueryResponse, cancellationToken));
                    ProcessMessage(completedMsg);
                    break;
                case ReaderState.BetweenResults:
                    break;
                case ReaderState.Consumed:
                case ReaderState.Closed:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.Assert(_state == ReaderState.BetweenResults);
            _hasRows = null;
            if ((_behavior & CommandBehavior.SingleResult) != 0 && _statementIndex == 0)
            {
                if (_state == ReaderState.BetweenResults)
                    await ConsumeAsync(cancellationToken);
                return false;
            }

            // We are now at the end of the previous result set. Read up to the next result set, if any.
            // Non-prepared statements receive ParseComplete, BindComplete, DescriptionRow/NoData,
            // prepared statements receive only BindComplete
            for (_statementIndex++; _statementIndex < _statements.Count; _statementIndex++)
            {
                if (IsPrepared)
                {
                    await _connector.ReadExpectingAsync<BindCompleteMessage>(cancellationToken);
                    // Row descriptions have already been populated in the statement objects at the
                    // Prepare phase
                    _rowDescription = _statements[_statementIndex].Description;
                }
                else // Non-prepared flow
                {
                    await _connector.ReadExpectingAsync<ParseCompleteMessage>(cancellationToken);
                    await _connector.ReadExpectingAsync<BindCompleteMessage>(cancellationToken);
                    var msg = await (_connector.ReadMessageAsync(DataRowLoadingMode.NonSequential, cancellationToken));
                    switch (msg.Code)
                    {
                        case BackendMessageCode.NoData:
                            _rowDescription = _statements[_statementIndex].Description = null;
                            break;
                        case BackendMessageCode.RowDescription:
                            // We have a resultset
                            _rowDescription = _statements[_statementIndex].Description = (RowDescriptionMessage)msg;
                            break;
                        default:
                            throw _connector.UnexpectedMessageReceived(msg.Code);
                    }
                }

                if (_rowDescription == null)
                {
                    // Statement did not generate a resultset (e.g. INSERT)
                    // Read and process its completion message and move on to the next
                    var msg = await (_connector.ReadMessageAsync(DataRowLoadingMode.NonSequential, cancellationToken));
                    if (msg.Code != BackendMessageCode.CompletedResponse && msg.Code != BackendMessageCode.EmptyQueryResponse)
                        throw _connector.UnexpectedMessageReceived(msg.Code);
                    ProcessMessage(msg);
                    continue;
                }

                // We got a new resultset.
                // Read the next message and store it in _pendingRow, this is to make sure that if the
                // statement generated an error, it gets thrown here and not on the first call to Read().
                if (_statementIndex == 0 && Command.Parameters.Any(p => p.IsOutputDirection))
                {
                    // If output parameters are present and this is the first row of the first resultset,
                    // we must read it in non-sequential mode because it will be traversed twice (once
                    // here for the parameters, then as a regular row).
                    _pendingMessage = await (_connector.ReadMessageAsync(DataRowLoadingMode.NonSequential, cancellationToken));
                    PopulateOutputParameters();
                }
                else
                    _pendingMessage = await (_connector.ReadMessageAsync(IsSequential ? DataRowLoadingMode.Sequential : DataRowLoadingMode.NonSequential, cancellationToken));
                _state = ReaderState.InResult;
                return true;
            }

            // There are no more queries, we're done. Read to the RFQ.
            ProcessMessage(_connector.ReadExpecting<ReadyForQueryMessage>());
            _rowDescription = null;
            return false;
        }

        async Task<bool> NextResultSchemaOnlyAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(IsSchemaOnly);
            for (_statementIndex++; _statementIndex < _statements.Count; _statementIndex++)
            {
                if (IsPrepared)
                {
                    // Row descriptions have already been populated in the statement objects at the
                    // Prepare phase
                    _rowDescription = _statements[_statementIndex].Description;
                }
                else
                {
                    await _connector.ReadExpectingAsync<ParseCompleteMessage>(cancellationToken);
                    await _connector.ReadExpectingAsync<ParameterDescriptionMessage>(cancellationToken);
                    var msg = await (_connector.ReadMessageAsync(DataRowLoadingMode.NonSequential, cancellationToken));
                    switch (msg.Code)
                    {
                        case BackendMessageCode.NoData:
                            _rowDescription = _statements[_statementIndex].Description = null;
                            break;
                        case BackendMessageCode.RowDescription:
                            // We have a resultset
                            _rowDescription = _statements[_statementIndex].Description = (RowDescriptionMessage)msg;
                            Command.FixupRowDescription(_rowDescription, _statementIndex == 0);
                            break;
                        default:
                            throw _connector.UnexpectedMessageReceived(msg.Code);
                    }
                }

                // Found a resultset
                if (_rowDescription != null)
                    return true;
            }

            // There are no more queries, we're done. Read to the RFQ.
            if (!IsPrepared)
            {
                ProcessMessage(_connector.ReadExpecting<ReadyForQueryMessage>());
                _rowDescription = null;
            }

            return false;
        }

        async Task<IBackendMessage> ReadMessageAsync(CancellationToken cancellationToken)
        {
            if (_pendingMessage != null)
            {
                var msg = _pendingMessage;
                _pendingMessage = null;
                return msg;
            }

            return await _connector.ReadMessageAsync(IsSequential ? DataRowLoadingMode.Sequential : DataRowLoadingMode.NonSequential, cancellationToken);
        }

        async Task<IBackendMessage> SkipUntilAsync(BackendMessageCode stopAt1, BackendMessageCode stopAt2, CancellationToken cancellationToken)
        {
            if (_pendingMessage != null)
            {
                if (_pendingMessage.Code == stopAt1 || _pendingMessage.Code == stopAt2)
                {
                    var msg = _pendingMessage;
                    _pendingMessage = null;
                    return msg;
                }

                var asDataRow = _pendingMessage as DataRowMessage;
                // ReSharper disable once UseNullPropagation
                if (asDataRow != null)
                    await asDataRow.ConsumeAsync(cancellationToken);
                _pendingMessage = null;
            }

            return await _connector.SkipUntilAsync(stopAt1, stopAt2, cancellationToken);
        }

        async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            if (IsSchemaOnly && IsPrepared)
            {
                _state = ReaderState.Consumed;
                return;
            }

            if (_row != null)
            {
                await _row.ConsumeAsync(cancellationToken);
                _row = null;
            }

            // Skip over the other result sets, processing only CommandCompleted for RecordsAffected
            while (true)
            {
                var msg = await (SkipUntilAsync(BackendMessageCode.CompletedResponse, BackendMessageCode.ReadyForQuery, cancellationToken));
                switch (msg.Code)
                {
                    case BackendMessageCode.CompletedResponse:
                        ProcessMessage(msg);
                        continue;
                    case BackendMessageCode.ReadyForQuery:
                        ProcessMessage(msg);
                        return;
                    default:
                        throw new NpgsqlException("Unexpected message of type " + msg.Code);
                }
            }
        }

        async Task<bool> IsDBNullInternalAsync(int ordinal, CancellationToken cancellationToken)
        {
            CheckRowAndOrdinal(ordinal);
            await Row.SeekToColumnAsync(ordinal, cancellationToken);
            return _row.IsColumnNull;
        }

        async Task<T> GetFieldValueInternalAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            CheckRowAndOrdinal(ordinal);
            var t = typeof (T);
            if (!t.IsArray)
            {
                if (t == typeof (object))
                    return (T)GetValue(ordinal);
                return await ReadColumnAsync<T>(ordinal, cancellationToken);
            }

            // Getting an array
            var fieldDescription = _rowDescription[ordinal];
            var handler = fieldDescription.Handler;
            // If the type handler can simply return the requested array, call it as usual. This is the case
            // of reading a string as char[], a bytea as a byte[]...
            var tHandler = handler as ITypeHandler<T>;
            if (tHandler != null)
                return await ReadColumnAsync<T>(ordinal, cancellationToken);
            // We need to treat this as an actual array type, these need special treatment because of
            // typing/generics reasons
            var elementType = t.GetElementType();
            var arrayHandler = handler as ArrayHandler;
            if (arrayHandler == null)
                throw new InvalidCastException($"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof (T).Name}");
            if (arrayHandler.GetElementFieldType(fieldDescription) == elementType)
                return (T)GetValue(ordinal);
            if (arrayHandler.GetElementPsvType(fieldDescription) == elementType)
                return (T)GetProviderSpecificValue(ordinal);
            throw new InvalidCastException($"Can't cast database type {handler.PgDisplayName} to {typeof (T).Name}");
        }

        async Task<T> ReadColumnWithoutCacheAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            CheckRowAndOrdinal(ordinal);
            _row.SeekToColumnStart(ordinal);
            Row.CheckNotNull();
            var fieldDescription = _rowDescription[ordinal];
            try
            {
                return await fieldDescription.Handler.ReadFullyAsync<T>(_row, Row.ColumnLen, cancellationToken, fieldDescription);
            }
            catch (SafeReadException e)
            {
                throw e.InnerException;
            }
            catch
            {
                _connector.Break();
                throw;
            }
        }

        async Task<T> ReadColumnAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            CheckRowAndOrdinal(ordinal);
            CachedValue<T> cache = null;
            if (IsCaching)
            {
                cache = _rowCache.Get<T>(ordinal);
                if (cache.IsSet)
                    return cache.Value;
            }

            var result = await (ReadColumnWithoutCacheAsync<T>(ordinal, cancellationToken));
            if (IsCaching)
            {
                Debug.Assert(cache != null);
                cache.Value = result;
            }

            return result;
        }
    }

    /// <summary>
    /// Large object manager. This class can be used to store very large files in a PostgreSQL database.
    /// </summary>
    public partial class NpgsqlLargeObjectManager
    {
        internal async Task<T> ExecuteFunctionAsync<T>(string function, CancellationToken cancellationToken, params object[] arguments)
        {
            using (var command = new NpgsqlCommand(function, _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = function;
                foreach (var argument in arguments)
                {
                    command.Parameters.Add(new NpgsqlParameter()
                    {Value = argument});
                }

                return (T)await (command.ExecuteScalarAsync(cancellationToken));
            }
        }

        internal async Task<int> ExecuteFunctionGetBytesAsync(string function, byte[] buffer, int offset, int len, CancellationToken cancellationToken, params object[] arguments)
        {
            using (var command = new NpgsqlCommand(function, _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                foreach (var argument in arguments)
                {
                    command.Parameters.Add(new NpgsqlParameter()
                    {Value = argument});
                }

                using (var reader = command.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                {
                    await reader.ReadAsync(cancellationToken);
                    return (int)reader.GetBytes(0, 0, buffer, offset, len);
                }
            }
        }

        public async Task<uint> CreateAsync(CancellationToken cancellationToken, uint preferredOid = 0)
        {
            return await ExecuteFunctionAsync<uint>("lo_create", cancellationToken, (int)preferredOid);
        }

        public async Task<NpgsqlLargeObjectStream> OpenReadAsync(uint oid, CancellationToken cancellationToken)
        {
            var fd = await (ExecuteFunctionAsync<int>("lo_open", cancellationToken, (int)oid, INV_READ));
            return new NpgsqlLargeObjectStream(this, oid, fd, false);
        }

        public async Task<NpgsqlLargeObjectStream> OpenReadWriteAsync(uint oid, CancellationToken cancellationToken)
        {
            var fd = await (ExecuteFunctionAsync<int>("lo_open", cancellationToken, (int)oid, INV_READ | INV_WRITE));
            return new NpgsqlLargeObjectStream(this, oid, fd, true);
        }

        public async Task UnlinkAsync(uint oid, CancellationToken cancellationToken)
        {
            await ExecuteFunctionAsync<object>("lo_unlink", cancellationToken, (int)oid);
        }

        public async Task ExportRemoteAsync(uint oid, string path, CancellationToken cancellationToken)
        {
            await ExecuteFunctionAsync<object>("lo_export", cancellationToken, (int)oid, path);
        }

        public async Task ImportRemoteAsync(string path, CancellationToken cancellationToken, uint oid = 0)
        {
            await ExecuteFunctionAsync<object>("lo_import", cancellationToken, path, (int)oid);
        }
    }

    /// <summary>
    /// An interface to remotely control the seekable stream for an opened large object on a PostgreSQL server.
    /// Note that the OpenRead/OpenReadWrite method as well as all operations performed on this stream must be wrapped inside a database transaction.
    /// </summary>
    public sealed partial class NpgsqlLargeObjectStream
    {
        public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset or count for this buffer");
            CheckDisposed();
            int chunkCount = Math.Min(count, _manager.MaxTransferBlockSize);
            int read = 0;
            while (read < count)
            {
                var bytesRead = await (_manager.ExecuteFunctionGetBytesAsync("loread", buffer, offset + read, count - read, cancellationToken, _fd, chunkCount));
                _pos += bytesRead;
                read += bytesRead;
                if (bytesRead < chunkCount)
                {
                    return read;
                }
            }

            return read;
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset or count for this buffer");
            CheckDisposed();
            if (!_writeable)
                throw new NotSupportedException("Write cannot be called on a stream opened with no write permissions");
            int totalWritten = 0;
            while (totalWritten < count)
            {
                var chunkSize = Math.Min(count - totalWritten, _manager.MaxTransferBlockSize);
                var bytesWritten = await (_manager.ExecuteFunctionAsync<int>("lowrite", cancellationToken, _fd, new ArraySegment<byte>(buffer, offset + totalWritten, chunkSize)));
                totalWritten += bytesWritten;
                if (bytesWritten != chunkSize)
                    throw new InvalidOperationException($"Internal Npgsql bug, please report");
                _pos += bytesWritten;
            }
        }

        async Task<long> GetLengthInternalAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            long old = _pos;
            long retval = await (SeekAsync(0, SeekOrigin.End, cancellationToken));
            if (retval != old)
                await SeekAsync(old, SeekOrigin.Begin, cancellationToken);
            return retval;
        }

        public async Task<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken)
        {
            if (origin < SeekOrigin.Begin || origin > SeekOrigin.End)
                throw new ArgumentException("Invalid origin");
            if (!Has64BitSupport && offset != (long)(int)offset)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset must fit in 32 bits for PostgreSQL versions older than 9.3");
            CheckDisposed();
            if (_manager.Has64BitSupport)
                return _pos = await (_manager.ExecuteFunctionAsync<long>("lo_lseek64", cancellationToken, _fd, offset, (int)origin));
            else
                return _pos = await (_manager.ExecuteFunctionAsync<int>("lo_lseek", cancellationToken, _fd, (int)offset, (int)origin));
        }

        public async Task FlushAsync(CancellationToken cancellationToken)
        {
        }

        public async Task SetLengthAsync(long value, CancellationToken cancellationToken)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (!Has64BitSupport && value != (long)(int)value)
                throw new ArgumentOutOfRangeException(nameof(value), "offset must fit in 32 bits for PostgreSQL versions older than 9.3");
            CheckDisposed();
            if (!_writeable)
                throw new NotSupportedException("SetLength cannot be called on a stream opened with no write permissions");
            if (_manager.Has64BitSupport)
                await _manager.ExecuteFunctionAsync<int>("lo_truncate64", cancellationToken, _fd, value);
            else
                await _manager.ExecuteFunctionAsync<int>("lo_truncate", cancellationToken, _fd, (int)value);
        }
    }

    sealed partial class ConnectorPool
    {
        internal async Task<NpgsqlConnector> AllocateAsync(NpgsqlConnection conn, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            NpgsqlConnector connector;
            Monitor.Enter(this);
            while (Idle.Count > 0)
            {
                connector = Idle.Pop();
                // An idle connector could be broken because of a keepalive
                if (connector.IsBroken)
                    continue;
                connector.Connection = conn;
                IncrementBusy();
                EnsurePruningTimerState();
                Monitor.Exit(this);
                return connector;
            }

            Debug.Assert(Busy <= _max);
            if (Busy == _max)
            {
                // TODO: Async cancellation
                var tcs = new TaskCompletionSource<NpgsqlConnector>();
                await EnqueueWaitingOpenAttemptAsync(tcs, cancellationToken);
                Monitor.Exit(this);
                try
                {
                    await WaitForTaskAsync(tcs.Task, timeout.TimeLeft, cancellationToken);
                }
                catch
                {
                    // We're here if the timeout expired or the cancellation token was triggered
                    // Re-lock and check in case the task was set to completed after coming out of the Wait
                    lock (this)
                    {
                        if (!tcs.Task.IsCompleted)
                        {
                            tcs.SetCanceled();
                            throw;
                        }
                    }
                }

                connector = tcs.Task.Result;
                connector.Connection = conn;
                return connector;
            }

            // No idle connectors are available, and we're under the pool's maximum capacity.
            IncrementBusy();
            Monitor.Exit(this);
            try
            {
                connector = new NpgsqlConnector(conn)
                {ClearCounter = _clearCounter};
                await connector.OpenAsync(timeout, cancellationToken);
                Counters.NumberOfPooledConnections.Increment();
                EnsureMinPoolSize(conn);
                return connector;
            }
            catch
            {
                lock (this)
                    DecrementBusy();
                throw;
            }
        }
    }

    partial class ReadBuffer
    {
        internal async Task EnsureAsync(int count, CancellationToken cancellationToken, bool dontBreakOnTimeouts = false)
        {
            Debug.Assert(count <= Size);
            count -= ReadBytesLeft;
            if (count <= 0)
            {
                return;
            }

            if (ReadPosition == _filledBytes)
            {
                Clear();
            }
            else if (count > Size - _filledBytes)
            {
                Array.Copy(_buf, ReadPosition, _buf, 0, ReadBytesLeft);
                _filledBytes = ReadBytesLeft;
                ReadPosition = 0;
            }

            try
            {
                while (count > 0)
                {
                    var toRead = Size - _filledBytes;
                    var read = await (Underlying.ReadAsync(_buf, _filledBytes, toRead, cancellationToken));
                    if (read == 0)
                        throw new EndOfStreamException();
                    count -= read;
                    _filledBytes += read;
                }
            }
            // We have a special case when reading async notifications - a timeout may be normal
            // shouldn't be fatal
            catch (IOException e)when (dontBreakOnTimeouts && (e.InnerException as SocketException)?.SocketErrorCode == SocketError.TimedOut)
            {
                throw new TimeoutException("Timeout while reading from stream");
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while reading from stream", e);
            }
        }

        internal async Task ReadMoreAsync(CancellationToken cancellationToken)
        {
            await EnsureAsync(ReadBytesLeft + 1, cancellationToken);
        }

        internal async Task<ReadBuffer> EnsureOrAllocateTempAsync(int count, CancellationToken cancellationToken)
        {
            if (count <= Size)
            {
                await EnsureAsync(count, cancellationToken);
                return this;
            }

            // Worst case: our buffer isn't big enough. For now, allocate a new buffer
            // and copy into it
            // TODO: Optimize with a pool later?
            var tempBuf = new ReadBuffer(Connector, Underlying, count, TextEncoding);
            CopyTo(tempBuf);
            Clear();
            await tempBuf.EnsureAsync(count, cancellationToken);
            return tempBuf;
        }

        internal async Task SkipAsync(long len, CancellationToken cancellationToken)
        {
            Debug.Assert(len >= 0);
            if (len > ReadBytesLeft)
            {
                len -= ReadBytesLeft;
                while (len > Size)
                {
                    Clear();
                    await EnsureAsync(Size, cancellationToken);
                    len -= Size;
                }

                Clear();
                await EnsureAsync((int)len, cancellationToken);
            }

            ReadPosition += (int)len;
        }

        internal async Task<int> ReadAllBytesAsync(byte[] output, int outputOffset, int len, bool readOnce, CancellationToken cancellationToken)
        {
            if (len <= ReadBytesLeft)
            {
                Array.Copy(_buf, ReadPosition, output, outputOffset, len);
                ReadPosition += len;
                return len;
            }

            Array.Copy(_buf, ReadPosition, output, outputOffset, ReadBytesLeft);
            var offset = outputOffset + ReadBytesLeft;
            var totalRead = ReadBytesLeft;
            Clear();
            try
            {
                while (totalRead < len)
                {
                    var read = await (Underlying.ReadAsync(output, offset, len - totalRead, cancellationToken));
                    if (read == 0)
                        throw new EndOfStreamException();
                    totalRead += read;
                    if (readOnce)
                        return totalRead;
                    offset += read;
                }
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while reading from stream", e);
            }

            return len;
        }
    }

#pragma warning restore CA1040
    abstract partial class TypeHandler
    {
        internal async Task<T> ReadFullyAsync<T>(DataRowMessage row, int len, CancellationToken cancellationToken, FieldDescription fieldDescription = null)
        {
            Debug.Assert(row.PosInColumn == 0);
            T result;
            try
            {
                result = await (ReadFullyAsync<T>(row.Buffer, len, cancellationToken, fieldDescription));
            }
            finally
            {
                // Important in case a SafeReadException was thrown, position must still be updated
                row.PosInColumn += row.ColumnLen;
            }

            return result;
        }
    }

    abstract partial class SimpleTypeHandler<T>
    {
        internal async override Task<T2> ReadFullyAsync<T2>(ReadBuffer buf, int len, CancellationToken cancellationToken, FieldDescription fieldDescription = null)
        {
            await buf.EnsureAsync(len, cancellationToken);
            var asTypedHandler = this as ISimpleTypeHandler<T2>;
            if (asTypedHandler == null)
                throw new InvalidCastException(fieldDescription == null ? "Can't cast database type to " + typeof (T2).Name : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof (T2).Name}");
            return asTypedHandler.Read(buf, len, fieldDescription);
        }
    }

    abstract partial class ChunkingTypeHandler<T>
    {
        internal async override Task<T2> ReadFullyAsync<T2>(ReadBuffer buf, int len, CancellationToken cancellationToken, FieldDescription fieldDescription = null)
        {
            var asTypedHandler = this as IChunkingTypeHandler<T2>;
            if (asTypedHandler == null)
                throw new InvalidCastException(fieldDescription == null ? "Can't cast database type to " + typeof (T2).Name : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof (T2).Name}");
            asTypedHandler.PrepareRead(buf, len, fieldDescription);
            T2 result;
            while (!asTypedHandler.Read(out result))
                await buf.ReadMoreAsync(cancellationToken);
            return result;
        }
    }

    partial class TypeHandlerRegistry
    {
        internal static async Task SetupAsync(NpgsqlConnector connector, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            // Note that there's a chicken and egg problem here - LoadBackendTypes below needs a functional 
            // connector to load the types, hence the strange initialization code here
            connector.TypeHandlerRegistry = new TypeHandlerRegistry(connector);
            AvailablePostgresTypes types;
            if (!BackendTypeCache.TryGetValue(connector.ConnectionString, out types))
                types = BackendTypeCache[connector.ConnectionString] = await (LoadBackendTypesAsync(connector, timeout, cancellationToken));
            connector.TypeHandlerRegistry._postgresTypes = types;
            connector.TypeHandlerRegistry.ActivateGlobalMappings();
        }

        static async Task<AvailablePostgresTypes> LoadBackendTypesAsync(NpgsqlConnector connector, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            var types = new AvailablePostgresTypes();
            using (var command = new NpgsqlCommand(connector.SupportsRangeTypes ? TypesQueryWithRange : TypesQueryWithoutRange, connector.Connection))
            {
                command.CommandTimeout = timeout.IsSet ? (int)timeout.TimeLeft.TotalSeconds : 0;
                command.AllResultTypesAreUnknown = true;
                using (var reader = command.ExecuteReader())
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        timeout.Check();
                        LoadBackendType(reader, types, connector);
                    }
                }
            }

            return types;
        }
    }

    partial class WriteBuffer
    {
        internal async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (_writePosition == 0)
                return;
            try
            {
                await Underlying.WriteAsync(_buf, 0, _writePosition, cancellationToken);
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while writing to stream", e);
            }

            try
            {
                await Underlying.FlushAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while flushing stream", e);
            }

            TotalBytesFlushed += _writePosition;
            _writePosition = 0;
        }

        internal async Task DirectWriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Debug.Assert(WritePosition == 0);
            try
            {
                await Underlying.WriteAsync(buffer, offset, count, cancellationToken);
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while writing to stream", e);
            }
        }
    }
}

namespace Npgsql.BackendMessages
{
    partial class DataRowSequentialMessage
    {
        internal async override Task SeekToColumnAsync(int column, CancellationToken cancellationToken)
        {
            CheckColumnIndex(column);
            if (column < Column)
                throw new InvalidOperationException($"Invalid attempt to read from column ordinal '{column}'. With CommandBehavior.SequentialAccess, you may only read from column ordinal '{Column}' or greater.");
            if (column == Column)
                return;
            // Skip to end of column if needed
            var remainingInColumn = (ColumnLen == -1 ? 0 : ColumnLen - PosInColumn);
            if (remainingInColumn > 0)
                await Buffer.SkipAsync(remainingInColumn, cancellationToken);
            // Shut down any streaming going on on the colun
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            // Skip over unwanted fields
            for (; Column < column - 1; Column++)
            {
                await Buffer.EnsureAsync(4, cancellationToken);
                var len = Buffer.ReadInt32();
                if (len != -1)
                    await Buffer.SkipAsync(len, cancellationToken);
            }

            await Buffer.EnsureAsync(4, cancellationToken);
            ColumnLen = Buffer.ReadInt32();
            PosInColumn = 0;
            Column = column;
        }

        internal async override Task ConsumeAsync(CancellationToken cancellationToken)
        {
            // Skip to end of column if needed
            var remainingInColumn = (ColumnLen == -1 ? 0 : ColumnLen - PosInColumn);
            if (remainingInColumn > 0)
                await Buffer.SkipAsync(remainingInColumn, cancellationToken);
            // Skip over the remaining columns in the row
            for (; Column < NumColumns - 1; Column++)
            {
                await Buffer.EnsureAsync(4, cancellationToken);
                var len = Buffer.ReadInt32();
                if (len != -1)
                    await Buffer.SkipAsync(len, cancellationToken);
            }
        }
    }
}

namespace Npgsql.Tls
{
    partial class TlsClientStream
    {
        async Task<bool> ReadRecordAsync(CancellationToken cancellationToken)
        {
            int packetLength = -1;
            while (true)
            {
                if (packetLength == -1 && _readEnd - _readStart >= 5)
                {
                    // We have at least a header in our buffer, so extract the length
                    packetLength = (_buf[_readStart + 3] << 8) | _buf[_readStart + 4];
                    if (packetLength > MaxEncryptedRecordLen)
                    {
                        SendAlertFatal(AlertDescription.RecordOverflow);
                    }
                }

                if (packetLength != -1 && 5 + packetLength <= _readEnd - _readStart)
                {
                    // The whole record fits in the buffer. We are done.
                    _packetLen = packetLength;
                    return true;
                }

                if (_readEnd - _readStart > 0 && _readStart > 0)
                {
                    // We only have a partial record in the buffer,
                    // move that to the beginning to be able to read as much as possible from the network.
                    Buffer.BlockCopy(_buf, _readStart, _buf, 0, _readEnd - _readStart);
                    _readEnd -= _readStart;
                    _readStart = 0;
                }

                if (packetLength == -1 || _readEnd < 5 + packetLength)
                {
                    if (_readStart == _readEnd)
                    {
                        // The read buffer is empty, so start reading at the start of the buffer
                        _readStart = 0;
                        _readEnd = 0;
                    }

                    int read = await (_baseStream.ReadAsync(_buf, _readEnd, _buf.Length - _readEnd, cancellationToken));
                    if (read == 0)
                    {
                        return false;
                    }

                    _readEnd += read;
                }
            }
        }

        async Task GetInitialHandshakeMessagesAsync(CancellationToken cancellationToken, bool allowApplicationData = false)
        {
            while (!_handshakeMessagesBuffer.HasServerHelloDone)
            {
                if (!await (ReadRecordAsync(cancellationToken)))
                    throw new IOException("Connection EOF in initial handshake");
                Decrypt();
                switch (_contentType)
                {
                    case ContentType.Alert:
                        await HandleAlertMessageAsync(cancellationToken);
                        break;
                    case ContentType.Handshake:
                        _handshakeMessagesBuffer.AddBytes(_buf, _plaintextStart, _plaintextLen, HandshakeMessagesBuffer.IgnoreHelloRequestsSetting.IgnoreHelloRequests);
                        if (_handshakeMessagesBuffer.Messages.Count > 5)
                        {
                            // There can never be more than 5 handshake messages in a handshake
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        }

                        break;
                    case ContentType.ApplicationData:
                        EnqueueReadData(allowApplicationData);
                        break;
                    default:
                        SendAlertFatal(AlertDescription.UnexpectedMessage);
                        break;
                }
            }

            var responseLen = TraverseHandshakeMessages();
            await _baseStream.WriteAsync(_buf, 0, responseLen, cancellationToken);
            await _baseStream.FlushAsync(cancellationToken);
            ResetWritePos();
            _waitingForChangeCipherSpec = true;
        }

        async Task WaitForHandshakeCompletedAsync(bool initialHandshake, CancellationToken cancellationToken)
        {
            for (;;)
            {
                if (!await (ReadRecordAsync(cancellationToken)))
                {
                    _eof = true;
                    throw new IOException("Unexpected connection EOF in handshake");
                }

                Decrypt();
                if (_contentType != ContentType.ChangeCipherSpec)
                {
                    EnqueueReadData(!initialHandshake);
                }
                else
                {
                    ParseChangeCipherSpec();
                    _waitingForChangeCipherSpec = false;
                    break;
                }
            }

            while (_handshakeMessagesBuffer.Messages.Count == 0)
            {
                if (!await (ReadRecordAsync(cancellationToken)))
                {
                    _eof = true;
                    throw new IOException("Unexpected connection EOF in handshake");
                }

                Decrypt();
                if (_contentType != ContentType.Handshake)
                {
                    EnqueueReadData(!initialHandshake);
                }
                else
                {
                    _handshakeMessagesBuffer.AddBytes(_buf, _plaintextStart, _plaintextLen, HandshakeMessagesBuffer.IgnoreHelloRequestsSetting.IgnoreHelloRequestsUntilFinished);
                }
            }

            if ((HandshakeType)_handshakeMessagesBuffer.Messages[0][0] == HandshakeType.Finished)
            {
                ParseFinishedMessage(_handshakeMessagesBuffer.Messages[0]);
                _handshakeMessagesBuffer.RemoveFirst(); // Leave possible hello requests after this position
            }
            else
            {
                SendAlertFatal(AlertDescription.UnexpectedMessage);
            }
        }

        async Task WriteAlertFatalAsync(AlertDescription description, CancellationToken cancellationToken)
        {
            _buf[0] = (byte)ContentType.Alert;
            Utils.WriteUInt16(_buf, 1, (ushort)_connState.TlsVersion);
            _buf[5 + _connState.IvLen] = (byte)AlertLevel.Fatal;
            _buf[5 + _connState.IvLen + 1] = (byte)description;
            int endPos = Encrypt(0, 2);
            await _baseStream.WriteAsync(_buf, 0, endPos, cancellationToken);
            await _baseStream.FlushAsync(cancellationToken);
            _baseStream.Dispose();
            _eof = true;
            _closed = true;
            _connState.Dispose();
            _pendingConnState?.Dispose();
            if (_temp512 != null)
                Utils.ClearArray(_temp512);
        }

        async Task SendClosureAlertAsync(CancellationToken cancellationToken)
        {
            _buf[0] = (byte)ContentType.Alert;
            Utils.WriteUInt16(_buf, 1, (ushort)_connState.TlsVersion);
            _buf[5 + _connState.IvLen] = (byte)AlertLevel.Warning;
            _buf[5 + _connState.IvLen + 1] = (byte)AlertDescription.CloseNotify;
            int endPos = Encrypt(0, 2);
            await _baseStream.WriteAsync(_buf, 0, endPos, cancellationToken);
            await _baseStream.FlushAsync(cancellationToken);
        }

        async Task HandleAlertMessageAsync(CancellationToken cancellationToken)
        {
            if (_plaintextLen != 2)
                SendAlertFatal(AlertDescription.DecodeError);
            var alertLevel = (AlertLevel)_buf[_plaintextStart];
            var alertDescription = (AlertDescription)_buf[_plaintextStart + 1];
            switch (alertDescription)
            {
                case AlertDescription.CloseNotify:
                    _eof = true;
                    try
                    {
                        await SendClosureAlertAsync(cancellationToken);
                    }
                    catch (IOException)
                    {
                    // Don't care about this fails (the other end has closed the connection so we couldn't write)
                    }

                    await // Now, did the stream end normally (end of stream) or was the connection reset?
                    // We read 0 bytes to find out. If end of stream, it will just return 0, otherwise an exception will be thrown, as we want.
                    // TODO: what to do with _closed? (_eof is true)
                    _baseStream.ReadAsync(_buf, 0, 0, cancellationToken);
                    _baseStream.Dispose();
                    break;
                default:
                    if (alertLevel == AlertLevel.Fatal)
                    {
                        _eof = true;
                        _baseStream.Dispose();
                        Dispose();
                        throw new IOException("TLS Fatal alert: " + alertDescription);
                    }

                    break;
            }
        }

        public async Task PerformInitialHandshakeAsync(string hostName, X509CertificateCollection clientCertificates, System.Net.Security.RemoteCertificateValidationCallback remoteCertificateValidationCallback, bool checkCertificateRevocation, CancellationToken cancellationToken)
        {
            if (_connState.CipherSuite != null || _pendingConnState != null || _closed)
                throw new InvalidOperationException("Already performed initial handshake");
            _hostName = hostName;
            _clientCertificates = clientCertificates;
            _remoteCertificationValidationCallback = remoteCertificateValidationCallback;
            _checkCertificateRevocation = checkCertificateRevocation;
            try
            {
                int offset = 0;
                SendHandshakeMessage(SendClientHello, ref offset, 0);
                await _baseStream.WriteAsync(_buf, 0, offset, cancellationToken);
                await _baseStream.FlushAsync(cancellationToken);
                await GetInitialHandshakeMessagesAsync(cancellationToken);
                var keyExchange = _connState.CipherSuite.KeyExchange;
                if (keyExchange == KeyExchange.RSA || keyExchange == KeyExchange.ECDH_ECDSA || keyExchange == KeyExchange.ECDH_RSA)
                {
                    await WaitForHandshakeCompletedAsync(true, cancellationToken);
                }
            }
            catch (ClientAlertException e)
            {
                await WriteAlertFatalAsync(e.Description, cancellationToken);
                throw new IOException(e.ToString(), e);
            }
        }

        public async override Task WriteAsync(byte[] buffer, int offset, int len, CancellationToken cancellationToken)
        {
#if CHECK_ARGUMENTS
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (len < 0 || len > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("len");
#endif
            CheckNotClosed();
            if (_connState.CipherSuite == null)
            {
                throw new InvalidOperationException("Must perform initial handshake before writing application data");
            }

            try
            {
                if (_pendingConnState != null && !_waitingForChangeCipherSpec)
                {
                    await GetInitialHandshakeMessagesAsync(cancellationToken, true);
                    await WaitForHandshakeCompletedAsync(false, cancellationToken);
                }

                CheckCanWrite();
                for (;;)
                {
                    int toWrite = Math.Min(WriteSpaceLeft, len);
                    Buffer.BlockCopy(buffer, offset, _buf, _writePos, toWrite);
                    _writePos += toWrite;
                    offset += toWrite;
                    len -= toWrite;
                    if (len == 0)
                    {
                        return;
                    }

                    await FlushAsync(cancellationToken);
                }
            }
            catch (ClientAlertException e)
            {
                await WriteAlertFatalAsync(e.Description, cancellationToken);
                throw new IOException(e.ToString(), e);
            }
        }

        public async override Task FlushAsync(CancellationToken cancellationToken)
        {
            CheckNotClosed();
            if (_writePos > _connState.WriteStartPos)
            {
                try
                {
                    _buf[0] = (byte)ContentType.ApplicationData;
                    Utils.WriteUInt16(_buf, 1, (ushort)_connState.TlsVersion);
                    int offset;
                    if (_connState.TlsVersion == TlsVersion.TLSv1_0)
                    {
                        // To avoid the BEAST attack, we add an empty application data record
                        offset = Encrypt(0, 0);
                        _buf[offset] = (byte)ContentType.ApplicationData;
                        Utils.WriteUInt16(_buf, offset + 1, (ushort)_connState.TlsVersion);
                    }
                    else
                    {
                        offset = 0;
                    }

                    int endPos = Encrypt(offset, _writePos - offset - 5 - _connState.IvLen);
                    await _baseStream.WriteAsync(_buf, 0, endPos, cancellationToken);
                    await _baseStream.FlushAsync(cancellationToken);
                    ResetWritePos();
                }
                catch (ClientAlertException e)
                {
                    await WriteAlertFatalAsync(e.Description, cancellationToken);
                    throw new IOException(e.ToString(), e);
                }
            }
        }

        public async override Task<int> ReadAsync(byte[] buffer, int offset, int len, CancellationToken cancellationToken)
        {
#if CHECK_ARGUMENTS
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (len < 0 || len > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("len");
#endif
            return await ReadInternalAsync(buffer, offset, len, false, false, cancellationToken);
        }

        async Task<int> ReadInternalAsync(byte[] buffer, int offset, int len, bool onlyProcessHandshake, bool readNewDataIfAvailable, CancellationToken cancellationToken)
        {
            await FlushAsync(cancellationToken);
            try
            {
                for (;;)
                {
                    // Handshake messages take priority over application data
                    if (_handshakeMessagesBuffer.Messages.Count > 0)
                    {
                        if (_waitingForFinished)
                        {
                            if (_handshakeMessagesBuffer.Messages[0][0] != (byte)HandshakeType.Finished)
                            {
                                SendAlertFatal(AlertDescription.UnexpectedMessage);
                            }

                            ParseFinishedMessage(_handshakeMessagesBuffer.Messages[0]);
                            _waitingForFinished = false;
                            _handshakeMessagesBuffer.RemoveFirst(); // There may be Hello Requests after Finished
                        }

                        if (_handshakeMessagesBuffer.Messages.Count > 0)
                        {
                            if (_pendingConnState == null)
                            {
                                // Not currently renegotiating, should be a hello request
                                if (_handshakeMessagesBuffer.Messages.Any(m => !HandshakeMessagesBuffer.IsHelloRequest(m)))
                                {
                                    SendAlertFatal(AlertDescription.UnexpectedMessage);
                                }

                                _renegotiationTempWriteBuf = new byte[_buf.Length];
                                byte[] bufSaved = _buf;
                                _buf = _renegotiationTempWriteBuf;
                                int writeOffset = 0;
                                SendHandshakeMessage(SendClientHello, ref writeOffset, _connState.IvLen);
                                await _baseStream.WriteAsync(_buf, 0, writeOffset, cancellationToken);
                                await _baseStream.FlushAsync(cancellationToken);
                                _buf = bufSaved;
                                _handshakeMessagesBuffer.ClearMessages();
                            }
                            else
                            {
                                // Ignore hello request messages when we are renegotiating,
                                // by setting ignoreHelloRequests to false below in AddBytes, if _pendingConnState != null
                                if (_waitingForChangeCipherSpec)
                                {
                                    SendAlertFatal(AlertDescription.UnexpectedMessage);
                                }

                                if (_handshakeMessagesBuffer.HasServerHelloDone)
                                {
                                    byte[] bufSaved = _buf;
                                    _buf = _renegotiationTempWriteBuf;
                                    var responseLen = TraverseHandshakeMessages();
                                    await _baseStream.WriteAsync(_buf, 0, responseLen, cancellationToken);
                                    await _baseStream.FlushAsync(cancellationToken);
                                    ResetWritePos();
                                    _waitingForChangeCipherSpec = true;
                                    _buf = bufSaved;
                                    _renegotiationTempWriteBuf = null;
                                }
                            }
                        }
                    }

                    if (_eof)
                    {
                        return 0;
                    }

                    if (onlyProcessHandshake)
                    {
                        // No data is available in our buffer and we're not waiting for the handshake to complete
                        if (_readStart == _readEnd && _decryptedReadPos == _decryptedReadEnd && !(_pendingConnState != null || _waitingForChangeCipherSpec || _waitingForFinished))
                        {
                            if (!readNewDataIfAvailable || !((NetworkStream)_baseStream).DataAvailable)
                                return 0;
                        // Else there is data available in the NetworkStream and we want to look at it. The record will be read and processed further down.
                        }

                        // There is application data available in our buffer
                        if (_bufferedReadData != null || _decryptedReadPos < _decryptedReadEnd)
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (_bufferedReadData != null)
                        {
                            var buf = _bufferedReadData.Peek();
                            var toRead = Math.Min(buf.Length - _posBufferedReadData, len);
                            Buffer.BlockCopy(buf, _posBufferedReadData, buffer, offset, toRead);
                            _posBufferedReadData += toRead;
                            _lenBufferedReadData -= toRead;
                            if (_posBufferedReadData == buf.Length)
                            {
                                _bufferedReadData.Dequeue();
                                _posBufferedReadData = 0;
                                if (_bufferedReadData.Count == 0)
                                {
                                    _bufferedReadData = null;
                                }
                            }

                            return toRead;
                        }

                        if (_decryptedReadPos < _decryptedReadEnd)
                        {
                            var toRead = Math.Min(_decryptedReadEnd - _decryptedReadPos, len);
                            Buffer.BlockCopy(_buf, _decryptedReadPos, buffer, offset, toRead);
                            _decryptedReadPos += toRead;
                            return toRead;
                        }
                    }

                    if (!await (ReadRecordAsync(cancellationToken)))
                    {
                        _eof = true;
                        return 0;
                    }

                    Decrypt();
                    if (_contentType == ContentType.ApplicationData)
                    {
                        if (_readConnState.ReadAes == null || _waitingForFinished)
                        {
                            // Bad state, cannot read application data with null cipher, or until finished has been received
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        }

                        _decryptedReadPos = _plaintextStart;
                        _decryptedReadEnd = _decryptedReadPos + _plaintextLen;
                        continue;
                    }
                    else if (_contentType == ContentType.ChangeCipherSpec)
                    {
                        ParseChangeCipherSpec();
                        _waitingForChangeCipherSpec = false;
                        _waitingForFinished = true;
                        continue;
                    }
                    else if (_contentType == ContentType.Handshake)
                    {
                        _handshakeMessagesBuffer.AddBytes(_buf, _plaintextStart, _plaintextLen, _pendingConnState != null ? HandshakeMessagesBuffer.IgnoreHelloRequestsSetting.IgnoreHelloRequestsUntilFinished : HandshakeMessagesBuffer.IgnoreHelloRequestsSetting.IncludeHelloRequests);
                        if (_handshakeMessagesBuffer.Messages.Count > 5)
                        {
                            // There can never be more than 5 handshake messages in a handshake
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        }
                    // The handshake message(s) will be processed in the loop's next iteration
                    }
                    else if (_contentType == ContentType.Alert)
                    {
                        await HandleAlertMessageAsync(cancellationToken);
                    }
                    else
                    {
                        SendAlertFatal(AlertDescription.UnexpectedMessage);
                    }
                }
            }
            catch (ClientAlertException e)
            {
                await WriteAlertFatalAsync(e.Description, cancellationToken);
                throw new IOException(e.ToString(), e);
            }
        }

        public async Task<bool> HasBufferedReadDataAsync(bool checkNetworkStream, CancellationToken cancellationToken)
        {
            if (_closed)
                return false;
            if (_lenBufferedReadData > 0)
                return true;
            if (_decryptedReadPos < _decryptedReadEnd)
                return true;
            if (_readStart == _readEnd && !checkNetworkStream)
                return false;
            // Otherwise there may be buffered unprocessed packets. We check if any of them is application data.
            int pos = _readStart;
            while (pos < _readEnd)
            {
                if ((ContentType)_buf[pos] == ContentType.ApplicationData)
                    return true;
                if (pos + 5 >= _readEnd)
                    break;
                pos += 3;
                int recordLen = Utils.ReadUInt16(_buf, ref pos);
                pos += recordLen;
            }

            // If none of them were application data, they should be handshake messages/change cipher suite.
            // Process potential renegotiation, but stop when application data is received, or the buffer(s) becomes empty.
            return await (ReadInternalAsync(null, 0, 0, true, checkNetworkStream, cancellationToken)) == 1;
        }
    }
}