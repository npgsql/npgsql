using System.IO;
using NUnit.Framework;

namespace Npgsql.Tests
{
    class WriteBufferTests
    {
        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1275")]
        public void WriteZeroChars()
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

        [SetUp]
        public void SetUp()
        {
            Underlying = new MemoryStream();
            WriteBuffer = new NpgsqlWriteBuffer(null, Underlying, NpgsqlReadBuffer.DefaultSize, PGUtil.UTF8Encoding);
        }

        // ReSharper disable once InconsistentNaming
        NpgsqlWriteBuffer WriteBuffer;
        // ReSharper disable once InconsistentNaming
        MemoryStream Underlying;
    }
}
