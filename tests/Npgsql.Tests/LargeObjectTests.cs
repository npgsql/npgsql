using Npgsql;
using Npgsql.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Tests
{
    [TestFixture]
    public class LargeObjectTests : TestBase
    {
        public LargeObjectTests(string backendVersion) : base(backendVersion) { }

        [Test]
        public void Test()
        {
            using (var transaction = Conn.BeginTransaction())
            {
                var manager = new NpgsqlLargeObjectManager(Conn);
                uint oid = manager.Create();
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