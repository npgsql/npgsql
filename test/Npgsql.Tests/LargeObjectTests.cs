#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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

using Npgsql;
using Npgsql.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Tests
{
    public class LargeObjectTests : TestBase
    {
        [Test]
        public void Test()
        {
            using (var conn = OpenConnection())
            using (var transaction = conn.BeginTransaction())
            {
                var manager = new NpgsqlLargeObjectManager(conn);
                var oid = manager.Create();
                using (var stream = manager.OpenReadWrite(oid))
                {
                    var buf = Encoding.UTF8.GetBytes("Hello");
                    stream.Write(buf, 0, buf.Length);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    var buf2 = new byte[buf.Length];
                    stream.Read(buf2, 0, buf2.Length);
                    Assert.That(buf.SequenceEqual(buf2));

                    Assert.AreEqual(5, stream.Position);

                    Assert.AreEqual(5, stream.Length);

                    stream.Seek(-1, System.IO.SeekOrigin.Current);
                    Assert.AreEqual((int)'o', stream.ReadByte());

                    manager.MaxTransferBlockSize = 3;

                    stream.Write(buf, 0, buf.Length);
                    stream.Seek(-5, System.IO.SeekOrigin.End);
                    var buf3 = new byte[100];
                    Assert.AreEqual(5, stream.Read(buf3, 0, 100));
                    Assert.That(buf.SequenceEqual(buf3.Take(5)));

                    stream.SetLength(43);
                    Assert.AreEqual(43, stream.Length);
                }

                manager.Unlink(oid);

                transaction.Rollback();
            }
        }
    }
}
