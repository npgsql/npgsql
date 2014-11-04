using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Npgsql.Messages
{
    internal class ParameterDescriptionMessage : IServerMessage
    {
        public int[] TypeOIDs { get; private set; }

        public ParameterDescriptionMessage(NpgsqlBufferedStream buf)
        {
            var numParams = buf.ReadInt16();
            TypeOIDs = new int[numParams];
            for (var i = 0; i < numParams; i++) {
                TypeOIDs[i] = buf.ReadInt32();
            }
        }

        public BackEndMessageCode Code { get { return BackEndMessageCode.ParameterDescription; } }
    }
}
