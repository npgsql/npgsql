using System;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public class FullTextSearchTests : TestBase
    {
        [Test]
        public void TsVector()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                var inputVec = NpgsqlTsVector.Parse(" a:12345C  a:24D a:25B b c d 1 2 a:25A,26B,27,28");

                cmd.CommandText = "Select :p";
                cmd.Parameters.AddWithValue("p", inputVec);
                var outputVec = cmd.ExecuteScalar();
                Assert.AreEqual(inputVec.ToString(), outputVec.ToString());
            }
        }

        [Test]
        public void TsQuery()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                var query = conn.PostgreSqlVersion < new Version(9, 6)
                    ? NpgsqlTsQuery.Parse("(a & !(c | d)) & (!!a&b) | ä | f")
                    : NpgsqlTsQuery.Parse("(a & !(c | d)) & (!!a&b) | ä | x <-> y | x <10> y | d <0> e | f");

                cmd.CommandText = "Select :p";
                cmd.Parameters.AddWithValue("p", query);
                var output = cmd.ExecuteScalar();
                Assert.AreEqual(query.ToString(), output.ToString());
            }
        }
    }
}
