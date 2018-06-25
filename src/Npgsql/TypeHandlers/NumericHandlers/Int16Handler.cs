#region License
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
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("smallint", NpgsqlDbType.Smallint, new[] { DbType.Int16, DbType.Byte, DbType.SByte }, new[] { typeof(short), typeof(byte), typeof(sbyte) }, DbType.Int16)]
    class Int16Handler : NpgsqlSimpleTypeHandler<short>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<sbyte>, INpgsqlSimpleTypeHandler<int>, INpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>, INpgsqlSimpleTypeHandler<decimal>
    {
        #region Read

        public override short Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => buf.ReadInt16();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((byte)Read(buf, len, fieldDescription));

        sbyte INpgsqlSimpleTypeHandler<sbyte>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((sbyte)Read(buf, len, fieldDescription));

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        float INpgsqlSimpleTypeHandler<float>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        decimal INpgsqlSimpleTypeHandler<decimal>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(short value, NpgsqlParameter parameter) => 2;
        public int ValidateAndGetLength(int value, NpgsqlParameter parameter)            => 2;
        public int ValidateAndGetLength(long value, NpgsqlParameter parameter)           => 2;
        public int ValidateAndGetLength(byte value, NpgsqlParameter parameter)           => 2;
        public int ValidateAndGetLength(sbyte value, NpgsqlParameter parameter)          => 2;
        public int ValidateAndGetLength(float value, NpgsqlParameter parameter)          => 2;
        public int ValidateAndGetLength(double value, NpgsqlParameter parameter)         => 2;
        public int ValidateAndGetLength(decimal value, NpgsqlParameter parameter)        => 2;

        public override void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(value);
        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(checked((short)value));
        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(checked((short)value));
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(value);
        public void Write(sbyte value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(value);
        public void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16((short)value);
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(checked((short)value));
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(checked((short)value));

        #endregion Write
    }
}
