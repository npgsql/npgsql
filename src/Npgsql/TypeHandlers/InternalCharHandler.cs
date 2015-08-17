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
    internal class InternalCharHandler : SimpleTypeHandler<char>,
        ISimpleTypeHandler<byte>, ISimpleTypeHandler<short>, ISimpleTypeHandler<int>, ISimpleTypeHandler<long>
    {
        #region Read

        public override char Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (char)buf.ReadByte();
        }

        byte ISimpleTypeHandler<byte>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte();
        }

        short ISimpleTypeHandler<short>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte();
        }

        int ISimpleTypeHandler<int>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte();
        }

        long ISimpleTypeHandler<long>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte();
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is byte))
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ToByte(value);
            }
            return 1;
        }

        public override void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteByte(value as byte? ?? Convert.ToByte(value));
        }

        #endregion
    }
}
