using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    internal class EmptyQueryMessage : BackendMessage
    {
        internal override BackendMessageCode Code { get { return BackendMessageCode.EmptyQueryResponse; } }
        internal static readonly EmptyQueryMessage Instance = new EmptyQueryMessage();
        EmptyQueryMessage() { }
    }
}
