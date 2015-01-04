using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.TypeHandlers.InternalTypesHandlers
{
    internal class OIDVectorHandler : TypeHandler<uint[]>
    {
        static readonly string[] _pgNames = { "oidvector" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Oidvector };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }

        public override bool AllowAutoInferring { get { return false; } }

        public override uint[] Read(NpgsqlBuffer buf, Messages.FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    string[] stringArr = buf.ReadString(len).Split(' ');
                    uint[] uintArr = new uint[stringArr.Length];
                    for (var i = 0; i < stringArr.Length; i++)
                        uintArr[i] = uint.Parse(stringArr[i]);
                    return uintArr;
                case FormatCode.Binary:
                    buf.Skip(12); // skip number of dims + has nulls flag + element oid
                    var length = buf.ReadInt32();
                    buf.Skip(4); // skip lower bound
                    var res = new uint[length];
                    for (var i = 0; i < length; i++)
                    {
                        buf.Skip(4); // skip length
                        res[i] = buf.ReadUInt32();
                    }
                    return res;
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            writer.WriteString(string.Join(" ", (uint[])value));
        }

        protected override int BinarySize(object value)
        {
            return 4 +
                12 + // dims + nulls + element oid
                4 + 4 + // length + lower bound
                8 * ((uint[])value).Length;
        }

        public override void WriteBinary(TypeHandlerRegistry registry, uint oid, object value, NpgsqlBuffer buf, List<int> sizeArr, ref int sizeArrPos)
        {
            var arr = (uint[])value;
            buf.EnsureWrite(24);
            buf.WriteInt32(20 + arr.Length * 8);
            buf.WriteInt32(1);
            buf.WriteInt32(0);
            buf.WriteInt32((int)registry.GetOidFromNpgsqlDbType(NpgsqlDbType.Oid));
            buf.WriteInt32(arr.Length);
            buf.WriteInt32(0); // oidvector has lower bound = 0

            foreach (var elem in arr)
            {
                if (buf.WriteSpaceLeft < 8)
                    buf.EnsureWrite(8);
                buf.WriteInt32(4);
                buf.WriteInt32((int)elem);
            }
        }
    }
}
