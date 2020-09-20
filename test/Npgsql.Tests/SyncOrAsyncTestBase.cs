using NUnit.Framework;

namespace Npgsql.Tests
{
    [TestFixture(ExecutionMode.Synchronous)]
    [TestFixture(ExecutionMode.Asynchronous)]
    public abstract class SyncOrAsyncTestBase : TestBase
    {
        protected bool IsAsync => ExecutionMode == ExecutionMode.Asynchronous;

        protected ExecutionMode ExecutionMode { get; }

        protected SyncOrAsyncTestBase(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }
    }

    public enum ExecutionMode
    {
        Synchronous,
        Asynchronous
    }
}
