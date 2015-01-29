using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    class NpgsqlBufferedStreamTests
    {
        [Test]
        public void Skip()
        {
            for (byte i = 0; i < 50; i++)
                Underlying.WriteByte(i);
            Underlying.Seek(0, SeekOrigin.Begin);

            Buffer.Ensure(10);
            Buffer.Skip(7);
            Assert.That(Buffer.ReadByte(), Is.EqualTo(7));
            Buffer.Skip(10);
            Buffer.Ensure(1);
            Assert.That(Buffer.ReadByte(), Is.EqualTo(18));
            Buffer.Skip(20);
            Buffer.Ensure(1);
            Assert.That(Buffer.ReadByte(), Is.EqualTo(39));
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

            Buffer.Ensure(5);
            Assert.That(Buffer.ReadSingle(), Is.EqualTo(expected));
            Assert.That(Buffer.ReadByte(), Is.EqualTo(8));
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

            Buffer.Ensure(9);
            Assert.That(Buffer.ReadDouble(), Is.EqualTo(expected));
            Assert.That(Buffer.ReadByte(), Is.EqualTo(8));
        }

        [Test]
        public void ReadChars()
        {
            const string expected = "This string is bigger than the buffer length";
            var bytes = Encoding.UTF8.GetBytes(expected);
            Underlying.Write(bytes, 0, bytes.Length);
            Underlying.Seek(0, SeekOrigin.Begin);

            var chars = new char[expected.Length + 5];
            int bytesRead, charsRead;
            Buffer.ReadChars(chars, 5, expected.Length, bytes.Length, out bytesRead, out charsRead);
            Assert.That(charsRead, Is.EqualTo(expected.Length));
            Assert.That(bytesRead, Is.EqualTo(bytes.Length));
            var actual = new string(chars, 5, expected.Length);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ReadCharsTooMany()
        {
            const string expected = "This string is bigger than the buffer length";
            var bytes = Encoding.UTF8.GetBytes(expected);
            Underlying.Write(bytes, 0, bytes.Length);
            Underlying.Seek(0, SeekOrigin.Begin);

            var chars = new char[expected.Length + 5];
            int bytesRead, charsRead;
            Buffer.ReadChars(chars, 0, expected.Length + 5, bytes.Length, out bytesRead, out charsRead);
            Assert.That(charsRead, Is.EqualTo(expected.Length));
            Assert.That(bytesRead, Is.EqualTo(bytes.Length));
            var actual = new string(chars, 0, expected.Length);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [SetUp]
        public void SetUp()
        {
            Underlying = new MemoryStream();
            Buffer = new NpgsqlBuffer(Underlying);
        }

        NpgsqlBuffer Buffer;
        MemoryStream Underlying;
    }
}
