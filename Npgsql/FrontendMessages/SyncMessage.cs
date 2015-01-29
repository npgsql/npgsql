using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class SyncMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'S';

        internal static readonly SyncMessage Instance = new SyncMessage();

        SyncMessage() {}

        internal override int Length { get { return 5; } }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(4);
        }

        public override string ToString() { return "[Sync]"; }
    }
}
