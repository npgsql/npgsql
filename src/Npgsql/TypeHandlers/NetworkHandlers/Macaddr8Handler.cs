#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Diagnostics;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("macaddr8", NpgsqlDbType.MacAddr8)]
    class Macaddr8Handler : SimpleTypeHandler<PhysicalAddress>, ISimpleTypeHandler<string>
    {
        internal Macaddr8Handler(PostgresType postgresType) : base(postgresType) { }

        #region Read

        public override PhysicalAddress Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(len == 6 || len == 8);

            var bytes = new byte[len];

            buf.ReadBytes(bytes, 0, len);
            return new PhysicalAddress(bytes);
        }

        string ISimpleTypeHandler<string>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).ToString();

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is PhysicalAddress address))
                throw CreateConversionException(value.GetType());

            switch (address.GetAddressBytes().Length)
            {
            case 6:
                return 6;
            case 8:
                return 8;
            default:
                throw new FormatException("MAC addresses must have length 6 in PostgreSQL");
            }
        }

        public int ValidateAndGetLength(string value, NpgsqlParameter parameter)
            => ValidateAndGetLength(PhysicalAddress.Parse(value), parameter);

        protected override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter)
        {
            var bytes = ((PhysicalAddress)value).GetAddressBytes();
            buf.WriteBytes(bytes, 0, bytes.Length);
        }

        public void Write(string value, WriteBuffer buf, NpgsqlParameter parameter)
            => Write(PhysicalAddress.Parse(value), buf, parameter);

        #endregion Write
    }
}
