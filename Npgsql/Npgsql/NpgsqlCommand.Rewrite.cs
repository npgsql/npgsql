// created on 18/11/2013

// Npgsql.NpgsqlCommand.Rewrite.cs
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Represents a SQL statement or function (stored procedure) to execute
    /// against a PostgreSQL database. This class cannot be inherited.
    /// </summary>
    public sealed partial class NpgsqlCommand : DbCommand, ICloneable
    {
        ///<summary>
        /// This method checks the connection state to see if the connection
        /// is set or it is open. If one of this conditions is not met, throws
        /// an InvalidOperationException
        ///</summary>
        private void CheckConnectionState()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "CheckConnectionState");

            // Check the connection state.
            if (Connector == null || Connector.State == ConnectionState.Closed)
            {
                throw new InvalidOperationException(resman.GetString("Exception_ConnectionNotOpen"));
            }
            if (Connector.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "There is already an open DataReader associated with this Command which must be closed first.");
            }
        }

        /// <summary>
        /// This method substitutes the <see cref="Npgsql.NpgsqlCommand.Parameters">Parameters</see>, if exist, in the command
        /// to their actual values.
        /// The parameter name format is <b>:ParameterName</b>.
        /// </summary>
        /// <returns>A version of <see cref="Npgsql.NpgsqlCommand.CommandText">CommandText</see> with the <see cref="Npgsql.NpgsqlCommand.Parameters">Parameters</see> inserted.</returns>
        internal byte[] GetCommandText()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetCommandText");

            byte[] ret = IsPrepared ? GetExecuteCommandText() : GetCommandText(false);

            return ret;
        }

        private Boolean CheckFunctionNeedsColumnDefinitionList()
        {
            // If and only if a function returns "record" and has no OUT ("o" in proargmodes), INOUT ("b"), or TABLE
            // ("t") return arguments to characterize the result columns, we must provide a column definition list.
            // See http://pgfoundry.org/forum/forum.php?thread_id=1075&forum_id=519
            // We would use our Output and InputOutput parameters to construct that column definition list.  If we have
            // no such parameters, skip the check: we could only construct "AS ()", which yields a syntax error.

            // Updated after 0.99.3 to support the optional existence of a name qualifying schema and allow for case insensitivity
            // when the schema or procedure name do not contain a quote.
            // The hard-coded schema name 'public' was replaced with code that uses schema as a qualifier, only if it is provided.

            String returnRecordQuery;

            StringBuilder parameterTypes = new StringBuilder("");

            // Process parameters

            Boolean seenDef = false;
            foreach (NpgsqlParameter p in Parameters)
            {
                if ((p.Direction == ParameterDirection.Input) || (p.Direction == ParameterDirection.InputOutput))
                {
                    parameterTypes.Append(Connection.Connector.OidToNameMapping[p.TypeInfo.Name].OID.ToString() + " ");
                }

                if ((p.Direction == ParameterDirection.Output) || (p.Direction == ParameterDirection.InputOutput))
                {
                    seenDef = true;
                }
            }

            if (!seenDef)
            {
                return false;
            }

            // Process schema name.

            String schemaName = String.Empty;
            String procedureName = String.Empty;

            String[] fullName = CommandText.Split('.');

            String predicate = "prorettype = ( select oid from pg_type where typname = 'record' ) "
                + "and proargtypes=:proargtypes and proname=:proname "
                // proargmodes && array['o','b','t']::"char"[] performs just as well, but it requires PostgreSQL 8.2.
                + "and ('o' = any (proargmodes) OR 'b' = any (proargmodes) OR 't' = any (proargmodes)) is not true";
            if (fullName.Length == 2)
            {
                returnRecordQuery =
                "select count(*) > 0 from pg_proc p left join pg_namespace n on p.pronamespace = n.oid where " + predicate + " and n.nspname=:nspname";

                schemaName = (fullName[0].IndexOf("\"") != -1) ? fullName[0] : fullName[0].ToLower();
                procedureName = (fullName[1].IndexOf("\"") != -1) ? fullName[1] : fullName[1].ToLower();
            }
            else
            {
                // Instead of defaulting don't use the nspname, as an alternative, query pg_proc and pg_namespace to try and determine the nspname.
                //schemaName = "public"; // This was removed after build 0.99.3 because the assumption that a function is in public is often incorrect.
                returnRecordQuery =
                    "select count(*) > 0 from pg_proc p where " + predicate;

                procedureName = (CommandText.IndexOf("\"") != -1) ? CommandText : CommandText.ToLower();
            }

            bool ret;

            using (NpgsqlCommand c = new NpgsqlCommand(returnRecordQuery, Connection))
            {
                c.Parameters.Add(new NpgsqlParameter("proargtypes", NpgsqlDbType.Oidvector));
                c.Parameters.Add(new NpgsqlParameter("proname", NpgsqlDbType.Name));

                c.Parameters[0].Value = parameterTypes.ToString();
                c.Parameters[1].Value = procedureName;

                if (schemaName != null && schemaName.Length > 0)
                {
                    c.Parameters.Add(new NpgsqlParameter("nspname", NpgsqlDbType.Name));
                    c.Parameters[2].Value = schemaName;
                }

                ret = (Boolean)c.ExecuteScalar();
            }

            return ret;
        }

        private void AddFunctionColumnListSupport(Stream st)
        {
            bool isFirstOutputOrInputOutput = true;

            PGUtil.WriteString(st, " AS (");

            for (int i = 0 ; i < Parameters.Count ; i++)
            {
                var p = Parameters[i];

                switch(p.Direction)
                {
                    case ParameterDirection.Output: case ParameterDirection.InputOutput:
                        if (isFirstOutputOrInputOutput)
                        {
                            isFirstOutputOrInputOutput = false;
                        }
                        else
                        {
                            st.WriteString(", ");
                        }

                        st
                            .WriteString(p.CleanName)
                            .WriteBytes((byte)ASCIIBytes.Space)
                            .WriteString(p.TypeInfo.Name);

                        break;
                }
            }

            st.WriteByte((byte)ASCIIBytes.ParenRight);
        }

        private class StringChunk
        {
            public readonly int Begin;
            public readonly int Length;

            public StringChunk(int begin, int length)
            {
                this.Begin = begin;
                this.Length = length;
            }
        }

        /// <summary>
        /// Process this.commandText, trimming each distinct command and substituting paramater
        /// tokens.
        /// </summary>
        /// <param name="prepare"></param>
        /// <returns>UTF8 encoded command ready to be sent to the backend.</returns>
        private byte[] GetCommandText(bool prepare)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetCommandText");

            MemoryStream commandBuilder = new MemoryStream();

            if (commandType == CommandType.TableDirect)
            {
                foreach (var table in commandText.Split(';'))
                {
                    if (table.Trim().Length == 0)
                    {
                        continue;
                    }

                    commandBuilder
                        .WriteString("SELECT * FROM ")
                        .WriteString(table)
                        .WriteString(";");
                }
            }
            else if (commandType == CommandType.StoredProcedure)
            {
                if (!prepare && !functionChecksDone)
                {
                    functionNeedsColumnListDefinition = Parameters.Count != 0 && CheckFunctionNeedsColumnDefinitionList();

                    functionChecksDone = true;
                }

                commandBuilder.WriteString("SELECT * FROM ");

                if (commandText.TrimEnd().EndsWith(")"))
                {
                    if (!AppendCommandReplacingParameterValues(commandBuilder, commandText, prepare, false))
                    {
                        throw new NpgsqlException("Multiple queries not supported for stored procedures");
                    }
                }
                else
                {
                    commandBuilder
                        .WriteString(commandText)
                        .WriteBytes((byte)ASCIIBytes.ParenLeft);

                    if (prepare)
                    {
                        AppendParameterPlaceHolders(commandBuilder);
                    }
                    else
                    {
                        AppendParameterValues(commandBuilder);
                    }

                    commandBuilder.WriteBytes((byte)ASCIIBytes.ParenRight);
                }

                if (!prepare && functionNeedsColumnListDefinition)
                {
                    AddFunctionColumnListSupport(commandBuilder);
                }
            }
            else
            {
                if (!AppendCommandReplacingParameterValues(commandBuilder, commandText, prepare, !prepare))
                {
                    throw new NpgsqlException("Multiple queries not supported for prepared statements");
                }
            }

            return commandBuilder.ToArray();
        }

        private void AppendParameterPlaceHolders(Stream dest)
        {
            bool first = true;

            for (int i = 0; i < parameters.Count; i++)
            {
                NpgsqlParameter parameter = parameters[i];

                if (
                    (parameter.Direction == ParameterDirection.Input) ||
                    (parameter.Direction == ParameterDirection.InputOutput)
                )
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        dest.WriteString(", ");
                    }

                    AppendParameterPlaceHolder(dest, parameter, i + 1);
                }
            }
        }

        private void AppendParameterPlaceHolder(Stream dest, NpgsqlParameter parameter, int paramNumber)
        {
            string parameterSize = "";

            dest.WriteBytes((byte)ASCIIBytes.ParenLeft);

            if (parameter.TypeInfo.UseSize && (parameter.Size > 0))
            {
                parameterSize = string.Format("({0})", parameter.Size);
            }

            if (parameter.UseCast)
            {
                dest.WriteString("${0}::{1}{2}", paramNumber, parameter.TypeInfo.CastName, parameterSize);
            }
            else
            {
                dest.WriteString("${0}{1}", paramNumber, parameterSize);
            }

            dest.WriteBytes((byte)ASCIIBytes.ParenRight);
        }

        private void AppendParameterValues(Stream dest)
        {
            bool first = true;

            for (int i = 0 ; i < parameters.Count ; i++)
            {
                NpgsqlParameter parameter = parameters[i];

                if (
                    (parameter.Direction == ParameterDirection.Input) ||
                    (parameter.Direction == ParameterDirection.InputOutput)
                )
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        dest.WriteString(", ");
                    }

                    AppendParameterValue(dest, parameter);
                }
            }
        }

        private void AppendParameterValue(Stream dest, NpgsqlParameter parameter)
        {
            byte[] serialised = parameter.TypeInfo.ConvertToBackend(parameter.NpgsqlValue, false, Connector.NativeToBackendTypeConverterOptions);

            // Add parentheses wrapping parameter value before the type cast to avoid problems with Int16.MinValue, Int32.MinValue and Int64.MinValue
            // See bug #1010543
            // Check if this parenthesis can be collapsed with the previous one about the array support. This way, we could use
            // only one pair of parentheses for the two purposes instead of two pairs.
            dest
                .WriteBytes((byte)ASCIIBytes.ParenLeft)
                .WriteBytes((byte)ASCIIBytes.ParenLeft)
                .WriteBytes(serialised)
                .WriteBytes((byte)ASCIIBytes.ParenRight);

            if (parameter.UseCast)
            {
                dest.WriteString("::{0}", parameter.TypeInfo.CastName);

                if (parameter.TypeInfo.UseSize && (parameter.Size > 0))
                {
                    dest.WriteString("({0})", parameter.Size);
                }
            }

            dest.WriteBytes((byte)ASCIIBytes.ParenRight);
        }

        private static bool IsParamNameChar(char ch)
        {
            if (ch < '.' || ch > 'z')
            {
                return false;
            }
            else
            {
                return ((byte)ParamNameCharTable.GetValue(ch) != 0);
            }
        }

        private static bool IsLetter(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z';
        }

        private static bool IsIdentifierStart(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_' || 128 <= ch && ch <= 255;
        }

        private static bool IsDollarTagIdentifier(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || 128 <= ch && ch <= 255;
        }

        private static bool IsIdentifier(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || ch == '$' || 128 <= ch && ch <= 255;
        }

        /// <summary>
        /// Append a region of a source command text to an output command, performing parameter token
        /// substitutions.
        /// </summary>
        /// <param name="dest">Stream to which to append output.</param>
        /// <param name="src">Command text.</param>
        /// <param name="prepare"></param>
        /// <param name="allowMultipleStatements"></param>
        /// <returns>false if the query has multiple statements which are not allowed</returns>
        private bool AppendCommandReplacingParameterValues(Stream dest, string src, bool prepare, bool allowMultipleStatements)
        {
            bool standardConformantStrings = connection != null && connection.Connector != null && connection.Connector.IsInitialized ? connection.UseConformantStrings : true;

            int currCharOfs = 0;
            int end = src.Length;
            char ch = '\0';
            char lastChar = '\0';
            int dollarTagStart = 0;
            int dollarTagEnd = 0;
            int currTokenBeg = 0;
            int blockCommentLevel = 0;

            Dictionary<NpgsqlParameter, int> paramOrdinalMap = null;

            if (prepare)
            {
                paramOrdinalMap = new Dictionary<NpgsqlParameter, int>();

                for (int i = 0 ; i < parameters.Count ; i++)
                {
                    paramOrdinalMap[parameters[i]] = i + 1;
                }
            }

            if (allowMultipleStatements && parameters.Count == 0)
            {
                dest.WriteString(src);
                return true;
            }

        None:
            if (currCharOfs >= end)
            {
                goto Finish;
            }
            lastChar = ch;
            ch = src[currCharOfs++];
        NoneContinue:
            for (; ; lastChar = ch, ch = src[currCharOfs++])
            {
                switch (ch)
                {
                    case '/':                                           goto BlockCommentBegin;
                    case '-':                                           goto LineCommentBegin;
                    case '\'': if (standardConformantStrings)           goto Quoted;                else goto Escaped;
                    case '$':  if (!IsIdentifier(lastChar))             goto DollarQuotedStart;     else break;
                    case '"':                                           goto DoubleQuoted;
                    case ':':  if (lastChar != ':')                     goto ParamStart;            else break;
                    case '@':  if (lastChar != '@')                     goto ParamStart;            else break;
                    case ';':  if (!allowMultipleStatements)            goto SemiColon;             else break;

                    case 'e':
                    case 'E':  if (!IsLetter(lastChar))                 goto EscapedStart;          else break;
                }

                if (currCharOfs >= end)
                {
                    goto Finish;
                }
            }
            
        ParamStart:
            if (currCharOfs < end)
            {
                lastChar = ch;
                ch = src[currCharOfs];
                if (IsParamNameChar(ch))
                {
                    if (currCharOfs - 1 > currTokenBeg)
                    {
                        dest.WriteString(src.Substring(currTokenBeg, currCharOfs - 1 - currTokenBeg));
                    }
                    currTokenBeg = currCharOfs++ - 1;
                    goto Param;
                }
                else
                {
                    currCharOfs++;
                    goto NoneContinue;
                }
            }
            goto Finish;

        Param:
            // We have already at least one character of the param name
            for (; ; )
            {
                lastChar = ch;
                if (currCharOfs >= end || !IsParamNameChar(ch = src[currCharOfs]))
                {
                    string paramName = src.Substring(currTokenBeg, currCharOfs - currTokenBeg);
                    NpgsqlParameter parameter;

                    if (parameters.TryGetValue(paramName, out parameter))
                    {
                        if (parameter.Direction == ParameterDirection.Input || parameter.Direction == ParameterDirection.InputOutput)
                        {
                            if (prepare)
                            {
                                AppendParameterPlaceHolder(dest, parameter, paramOrdinalMap[parameter]);
                            }
                            else
                            {
                                AppendParameterValue(dest, parameter);
                            }
                            currTokenBeg = currCharOfs;
                        }
                    }

                    if (currCharOfs >= end)
                    {
                        goto Finish;
                    }

                    currCharOfs++;
                    goto NoneContinue;
                }
                else
                {
                    currCharOfs++;
                }
            }

        Quoted:
            while (currCharOfs < end)
            {
                if (src[currCharOfs++] == '\'')
                {
                    ch = '\0';
                    goto None;
                }
            }
            goto Finish;

        DoubleQuoted:
            while (currCharOfs < end)
            {
                if (src[currCharOfs++] == '"')
                {
                    ch = '\0';
                    goto None;
                }
            }
            goto Finish;

        EscapedStart:
            if (currCharOfs < end)
            {
                lastChar = ch;
                ch = src[currCharOfs++];
                if (ch == '\'')
                {
                    goto Escaped;
                }
                goto NoneContinue;
            }
            goto Finish;

        Escaped:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\'')
                {
                    goto MaybeConcatenatedEscaped;
                }
                if (ch == '\\')
                {
                    if (currCharOfs >= end)
                    {
                        goto Finish;
                    }
                    currCharOfs++;
                }
            }
            goto Finish;

        MaybeConcatenatedEscaped:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\r' || ch == '\n')
                {
                    goto MaybeConcatenatedEscaped2;
                }
                if (ch != ' ' && ch != '\t' && ch != '\f')
                {
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        MaybeConcatenatedEscaped2:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\'')
                {
                    goto Escaped;
                }
                if (ch == '-')
                {
                    if (currCharOfs >= end)
                    {
                        goto Finish;
                    }
                    ch = src[currCharOfs++];
                    if (ch == '-')
                    {
                        goto MaybeConcatenatedEscapeAfterComment;
                    }
                    lastChar = '\0';
                    goto NoneContinue;

                }
                if (ch != ' ' && ch != '\t' && ch != '\n' & ch != '\r' && ch != '\f')
                {
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        MaybeConcatenatedEscapeAfterComment:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\r' || ch == '\n')
                {
                    goto MaybeConcatenatedEscaped2;
                }
            }
            goto Finish;

        DollarQuotedStart:
            if (currCharOfs < end)
            {
                ch = src[currCharOfs];
                if (ch == '$')
                {
                    // Empty tag
                    dollarTagStart = dollarTagEnd = currCharOfs;
                    currCharOfs++;
                    goto DollarQuoted;
                }
                if (IsIdentifierStart(ch))
                {
                    dollarTagStart = currCharOfs;
                    currCharOfs++;
                    goto DollarQuotedInFirstDelim;
                }
                lastChar = '$';
                currCharOfs++;
                goto NoneContinue;
            }
            goto Finish;

        DollarQuotedInFirstDelim:
            while (currCharOfs < end)
            {
                lastChar = ch;
                ch = src[currCharOfs++];
                if (ch == '$')
                {
                    dollarTagEnd = currCharOfs - 1;
                    goto DollarQuoted;
                }
                if (!IsDollarTagIdentifier(ch))
                {
                    goto NoneContinue;
                }
            }
            goto Finish;

        DollarQuoted:
            {
                string tag = src.Substring(dollarTagStart - 1, dollarTagEnd - dollarTagStart + 2);
                int pos = src.IndexOf(tag, dollarTagEnd + 1); // Not linear time complexity, but that's probably not a problem, since PostgreSQL backend's isn't either
                if (pos == -1)
                {
                    currCharOfs = end;
                    goto Finish;
                }
                currCharOfs = pos + dollarTagEnd - dollarTagStart + 2;
                ch = '\0';
                goto None;
            }

        LineCommentBegin:
            if (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '-')
                {
                    goto LineComment;
                }
                lastChar = '\0';
                goto NoneContinue;
            }
            goto Finish;

        LineComment:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '\r' || ch == '\n')
                {
                    goto None;
                }
            }
            goto Finish;

        BlockCommentBegin:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '*')
                {
                    blockCommentLevel++;
                    goto BlockComment;
                }
                if (ch != '/')
                {
                    if (blockCommentLevel > 0)
                    {
                        goto BlockComment;
                    }
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        BlockComment:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '*')
                {
                    goto BlockCommentEnd;
                }
                if (ch == '/')
                {
                    goto BlockCommentBegin;
                }
            }
            goto Finish;

        BlockCommentEnd:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch == '/')
                {
                    if (--blockCommentLevel > 0)
                    {
                        goto BlockComment;
                    }
                    goto None;
                }
                if (ch != '*')
                {
                    goto BlockComment;
                }
            }
            goto Finish;

        SemiColon:
            while (currCharOfs < end)
            {
                ch = src[currCharOfs++];
                if (ch != ' ' && ch != '\t' && ch != '\n' & ch != '\r' && ch != '\f') // We don't check for comments after the last ; yet
                {
                    return false;
                }
            }
            // implicit goto Finish

        Finish:
            dest.WriteString(src.Substring(currTokenBeg, end - currTokenBeg));
            return true;
        }

        private byte[] GetExecuteCommandText()
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetPreparedCommandText");

            MemoryStream result = new MemoryStream();

            result.WriteString("EXECUTE {0}", planName);

            if(parameters.Count != 0)
            {
                result.WriteByte((byte)ASCIIBytes.ParenLeft);

                for (int i = 0 ; i < Parameters.Count ; i++)
                {
                    var p = Parameters[i];

                    if (i > 0)
                    {
                        result.WriteByte((byte)ASCIIBytes.Comma);
                    }

                    // Add parentheses wrapping parameter value before the type cast to avoid problems with Int16.MinValue, Int32.MinValue and Int64.MinValue
                    // See bug #1010543
                    result.WriteByte((byte)ASCIIBytes.ParenLeft);

                    byte[] serialization;

                    serialization = p.TypeInfo.ConvertToBackend(p.Value, false, Connector.NativeToBackendTypeConverterOptions);

                    result
                        .WriteBytes(serialization)
                        .WriteBytes((byte)ASCIIBytes.ParenRight);

                    if (p.UseCast)
                    {
                        PGUtil.WriteString(result, string.Format("::{0}", p.TypeInfo.CastName));

                        if (p.TypeInfo.UseSize && (p.Size > 0))
                        {
                            result.WriteString("({0})", p.Size);
                        }
                    }
                }

                result.WriteByte((byte)ASCIIBytes.ParenRight);
            }

            return result.ToArray();
        }
    }
}
