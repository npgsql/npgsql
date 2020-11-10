using System.Collections;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    [TestFixture(MultiplexingMode.NonMultiplexing, false)]
    [TestFixture(MultiplexingMode.NonMultiplexing, true)]
    [TestFixture(MultiplexingMode.Multiplexing, false)]
    [TestFixture(MultiplexingMode.Multiplexing, true)]
    public class LTxtQueryTests : TypeHandlerTestBase<string>
    {
        public LTxtQueryTests(MultiplexingMode multiplexingMode, bool useTypeName) : base(
            multiplexingMode,
            useTypeName ? null : NpgsqlDbType.LTxtQuery,
            useTypeName ? "ltxtquery" : null,
            minVersion: "13.0")
        { }

        public static IEnumerable TestCases() => new[]
        {
            new object[] { "'Science & Astronomy'::ltxtquery", "Science & Astronomy" }
        };
    }
}
