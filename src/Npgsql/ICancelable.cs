using System;

namespace Npgsql
{
    interface ICancelable : IDisposable
    {
        void Cancel();
    }
}
