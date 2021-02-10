using System;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public class FullTextSearchTests : MultiplexingTestBase
    {
        public FullTextSearchTests(MultiplexingMode multiplexingMode)
            : base(multiplexingMode) { }

        [Test]
        public async Task TsVector()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            var inputVec = NpgsqlTsVector.Parse(" a:12345C  a:24D a:25B b c d 1 2 a:25A,26B,27,28");

            cmd.CommandText = "Select :p";
            cmd.Parameters.AddWithValue("p", inputVec);
            var outputVec = await cmd.ExecuteScalarAsync();
            Assert.AreEqual(inputVec.ToString(), outputVec!.ToString());
        }
    }
}
