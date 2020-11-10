using System.Collections;

namespace Npgsql.Tests.Types
{
    public class LTxtQueryTests : TypeHandlerTestBase<string>
    {
        public LTxtQueryTests(MultiplexingMode multiplexingMode)
            : base(multiplexingMode, "ltxtquery", "13.0") { }

        public static IEnumerable TestCases() => new[]
        {
            new object[] { "'Science & Astronomy'::ltxtquery", "Science & Astronomy" }
        };
    }
}
