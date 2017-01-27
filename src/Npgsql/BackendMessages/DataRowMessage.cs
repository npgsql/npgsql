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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.BackendMessages
{
    abstract class DataRowMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.DataRow;

        protected internal ReadBuffer Buffer { get; protected set; }

        /// <summary>
        /// The number of columns in the current row
        /// </summary>
        internal int NumColumns;

        /// <summary>
        /// The index of the column that we're on, i.e. that has already been parsed, is
        /// is memory and can be retrieved. Initialized to -1
        /// </summary>
        internal int Column;

        /// <summary>
        /// For streaming types (e.g. bytea, text), holds the current byte position within the column.
        /// Does not include the length prefix.
        /// </summary>
        internal int PosInColumn;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the byte length of the column.
        /// Does not include the length prefix.
        /// </summary>
        internal int ColumnLen;

        internal bool IsColumnNull => ColumnLen == -1;

        internal abstract DataRowMessage Load(ReadBuffer buf);

        /// <summary>
        /// Places our position at the beginning of the given column, after the 4-byte length.
        /// The length is available in ColumnLen.
        /// </summary>
        internal abstract Task SeekToColumn(int column, bool async);
        internal abstract Task SeekInColumn(int posInColumn, bool async);

        /// <summary>
        /// Returns a stream for the current column.
        /// </summary>
        internal abstract Stream GetStream();

        /// <summary>
        /// Consumes the current row, allowing the reader to read in the next one.
        /// </summary>
        internal abstract Task Consume(bool async);

        // TODO: Possibly make this non-async for NonSequential
        internal async Task SeekToColumnStart(int column, bool async)
        {
            await SeekToColumn(column, async);
            if (PosInColumn != 0)
                await SeekInColumn(0, async);
        }

        #region Checks

        // ReSharper disable once UnusedParameter.Global
        protected void CheckColumnIndex(int column)
        {
            if (column < 0 || column >= NumColumns)
            {
                throw new IndexOutOfRangeException("Column index out of range");
            }
        }

        internal void CheckNotNull()
        {
            if (IsColumnNull)
            {
                throw new InvalidCastException("Column is null");
            }
        }

        #endregion
    }
}
