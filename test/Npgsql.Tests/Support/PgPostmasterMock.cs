using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Util;

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
        readonly Task _acceptTask;
        readonly List<PgServerMock> _allServers = new List<PgServerMock>();
        readonly Queue<PgServerMock> _pendingServers = new Queue<PgServerMock>();
        readonly Queue<(int ProcessId, int Secret)> _pendingCancellationRequests
            = new Queue<(int ProcessId, int Secret)>();
        int _processIdCounter;

        internal string ConnectionString { get; }

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
            _acceptTask = AcceptAndAuthenticate();
        }

        async Task AcceptAndAuthenticate()
        {
            while (true)
            {
                var clientSocket = await _socket.AcceptAsync();

                var stream = new NetworkStream(clientSocket, true);
                var readBuffer = new NpgsqlReadBuffer(null!, stream, clientSocket, ReadBufferSize, Encoding, RelaxedEncoding);
                var writeBuffer = new NpgsqlWriteBuffer(null!, stream, clientSocket, WriteBufferSize, Encoding);

                await readBuffer.EnsureAsync(4);
                var len = readBuffer.ReadInt32();
                await readBuffer.EnsureAsync(len - 4);

                if (readBuffer.ReadInt32() == CancelRequestCode)
                {
                    _pendingCancellationRequests.Enqueue((readBuffer.ReadInt32(), readBuffer.ReadInt32()));
                    readBuffer.Dispose();
                    writeBuffer.Dispose();
                    stream.Dispose();
                    continue;
                }

                // This is not a cancellation, "spawn" a new server
                readBuffer.ReadPosition -= 8;
                var serverMock = new PgServerMock(stream, readBuffer, writeBuffer, ++_processIdCounter);

                _allServers.Add(serverMock);
                _pendingServers.Enqueue(serverMock);

                await serverMock.Startup();
            }

            // ReSharper disable once FunctionNeverReturns
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
                await _acceptTask;
            }
            catch
            {
                // Swallow all exceptions
            }

            // Destroy all servers created by this postmaster
            foreach (var server in _allServers)
                server.Dispose();
        }
    }
}
