using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Npgsql.FrontendMessages
{
    class CopyFailMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'f';

        [CanBeNull]
        readonly string _errorMessage;
        readonly int _errorMessageLen;

        internal CopyFailMessage() {}

        internal CopyFailMessage(string errorMessage="")
        {
            if (errorMessage.Length > 1024)
                throw new ArgumentException("CopyFail message must be 1024 characters or less");
            _errorMessage = errorMessage;
            _errorMessageLen = PGUtil.UTF8Encoding.GetByteCount(_errorMessage);
        }

        internal override int Length => 1 + 4 + (_errorMessageLen + 1);

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);
            // Error message not supported for now
            Debug.Assert(_errorMessage == null);
            buf.WriteByte(0);
        }

        public override string ToString() => "[CopyFail]";
    }
}
