#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using Npgsql.BackendMessages;
using NpgsqlTypes;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type handler for the Postgresql "char" type, used only internally
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-character.html
    /// </remarks>
    [TypeMapping("char", NpgsqlDbType.InternalChar)]
    class InternalCharHandler : SimpleTypeHandler<char>,
        ISimpleTypeHandler<byte>, ISimpleTypeHandler<short>, ISimpleTypeHandler<int>, ISimpleTypeHandler<long>
    {
        internal InternalCharHandler(PostgresType postgresType) : base(postgresType) { }

        #region Read

        public override char Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => (char)buf.ReadByte();

        byte ISimpleTypeHandler<byte>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => buf.ReadByte();

        short ISimpleTypeHandler<short>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => buf.ReadByte();

        int ISimpleTypeHandler<int>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => buf.ReadByte();

        long ISimpleTypeHandler<long>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => buf.ReadByte();

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            if (!(value is byte))
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ToByte(value);
            }
            return 1;
        }

        protected override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null)
        {
            buf.WriteByte(value as byte? ?? Convert.ToByte(value));
        }

        #endregion
    }
}
