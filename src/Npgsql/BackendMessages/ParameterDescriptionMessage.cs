using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.BackendMessages
{
    internal class ParameterDescriptionMessage : IBackendMessage
    {
        internal List<uint> TypeOIDs { get; private set; }

        internal ParameterDescriptionMessage()
        {
            TypeOIDs = new List<uint>();
        }

        internal ParameterDescriptionMessage Load(NpgsqlBuffer buf)
        {
            var numParams = buf.ReadInt16();
            TypeOIDs.Clear();
            for (var i = 0; i < numParams; i++) {
                TypeOIDs.Add(buf.ReadUInt32());
            }
            return this;
        }

        public BackendMessageCode Code { get { return BackendMessageCode.ParameterDescription; } }
    }
}
