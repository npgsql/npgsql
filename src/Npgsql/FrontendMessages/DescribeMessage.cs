using System.Diagnostics;
using System.Linq;

namespace Npgsql.FrontendMessages
{
    class DescribeMessage : SimpleFrontendMessage
    {
        /// <summary>
        /// The name of the prepared statement or portal to describe (an empty string selects the unnamed prepared statement or portal).
        /// </summary>
        internal string Name { get; set; }
        /// <summary>
        /// Whether to describe a statement or a portal
        /// </summary>
        internal StatementOrPortal StatementOrPortal { get; set; }

        const byte Code = (byte)'D';

        internal DescribeMessage Populate(StatementOrPortal type, string name = null)
        {
            StatementOrPortal = type;
            Name = name ?? "";
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

        public override string ToString() => $"[Describe({StatementOrPortal}={Name})]";
    }
}
