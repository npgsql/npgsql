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

        [SetUp]
        public void SetUp()
        {
            Underlying = new MemoryStream();
            Buffer = new NpgsqlBufferedStream(Underlying, 10, Encoding.UTF8);
        }

        NpgsqlBufferedStream Buffer;
        MemoryStream Underlying;
    }
}
