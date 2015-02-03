using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;

namespace Npgsql.TypeHandlers.InternalTypesHandlers
{
    [TypeMapping("oidvector", NpgsqlDbType.Oidvector)]
    internal class OIDVectorHandler : TypeHandler<uint[]>, IChunkingTypeReader<uint[]>, IChunkingTypeWriter
    {
        public uint[] Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            throw new NotImplementedException();
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
        }

        internal int ValidateAndGetLength(object value)
        {
            return
                12 + // dims + nulls + element oid
                4 + 4 + // length + lower bound
                8 * ((uint[])value).Length;
        }

        public void PrepareWrite(NpgsqlBuffer buf, object value)
        {
            throw new NotImplementedException();
        }

        public bool Write(ref byte[] directBuf)
        {
            throw new NotImplementedException();
        }

        internal void WriteBinary(object value, NpgsqlBuffer buf)
        {
            throw new NotImplementedException();
            /*
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
             */
        }

        public void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            throw new NotImplementedException();
        }

        public bool Read(out uint[] result)
        {
            throw new NotImplementedException();
        }

        int IChunkingTypeWriter.ValidateAndGetLength(object value)
        {
            return ValidateAndGetLength(value);
        }
    }
}
