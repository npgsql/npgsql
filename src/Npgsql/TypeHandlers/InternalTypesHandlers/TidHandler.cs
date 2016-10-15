#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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

using System.Diagnostics;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.InternalTypesHandlers
{
    [TypeMapping("tid", NpgsqlDbType.Tid, typeof(NpgsqlTid))]
    class TidHandler : SimpleTypeHandler<NpgsqlTid>, ISimpleTypeHandler<string>
    {
        internal TidHandler(PostgresType postgresType) : base(postgresType) { }

        public override NpgsqlTid Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(len == 6);

            var blockNumber = buf.ReadUInt32();
            var offsetNumber = buf.ReadUInt16();

            return new NpgsqlTid(blockNumber, offsetNumber);
        }

        string ISimpleTypeHandler<string>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).ToString();

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            if (!(value is NpgsqlTid))
                throw CreateConversionException(value.GetType());
            return 6;
        }

        protected override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null)
        {
            var tid = (NpgsqlTid)value;
            buf.WriteUInt32(tid.BlockNumber);
            buf.WriteUInt16(tid.OffsetNumber);
        }
    }
}
