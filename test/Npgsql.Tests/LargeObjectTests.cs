using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Npgsql.Tests;

#pragma warning disable CS0618 // Large object support is obsolete

public class LargeObjectTests : TestBase
{
    [Test]
    public void Test()
    {
        using var conn = OpenConnection();
        using var transaction = conn.BeginTransaction();
        var manager = new NpgsqlLargeObjectManager(conn);
        var oid = manager.Create();
        using (var stream = manager.OpenReadWrite(oid))
        {
            var buf = "Hello"u8.ToArray();
            stream.Write(buf, 0, buf.Length);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            var buf2 = new byte[buf.Length];
            stream.ReadExactly(buf2, 0, buf2.Length);
            Assert.That(buf.SequenceEqual(buf2));

            Assert.That(stream.Position, Is.EqualTo(5));

            Assert.That(stream.Length, Is.EqualTo(5));

            stream.Seek(-1, System.IO.SeekOrigin.Current);
            Assert.That(stream.ReadByte(), Is.EqualTo((int)'o'));

            manager.MaxTransferBlockSize = 3;

            stream.Write(buf, 0, buf.Length);
            stream.Seek(-5, System.IO.SeekOrigin.End);
            var buf3 = new byte[100];
            Assert.That(stream.Read(buf3, 0, 100), Is.EqualTo(5));
            Assert.That(buf.SequenceEqual(buf3.Take(5)));

            stream.SetLength(43);
            Assert.That(stream.Length, Is.EqualTo(43));
        }

        manager.Unlink(oid);

        transaction.Rollback();
    }
}
