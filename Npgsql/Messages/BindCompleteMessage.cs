using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    internal class BindCompleteMessage : ServerMessage
    {
        internal override BackEndMessageCode Code { get { return BackEndMessageCode.BindComplete; } }
        internal static readonly BindCompleteMessage Instance = new BindCompleteMessage();
        BindCompleteMessage() { }
    }
}
