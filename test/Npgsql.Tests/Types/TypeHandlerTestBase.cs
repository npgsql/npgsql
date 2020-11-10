using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    public abstract class TypeHandlerTestBase<T> : MultiplexingTestBase
    {
        readonly NpgsqlDbType? _npgsqlDbType;
        readonly string? _typeName;
        readonly string? _minVersion;

        protected TypeHandlerTestBase(MultiplexingMode multiplexingMode, NpgsqlDbType? npgsqlDbType, string? typeName, string? minVersion = null)
            : base(multiplexingMode) => (_npgsqlDbType, _typeName, _minVersion) = (npgsqlDbType, typeName, minVersion);

        [OneTimeSetUp]
        public async Task MinimumPgVersion()
        {
            if (_minVersion is string minVersion)
            {
                using var conn = await OpenConnectionAsync();
                TestUtil.MinimumPgVersion(conn, minVersion);
            }
        }

        [Test]
        [TestCaseSource("TestCases")]
        public async Task Read(string query, T expected)
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand($"SELECT {query}", conn);

            Assert.AreEqual(await cmd.ExecuteScalarAsync(), expected);
        }

        [Test]
        [TestCaseSource("TestCases")]
        public async Task Write(string query, T expected)
        {
            var parameter = new NpgsqlParameter<T>("p", expected);

            if (_npgsqlDbType != null)
                parameter.NpgsqlDbType = _npgsqlDbType.Value;

            if (_typeName != null)
                parameter.DataTypeName = _typeName;

            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand($"SELECT {query}::text = @p::text", conn)
            {
                Parameters = { parameter }
            };

            Assert.That(await cmd.ExecuteScalarAsync(), Is.True);
        }
    }
}
