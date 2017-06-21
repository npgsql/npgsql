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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.TypeHandlers;

namespace Npgsql.BackendMessages
{
    class DataRowSequentialMessage : DataRowMessage
    {
        /// <summary>
        /// A stream that has been opened on this colun, and needs to be disposed of when the column is consumed.
        /// </summary>
        [CanBeNull]
        IDisposable _stream;

        internal override DataRowMessage Load(ReadBuffer buf)
        {
            buf.Ensure(sizeof(short));
            NumColumns = buf.ReadInt16();
            Buffer = buf;
            Column = -1;
            ColumnLen = -1;
            PosInColumn = 0;
            return this;
        }

        /// <summary>
        /// Places our position at the beginning of the given column, after the 4-byte length.
        /// The length is available in ColumnLen.
        /// </summary>
        internal override async Task SeekToColumn(int column, bool async)
        {
            CheckColumnIndex(column);

            if (column < Column)
                throw new InvalidOperationException($"Invalid attempt to read from column ordinal '{column}'. With CommandBehavior.SequentialAccess, you may only read from column ordinal '{Column}' or greater.");

            if (column == Column)
                return;

            // Skip to end of column if needed
            var remainingInColumn = (ColumnLen == -1 ? 0 : ColumnLen - PosInColumn);
            if (remainingInColumn > 0)
                await Buffer.Skip(remainingInColumn, async);

            // Shut down any streaming going on on the colun
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            // Skip over unwanted fields
            for (; Column < column - 1; Column++)
            {
                await Buffer.Ensure(4, async);
                var len = Buffer.ReadInt32();
                if (len != -1)
                    await Buffer.Skip(len, async);
            }

            await Buffer.Ensure(4, async);
            ColumnLen = Buffer.ReadInt32();
            PosInColumn = 0;
            Column = column;
        }

        internal override async Task SeekInColumn(int posInColumn, bool async)
        {
            if (posInColumn < PosInColumn)
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");

            if (posInColumn > ColumnLen)
                posInColumn = ColumnLen;

            if (posInColumn > PosInColumn)
            {
                await Buffer.Skip(posInColumn - PosInColumn, async);
                PosInColumn = posInColumn;
            }
        }

        internal override Stream GetStream()
        {
            Debug.Assert(PosInColumn == 0);
            if (_stream != null)
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");
            var stream = new SequentialByteaStream(this);
            _stream = stream;
            return stream;
        }

        internal override async Task Consume(bool async)
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            // Skip to end of column if needed
            var remainingInColumn = ColumnLen == -1 ? 0 : ColumnLen - PosInColumn;
            if (remainingInColumn > 0)
                await Buffer.Skip(remainingInColumn, async);

            // Skip over the remaining columns in the row
            for (; Column < NumColumns - 1; Column++)
            {
                await Buffer.Ensure(4, async);
                var len = Buffer.ReadInt32();
                if (len != -1)
                    await Buffer.Skip(len, async);
            }
        }
    }
}
