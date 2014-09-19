using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    public class AsyncTests : TestBase
    {
        public AsyncTests(string backendVersion) : base(backendVersion) {}

        [Test]
        public async void Read()
        {
            using (var cmd = new NpgsqlCommand("SELECT 1", Conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                await reader.ReadAsync();
                Assert.That(reader[0], Is.EqualTo(1));
            }
        }
    }
}
