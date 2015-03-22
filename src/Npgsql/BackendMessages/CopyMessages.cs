using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.BackendMessages
{
    abstract class CopyResponseMessageBase : IBackendMessage
    {
        public abstract BackendMessageCode Code { get; }

        internal bool IsBinary { get; private set; }
        internal short NumColumns { get; private set; }
        internal List<FormatCode> ColumnFormatCodes { get; private set; }

        internal CopyResponseMessageBase()
        {
            ColumnFormatCodes = new List<FormatCode>();
        }

        internal void Load(NpgsqlBuffer buf)
        {
            ColumnFormatCodes.Clear();

            var binaryIndicator = buf.ReadByte();
            switch (binaryIndicator) {
            case 0:
                IsBinary = false;
                break;
            case 1:
                IsBinary = true;
                break;
            default:
                throw new Exception("Invalid binary indicator in CopyInResponse message: " + binaryIndicator);
            }

            NumColumns = buf.ReadInt16();
            for (var i = 0; i < NumColumns; i++)
                ColumnFormatCodes.Add((FormatCode)buf.ReadInt16());
        }
    }

    class CopyInResponseMessage : CopyResponseMessageBase
    {
        public override BackendMessageCode Code { get { return BackendMessageCode.CopyInResponse; } }

        internal new CopyInResponseMessage Load(NpgsqlBuffer buf)
        {
            base.Load(buf);
            return this;
        }
    }

    class CopyOutResponseMessage : CopyResponseMessageBase
    {
        public override BackendMessageCode Code { get { return BackendMessageCode.CopyOutResponse; } }

        internal new CopyOutResponseMessage Load(NpgsqlBuffer buf)
        {
            base.Load(buf);
            return this;
        }
    }

    class CopyBothResponseMessage : CopyResponseMessageBase
    {
        public override BackendMessageCode Code { get { return BackendMessageCode.CopyBothResponse; } }

        internal new CopyBothResponseMessage Load(NpgsqlBuffer buf)
        {
            base.Load(buf);
            return this;
        }
    }

    /// <summary>
    /// Note that this message doesn't actually contain the data, but only the length. Data is processed
    /// directly from the connector's buffer.
    /// </summary>
    class CopyDataMessage : IBackendMessage
    {
        public BackendMessageCode Code { get { return BackendMessageCode.CopyData; } }

        public int Length { get; private set; }

        internal CopyDataMessage Load(int len)
        {
            Length = len;
            return this;
        }
    }

    /// <remarks>
    /// Note: This message is both a frontend and a backend message
    /// </remarks>
    class CopyDoneMessage : SimpleFrontendMessage, IBackendMessage
    {
        public BackendMessageCode Code { get { return BackendMessageCode.CopyDone; } }
        internal static readonly CopyDoneMessage Instance = new CopyDoneMessage();
        CopyDoneMessage() { }

        internal override int Length { get { return 5; } }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteByte((byte)BackendMessageCode.CopyDone);
            buf.WriteInt32(4);
        }
    }
}
