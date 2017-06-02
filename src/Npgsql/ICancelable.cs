using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql
{
    interface ICancelable : IDisposable
    {
        /// <summary>
        /// Indicates that the operation must be cancelled when the connection is closing.
        /// </summary>
        bool CancellationRequired { get; }

        /// <summary>
        /// Terminates the ongoing operation.
        /// </summary>
        void Cancel();
    }
}
