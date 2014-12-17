using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    internal class EmptyQueryMessage : ServerMessage
    {
        internal override BackEndMessageCode Code { get { return BackEndMessageCode.EmptyQueryResponse; } }
        internal static readonly EmptyQueryMessage Instance = new EmptyQueryMessage();
        EmptyQueryMessage() { }
    }
}
