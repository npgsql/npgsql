using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.BackendMessages
{
    internal class ReadyForQueryMessage : BackendMessage
    {
        internal override BackendMessageCode Code { get { return BackendMessageCode.ReadyForQuery; } }

        internal TransactionStatus TransactionStatusIndicator { get; private set; }

        internal ReadyForQueryMessage Load(NpgsqlBuffer buf) {
            TransactionStatusIndicator = (TransactionStatus)buf.ReadByte();
            return this;
        }
    }
}
