using System;
using System.Collections.Generic;
using Npgsql.Util;

namespace Npgsql.BackendMessages
{
    abstract class CopyResponseMessageBase : IBackendMessage
    {
        public abstract BackendMessageCode Code { get; }

        internal bool IsBinary { get; private set; }
        internal short NumColumns { get; private set; }
        internal List<FormatCode> ColumnFormatCodes { get; }

        internal CopyResponseMessageBase()
        {
            ColumnFormatCodes = new List<FormatCode>();
        }

        internal void Load(NpgsqlReadBuffer buf)
        {
            ColumnFormatCodes.Clear();

            var binaryIndicator = buf.ReadByte();
            IsBinary = binaryIndicator switch
            {
                0 => false,
                1 => true,
                _ => throw new Exception("Invalid binary indicator in CopyInResponse message: " + binaryIndicator)
            };

            NumColumns = buf.ReadInt16();
            for (var i = 0; i < NumColumns; i++)
                ColumnFormatCodes.Add((FormatCode)buf.ReadInt16());
        }
    }

    class CopyInResponseMessage : CopyResponseMessageBase
    {
        public override BackendMessageCode Code => BackendMessageCode.CopyInResponse;

        internal new CopyInResponseMessage Load(NpgsqlReadBuffer buf)
        {
            base.Load(buf);
            return this;
        }
    }

    class CopyOutResponseMessage : CopyResponseMessageBase
    {
        public override BackendMessageCode Code => BackendMessageCode.CopyOutResponse;

        internal new CopyOutResponseMessage Load(NpgsqlReadBuffer buf)
        {
            base.Load(buf);
            return this;
        }
    }

    class CopyBothResponseMessage : CopyResponseMessageBase
    {
        public override BackendMessageCode Code => BackendMessageCode.CopyBothResponse;

        internal new CopyBothResponseMessage Load(NpgsqlReadBuffer buf)
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
        public BackendMessageCode Code => BackendMessageCode.CopyData;

        public int Length { get; private set; }

        internal CopyDataMessage Load(int len)
        {
            Length = len;
            return this;
        }
    }

    class CopyDoneMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.CopyDone;
        internal static readonly CopyDoneMessage Instance = new CopyDoneMessage();
        CopyDoneMessage() { }
    }
}
