using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.BackendMessages
{
    internal class BackendKeyDataMessage : IBackendMessage
    {
        public BackendMessageCode Code { get { return BackendMessageCode.BackendKeyData; } }

        internal int BackendProcessId { get; private set; }
        internal int BackendSecretKey { get; private set; }

        internal BackendKeyDataMessage(NpgsqlBuffer buf)
        {
            BackendProcessId = buf.ReadInt32();
            BackendSecretKey = buf.ReadInt32();
        }
    }
}
