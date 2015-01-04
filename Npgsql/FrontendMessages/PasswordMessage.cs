using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class PasswordMessage : SimpleFrontendMessage
    {
        internal byte[] Password { get; set; }

        const byte Code = (byte)'p';

        internal PasswordMessage(byte[] password)
        {
            Password = password;
        }

        internal override int Length { get { return 4 + Password.Length; } }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(Length);
            buf.WriteBytes(Password);
        }

        public override string ToString() { return "[Password]"; }
    }
}
