using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-numeric.html
    /// </remarks>
    internal class DoubleHandler : TypeHandler<double>
    {
        static readonly string[] _pgNames = { "float8" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        public override double Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return Double.Parse(buf.ReadString(len), CultureInfo.InvariantCulture);
                case FormatCode.Binary:
                    return buf.ReadDouble();
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }
    }
}
