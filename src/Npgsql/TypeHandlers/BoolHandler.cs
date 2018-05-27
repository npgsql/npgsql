﻿#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-boolean.html
    /// </remarks>
    [TypeMapping("boolean", NpgsqlDbType.Boolean, DbType.Boolean, typeof(bool))]
    class BoolHandler : NpgsqlSimpleTypeHandler<bool>, INpgsqlSimpleTypeHandler<IConvertible>
    {
        const int BoolLength = 1;

        public override bool Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => ReadBool(buf);

        IConvertible INpgsqlSimpleTypeHandler<IConvertible>.Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription)
            => ReadBool(buf);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool ReadBool(NpgsqlReadBuffer buf)
            => buf.ReadByte() != 0;

        public override int ValidateAndGetLength(bool value, NpgsqlParameter parameter)
            => BoolLength;

        public int ValidateAndGetLength(IConvertible value, NpgsqlParameter parameter)
        {
            var converted = Convert.ToBoolean(value);
            if (parameter == null)
                throw CreateConversionButNoParamException(value.GetType());
            parameter.ConvertedValue = converted;
            return BoolLength;
        }

        public override void Write(bool value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => WriteBool(buf, value);

        public void Write(IConvertible value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            Debug.Assert(parameter != null);
            WriteBool(buf, (bool)parameter.ConvertedValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void WriteBool(NpgsqlWriteBuffer buf, bool value)
            => buf.WriteByte(value ? (byte)1 : (byte)0);

    }
}
