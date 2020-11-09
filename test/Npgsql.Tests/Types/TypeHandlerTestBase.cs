using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types
{
    public abstract class TypeHandlerTestBase<T> : MultiplexingTestBase
    {
        readonly string? _typeName;
        readonly string? _minVersion;

        protected TypeHandlerTestBase(MultiplexingMode multiplexingMode, string? typeName = null, string? minVersion = null)
            : base(multiplexingMode) => (_typeName, _minVersion) = (typeName, minVersion);

        [Test]
        [TestCaseSource("TestCases")]
        public async Task Read(string query, T expected)
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand($"SELECT {query}", conn);

            if (_minVersion is string minVersion)
                MinimumPgVersion(conn, minVersion);

            Assert.AreEqual(await cmd.ExecuteScalarAsync(), expected);
        }

        [Test]
        [TestCaseSource("TestCases")]
        public async Task Write(string query, T expected)
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand($"SELECT {query}::text = @p::text", conn)
            {
                Parameters = { new NpgsqlParameter<T>("p", expected) { DataTypeName = _typeName } }
            };

            if (_minVersion is string minVersion)
                MinimumPgVersion(conn, minVersion);

            Assert.That(await cmd.ExecuteScalarAsync(), Is.True);
        }
    }
}
