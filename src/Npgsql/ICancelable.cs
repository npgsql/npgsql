using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql
{
    internal interface ICancelable : IDisposable
    {
        void Cancel();
    }
}
