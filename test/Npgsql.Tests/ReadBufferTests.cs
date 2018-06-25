#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql;
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
            ReadBuffer = new NpgsqlReadBuffer(null, Underlying, NpgsqlReadBuffer.DefaultSize, PGUtil.UTF8Encoding);
        }

        // ReSharper disable once InconsistentNaming
        NpgsqlReadBuffer ReadBuffer;
        // ReSharper disable once InconsistentNaming
        MemoryStream Underlying;
    }
}
