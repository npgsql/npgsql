using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Util;

readonly struct TaskSchedulerAwaitable(TaskScheduler scheduler) : ICriticalNotifyCompletion
{
    public void GetResult() {}
    public bool IsCompleted => false;

    public void OnCompleted(Action continuation)
    {
        var task = Task.Factory.StartNew(continuation, CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            scheduler: scheduler);

        // Exceptions should never happen as the continuation should be the async statemachine.
        // It normally does its own error handling through the returned task unless it's an async void returning method.
        // In which case we should absolutely let it bubble up to TaskScheduler.UnobservedTaskException.
        OnFaulted(task);

        [Conditional("DEBUG")]
        static void OnFaulted(Task task)
        {
            task.ContinueWith(t => Debug.Fail("Task scheduler task threw an unobserved exception"), TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

    public TaskSchedulerAwaitable GetAwaiter() => this;
}
