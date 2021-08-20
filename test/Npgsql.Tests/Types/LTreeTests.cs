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
    public class LTreeTests : TypeHandlerTestBase<string>
    {
        public LTreeTests(MultiplexingMode multiplexingMode, bool useTypeName) : base(
            multiplexingMode,
            useTypeName ? null : NpgsqlDbType.LTree,
            useTypeName ? "ltree" : null)
        { }

        public static IEnumerable TestCases() => new[]
        {
            new object[] { "'Top.Science.Astronomy'::ltree", "Top.Science.Astronomy" }
        };

        [OneTimeSetUp]
        public async Task SetUp()
        {
            using var conn = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(conn, "13.0");
            await TestUtil.EnsureExtensionAsync(conn, "ltree");
        }
    }
}
