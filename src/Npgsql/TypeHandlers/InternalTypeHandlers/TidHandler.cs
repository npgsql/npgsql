using System.Diagnostics;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.InternalTypeHandlers
{
    [TypeMapping("tid", NpgsqlDbType.Tid, typeof(NpgsqlTid))]
    class TidHandler : NpgsqlSimpleTypeHandler<NpgsqlTid>
    {
        #region Read

        public override NpgsqlTid Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(len == 6);

            var blockNumber = buf.ReadUInt32();
            var offsetNumber = buf.ReadUInt16();

            return new NpgsqlTid(blockNumber, offsetNumber);
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(NpgsqlTid value, NpgsqlParameter parameter)
            => 6;

        public override void Write(NpgsqlTid value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteUInt32(value.BlockNumber);
            buf.WriteUInt16(value.OffsetNumber);
        }

        #endregion Write
    }
}
