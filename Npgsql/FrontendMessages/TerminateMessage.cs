using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class TerminateMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'X';

        internal static readonly TerminateMessage Instance = new TerminateMessage();

        TerminateMessage() { }

        internal override int Length { get { return 5; } }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(4);
        }

        public override string ToString() { return "[Terminate]"; }
    }
}
