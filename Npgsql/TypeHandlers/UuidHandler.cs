using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-uuid.html
    /// </remarks>
    [TypeMapping("uuid", NpgsqlDbType.Uuid, DbType.Guid, typeof(Guid))]
    internal class UuidHandler : TypeHandler<Guid>,
        ISimpleTypeReader<Guid>, ISimpleTypeWriter,
        ISimpleTypeReader<string>
    {
        public Guid Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            buf.Ensure(16);
            var a = buf.ReadInt32();
            var b = buf.ReadInt16();
            var c = buf.ReadInt16();
            var d = new byte[8];
            buf.ReadBytes(d, 0, 8, true);
            return new Guid(a, b, c, d);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }

        #region Write

        public int ValidateAndGetLength(object value) { return 16; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var bytes = ((Guid)value).ToByteArray();

            buf.WriteInt32(BitConverter.ToInt32(bytes, 0));
            buf.WriteInt16(BitConverter.ToInt16(bytes, 4));
            buf.WriteInt16(BitConverter.ToInt16(bytes, 6));
            buf.WriteBytesSimple(bytes, 8, 8);
        }

        #endregion
    }
}
