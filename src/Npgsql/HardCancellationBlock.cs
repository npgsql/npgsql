using System;
using System.Threading;

namespace Npgsql
{
    internal class HardCancellationBlock : IDisposable
    {
        bool disposed;

        static readonly AsyncLocal<bool> _entered = new AsyncLocal<bool>();

        public static bool Entered => _entered.Value;

        public static IDisposable Enter()
        {
            var context = new HardCancellationBlock();
            if (Entered)
                context.disposed = true;
            else
                _entered.Value = true;
            return context;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                _entered.Value = false;
                disposed = true;
            }
        }
    }
}
