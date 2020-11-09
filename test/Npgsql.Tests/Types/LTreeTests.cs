using System.Collections;

namespace Npgsql.Tests.Types
{
    public class LTreeTests : TypeHandlerTestBase<string>
    {
        public LTreeTests(MultiplexingMode multiplexingMode)
            : base(multiplexingMode, "ltree") { }

        public static IEnumerable TestCases() => new[]
        {
            new object[] { "'Top.Science.Astronomy'::ltree", "Top.Science.Astronomy" }
        };
    }
}
