namespace Npgsql.FrontendMessages
{
    class TerminateMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'X';

        internal static readonly TerminateMessage Instance = new TerminateMessage();

        TerminateMessage() { }

        internal override int Length => 1 + 4;

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(4);
        }

        public override string ToString() => "[Terminate]";
    }
}
