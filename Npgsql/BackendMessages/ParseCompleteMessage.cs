using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    internal class ParseCompleteMessage : BackendMessage
    {
        internal override BackendMessageCode Code { get { return BackendMessageCode.ParseComplete; } }
        internal static readonly ParseCompleteMessage Instance = new ParseCompleteMessage();
        ParseCompleteMessage() { }
    }
}
