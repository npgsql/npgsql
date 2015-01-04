using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    internal class ReadyForQueryMessage : BackendMessage
    {
        internal override BackendMessageCode Code { get { return BackendMessageCode.ReadyForQuery; } }

        internal TransactionStatusIndicator TransactionStatusIndicator { get; private set; }

        internal ReadyForQueryMessage Load(NpgsqlBuffer buf) {
            TransactionStatusIndicator = (TransactionStatusIndicator)buf.ReadByte();
            return this;
        }
    }

    internal enum TransactionStatusIndicator
    {
        Idle                     = 'I',
        InTransactionBlock       = 'B',
        InFailedTransactionBlock = 'E'
    }
}
