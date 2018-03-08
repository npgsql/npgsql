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
using JetBrains.Annotations;

namespace Npgsql.FrontendMessages
{
    class CopyFailMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'f';

        [CanBeNull]
        readonly string _errorMessage;
        readonly int _errorMessageLen;

        internal CopyFailMessage() {}

        internal CopyFailMessage(string errorMessage="")
        {
            if (errorMessage.Length > 1024)
                throw new ArgumentException("CopyFail message must be 1024 characters or less");
            _errorMessage = errorMessage;
            _errorMessageLen = PGUtil.UTF8Encoding.GetByteCount(_errorMessage);
        }

        internal override int Length => 1 + 4 + (_errorMessageLen + 1);

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);
            // Error message not supported for now
            Debug.Assert(_errorMessage == null);
            buf.WriteByte(0);
        }

        public override string ToString() => "[CopyFail]";
    }
}
