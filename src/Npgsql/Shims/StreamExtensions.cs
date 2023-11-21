#if !NET7_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;

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
    }
}
#endif
