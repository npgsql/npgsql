using System.Data;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("double precision", NpgsqlDbType.Double, DbType.Double, typeof(double))]
    class DoubleHandler : NpgsqlSimpleTypeHandler<double>
    {
        public override double Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => buf.ReadDouble();

        public override int ValidateAndGetLength(double value, NpgsqlParameter parameter)
            => 8;

        public override void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteDouble(value);
    }
}
