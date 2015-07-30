using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// The way how to order bytes.
    /// </summary>
    enum ByteOrder
    {
        /// <summary>
        /// Most significant byte first (XDR)
        /// </summary>
        MSB = 0,
        /// <summary>
        /// Less significant byte first (NDR)
        /// </summary>
        LSB = 1
    }
}
