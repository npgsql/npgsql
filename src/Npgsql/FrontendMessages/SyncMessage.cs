namespace Npgsql.FrontendMessages
{
    class SyncMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'S';

        internal static readonly SyncMessage Instance = new SyncMessage();

        SyncMessage() {}

        internal override int Length => 1 + 4;

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(4);
        }

        public override string ToString() => "[Sync]";
    }
}
