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
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers
{
    [TypeMapping("record")]
    class RecordHandlerFactory : NpgsqlTypeHandlerFactory<object[]>
    {
        protected override NpgsqlTypeHandler<object[]> Create(NpgsqlConnection conn)
            => new RecordHandler(conn.Connector.TypeMapper);
    }

    /// <summary>
    /// Type handler for PostgreSQL record types.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-pseudo.html
    ///
    /// Encoding (identical to composite):
    /// A 32-bit integer with the number of columns, then for each column:
    /// * An OID indicating the type of the column
    /// * The length of the column(32-bit integer), or -1 if null
    /// * The column data encoded as binary
    /// </remarks>
    class RecordHandler : NpgsqlTypeHandler<object[]>
    {
        readonly ConnectorTypeMapper _typeMapper;

        public RecordHandler(ConnectorTypeMapper typeMapper)
        {
            _typeMapper = typeMapper;
        }

        #region Read

        public override async ValueTask<object[]> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var fieldCount = buf.ReadInt32();
            var result = new object[fieldCount];

            for (var i = 0; i < fieldCount; i++)
            {
                await buf.Ensure(8, async);
                var typeOID = buf.ReadUInt32();
                var fieldLen = buf.ReadInt32();
                if (fieldLen == -1)  // Null field, simply skip it and leave at default
                    continue;
                result[i] = await _typeMapper.GetByOID(typeOID).ReadAsObject(buf, fieldLen, async);
            }

            return result;
        }

        #endregion

        #region Write (unsupported)

        public override int ValidateAndGetLength(object[] value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => throw new NotSupportedException("Can't write record types");

        public override Task Write(object[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => throw new NotSupportedException("Can't write record types");

        #endregion
    }
}
