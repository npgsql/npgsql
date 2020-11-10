using System.Collections;

namespace Npgsql.Tests.Types
{
    public class LTreeTests : TypeHandlerTestBase<string>
    {
        public LTreeTests(MultiplexingMode multiplexingMode)
            : base(multiplexingMode, "ltree", "13.0") { }

        public static IEnumerable TestCases() => new[]
        {
            new object[] { "'Top.Science.Astronomy'::ltree", "Top.Science.Astronomy" }
        };
    }
}
