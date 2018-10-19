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

using System.Diagnostics;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using Npgsql.TypeMapping;
using Npgsql.TypeHandling;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Npgsql.TypeHandlers.InternalTypesHandlers
{
    [TypeMapping("pg_lsn", NpgsqlDbType.Lsn, typeof(NpgsqlLsn))]
    class LsnHandler : NpgsqlSimpleTypeHandler<NpgsqlLsn>, INpgsqlTypeHandler<string>
    {
        public override NpgsqlLsn Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(len == 8);

            var upper = buf.ReadUInt32();
            var lower = buf.ReadUInt32();

            return new NpgsqlLsn(upper, lower);
        }

        public override int ValidateAndGetLength(NpgsqlLsn value, NpgsqlParameter parameter = null)
            => 8;

        public int ValidateAndGetLength(string value, ref NpgsqlLengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter)
        {
            var parsed = NpgsqlLsn.Parse(value);
            return ValidateAndGetLength(parsed, ref lengthCache, parameter);
        }

        public override void Write(NpgsqlLsn value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null)
        {
            buf.WriteUInt32(value.Upper);
            buf.WriteUInt32(value.Lower);
        }

        public Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, [CanBeNull] NpgsqlParameter parameter, bool async)
        {
            var parsed = NpgsqlLsn.Parse(value);
            return WriteWithLengthInternal(parsed, buf, lengthCache, parameter, async);
        }

        async ValueTask<string> INpgsqlTypeHandler<string>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
        {
            await buf.Ensure(8, async);

            var upper = buf.ReadUInt32();
            var lower = buf.ReadUInt32();

            var value = new NpgsqlLsn(upper, lower);

            return value.ToString();
        }
    }
}
