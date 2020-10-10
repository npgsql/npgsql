using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        readonly Queue<PgServerMock> _pendingServers = new Queue<PgServerMock>();
        readonly Queue<(int ProcessId, int Secret)> _pendingCancellationRequests
            = new Queue<(int ProcessId, int Secret)>();
        Task? _acceptClientsTask;
        int _processIdCounter;

        internal string ConnectionString { get; }

        internal static PgPostmasterMock Start(string? connectionString = null)
        {
            var mock = new PgPostmasterMock(connectionString);
            mock.AcceptClients();
            return mock;
        }

        internal PgPostmasterMock(string? connectionString = null)
        {
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
            _acceptClientsTask = DoAcceptClients();

            async Task DoAcceptClients()
            {
                while (true)
                {
                    var serverOrCancellationRequest = await Accept();
                    if (serverOrCancellationRequest.Server is { } server)
                    {
                        _pendingServers.Enqueue(server);
                        await server.Startup();
                    }
                    else
                        _pendingCancellationRequests.Enqueue(serverOrCancellationRequest.CancellationRequest!.Value);
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
            var serverOrCancellationRequest = await Accept();
            if (serverOrCancellationRequest.Server is null)
                throw new InvalidOperationException("Expected new server connection but a cancellation request occurred instead");
            return serverOrCancellationRequest.Server;
        }

        internal PgServerMock GetPendingServer()
            => _pendingServers.TryDequeue(out var server)
                ? server
                : throw new InvalidOperationException("No pending server");

        internal (int ProcessId, int Secret) GetPendingCancellationRequest()
            => _pendingCancellationRequests.TryDequeue(out var cancellationRequest)
                ? cancellationRequest
                : throw new InvalidOperationException("No pending cancellation request");

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
