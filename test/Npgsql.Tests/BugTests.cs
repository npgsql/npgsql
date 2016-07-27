using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class BugTests : TestBase
    {
        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1210")]
        public void ManyParametersWithMixedFormatCode()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                var sb = new StringBuilder("SELECT @text_param");
                cmd.Parameters.AddWithValue("@text_param", "some_text");
                for (var i = 0; i < conn.BufferSize; i++)
                {
                    var paramName = $"@binary_param{i}";
                    sb.Append(",");
                    sb.Append(paramName);
                    cmd.Parameters.AddWithValue(paramName, 8);
                }
                cmd.CommandText = sb.ToString();

                Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("54000")
                );
            }
        }
    }
}
