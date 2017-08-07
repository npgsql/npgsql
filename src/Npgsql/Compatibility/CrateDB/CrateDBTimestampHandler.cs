using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.Compatibility.CrateDB
{
    class CrateDBTimestampHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        protected override NpgsqlTypeHandler<DateTime> Create(NpgsqlConnection conn) => new CrateDBTimestampHandler();
    }

    class CrateDBTimestampHandler : NpgsqlSimpleTypeHandler<DateTime>
    {
        internal static readonly DateTime PostgresEpoch = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        
        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var value = buf.ReadDouble();
            try
            {
                return PostgresEpoch.AddSeconds(value);
            }
            catch (Exception e)
            {
                throw new NpgsqlSafeReadException(e);
            }
        }

        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter parameter) => 8;

        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            var diff = ((DateTime)value).ToUniversalTime() - PostgresEpoch;
            buf.WriteDouble(diff.TotalSeconds);
        }
    }
}
