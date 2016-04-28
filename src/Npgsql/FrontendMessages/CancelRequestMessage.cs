﻿#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
    class CancelRequestMessage : SimpleFrontendMessage
    {
        internal int BackendProcessId { get; }
        internal int BackendSecretKey { get; }

        const int CancelRequestCode = 1234 << 16 | 5678;

        internal CancelRequestMessage(int backendProcessId, int backendSecretKey)
        {
            BackendProcessId = backendProcessId;
            BackendSecretKey = backendSecretKey;
        }

        internal override int Length => 16;

        internal override void WriteFully(WriteBuffer buf)
        {
            Contract.Requires(BackendProcessId != 0);

            buf.WriteInt32(Length);
            buf.WriteInt32(CancelRequestCode);
            buf.WriteInt32(BackendProcessId);
            buf.WriteInt32(BackendSecretKey);
        }

        public override string ToString()
        {
            return $"[CancelRequest(BackendProcessId={BackendProcessId})]";
        }
    }
}
