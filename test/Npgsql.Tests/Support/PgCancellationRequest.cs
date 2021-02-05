using System.IO;
using Npgsql.Internal;

namespace Npgsql.Tests.Support
{
    class PgCancellationRequest
    {
        readonly NpgsqlReadBuffer _readBuffer;
        readonly NpgsqlWriteBuffer _writeBuffer;
        readonly Stream _stream;

        public int ProcessId { get; }
        public int Secret { get; }

        bool completed;

        public PgCancellationRequest(NpgsqlReadBuffer readBuffer, NpgsqlWriteBuffer writeBuffer, Stream stream, int processId, int secret)
        {
            _readBuffer = readBuffer;
            _writeBuffer = writeBuffer;
            _stream = stream;

            ProcessId = processId;
            Secret = secret;
        }

        public void Complete()
        {
            if (completed)
                return;

            _readBuffer.Dispose();
            _writeBuffer.Dispose();
            _stream.Dispose();

            completed = true;
        }
    }
}
