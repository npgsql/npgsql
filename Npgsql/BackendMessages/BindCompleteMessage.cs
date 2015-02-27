using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.BackendMessages
{
    internal class BindCompleteMessage : IBackendMessage
    {
        public BackendMessageCode Code { get { return BackendMessageCode.BindComplete; } }
        internal static readonly BindCompleteMessage Instance = new BindCompleteMessage();
        BindCompleteMessage() { }
    }
}
