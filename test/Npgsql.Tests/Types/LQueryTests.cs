using System.Collections;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    [TestFixture(MultiplexingMode.NonMultiplexing, false)]
    [TestFixture(MultiplexingMode.NonMultiplexing, true)]
    [TestFixture(MultiplexingMode.Multiplexing, false)]
    [TestFixture(MultiplexingMode.Multiplexing, true)]
    public class LQueryTests : TypeHandlerTestBase<string>
    {
        public LQueryTests(MultiplexingMode multiplexingMode, bool useTypeName) : base(
            multiplexingMode,
            useTypeName ? null : NpgsqlDbType.LQuery,
            useTypeName ? "lquery" : null,
            minVersion: "13.0")
        { }

        public static IEnumerable TestCases() => new[]
        {
            new object[] { "'Top.Science.*'::lquery", "Top.Science.*" }
        };

        [OneTimeSetUp]
        public async Task SetUp()
        {
            using var conn = await OpenConnectionAsync();
            await TestUtil.EnsureExtensionAsync(conn, "ltree");
        }
    }
}
