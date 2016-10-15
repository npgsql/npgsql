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

using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-boolean.html
    /// </remarks>
    [TypeMapping("bool", NpgsqlDbType.Boolean, DbType.Boolean, typeof(bool))]
    class BoolHandler : SimpleTypeHandler<bool>
    {
        internal BoolHandler(PostgresType postgresType) : base(postgresType) {}

        public override bool Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => buf.ReadByte() != 0;

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            if (!(value is bool))
            {
                var converted = Convert.ToBoolean(value);
                if (parameter == null)
                    throw CreateConversionButNoParamException(value.GetType());
                parameter.ConvertedValue = converted;
            }
            return 1;
        }

        protected override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null)
        {
            if (parameter?.ConvertedValue != null)
                value = parameter.ConvertedValue;
            buf.WriteByte((bool)value ? (byte)1 : (byte)0);
        }
    }
}
