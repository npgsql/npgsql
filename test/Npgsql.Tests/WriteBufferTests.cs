using System.IO;
using Npgsql.Internal;
using Npgsql.Util;
using NUnit.Framework;

namespace Npgsql.Tests
{
    class WriteBufferTests
    {
        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1275")]
        public void Write_zero_characters()
        {
            // Fill up the buffer entirely
            WriteBuffer.WriteBytes(new byte[WriteBuffer.Size], 0, WriteBuffer.Size);
            Assert.That(WriteBuffer.WriteSpaceLeft, Is.Zero);

            int charsUsed;
            bool completed;
            WriteBuffer.WriteStringChunked("hello", 0, 5, true, out charsUsed, out completed);
            Assert.That(charsUsed, Is.Zero);
            Assert.That(completed, Is.False);
            WriteBuffer.WriteStringChunked("hello".ToCharArray(), 0, 5, true, out charsUsed, out completed);
            Assert.That(charsUsed, Is.Zero);
            Assert.That(completed, Is.False);
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2849")]
        public void Chunked_string_encoding_fits()
        {
            WriteBuffer.WriteBytes(new byte[WriteBuffer.Size - 1], 0, WriteBuffer.Size - 1);
            Assert.That(WriteBuffer.WriteSpaceLeft, Is.EqualTo(1));

            var charsUsed = 1;
            var completed = true;
            // This unicode character is three bytes when encoded in UTF8
            Assert.That(() => WriteBuffer.WriteStringChunked("\uD55C", 0, 1, true, out charsUsed, out completed), Throws.Nothing);
            Assert.That(charsUsed, Is.EqualTo(0));
            Assert.That(completed, Is.False);
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2849")]
        public void Chunked_byte_array_encoding_fits()
        {
            WriteBuffer.WriteBytes(new byte[WriteBuffer.Size - 1], 0, WriteBuffer.Size - 1);
            Assert.That(WriteBuffer.WriteSpaceLeft, Is.EqualTo(1));

            var charsUsed = 1;
            var completed = true;
            // This unicode character is three bytes when encoded in UTF8
            Assert.That(() => WriteBuffer.WriteStringChunked("\uD55C".ToCharArray(), 0, 1, true, out charsUsed, out completed), Throws.Nothing);
            Assert.That(charsUsed, Is.EqualTo(0));
            Assert.That(completed, Is.False);
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3733")]
        public void Chunked_string_encoding_fits_with_surrogates()
        {
            WriteBuffer.WriteBytes(new byte[WriteBuffer.Size - 1]);
            Assert.That(WriteBuffer.WriteSpaceLeft, Is.EqualTo(1));

            var charsUsed = 1;
            var completed = true;
            var cyclone = "🌀";

            Assert.That(() => WriteBuffer.WriteStringChunked(cyclone, 0, cyclone.Length, true, out charsUsed, out completed), Throws.Nothing);
            Assert.That(charsUsed, Is.EqualTo(0));
            Assert.That(completed, Is.False);
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3733")]
        public void Chunked_char_array_encoding_fits_with_surrogates()
        {
            WriteBuffer.WriteBytes(new byte[WriteBuffer.Size - 1]);
            Assert.That(WriteBuffer.WriteSpaceLeft, Is.EqualTo(1));

            var charsUsed = 1;
            var completed = true;
            var cyclone = "🌀";

            Assert.That(() => WriteBuffer.WriteStringChunked(cyclone.ToCharArray(), 0, cyclone.Length, true, out charsUsed, out completed), Throws.Nothing);
            Assert.That(charsUsed, Is.EqualTo(0));
            Assert.That(completed, Is.False);
        }

#pragma warning disable CS8625
        [SetUp]
        public void SetUp()
        {
            Underlying = new MemoryStream();
            WriteBuffer = new NpgsqlWriteBuffer(null, Underlying, null, NpgsqlReadBuffer.DefaultSize, PGUtil.UTF8Encoding);
        }
#pragma warning restore CS8625

        // ReSharper disable once InconsistentNaming
        NpgsqlWriteBuffer WriteBuffer = default!;
        // ReSharper disable once InconsistentNaming
        MemoryStream Underlying = default!;
    }
}
