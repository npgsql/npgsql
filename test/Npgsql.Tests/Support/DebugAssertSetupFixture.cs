using System.Diagnostics;
using System.Linq;
using Npgsql.Tests;
using NUnit.Framework;

#if NET451 || NET45
// ReSharper disable once CheckNamespace
[SetUpFixture]
public class DebugAssertSetupFixture
{
    [OneTimeSetUp]
    public void Setup()
    {
        if (!TestUtil.IsOnBuildServer)
            return;

        var listener = Debug.Listeners.OfType<DefaultTraceListener>().FirstOrDefault();
        if (listener != null)
            listener.AssertUiEnabled = false;

        Debug.Listeners.Add(new ExceptionTraceListener());
    }

    public class ExceptionTraceListener : TraceListener
    {
        public override void Fail(string message) => Assert.Fail(message);
        public override void Write(string message) {}
        public override void WriteLine(string message) {}
    }
}
#endif
