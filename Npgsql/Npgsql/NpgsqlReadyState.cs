// Npgsql.NpgsqlReadyState.cs
//
// Author:
//     Dave Joyner <d4ljoyn@yahoo.com>
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Npgsql
{
    internal sealed class NpgsqlReadyState : NpgsqlState
    {
        public static readonly NpgsqlReadyState Instance = new NpgsqlReadyState();

        private readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        private NpgsqlReadyState()
            : base()
        {
        }

        public override void Query(NpgsqlConnector context, NpgsqlQuery query)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Query");

            query.WriteToStream(context.Stream);
        }

        public override void Parse(NpgsqlConnector context, NpgsqlParse parse)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Parse");

            parse.WriteToStream(context.Stream);
        }

        public override void Sync(NpgsqlConnector context)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Sync");

            NpgsqlSync.Default.WriteToStream(context.Stream);
        }

        public override void Bind(NpgsqlConnector context, NpgsqlBind bind)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Bind");

            bind.WriteToStream(context.Stream);
        }

        public override void Describe(NpgsqlConnector context, NpgsqlDescribe describe)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Describe");

            describe.WriteToStream(context.Stream);
        }

        public override void Execute(NpgsqlConnector context, NpgsqlExecute execute)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Execute");

            execute.WriteToStream(context.Stream);
        }

        public override void Close(NpgsqlConnector context)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "Close");
            Stream stream = context.Stream;
            try
            {
                stream
                    .WriteBytes((byte) FrontEndMessageCode.Termination)
                    .WriteInt32(4)
                    .Flush();
            }
            catch
            {
                //Error writting termination message to stream, nothing we can do.
            }

            try
            {
                stream.Close();
            }
            catch
            {
            }

            context.Stream = null;
            context.BaseStream = null;
            ChangeState(context, NpgsqlClosedState.Instance);
        }
    }
}
