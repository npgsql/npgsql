// Non-generic TaskCompletionSource available
#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP3_1
using System.Threading.Tasks;

namespace Npgsql.Util
{
    class TaskCompletionSource
    {
        readonly TaskCompletionSource<int> _underlying = new TaskCompletionSource<int>();
        internal void SetResult() => _underlying.SetResult(0);
        internal Task Task => _underlying.Task;
    }
}
#endif
