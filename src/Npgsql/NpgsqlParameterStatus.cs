// created on 8/6/2003 at 13:57
// Npgsql.NpgsqlAsciiRow.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
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

using System.IO;

namespace Npgsql
{
    /// <summary>
    /// This class represents the ParameterStatus message sent from PostgreSQL
    /// server.
    /// </summary>
    ///
    internal sealed class NpgsqlParameterStatus
    {
        public readonly string Parameter;
        public readonly string ParameterValue;

        public NpgsqlParameterStatus(Stream stream)
        {
            //Read message length
            PGUtil.EatStreamBytes(stream, 4);
            Parameter = PGUtil.ReadString(stream);
            ParameterValue = PGUtil.ReadString(stream);
        }
        public NpgsqlParameterStatus(string parameter, string parameterValue)
        {
            Parameter = parameter;
            ParameterValue = parameterValue;
        }
    }
}
