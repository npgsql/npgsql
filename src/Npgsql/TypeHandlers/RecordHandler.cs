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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers
{
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
    [TypeMapping("record")]
    class RecordHandler : ChunkingTypeHandler<object[]>
    {
        readonly TypeHandlerRegistry _registry;

        public RecordHandler(PostgresType postgresType, TypeHandlerRegistry registry)
            : base(postgresType)
        {
            _registry = registry;
        }

        #region Read

        public override async ValueTask<object[]> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
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
                result[i] = await _registry[typeOID].ReadAsObject(buf, fieldLen, async);
            }

            return result;
        }

        #endregion

        #region Write (unsupported)

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter)
            => throw new NotSupportedException("Can't write record types");

        protected override Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter, bool async, CancellationToken cancellationToken)
            => throw new NotSupportedException("Can't write record types");

        #endregion
    }
}
