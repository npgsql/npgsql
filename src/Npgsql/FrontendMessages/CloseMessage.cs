using System.Diagnostics;
using System.Linq;

namespace Npgsql.FrontendMessages
{
    class CloseMessage : SimpleFrontendMessage
    {
        /// <summary>
        /// The name of the prepared statement or portal to close (an empty string selects the unnamed prepared statement or portal).
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Whether to close a statement or a portal
        /// </summary>
        internal StatementOrPortal StatementOrPortal { get; private set; }

        const byte Code = (byte)'C';

        internal CloseMessage Populate(StatementOrPortal type, string name="")
        {
            StatementOrPortal = type;
            Name = name;
            return this;
        }

        internal override int Length => 1 + 4 + 1 + (Name.Length + 1);

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            Debug.Assert(Name != null && Name.All(c => c < 128));

            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);
            buf.WriteByte((byte)StatementOrPortal);
            buf.WriteNullTerminatedString(Name);
        }

        public override string ToString() => $"[Close {StatementOrPortal}={Name}]";
    }
}
