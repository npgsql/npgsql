namespace Npgsql.FrontendMessages
{
    class FlushMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'H';

        internal static readonly FlushMessage Instance = new FlushMessage();

        FlushMessage() {}

        internal override int Length => 1 + 4;

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(4);
        }

        public override string ToString() { return "[Flush]"; }
    }
}
