using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    internal class NoDataMessage : IServerMessage
    {
        public BackEndMessageCode Code { get { return BackEndMessageCode.NoData; } }
        internal static readonly NoDataMessage Instance = new NoDataMessage();
        NoDataMessage() { }
    }
}
