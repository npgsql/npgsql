using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class FlushMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'H';

        internal static readonly FlushMessage Instance = new FlushMessage();

        FlushMessage() {}

        internal override int Length { get { return 5; } }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(4);
        }

        public override string ToString() { return "[Flush]"; }
    }
}
