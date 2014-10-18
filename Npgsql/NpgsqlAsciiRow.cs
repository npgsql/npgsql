// created on 13/6/2002 at 21:06

// Npgsql.NpgsqlAsciiRow.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Data;
using System.IO;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Implements <see cref="RowReader"/> for version 3 of the protocol.
    /// </summary>
    internal sealed partial class StringRowReader : RowReader
    {
        private readonly int _messageSize;
        private int? _nextFieldSize;

        public StringRowReader(Stream inputStream)
            : base(inputStream)
        {
            _messageSize = inputStream.ReadInt32();
        }

        public override void SetRowDescription(NpgsqlRowDescription rowDesc)
        {
            if (Stream.ReadInt16() != rowDesc.NumFields)
            {
                throw new DataException();
            }

            _rowDesc = rowDesc;
        }

        [GenerateAsync(withOverride: true)]
        protected override object ReadNext()
        {
            int fieldSize = GetThisFieldCount();
            if (fieldSize >= _messageSize)
            {
                AbandonShip();
            }
            _nextFieldSize = null;

            // Check if this field is null
            if (fieldSize == -1) // Null value
            {
                return DBNull.Value;
            }

            NpgsqlRowDescription.FieldData field_descr = FieldData;

            byte[] buffer = new byte[fieldSize];
            Stream.CheckedStreamRead(buffer, 0, fieldSize);

            try
            {
                if (field_descr.FormatCode == FormatCode.Text)
                {
                    return
                        NpgsqlTypesHelper.ConvertBackendStringToSystemType(field_descr.TypeInfo, buffer,
                                                                           field_descr.TypeSize, field_descr.TypeModifier);
                }
                else
                {
                    return
                        NpgsqlTypesHelper.ConvertBackendBytesToSystemType(field_descr.TypeInfo, buffer, fieldSize,
                                                                          field_descr.TypeModifier);
                }
            }
            catch (InvalidCastException ice)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(ex.Message, ex);
            }
        }

        [GenerateAsync]
        private void AbandonShip()
        {
            //field size will always be smaller than message size
            //but if we fall out of sync with the stream due to an error then we will probably hit
            //such a situation soon as bytes from elsewhere in the stream get interpreted as a size.
            //so if we see this happens, we know we've lost the stream - our best option is to just give up on it,
            //and have the connector recovered later.
            try
            {
                Stream
                    .WriteByte(ASCIIByteArrays.TerminationMessageCode)
                    .WriteInt32(4)
                    .Flush();
            }
            catch
            {
            }
            try
            {
                Stream.Close();
            }
            catch
            {
            }
            throw new DataException();
        }

        [GenerateAsync(withOverride: true)]
        protected override void SkipOne()
        {
            int fieldSize = GetThisFieldCount();
            if (fieldSize >= _messageSize)
            {
                AbandonShip();
            }
            _nextFieldSize = null;
            Stream.EatStreamBytes(fieldSize);
        }

        public override bool IsNull
        {
            get { return GetThisFieldCount() == -1; }
        }

        [GenerateAsync]
        private int GetThisFieldCount()
        {
            if (_nextFieldSize.HasValue)
                return _nextFieldSize.Value;
            int s;
            _nextFieldSize = s = Stream.ReadInt32();
            return s;
        }

        protected override int GetNextFieldCount()
        {
            int ret = GetThisFieldCount();
            _nextFieldSize = null;
            return ret;
        }

        public override void Dispose()
        {
            if (_rowDesc == null)
            {
                // If _rowdesc is null, then only the message length integer has been read;
                // read the entire message and disgard it.
                Stream.EatStreamBytes(this._messageSize - 4);
            }
            else
            {
                CurrentStreamer = null;
                Skip(_rowDesc.NumFields - _currentField - 1);
            }
        }
    }
}
