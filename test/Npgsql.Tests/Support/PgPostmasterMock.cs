using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Npgsql.Util;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests.Support
{
    class PgPostmasterMock : IAsyncDisposable
    {
        const int ReadBufferSize = 8192;
        const int WriteBufferSize = 8192;
        const int CancelRequestCode = 1234 << 16 | 5678;

        static readonly Encoding Encoding = PGUtil.UTF8Encoding;
        static readonly Encoding RelaxedEncoding = PGUtil.RelaxedUTF8Encoding;

        readonly Socket _socket;
        readonly List<PgServerMock> _allServers = new List<PgServerMock>();
        bool _acceptingClients;
        Task? _acceptClientsTask;
        int _processIdCounter;

        ChannelWriter<ServerOrCancellationRequest> _pendingRequestsWriter { get; }
        internal ChannelReader<ServerOrCancellationRequest> PendingRequestsReader { get; }

        internal string ConnectionString { get; }

        internal static PgPostmasterMock Start(string? connectionString = null)
        {
            var mock = new PgPostmasterMock(connectionString);
            mock.AcceptClients();
            return mock;
        }

        internal PgPostmasterMock(string? connectionString = null)
        {
            var pendingRequestsChannel = Channel.CreateUnbounded<ServerOrCancellationRequest>();
            PendingRequestsReader = pendingRequestsChannel.Reader;
            _pendingRequestsWriter = pendingRequestsChannel.Writer;

            var connectionStringBuilder =
                new NpgsqlConnectionStringBuilder(connectionString ?? TestUtil.ConnectionString);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endpoint = new IPEndPoint(IPAddress.Loopback, 0);
            _socket.Bind(endpoint);
            var localEndPoint = (IPEndPoint)_socket.LocalEndPoint!;
            connectionStringBuilder.Host = localEndPoint.Address.ToString();
            connectionStringBuilder.Port = localEndPoint.Port;
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
                    var serverOrCancellationRequest = await Accept();
                    if (serverOrCancellationRequest.Server is { } server)
                    {
                        // Hand off the new server to the client test only once startup is complete, to avoid reading/writing in parallel
                        // during startup. Don't wait for all this to complete - continue to accept other connections in case that's needed.
                        _ = server.Startup().ContinueWith(t => _pendingRequestsWriter.WriteAsync(serverOrCancellationRequest));
                    }
                    else
                    {
                        await _pendingRequestsWriter.WriteAsync(serverOrCancellationRequest);
                    }
                }

                // ReSharper disable once FunctionNeverReturns
            }
        }

        internal async Task<ServerOrCancellationRequest> Accept()
        {
            var clientSocket = await _socket.AcceptAsync();

            var stream = new NetworkStream(clientSocket, true);
            var readBuffer = new NpgsqlReadBuffer(null!, stream, clientSocket, ReadBufferSize, Encoding,
                RelaxedEncoding);
            var writeBuffer = new NpgsqlWriteBuffer(null!, stream, clientSocket, WriteBufferSize, Encoding);

            await readBuffer.EnsureAsync(4);
            var len = readBuffer.ReadInt32();
            await readBuffer.EnsureAsync(len - 4);

            if (readBuffer.ReadInt32() == CancelRequestCode)
            {
                readBuffer.Dispose();
                writeBuffer.Dispose();
                stream.Dispose();
                return new ServerOrCancellationRequest((readBuffer.ReadInt32(), readBuffer.ReadInt32()));
            }

            // This is not a cancellation, "spawn" a new server
            readBuffer.ReadPosition -= 8;
            var server = new PgServerMock(stream, readBuffer, writeBuffer, ++_processIdCounter);
            _allServers.Add(server);
            return new ServerOrCancellationRequest(server);
        }

        internal async Task<PgServerMock> AcceptServer()
        {
            if (_acceptingClients)
                throw new InvalidOperationException($"Already accepting clients via {nameof(AcceptClients)}");
            var serverOrCancellationRequest = await Accept();
            if (serverOrCancellationRequest.Server is null)
                throw new InvalidOperationException("Expected a server connection but got a cancellation request instead");
            return serverOrCancellationRequest.Server;
        }

        internal async Task<(int ProcessId, int Secret)> AcceptCancellationRequest()
        {
            if (_acceptingClients)
                throw new InvalidOperationException($"Already accepting clients via {nameof(AcceptClients)}");
            var serverOrCancellationRequest = await Accept();
            if (serverOrCancellationRequest.CancellationRequest is null)
                throw new InvalidOperationException("Expected a cancellation request but got a server connection instead");
            return serverOrCancellationRequest.CancellationRequest.Value;
        }

        internal async ValueTask<PgServerMock> WaitForServerConnection()
        {
            var serverOrCancellationRequest = await PendingRequestsReader.ReadAsync();
            if (serverOrCancellationRequest.Server is null)
                throw new InvalidOperationException("Expected a server connection but got a cancellation request instead");
            return serverOrCancellationRequest.Server;
        }

        internal async ValueTask<(int ProcessId, int Secret)> WaitForCancellationRequest()
        {
            var serverOrCancellationRequest = await PendingRequestsReader.ReadAsync();
            if (serverOrCancellationRequest.CancellationRequest is null)
                throw new InvalidOperationException("Expected cancellation request but got a server connection instead");
            return serverOrCancellationRequest.CancellationRequest.Value;
        }

        public async ValueTask DisposeAsync()
        {
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

            public ServerOrCancellationRequest((int ProcessId, int Secret) cancellationRequest)
            {
                Server = null;
                CancellationRequest = cancellationRequest;
            }

            internal PgServerMock? Server { get; }
            internal (int ProcessId, int Secret)? CancellationRequest { get; }
        }
    }
}
