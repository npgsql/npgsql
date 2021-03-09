using System.Reflection;
using System.Threading.Tasks;
using Npgsql.Internal.TypeHandling;

namespace Npgsql.Internal.TypeHandlers.CompositeHandlers
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
                : AwaitTask(task);

            static async ValueTask<object?> AwaitTask(ValueTask<T> task) => await task;
        }
    }
}
