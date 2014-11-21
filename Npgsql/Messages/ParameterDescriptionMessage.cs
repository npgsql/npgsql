using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Npgsql.Messages
{
    internal class ParameterDescriptionMessage : IServerMessage
    {
        internal List<int> TypeOIDs { get; private set; }

        internal ParameterDescriptionMessage()
        {
            TypeOIDs = new List<int>();
        }

        internal ParameterDescriptionMessage Read(NpgsqlBufferedStream buf)
        {
            var numParams = buf.ReadInt16();
            TypeOIDs.Clear();
            for (var i = 0; i < numParams; i++) {
                TypeOIDs.Add(buf.ReadInt32());
            }
            return this;
        }

        public BackEndMessageCode Code { get { return BackEndMessageCode.ParameterDescription; } }
    }
}
