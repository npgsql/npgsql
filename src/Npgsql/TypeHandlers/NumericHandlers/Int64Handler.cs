#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("int8", NpgsqlDbType.Bigint, DbType.Int64, typeof(long))]
    internal class Int64Handler : SimpleTypeHandler<long>,
        ISimpleTypeHandler<byte>, ISimpleTypeHandler<short>, ISimpleTypeHandler<int>,
        ISimpleTypeHandler<float>, ISimpleTypeHandler<double>, ISimpleTypeHandler<decimal>,
    ISimpleTypeHandler<string>
    {
        public override long Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadInt64();
        }

        byte ISimpleTypeHandler<byte>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (byte)Read(buf, len, fieldDescription);
        }

        short ISimpleTypeHandler<short>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (short)Read(buf, len, fieldDescription);
        }

        int ISimpleTypeHandler<int>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (int)Read(buf, len, fieldDescription);
        }

        float ISimpleTypeHandler<float>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription);
        }

        double ISimpleTypeHandler<double>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription);
        }

        decimal ISimpleTypeHandler<decimal>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription);
        }

        string ISimpleTypeHandler<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is long))
            {
                var converted = Convert.ToInt64(value);
                if (parameter == null)
                {
                    throw CreateConversionButNoParamException(value.GetType());
                }
                parameter.ConvertedValue = converted;
            }
            return 8;
        }

        public override void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            if (parameter?.ConvertedValue != null) {
                value = parameter.ConvertedValue;
            }
            buf.WriteInt64((long)value);
        }
    }
}
