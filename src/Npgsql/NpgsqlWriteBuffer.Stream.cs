using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    public sealed partial class NpgsqlWriteBuffer
    {
        sealed class ParameterStream : NpgsqlStream
        {
            readonly NpgsqlWriteBuffer _buf;

            internal ParameterStream(NpgsqlWriteBuffer buf)
                : base(canWrite: true)
                => _buf = buf;

            internal void Init()
                => IsDisposed = false;

            internal protected override Task FlushAsync(CancellationToken cancellationToken, bool async) => Task.CompletedTask;

            /// <inheritdoc />
            internal protected override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool async)
            {
                while (count > 0)
                {
                    var left = _buf.WriteSpaceLeft;
                    if (left == 0)
                        return WriteLong(buffer, offset, count, async);

                    var slice = Math.Min(count, left);
                    _buf.WriteBytes(buffer, offset, slice);
                    offset += slice;
                    count -= slice;
                }

                return Task.CompletedTask;
            }

            async Task WriteLong(byte[] buffer, int offset, int count, bool async)
            {
                while (count > 0)
                {
                    var left = _buf.WriteSpaceLeft;
                    if (left == 0)
                    {
                        await _buf.Flush(async);
                        continue;
                    }
                    var slice = Math.Min(count, left);
                    _buf.WriteBytes(buffer, offset, slice);
                    offset += slice;
                    count -= slice;
                }
            }
        }
    }
}
