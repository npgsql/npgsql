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
            using var conn = OpenConnection();
            var queryString = conn.PostgreSqlVersion < new Version(9, 6)
                ? "'a' & !( 'c' | 'd' ) & !!'a' & 'b' | 'ä' | 'f'"
                : "'a' & !( 'c' | 'd' ) & !!'a' & 'b' | 'ä' | 'x' <-> 'y' | 'x' <10> 'y' | 'd' <0> 'e' | 'f'";
            var query = NpgsqlTsQuery.Parse(queryString);

            using var cmd = new NpgsqlCommand("SELECT @s::tsquery, @q, @q::text", conn)
            {
                Parameters =
                {
                    new NpgsqlParameter("s", queryString),
                    new NpgsqlParameter("q", query),
                }
            };

            using var reader = cmd.ExecuteReader();
            reader.ReadAsync();

            Assert.AreEqual(query.ToString(), reader.GetFieldValue<NpgsqlTsQuery>(0).ToString());
            Assert.AreEqual(query.ToString(), reader.GetFieldValue<NpgsqlTsQuery>(1).ToString());
            Assert.AreEqual(queryString, reader.GetFieldValue<string>(2));
        }
    }
}
