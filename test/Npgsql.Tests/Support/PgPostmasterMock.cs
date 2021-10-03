using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Util;

namespace Npgsql.Tests.Support
{
    class PgPostmasterMock : IAsyncDisposable
    {
        const int ReadBufferSize = 8192;
        const int WriteBufferSize = 8192;
        const int CancelRequestCode = 1234 << 16 | 5678;
        const int SslRequest = 80877103;

        static readonly Encoding Encoding = PGUtil.UTF8Encoding;
        static readonly Encoding RelaxedEncoding = PGUtil.RelaxedUTF8Encoding;

        readonly Socket _socket;
        readonly List<PgServerMock> _allServers = new();
        bool _acceptingClients;
        Task? _acceptClientsTask;
        int _processIdCounter;

        readonly bool _completeCancellationImmediately;
        readonly MockState _state;

        ChannelWriter<ServerOrCancellationRequest> _pendingRequestsWriter { get; }
        internal ChannelReader<ServerOrCancellationRequest> PendingRequestsReader { get; }

        internal string ConnectionString { get; }
        internal string Host { get; }
        internal int Port { get; }

        internal static PgPostmasterMock Start(
            string? connectionString = null,
            bool completeCancellationImmediately = true,
            MockState state = MockState.MultipleHostsDisabled)
        {
            var mock = new PgPostmasterMock(connectionString, completeCancellationImmediately, state);
            mock.AcceptClients();
            return mock;
        }

        internal PgPostmasterMock(
            string? connectionString = null,
            bool completeCancellationImmediately = true,
            MockState state = MockState.MultipleHostsDisabled)
        {
            var pendingRequestsChannel = Channel.CreateUnbounded<ServerOrCancellationRequest>();
            PendingRequestsReader = pendingRequestsChannel.Reader;
            _pendingRequestsWriter = pendingRequestsChannel.Writer;

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            _completeCancellationImmediately = completeCancellationImmediately;
            _state = state;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endpoint = new IPEndPoint(IPAddress.Loopback, 0);
            _socket.Bind(endpoint);

            // In some cases we can attempt to connect to a port, which was already in use (doesn't have to be a mock).
            // Clearing the cached state, so the previous state is not leaking.
            if (state != MockState.MultipleHostsDisabled)
            {
                var afterBindEndport = _socket.LocalEndPoint as IPEndPoint;
                Debug.Assert(afterBindEndport is not null);
                var host = afterBindEndport.Address.ToString();
                var port = afterBindEndport.Port;
                ClusterStateCache.RemoveClusterState(host, port);
            }

            var localEndPoint = (IPEndPoint)_socket.LocalEndPoint!;
            Host = localEndPoint.Address.ToString();
            Port = localEndPoint.Port;
            connectionStringBuilder.Host = Host;
            connectionStringBuilder.Port = Port;
            connectionStringBuilder.ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading;
            ConnectionString = connectionStringBuilder.ConnectionString;

            _socket.Listen(5);
        }

        void AcceptClients()
        {
            _acceptingClients = true;
            _acceptClientsTask = DoAcceptClients();

            async Task DoAcceptClients()
            {
                while (true)
                {
                    var serverOrCancellationRequest = await Accept(_completeCancellationImmediately);
                    if (serverOrCancellationRequest.Server is { } server)
                    {
                        // Hand off the new server to the client test only once startup is complete, to avoid reading/writing in parallel
                        // during startup. Don't wait for all this to complete - continue to accept other connections in case that's needed.
                        _ = server.Startup(_state).ContinueWith(t => _pendingRequestsWriter.WriteAsync(serverOrCancellationRequest));
                    }
                    else
                    {
                        await _pendingRequestsWriter.WriteAsync(serverOrCancellationRequest);
                    }
                }

                // ReSharper disable once FunctionNeverReturns
            }
        }

