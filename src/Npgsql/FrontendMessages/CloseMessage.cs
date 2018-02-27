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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class CloseMessage : SimpleFrontendMessage
    {
        /// <summary>
        /// The name of the prepared statement or portal to close (an empty string selects the unnamed prepared statement or portal).
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Whether to close a statement or a portal
        /// </summary>
        internal StatementOrPortal StatementOrPortal { get; private set; }

        const byte Code = (byte)'C';

        internal CloseMessage Populate(StatementOrPortal type, string name="")
        {
            StatementOrPortal = type;
            Name = name;
            return this;
        }

        internal override int Length => 1 + 4 + 1 + (Name.Length + 1);

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            Debug.Assert(Name != null && Name.All(c => c < 128));

            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);
            buf.WriteByte((byte)StatementOrPortal);
            buf.WriteNullTerminatedString(Name);
        }

        public override string ToString() => $"[Close {StatementOrPortal}={Name}]";
    }
}
