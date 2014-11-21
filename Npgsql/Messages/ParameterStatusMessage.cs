using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Npgsql.Messages
{
    internal class ParameterStatusMessage : IServerMessage
    {
        internal string ParameterName  { get; private set; }
        internal string ParameterValue { get; private set; }

        internal ParameterStatusMessage Read(NpgsqlBufferedStream buf)
        {
            ParameterName = buf.ReadNullTerminatedString();
            ParameterValue = buf.ReadNullTerminatedString();
            return this;
        }

        public BackEndMessageCode Code { get { return BackEndMessageCode.ParameterStatus; } }
    }
}
