using System;
using System.IO;
using NUnit.Framework;

namespace Npgsql.Tests
{
    class ReadBufferTests
    {
        [Test]
        public void Skip()
        {
            for (byte i = 0; i < 50; i++)
                Underlying.WriteByte(i);
            Underlying.Seek(0, SeekOrigin.Begin);

            ReadBuffer.Ensure(10);
            ReadBuffer.Skip(7);
            Assert.That(ReadBuffer.ReadByte(), Is.EqualTo(7));
            ReadBuffer.Skip(10);
            ReadBuffer.Ensure(1);
            Assert.That(ReadBuffer.ReadByte(), Is.EqualTo(18));
            ReadBuffer.Skip(20);
            ReadBuffer.Ensure(1);
            Assert.That(ReadBuffer.ReadByte(), Is.EqualTo(39));
        }

        [Test]
        public void ReadSingle()
        {
            const float expected = 8.7f;
            var bytes = BitConverter.GetBytes(expected);
            Array.Reverse(bytes);
            Underlying.Write(bytes, 0, bytes.Length);
            Underlying.WriteByte(8);
            Underlying.Seek(0, SeekOrigin.Begin);

            ReadBuffer.Ensure(5);
            Assert.That(ReadBuffer.ReadSingle(), Is.EqualTo(expected));
            Assert.That(ReadBuffer.ReadByte(), Is.EqualTo(8));
        }

        [Test]
        public void ReadDouble()
        {
            const double expected = 8.7;
            var bytes = BitConverter.GetBytes(expected);
            Array.Reverse(bytes);
            Underlying.Write(bytes, 0, bytes.Length);
            Underlying.WriteByte(8);
            Underlying.Seek(0, SeekOrigin.Begin);

            ReadBuffer.Ensure(9);
            Assert.That(ReadBuffer.ReadDouble(), Is.EqualTo(expected));
            Assert.That(ReadBuffer.ReadByte(), Is.EqualTo(8));
        }

        [SetUp]
        public void SetUp()
        {
            Underlying = new MemoryStream();
            ReadBuffer = new NpgsqlReadBuffer(null, Underlying, NpgsqlReadBuffer.DefaultSize, PGUtil.UTF8Encoding, PGUtil.RelaxedUTF8Encoding);
        }

        // ReSharper disable once InconsistentNaming
        NpgsqlReadBuffer ReadBuffer;
        // ReSharper disable once InconsistentNaming
        MemoryStream Underlying;
    }
}
