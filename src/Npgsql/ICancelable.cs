using System;
using System.Threading.Tasks;

namespace Npgsql
{
    interface ICancelable : IDisposable, IAsyncDisposable
    {
        void Cancel();

        Task CancelAsync();
    }
}
