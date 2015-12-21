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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("macaddr", NpgsqlDbType.MacAddr, typeof(PhysicalAddress))]
    internal class MacaddrHandler : SimpleTypeHandler<PhysicalAddress>, ISimpleTypeHandler<string>
    {
        public override PhysicalAddress Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            Contract.Assume(len == 6);

            var bytes = new byte[6];

            buf.ReadBytes(bytes, 0, 6);
            return new PhysicalAddress(bytes);
        }

        string ISimpleTypeHandler<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            var address = value as PhysicalAddress;
            if (address == null)
                throw CreateConversionException(value.GetType());
            if (address.GetAddressBytes().Length != 6)
                throw new FormatException("MAC addresses must have length 6 in PostgreSQL");
            return 6;
        }

        public override void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteBytes(((PhysicalAddress)value).GetAddressBytes(), 0, 6);
        }
    }
}
