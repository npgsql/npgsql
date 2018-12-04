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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class StartupMessage : SimpleFrontendMessage
    {
        readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();
        int _length;

        const int ProtocolVersion3 = 3 << 16; // 196608

        internal string this[string key]
        {
            set => _parameters[key] = value;
        }

        internal override int Length
        {
            get
            {
                _length = 4 + // len
                          4 + // protocol version
                          1;  // trailing zero byte

                foreach (var kvp in _parameters)
                    _length += PGUtil.UTF8Encoding.GetByteCount(kvp.Key) + 1 +
                               PGUtil.UTF8Encoding.GetByteCount(kvp.Value) + 1;
                return _length;
            }
        }

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteInt32(_length);
            buf.WriteInt32(ProtocolVersion3);

            foreach (var kv in _parameters)
            {
                buf.WriteString(kv.Key);
                buf.WriteByte(0);
                buf.WriteString(kv.Value);
                buf.WriteByte(0);
            }

            buf.WriteByte(0);
        }
    }
}
