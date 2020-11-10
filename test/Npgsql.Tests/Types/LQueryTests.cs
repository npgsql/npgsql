﻿using System.Collections;

namespace Npgsql.Tests.Types
{
    public class LQueryTests : TypeHandlerTestBase<string>
    {
        public LQueryTests(MultiplexingMode multiplexingMode)
            : base(multiplexingMode, "lquery", "13.0") { }

        public static IEnumerable TestCases() => new[]
        {
            new object[] { "'Top.Science.*'::lquery", "Top.Science.*" }
        };
    }
}
