using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    internal class ReadyForQueryMessage : IServerMessage
    {
        public BackEndMessageCode Code { get { return BackEndMessageCode.ReadyForQuery; } }

        internal TransactionStatusIndicator TransactionStatusIndicator { get; private set; }

        internal ReadyForQueryMessage Read(NpgsqlBufferedStream buf) {
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
