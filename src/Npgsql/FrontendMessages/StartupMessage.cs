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
using System.Text;

namespace Npgsql.FrontendMessages
{
    class StartupMessage : SimpleFrontendMessage
    {
        readonly Dictionary<byte[], byte[]> _parameters = new Dictionary<byte[], byte[]>();
        int _length;

        const int ProtocolVersion3 = 3 << 16; // 196608

        internal string this[string key]
        {
            set { _parameters[PGUtil.UTF8Encoding.GetBytes(key)] = PGUtil.UTF8Encoding.GetBytes(value); }
        }

        internal override int Length
        {
            get
            {
                return _length = 4 + // len
                                 4 + // protocol version
                                 _parameters.Select(kv => kv.Key.Length + kv.Value.Length + 2).Sum() +
                                 1; // trailing zero byte
            }
        }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteInt32(_length);
            buf.WriteInt32(ProtocolVersion3);

            foreach (var kv in _parameters)
            {
                buf.WriteBytesNullTerminated(kv.Key);
                buf.WriteBytesNullTerminated(kv.Value);
            }

            buf.WriteByte(0);
        }
    }
}
