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
using JetBrains.Annotations;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-uuid.html
    /// </remarks>
    [TypeMapping("uuid", NpgsqlDbType.Uuid, DbType.Guid, typeof(Guid))]
    class UuidHandler : SimpleTypeHandler<Guid>, ISimpleTypeHandler<string>
    {
        internal UuidHandler(PostgresType postgresType) : base(postgresType) { }

        public override Guid Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            buf.Ensure(16);
            var a = buf.ReadInt32();
            var b = buf.ReadInt16();
            var c = buf.ReadInt16();
            var d = new byte[8];
            buf.ReadBytes(d, 0, 8);
            return new Guid(a, b, c, d);
        }

        string ISimpleTypeHandler<string>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).ToString();

        #region Write

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            var asString = value as string;
            if (value is string)
            {
                var converted = Guid.Parse(asString);
                if (parameter == null)
                    throw CreateConversionButNoParamException(value.GetType());
                parameter.ConvertedValue = converted;
            }
            else if (!(value is Guid))
                throw CreateConversionException(value.GetType());
            return 16;
        }

        protected override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null)
        {
            if (parameter?.ConvertedValue != null)
                value = parameter.ConvertedValue;

            var bytes = ((Guid)value).ToByteArray();

            buf.WriteInt32(BitConverter.ToInt32(bytes, 0));
            buf.WriteInt16(BitConverter.ToInt16(bytes, 4));
            buf.WriteInt16(BitConverter.ToInt16(bytes, 6));
            buf.WriteBytes(bytes, 8, 8);
        }

        #endregion
    }
}
