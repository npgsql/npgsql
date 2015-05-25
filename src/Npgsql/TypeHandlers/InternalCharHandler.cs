using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type handler for the Postgresql "char" type, used only internally
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-character.html
    /// </remarks>
    [TypeMapping("char", NpgsqlDbType.InternalChar)]
    internal class InternalCharHandler : TypeHandler<char>,
        ISimpleTypeReader<char>, ISimpleTypeWriter,
        ISimpleTypeReader<byte>, ISimpleTypeReader<short>, ISimpleTypeReader<int>, ISimpleTypeReader<long>
    {
        #region Read

        public char Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (char)buf.ReadByte();
        }

        byte ISimpleTypeReader<byte>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte();
        }

        short ISimpleTypeReader<short>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte();
        }

        int ISimpleTypeReader<int>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte();
        }

        long ISimpleTypeReader<long>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte();
        }

        #endregion

        #region Write

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is byte))
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ToByte(value);
            }
            return 1;
        }

        public void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteByte(value as byte? ?? Convert.ToByte(value));
        }

        #endregion
    }
}
