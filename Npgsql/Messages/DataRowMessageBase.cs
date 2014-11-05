using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    internal abstract class DataRowMessageBase : IServerMessage
    {
        internal abstract NpgsqlValue Get(int ordinal);
        internal abstract long GetBytes(int column, long posInColumn, byte[] output, int outputOffset, int len);

        internal RowDescriptionMessage Description { get; set; }
        internal abstract void Consume();
        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }
    }
}
