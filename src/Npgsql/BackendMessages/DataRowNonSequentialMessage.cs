#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Npgsql.BackendMessages
{
    class DataRowNonSequentialMessage : DataRowMessage
    {
        readonly List<int> _columnOffsets = new List<int>();
        int _endOffset;
        /// <summary>
        /// List of all streams that have been opened on this row, and need to be disposed of when the row
        /// is consumed.
        /// </summary>
        [CanBeNull]
        List<IDisposable> _streams;

        internal override DataRowMessage Load(ReadBuffer buf)
        {
            NumColumns = buf.ReadInt16();
            Buffer = buf;
            Column = -1;
            ColumnLen = -1;
            PosInColumn = 0;
            // TODO: One big row with many columns will make the DataRow's _columnOffsets big forever...
            _columnOffsets.Clear();
            for (var i = 0; i < NumColumns; i++)
            {
                _columnOffsets.Add(buf.ReadPosition);
                var len = buf.ReadInt32();
                if (len != -1)
                {
                    buf.Seek(len, SeekOrigin.Current);
                }
            }
            _endOffset = buf.ReadPosition;
            return this;
        }

        internal override Task SeekToColumn(int column, bool async)
        {
            CheckColumnIndex(column);

            if (Column != column)
            {
                Buffer.Seek(_columnOffsets[column], SeekOrigin.Begin);
                Column = column;
                ColumnLen = Buffer.ReadInt32();
                PosInColumn = 0;
            }

            return PGUtil.CompletedTask;
        }

        internal override Task SeekInColumn(int posInColumn, bool async)
        {
            if (posInColumn > ColumnLen) {
                posInColumn = ColumnLen;
            }

            Buffer.Seek(_columnOffsets[Column] + 4 + posInColumn, SeekOrigin.Begin);
            PosInColumn = posInColumn;
            return PGUtil.CompletedTask;
        }

        internal override Stream GetStream()
        {
            Debug.Assert(PosInColumn == 0);
            var s = Buffer.GetMemoryStream(ColumnLen);
            if (_streams == null) {
                _streams = new List<IDisposable>();
            }
            _streams.Add(s);
            return s;
        }

        internal override Task Consume(bool async)
        {
            Buffer.Seek(_endOffset, SeekOrigin.Begin);
            if (_streams != null)
            {
                foreach (var stream in _streams) {
                    stream.Dispose();
                }
                _streams.Clear();
            }
            return PGUtil.CompletedTask;
        }
    }
}
