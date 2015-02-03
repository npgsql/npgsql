using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.BackendMessages
{
    internal class NoDataMessage : BackendMessage
    {
        internal override BackendMessageCode Code { get { return BackendMessageCode.NoData; } }
        internal static readonly NoDataMessage Instance = new NoDataMessage();
        NoDataMessage() { }
    }
}
