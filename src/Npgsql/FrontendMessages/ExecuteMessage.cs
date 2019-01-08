using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class ExecuteMessage : SimpleFrontendMessage
    {
        internal static readonly ExecuteMessage DefaultExecute = new ExecuteMessage();

        internal string Portal { get; private set; } = "";
        internal int MaxRows { get; private set; }

        const byte Code = (byte)'E';

        internal ExecuteMessage Populate(string portal = "", int maxRows = 0)
        {
            Portal = portal;
            MaxRows = maxRows;
            return this;
        }

        internal ExecuteMessage Populate(int maxRows) => Populate("", maxRows);

        internal override int Length => 1 + 4 + 1 + 4;

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            Debug.Assert(Portal != null && Portal.All(c => c < 128));

            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);
            Debug.Assert(Portal == string.Empty);
            buf.WriteByte(0);   // Portal is always an empty string
            buf.WriteInt32(MaxRows);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[Execute");
            if (Portal != "" && MaxRows != 0)
            {
                sb.Append("Portal=").Append(Portal);
                sb.Append("MaxRows=").Append(MaxRows);
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
