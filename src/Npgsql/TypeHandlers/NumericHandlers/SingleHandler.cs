using System.Data;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("real", NpgsqlDbType.Real, DbType.Single, typeof(float))]
    class SingleHandler : NpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>
    {
        #region Read

        public override float Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => buf.ReadSingle();

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        public int ValidateAndGetLength(double value, NpgsqlParameter parameter)
            => 4;

        public override int ValidateAndGetLength(float value, NpgsqlParameter parameter)
            => 4;

        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteSingle((float)value);

        public override void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteSingle(value);

        #endregion Write
    }
}
