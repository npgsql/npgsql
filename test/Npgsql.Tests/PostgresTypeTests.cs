using System.Linq;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.TypeMapping;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class PostgresTypeTests : TestBase
    {
        [Test]
        public async Task Base()
        {
            var databaseInfo = await GetDatabaseInfo();

            var text = databaseInfo.BaseTypes.Single(a => a.Name == "text");
            Assert.That(text.DisplayName, Is.EqualTo("text"));
            Assert.That(text.Namespace, Is.EqualTo("pg_catalog"));
            Assert.That(text.FullName, Is.EqualTo("pg_catalog.text"));
        }

        [Test]
        public async Task Array()
        {
            var databaseInfo = await GetDatabaseInfo();

            var textArray = databaseInfo.ArrayTypes.Single(a => a.Name == "text[]");
            Assert.That(textArray.DisplayName, Is.EqualTo("text[]"));
            Assert.That(textArray.Namespace, Is.EqualTo("pg_catalog"));
            Assert.That(textArray.FullName, Is.EqualTo("pg_catalog.text[]"));

            var text = databaseInfo.BaseTypes.Single(a => a.Name == "text");
            Assert.That(textArray.Element, Is.SameAs(text));
            Assert.That(text.Array, Is.SameAs(textArray));
        }

        [Test]
        public async Task Range()
        {
            var databaseInfo = await GetDatabaseInfo();

            var intRange = databaseInfo.RangeTypes.Single(a => a.Name == "int4range");
            Assert.That(intRange.DisplayName, Is.EqualTo("int4range"));
            Assert.That(intRange.Namespace, Is.EqualTo("pg_catalog"));
            Assert.That(intRange.FullName, Is.EqualTo("pg_catalog.int4range"));

            var integer = databaseInfo.BaseTypes.Single(a => a.Name == "integer");
            Assert.That(intRange.Subtype, Is.SameAs(integer));
            Assert.That(integer.Range, Is.SameAs(intRange));
        }

        [Test]
        public async Task Multirange()
        {
            await using (var conn = await OpenConnectionAsync())
                TestUtil.MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");

            var databaseInfo = await GetDatabaseInfo();

            var intMultirange = databaseInfo.MultirangeTypes.Single(a => a.Name == "int4multirange");
            Assert.That(intMultirange.DisplayName, Is.EqualTo("int4multirange"));
            Assert.That(intMultirange.Namespace, Is.EqualTo("pg_catalog"));
            Assert.That(intMultirange.FullName, Is.EqualTo("pg_catalog.int4multirange"));

            var intRange = databaseInfo.RangeTypes.Single(a => a.Name == "int4range");
            Assert.That(intMultirange.Subrange, Is.SameAs(intRange));
            Assert.That(intRange.Multirange, Is.SameAs(intMultirange));
        }

        async Task<NpgsqlDatabaseInfo> GetDatabaseInfo()
        {
            await using var conn = await OpenConnectionAsync();
            return ((ConnectorTypeMapper)conn.TypeMapper).DatabaseInfo;
        }
    }
}
