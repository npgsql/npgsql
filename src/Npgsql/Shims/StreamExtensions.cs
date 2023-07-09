#if NETSTANDARD2_0 || !NET7_0_OR_GREATER
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

// ReSharper disable once CheckNamespace
namespace System.IO
{
    // Helpers to read/write Span/Memory<byte> to Stream before netstandard 2.1
    static class StreamExtensions
    {
        public static void ReadExactly(this Stream stream, Span<byte> buffer)
        {
            var totalRead = 0;
            while (totalRead < buffer.Length)
            {
                var read = stream.Read(buffer.Slice(totalRead));
                if (read is 0)
                    throw new EndOfStreamException();

                totalRead += read;
            }
        }

        public static async ValueTask ReadExactlyAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var totalRead = 0;
            while (totalRead < buffer.Length)
            {
                var read = await stream.ReadAsync(buffer.Slice(totalRead), cancellationToken).ConfigureAwait(false);
                if (read is 0)
                    throw new EndOfStreamException();

                totalRead += read;
            }
        }

#if NETSTANDARD2_0
        public static int Read(this Stream stream, Span<byte> buffer)
        {
            var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                var numRead = stream.Read(sharedBuffer, 0, buffer.Length);
                new Span<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);
                return numRead;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        public static async ValueTask<int> ReadAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                var result = await stream.ReadAsync(sharedBuffer, 0, buffer.Length, cancellationToken);
                new Span<byte>(sharedBuffer, 0, result).CopyTo(buffer.Span);
                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        public static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
        {
            var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                buffer.CopyTo(sharedBuffer);
                stream.Write(sharedBuffer, 0, buffer.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        public static async ValueTask WriteAsync(this Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            buffer.Span.CopyTo(sharedBuffer);
            try
            {
                await stream.WriteAsync(sharedBuffer, 0, buffer.Length, cancellationToken);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }
#endif
    }
}
#endif
