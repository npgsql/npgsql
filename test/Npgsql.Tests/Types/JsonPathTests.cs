﻿using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public class JsonPathTests : MultiplexingTestBase
    {
        public JsonPathTests(MultiplexingMode multiplexingMode)
            : base(multiplexingMode) { }

        static readonly object[] ReadWriteCases = new[]
        {
            new object[] { "'$'::text", "$" },
            new object[] { "'$\"varname\"'::text", "$\"varname\"" },
        };

        [Test]
        [TestCaseSource(nameof(ReadWriteCases))]
        public async Task Read(string query, string expected)
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT " + query, conn);
            using var rdr = await cmd.ExecuteReaderAsync();

            rdr.Read();
            Assert.That(rdr.GetFieldValue<string>(0), Is.EqualTo(expected));
            Assert.That(rdr.GetTextReader(0).ReadToEnd(), Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(ReadWriteCases))]
        public async Task Write(string query, string expected)
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p, @p::text = " + query, conn) { Parameters = { new NpgsqlParameter("p", NpgsqlDbType.JsonPath) { Value = expected } } };
            using var rdr = await cmd.ExecuteReaderAsync();

            rdr.Read();
            Assert.That(rdr.GetFieldValue<string>(0), Is.EqualTo(expected));
            Assert.That(rdr.GetFieldValue<bool>(1));
        }
    }
}
