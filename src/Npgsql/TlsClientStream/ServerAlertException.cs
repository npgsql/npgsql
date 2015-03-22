using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TlsClientStream
{
    internal class ServerAlertException : Exception
    {
        AlertDescription Description { get; set; }

        public ServerAlertException(AlertDescription description) : base(description.ToString())
        {
            Description = description;
        }
    }
}
