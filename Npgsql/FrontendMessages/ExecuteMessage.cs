using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Npgsql.FrontendMessages
{
    class ExecuteMessage : SimpleFrontendMessage
    {
        internal string Portal { get; private set; }
        internal int MaxRows { get; private set; }

        const byte Code = (byte)'E';

        internal ExecuteMessage() {}

        internal ExecuteMessage(string portal="", int maxRows=0)
        {
            Populate(portal, maxRows);
        }

        internal ExecuteMessage Populate(string portal = "", int maxRows = 0)
        {
            Portal = portal;
            //MaxRows = maxRows;
            return this;
        }

        internal override int Length
        {
            get { return 1 + 4 + Portal.Length + 4; }
        }

        internal override void Write(NpgsqlBuffer buf)
        {
            Contract.Requires(Portal != null && Portal.All(c => c < 128));

            // TODO: Recycle?
            var portalNameBytes = Encoding.ASCII.GetBytes(Portal);
            buf
                .WriteByte(Code)
                .WriteInt32(Length)
                .WriteBytesNullTerminated(portalNameBytes)
                .WriteInt32(MaxRows);
        }

        public override string ToString()
        {
            return String.Format("[Execute(Portal={0},MaxRows={1}]", Portal, MaxRows);
        }
    }
}
