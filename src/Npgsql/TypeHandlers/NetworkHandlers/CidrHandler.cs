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

using System.Net;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

#pragma warning disable 618

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("cidr", NpgsqlDbType.Cidr)]
    class CidrHandler : NpgsqlSimpleTypeHandler<(IPAddress Address, int Subnet)>, INpgsqlSimpleTypeHandler<NpgsqlInet>
    {
        public override (IPAddress Address, int Subnet) Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => InetHandler.DoRead(buf, len, fieldDescription, true);

        NpgsqlInet INpgsqlSimpleTypeHandler<NpgsqlInet>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
        {
            var (address, subnet) = Read(buf, len, fieldDescription);
            return new NpgsqlInet(address, subnet);
        }

        public override int ValidateAndGetLength((IPAddress Address, int Subnet) value, NpgsqlParameter parameter)
            => InetHandler.GetLength(value.Address);

        public int ValidateAndGetLength(NpgsqlInet value, NpgsqlParameter parameter)
            => InetHandler.GetLength(value.Address);

        public override void Write((IPAddress Address, int Subnet) value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => InetHandler.DoWrite(value.Address, value.Subnet, buf, true);

        public void Write(NpgsqlInet value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => InetHandler.DoWrite(value.Address, value.Netmask, buf, true);
    }
}
