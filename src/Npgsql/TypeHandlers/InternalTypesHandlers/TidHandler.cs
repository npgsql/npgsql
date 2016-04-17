using System;
using System.Diagnostics.Contracts;
using System.Net.NetworkInformation;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.InternalTypesHandlers
{
    [TypeMapping("tid", NpgsqlDbType.Tid, typeof(NpgsqlTid))]
    internal class TidHandler : SimpleTypeHandler<NpgsqlTid>, ISimpleTypeHandler<string>
    {
        public override NpgsqlTid Read(ReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            Contract.Assume(len == 6);

            uint blockNumber = buf.ReadUInt32();
            ushort offsetNumber = buf.ReadUInt16();

            return new NpgsqlTid(blockNumber, offsetNumber);
        }

        string ISimpleTypeHandler<string>.Read(ReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is NpgsqlTid))
                throw CreateConversionException(value.GetType());
            return 6;
        }

        public override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter)
        {
            var tid = (NpgsqlTid)value;
            buf.WriteUInt32(tid.BlockNumber);
            buf.WriteUInt16(tid.OffsetNumber);
        }
    }
}
