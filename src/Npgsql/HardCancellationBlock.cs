using System;
using System.Threading;

namespace Npgsql
{
    readonly struct HardCancellationBlock : IDisposable
    {
        static readonly AsyncLocal<bool> _entered = new AsyncLocal<bool>();

        public static bool Entered => _entered.Value;

        readonly bool _outerBlock;

        public HardCancellationBlock(bool outerBlock) =>_outerBlock = outerBlock;

        public static IDisposable Enter()
        {
            var context = new HardCancellationBlock(!Entered);
            if (!Entered)
                _entered.Value = true;
            return context;
        }

        public void Dispose()
        {
            if (_outerBlock)
                _entered.Value = false;
        }
    }
}
