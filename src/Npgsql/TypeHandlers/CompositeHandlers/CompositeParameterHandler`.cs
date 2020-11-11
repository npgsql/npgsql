using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    sealed class CompositeParameterHandler<T> : CompositeParameterHandler
    {
        public CompositeParameterHandler(NpgsqlTypeHandler handler, ParameterInfo parameterInfo)
            : base(handler, parameterInfo) { }

        public override ValueTask<object?> Read(NpgsqlReadBuffer buffer, bool async)
        {
            var task = Read<T>(buffer, async);
            return task.IsCompleted
                ? new ValueTask<object?>(task.Result)
                : AwaitTask();

            async ValueTask<object?> AwaitTask() => await task;
        }
    }
}
