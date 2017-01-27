using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Npgsql.Tests.Util.Logging
{
    public class TestLoggerProvider : ILoggerProvider
    {
        readonly TestLoggerSink _sink;

        public TestLoggerProvider(TestLoggerSink sink)
        {
            _sink = sink;
        }

        public ILogger CreateLogger(string name) => new TestLogger(name, _sink);
        public void Dispose() {}
    }
}