namespace Npgsql.FrontendMessages
{
    class SSLRequestMessage : SimpleFrontendMessage
    {
        internal static readonly SSLRequestMessage Instance = new SSLRequestMessage();

        SSLRequestMessage() {}

        internal override int Length => 8;

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteInt32(Length);
            buf.WriteInt32(80877103);
        }

        public override string ToString() => "[SSLRequest]";
    }
}
