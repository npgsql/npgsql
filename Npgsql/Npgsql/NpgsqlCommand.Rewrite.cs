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

            byte[] ret = string.IsNullOrEmpty(planName) ? GetCommandText(false, false) : GetExecuteCommandText();
            // In constructing the command text, we potentially called internal
            // queries.  Reset command timeout and SQL sent.
            m_Connector.Mediator.ResetResponses();
            m_Connector.Mediator.CommandTimeout = CommandTimeout;

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

            // reset any responses just before getting new ones
            m_Connector.Mediator.ResetResponses();

            // Set command timeout.
            m_Connector.Mediator.CommandTimeout = CommandTimeout;

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
        /// <param name="forExtendQuery"></param>
        /// <returns>UTF8 encoded command ready to be sent to the backend.</returns>
        private byte[] GetCommandText(bool prepare, bool forExtendQuery)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "GetCommandText");

            MemoryStream commandBuilder = new MemoryStream();
            StringChunk[] chunks;

            chunks = GetDistinctTrimmedCommands(commandText);

            if (chunks.Length > 1)
            {
                if (prepare || commandType == CommandType.StoredProcedure)
                {
                    throw new NpgsqlException("Multiple queries not supported for this command type");
                }
            }

            foreach (StringChunk chunk in chunks)
            {
                if (commandBuilder.Length > 0)
                {
                    commandBuilder
                        .WriteBytes((byte)ASCIIBytes.SemiColon)
                        .WriteBytes(ASCIIByteArrays.LineTerminator);
                }

                if (prepare && ! forExtendQuery)
                {
                    commandBuilder
                        .WriteString("PREPARE ")
                        .WriteString(planName)
                        .WriteString(" AS ");
                }

                if (commandType == CommandType.StoredProcedure)
                {
                    if (! prepare && ! functionChecksDone)
                    {
                        functionNeedsColumnListDefinition = Parameters.Count != 0 && CheckFunctionNeedsColumnDefinitionList();

                        functionChecksDone = true;
                    }

                    commandBuilder.WriteString(
                        Connector.SupportsPrepare
                        ? "SELECT * FROM " // This syntax is only available in 7.3+ as well SupportsPrepare.
                        : "SELECT " //Only a single result return supported. 7.2 and earlier.
                    );

                    if (commandText[chunk.Begin + chunk.Length - 1] == ')')
                    {
                        AppendCommandReplacingParameterValues(commandBuilder, commandText, chunk.Begin, chunk.Length, prepare, forExtendQuery);
                    }
                    else
                    {
                        commandBuilder
                            .WriteString(commandText.Substring(chunk.Begin, chunk.Length))
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

                    if (! prepare && functionNeedsColumnListDefinition)
                    {
                        AddFunctionColumnListSupport(commandBuilder);
                    }
                }
                else if (commandType == CommandType.TableDirect)
                {
                    commandBuilder
                        .WriteString("SELECT * FROM ")
                        .WriteString(commandText.Substring(chunk.Begin, chunk.Length));
                }
                else
                {
                    AppendCommandReplacingParameterValues(commandBuilder, commandText, chunk.Begin, chunk.Length, prepare, forExtendQuery);
                }
            }

            return commandBuilder.ToArray();
        }

        /// <summary>
        /// Find the beginning and end of each distinct SQL command and produce
        /// a list of descriptors, one for each command.  Commands described are trimmed of
        /// leading and trailing white space and their terminating semi-colons.
        /// </summary>
        /// <param name="src">Raw command text.</param>
        /// <returns>List of chunk descriptors.</returns>
        private static StringChunk[] GetDistinctTrimmedCommands(string src)
        {
            bool inQuote = false;
            bool quoteEscape = false;
            int currCharOfs = -1;
            int currChunkBeg = 0;
            int currChunkRawLen = 0;
            int currChunkTrimLen = 0;
            List<StringChunk> chunks = new List<StringChunk>();

            foreach (char ch in src)
            {
                currCharOfs++;

                // goto label for character re-evaluation:
                ProcessCharacter:

                if (! inQuote)
                {
                    switch (ch)
                    {
                        case '\'' :
                            inQuote = true;

                            currChunkRawLen++;
                            currChunkTrimLen = currChunkRawLen;

                            break;

                        case ';' :
                            if (currChunkTrimLen > 0)
                            {
                                chunks.Add(new StringChunk(currChunkBeg, currChunkTrimLen));
                            }

                            currChunkBeg = currCharOfs + 1;
                            currChunkRawLen = 0;
                            currChunkTrimLen = 0;

                            break;

                        case ' ' :
                        case '\t' :
                        case '\r' :
                        case '\n' :
                            if (currChunkTrimLen == 0)
                            {
                                currChunkBeg++;
                            }
                            else
                            {
                                currChunkRawLen++;
                            }

                            break;

                        default :
                            currChunkRawLen++;
                            currChunkTrimLen = currChunkRawLen;

                            break;

                    }
                }
                else
                {
                    switch (ch)
                    {
                        case '\'' :
                            if (quoteEscape)
                            {
                                quoteEscape = false;
                            }
                            else
                            {
                                quoteEscape = true;
                            }

                            currChunkRawLen++;
                            currChunkTrimLen = currChunkRawLen;

                            break;

                        default :
                            if (quoteEscape)
                            {
                                quoteEscape = false;
                                inQuote = false;

                                // Re-evaluate this character
                                goto ProcessCharacter;
                            }
                            else
                            {
                                currChunkRawLen++;
                                currChunkTrimLen = currChunkRawLen;
                            }

                            break;

                    }
                }
            }

            if (currChunkTrimLen > 0)
            {
                chunks.Add(new StringChunk(currChunkBeg, currChunkTrimLen));
            }

            return chunks.ToArray();
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
            byte[] serialised = parameter.TypeInfo.ConvertToBackend(parameter.Value, false, Connector.NativeToBackendTypeConverterOptions);

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

        private enum TokenType
        {
            None,
            Quoted,
            Param,
            Colon,
            FullTextMatchOp
        }

        /// <summary>
        /// Append a region of a source command text to an output command, performing parameter token
        /// substitutions.
        /// </summary>
        /// <param name="dest">Stream to which to append output.</param>
        /// <param name="src">Command text.</param>
        /// <param name="begin">Starting index within src.</param>
        /// <param name="length">Length of region to be processed.</param>
        /// <param name="prepare"></param>
        /// <param name="forExtendedQuery"></param>
        private void AppendCommandReplacingParameterValues(Stream dest, string src, int begin, int length, bool prepare, bool forExtendedQuery)
        {
            char lastChar = '\0';
            TokenType currTokenType = TokenType.None;
            char paramMarker = '\0';
            int currTokenBeg = begin;
            int currTokenLen = 0;
            Dictionary<NpgsqlParameter, int> paramOrdinalMap = null;
            int end = begin + length;

            if (prepare)
            {
                paramOrdinalMap = new Dictionary<NpgsqlParameter, int>();

                for (int i = 0 ; i < parameters.Count ; i++)
                {
                    paramOrdinalMap[parameters[i]] = i + 1;
                }
            }

            for (int currCharOfs = begin ; currCharOfs < end ; currCharOfs++)
            {
                char ch = src[currCharOfs];

                // goto label for character re-evaluation:
                ProcessCharacter:

                switch (currTokenType)
                {
                    case TokenType.None :
                        switch (ch)
                        {
                            case '\'' :
                                if (currTokenLen > 0)
                                {
                                    dest.WriteString(src.Substring(currTokenBeg, currTokenLen));
                                }

                                currTokenType = TokenType.Quoted;

                                currTokenBeg = currCharOfs;
                                currTokenLen = 1;

                                break;

                            case ':' :
                                if (currTokenLen > 0)
                                {
                                    dest.WriteString(src.Substring(currTokenBeg, currTokenLen));
                                }

                                currTokenType = TokenType.Colon;

                                currTokenBeg = currCharOfs;
                                currTokenLen = 1;

                                break;

                            case '<' :
                            case '@' :
                                if (currTokenLen > 0)
                                {
                                    dest.WriteString(src.Substring(currTokenBeg, currTokenLen));
                                }

                                currTokenType = TokenType.FullTextMatchOp;

                                currTokenBeg = currCharOfs;
                                currTokenLen = 1;

                                break;

                            default :
                                currTokenLen++;

                                break;

                        }

                        break;

                    case TokenType.Param :
                        if (IsParamNameChar(ch))
                        {
                            currTokenLen++;
                        }
                        else
                        {
                            string paramName = src.Substring(currTokenBeg, currTokenLen);
                            NpgsqlParameter parameter;
                            bool wroteParam = false;

                            if (parameters.TryGetValue(paramName, out parameter))
                            {
                                if (
                                    (parameter.Direction == ParameterDirection.Input) ||
                                    (parameter.Direction == ParameterDirection.InputOutput)
                                )
                                {
                                    if (prepare)
                                    {
                                        AppendParameterPlaceHolder(dest, parameter, paramOrdinalMap[parameter]);
                                    }
                                    else
                                    {
                                        AppendParameterValue(dest, parameter);
                                    }
                                }

                                wroteParam = true;
                            }

                            if (! wroteParam)
                            {
                                dest.WriteString("{0}{1}", paramMarker, paramName);
                            }

                            currTokenType = TokenType.None;
                            currTokenBeg = currCharOfs;
                            currTokenLen = 0;

                            // Re-evaluate this character
                            goto ProcessCharacter;
                        }

                        break;

                    case TokenType.Quoted :
                        switch (ch)
                        {
                            case '\'' :
                                currTokenLen++;

                                break;

                            default :
                                if (currTokenLen > 1 && lastChar == '\'')
                                {
                                    dest.WriteString(src.Substring(currTokenBeg, currTokenLen));

                                    currTokenType = TokenType.None;
                                    currTokenBeg = currCharOfs;
                                    currTokenLen = 0;

                                    // Re-evaluate this character
                                    goto ProcessCharacter;
                                }
                                else
                                {
                                    currTokenLen++;
                                }

                                break;

                        }

                        break;

                    case TokenType.Colon :
                        if (IsParamNameChar(ch))
                        {
                            // Switch to parameter name token, include this character.
                            currTokenType = TokenType.Param;

                            currTokenBeg = currCharOfs;
                            currTokenLen = 1;
                            paramMarker = ':';
                        }
                        else
                        {
                            // Demote to the unknown token type and continue.
                            currTokenType = TokenType.None;
                            currTokenLen++;
                        }

                        break;

                    case TokenType.FullTextMatchOp :
                        if (lastChar == '@' && IsParamNameChar(ch))
                        {
                            // Switch to parameter name token, include this character.
                            currTokenType = TokenType.Param;

                            currTokenBeg = currCharOfs;
                            currTokenLen = 1;
                            paramMarker = '@';
                        }
                        else
                        {
                            // Demote to the unknown token type and continue.
                            currTokenType = TokenType.None;
                            currTokenLen++;
                        }

                        break;

                }

                lastChar = ch;
            }

            switch (currTokenType)
            {
                case TokenType.Param :
                    string paramName = src.Substring(currTokenBeg, currTokenLen);
                    NpgsqlParameter parameter;
                    bool wroteParam = false;

                    if (parameters.TryGetValue(paramName, out parameter))
                    {
                        if (
                            (parameter.Direction == ParameterDirection.Input) ||
                            (parameter.Direction == ParameterDirection.InputOutput)
                        )
                        {
                            if (prepare)
                            {
                                AppendParameterPlaceHolder(dest, parameter, paramOrdinalMap[parameter]);
                            }
                            else
                            {
                                AppendParameterValue(dest, parameter);
                            }
                        }

                        wroteParam = true;
                    }

                    if (! wroteParam)
                    {
                        dest.WriteString("{0}{1}", paramMarker, paramName);
                    }

                    break;

                default :
                    if (currTokenLen > 0)
                    {
                        dest.WriteString(src.Substring(currTokenBeg, currTokenLen));
                    }

                    break;

            }
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
