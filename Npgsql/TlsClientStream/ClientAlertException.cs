using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TlsClientStream
{
    internal class ClientAlertException : Exception
    {
        public AlertDescription Description { get; set; }

        public ClientAlertException(AlertDescription description, string message = null)
            : base(description.ToString() + (message != null ? ": " + message : ""))
        {
            Description = description;
        }
    }
}
