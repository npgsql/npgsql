using System.IO;
using Npgsql.Internal;

namespace Npgsql.Tests.Support;

class PgCancellationRequest(NpgsqlReadBuffer readBuffer, NpgsqlWriteBuffer writeBuffer, Stream stream, int processId, int secret)
{
    public int ProcessId { get; } = processId;
    public int Secret { get; } = secret;

    bool completed;

    public void Complete()
    {
        if (completed)
            return;

        readBuffer.Dispose();
        writeBuffer.Dispose();
        stream.Dispose();

        completed = true;
    }
}