        async Task<ServerOrCancellationRequest> Accept(bool completeCancellationImmediately)
        {
            var clientSocket = await _socket.AcceptAsync();

            var stream = new NetworkStream(clientSocket, true);
            var readBuffer = new NpgsqlReadBuffer(null!, stream, clientSocket, ReadBufferSize, Encoding,
                RelaxedEncoding);
            var writeBuffer = new NpgsqlWriteBuffer(null!, stream, clientSocket, WriteBufferSize, Encoding);

            await readBuffer.EnsureAsync(4);
            var len = readBuffer.ReadInt32();
            await readBuffer.EnsureAsync(len - 4);

            var request = readBuffer.ReadInt32();
            if (request == SslRequest)
            {
                writeBuffer.WriteByte((byte)'N');
                await writeBuffer.Flush(async: true);

                await readBuffer.EnsureAsync(4);
                len = readBuffer.ReadInt32();
                await readBuffer.EnsureAsync(len - 4);
                request = readBuffer.ReadInt32();
            }

            if (request == CancelRequestCode)
            {
                var cancellationRequest = new PgCancellationRequest(readBuffer, writeBuffer, stream, readBuffer.ReadInt32(), readBuffer.ReadInt32());
                if (completeCancellationImmediately)
                {
                    cancellationRequest.Complete();
                }

                return new ServerOrCancellationRequest(cancellationRequest);
            }

            // This is not a cancellation, "spawn" a new server
            readBuffer.ReadPosition -= 8;
            var server = new PgServerMock(stream, readBuffer, writeBuffer, ++_processIdCounter);
            _allServers.Add(server);
            return new ServerOrCancellationRequest(server);
        }

        internal async Task<PgServerMock> AcceptServer(bool completeCancellationImmediately = true)
        {
            if (_acceptingClients)
                throw new InvalidOperationException($"Already accepting clients via {nameof(AcceptClients)}");
            var serverOrCancellationRequest = await Accept(completeCancellationImmediately);
            if (serverOrCancellationRequest.Server is null)
                throw new InvalidOperationException("Expected a server connection but got a cancellation request instead");
            return serverOrCancellationRequest.Server;
        }

        internal async Task<PgCancellationRequest> AcceptCancellationRequest()
        {
            if (_acceptingClients)
                throw new InvalidOperationException($"Already accepting clients via {nameof(AcceptClients)}");
            var serverOrCancellationRequest = await Accept(completeCancellationImmediately: true);
            if (serverOrCancellationRequest.CancellationRequest is null)
                throw new InvalidOperationException("Expected a cancellation request but got a server connection instead");
            return serverOrCancellationRequest.CancellationRequest;
        }

        internal async ValueTask<PgServerMock> WaitForServerConnection()
        {
            var serverOrCancellationRequest = await PendingRequestsReader.ReadAsync();
            if (serverOrCancellationRequest.Server is null)
                throw new InvalidOperationException("Expected a server connection but got a cancellation request instead");
            return serverOrCancellationRequest.Server;
        }

        internal async ValueTask<PgCancellationRequest> WaitForCancellationRequest()
        {
            var serverOrCancellationRequest = await PendingRequestsReader.ReadAsync();
            if (serverOrCancellationRequest.CancellationRequest is null)
                throw new InvalidOperationException("Expected cancellation request but got a server connection instead");
            return serverOrCancellationRequest.CancellationRequest;
        }

        public async ValueTask DisposeAsync()
        {
            if (_state != MockState.MultipleHostsDisabled)
            {
                var endpoint = _socket.LocalEndPoint as IPEndPoint;
                Debug.Assert(endpoint is not null);
                var host = endpoint.Address.ToString();
                var port = endpoint.Port;
                ClusterStateCache.RemoveClusterState(host, port);
            }
            
            // Stop accepting new connections
            _socket.Dispose();
            try
            {
                var acceptTask = _acceptClientsTask;
                if (acceptTask != null)
                    await acceptTask;
            }
            catch
            {
                // Swallow all exceptions
            }

            // Destroy all servers created by this postmaster
            foreach (var server in _allServers)
                server.Dispose();
        }

        internal readonly struct ServerOrCancellationRequest
        {
            public ServerOrCancellationRequest(PgServerMock server)
            {
                Server = server;
                CancellationRequest = null;
            }

            public ServerOrCancellationRequest(PgCancellationRequest cancellationRequest)
            {
                Server = null;
                CancellationRequest = cancellationRequest;
            }

            internal PgServerMock? Server { get; }
            internal PgCancellationRequest? CancellationRequest { get; }
        }
    }

    public enum MockState
    {
        MultipleHostsDisabled = 0,
        Primary = 1,
        PrimaryReadOnly = 2,
        Standby = 3
    }
}
