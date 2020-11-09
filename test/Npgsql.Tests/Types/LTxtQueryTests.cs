using System.Collections;

namespace Npgsql.Tests.Types
{
    public class LTxtQueryTests : TypeHandlerTestBase<string>
    {
        public LTxtQueryTests(MultiplexingMode multiplexingMode)
            : base(multiplexingMode, "ltxtquery") { }

        public static IEnumerable TestCases() => new[]
        {
            new object[] { "'Science & Astronomy'::ltxtquery", "Science & Astronomy" }
        };
    }
}
