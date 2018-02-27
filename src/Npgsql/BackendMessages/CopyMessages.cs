#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;

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

    /// <remarks>
    /// Note: This message is both a frontend and a backend message
    /// </remarks>
    class CopyDoneMessage : SimpleFrontendMessage, IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.CopyDone;
        internal static readonly CopyDoneMessage Instance = new CopyDoneMessage();
        CopyDoneMessage() { }

        internal override int Length => 5;

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte((byte)BackendMessageCode.CopyDone);
            buf.WriteInt32(4);
        }
    }
}
