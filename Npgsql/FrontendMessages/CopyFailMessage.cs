using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Npgsql.FrontendMessages
{
    class CopyFailMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'f';

        readonly string _errorMessage;
        readonly int _errorMessageLen;

        internal CopyFailMessage() {}

        internal CopyFailMessage(string errorMessage="")
        {
            if (errorMessage.Length > 1024) {
                throw new ArgumentException("CopyFail message must be 1024 characters or less");
            }
            _errorMessage = errorMessage;
            _errorMessageLen = PGUtil.UTF8Encoding.GetByteCount(_errorMessage);
        }

        internal override int Length
        {
            get { return 1 + 4 + _errorMessageLen + 1; }
        }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(4 + _errorMessageLen + 1);
            if (_errorMessageLen == 0)
            {
                buf.WriteByte(0);
            }
            else
            {
                buf.WriteBytesNullTerminated(PGUtil.UTF8Encoding.GetBytes(_errorMessage));
            }
        }

        public override string ToString() { return "[CopyFail]"; }
    }
}
