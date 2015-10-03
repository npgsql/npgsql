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
using System.IO;
using System.Linq;

namespace Npgsql
{
    static class SqlQueryParser
    {
        static readonly Array ParamNameCharTable;

        static SqlQueryParser()
        {
            ParamNameCharTable = BuildParameterNameCharacterTable();
        }

        /// <summary>
        /// Receives a raw SQL query as passed in by the user, and performs some processing necessary
        /// before sending to the backend.
        /// This includes doing parameter placebolder processing (@p => $1), and splitting the query
        /// up by semicolons if needed (SELECT 1; SELECT 2)
        /// </summary>
        /// <param name="sql">Raw user-provided query.</param>
        /// <param name="standardConformantStrings">Whether the PostgreSQL session is configured to use standard conformant strings.</param>
        /// <param name="parameters">The parameters configured on the <see cref="NpgsqlCommand"/> of this query.</param>
        /// <param name="queries">An empty list to be populated with the queries parsed by this method</param>
        static internal void ParseRawQuery(string sql, bool standardConformantStrings, NpgsqlParameterCollection parameters, List<NpgsqlStatement> queries)
        {
            Contract.Requires(sql != null);
            Contract.Requires(queries != null && !queries.Any());

            var currCharOfs = 0;
            var end = sql.Length;
            var ch = '\0';
            int dollarTagStart;
            int dollarTagEnd;
            var currTokenBeg = 0;
            var blockCommentLevel = 0;

            queries.Clear();
            // TODO: Recycle
            var paramIndexMap = new Dictionary<string, int>();
            var currentSql = new StringWriter();
            var currentParameters = new List<NpgsqlParameter>();

        None:
            if (currCharOfs >= end) {
                goto Finish;
            }
            var lastChar = ch;
            ch = sql[currCharOfs++];
        NoneContinue:
            for (; ; lastChar = ch, ch = sql[currCharOfs++]) {
                switch (ch) {
                case '/':
                    goto BlockCommentBegin;
                case '-':
                    goto LineCommentBegin;
                case '\'':
                    if (standardConformantStrings)
                        goto Quoted;
                    else
                        goto Escaped;
                case '$':
                    if (!IsIdentifier(lastChar))
                        goto DollarQuotedStart;
                    else
                        break;
                case '"':
                    goto DoubleQuoted;
                case ':':
                    if (lastChar != ':')
                        goto ParamStart;
                    else
                        break;
                case '@':
                    if (lastChar != '@')
                        goto ParamStart;
                    else
                        break;
                case ';':
                    goto SemiColon;

                case 'e':
                case 'E':
                    if (!IsLetter(lastChar))
                        goto EscapedStart;
                    else
                        break;
                }

                if (currCharOfs >= end) {
                    goto Finish;
                }
            }

        ParamStart:
            if (currCharOfs < end) {
                lastChar = ch;
                ch = sql[currCharOfs];
                if (IsParamNameChar(ch)) {
                    if (currCharOfs - 1 > currTokenBeg) {
                        currentSql.Write(sql.Substring(currTokenBeg, currCharOfs - 1 - currTokenBeg));
                    }
                    currTokenBeg = currCharOfs++ - 1;
                    goto Param;
                } else {
                    currCharOfs++;
                    goto NoneContinue;
                }
            }
            goto Finish;

        Param:
            // We have already at least one character of the param name
            for (; ; ) {
                lastChar = ch;
                if (currCharOfs >= end || !IsParamNameChar(ch = sql[currCharOfs])) {
                    var paramName = sql.Substring(currTokenBeg, currCharOfs - currTokenBeg);

                    int index;
                    if (!paramIndexMap.TryGetValue(paramName, out index)) {
                        // Parameter hasn't been seen before in this query
                        NpgsqlParameter parameter;
                        if (!parameters.TryGetValue(paramName, out parameter)) {
                            throw new Exception(
                                $"Parameter '{paramName}' referenced in SQL but not found in parameter list");
                        }

                        if (!parameter.IsInputDirection) {
                            throw new Exception(
                                $"Parameter '{paramName}' referenced in SQL but is an out-only parameter");
                        }

                        currentParameters.Add(parameter);
                        index = paramIndexMap[paramName] = currentParameters.Count;
                    }
                    currentSql.Write('$');
                    currentSql.Write(index);
                    currTokenBeg = currCharOfs;

                    if (currCharOfs >= end) {
                        goto Finish;
                    }

                    currCharOfs++;
                    goto NoneContinue;
                } else {
                    currCharOfs++;
                }
            }

        Quoted:
            while (currCharOfs < end) {
                if (sql[currCharOfs++] == '\'') {
                    ch = '\0';
                    goto None;
                }
            }
            goto Finish;

        DoubleQuoted:
            while (currCharOfs < end) {
                if (sql[currCharOfs++] == '"') {
                    ch = '\0';
                    goto None;
                }
            }
            goto Finish;

        EscapedStart:
            if (currCharOfs < end) {
                lastChar = ch;
                ch = sql[currCharOfs++];
                if (ch == '\'') {
                    goto Escaped;
                }
                goto NoneContinue;
            }
            goto Finish;

        Escaped:
            while (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '\'') {
                    goto MaybeConcatenatedEscaped;
                }
                if (ch == '\\') {
                    if (currCharOfs >= end) {
                        goto Finish;
                    }
                    currCharOfs++;
                }
            }
            goto Finish;

        MaybeConcatenatedEscaped:
            while (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '\r' || ch == '\n') {
                    goto MaybeConcatenatedEscaped2;
                }
                if (ch != ' ' && ch != '\t' && ch != '\f') {
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        MaybeConcatenatedEscaped2:
            while (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '\'') {
                    goto Escaped;
                }
                if (ch == '-') {
                    if (currCharOfs >= end) {
                        goto Finish;
                    }
                    ch = sql[currCharOfs++];
                    if (ch == '-') {
                        goto MaybeConcatenatedEscapeAfterComment;
                    }
                    lastChar = '\0';
                    goto NoneContinue;

                }
                if (ch != ' ' && ch != '\t' && ch != '\n' & ch != '\r' && ch != '\f') {
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        MaybeConcatenatedEscapeAfterComment:
            while (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '\r' || ch == '\n') {
                    goto MaybeConcatenatedEscaped2;
                }
            }
            goto Finish;

        DollarQuotedStart:
            if (currCharOfs < end) {
                ch = sql[currCharOfs];
                if (ch == '$') {
                    // Empty tag
                    dollarTagStart = dollarTagEnd = currCharOfs;
                    currCharOfs++;
                    goto DollarQuoted;
                }
                if (IsIdentifierStart(ch)) {
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
            while (currCharOfs < end) {
                lastChar = ch;
                ch = sql[currCharOfs++];
                if (ch == '$') {
                    dollarTagEnd = currCharOfs - 1;
                    goto DollarQuoted;
                }
                if (!IsDollarTagIdentifier(ch)) {
                    goto NoneContinue;
                }
            }
            goto Finish;

        DollarQuoted: {
                var tag = sql.Substring(dollarTagStart - 1, dollarTagEnd - dollarTagStart + 2);
                var pos = sql.IndexOf(tag, dollarTagEnd + 1); // Not linear time complexity, but that's probably not a problem, since PostgreSQL backend's isn't either
                if (pos == -1) {
                    currCharOfs = end;
                    goto Finish;
                }
                currCharOfs = pos + dollarTagEnd - dollarTagStart + 2;
                ch = '\0';
                goto None;
            }

        LineCommentBegin:
            if (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '-') {
                    goto LineComment;
                }
                lastChar = '\0';
                goto NoneContinue;
            }
            goto Finish;

        LineComment:
            while (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '\r' || ch == '\n') {
                    goto None;
                }
            }
            goto Finish;

        BlockCommentBegin:
            while (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '*') {
                    blockCommentLevel++;
                    goto BlockComment;
                }
                if (ch != '/') {
                    if (blockCommentLevel > 0) {
                        goto BlockComment;
                    }
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        BlockComment:
            while (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '*') {
                    goto BlockCommentEnd;
                }
                if (ch == '/') {
                    goto BlockCommentBegin;
                }
            }
            goto Finish;

        BlockCommentEnd:
            while (currCharOfs < end) {
                ch = sql[currCharOfs++];
                if (ch == '/') {
                    if (--blockCommentLevel > 0) {
                        goto BlockComment;
                    }
                    goto None;
                }
                if (ch != '*') {
                    goto BlockComment;
                }
            }
            goto Finish;

        SemiColon:
            currentSql.Write(sql.Substring(currTokenBeg, currCharOfs - currTokenBeg - 1));
            queries.Add(new NpgsqlStatement(currentSql.ToString(), currentParameters));
            while (currCharOfs < end) {
                ch = sql[currCharOfs];
                if (char.IsWhiteSpace(ch)) {
                    currCharOfs++;
                    continue;
                }
                // TODO: Handle end of line comment? Although psql doesn't seem to handle them...

                currTokenBeg = currCharOfs;
                paramIndexMap.Clear();
                if (queries.Count > NpgsqlCommand.MaxStatements) {
                    throw new NotSupportedException(
                        $"A single command cannot contain more than {NpgsqlCommand.MaxStatements} queries");
                }
                currentSql = new StringWriter();
                currentParameters = new List<NpgsqlParameter>();
                goto None;
            }
            return;

        Finish:
            currentSql.Write(sql.Substring(currTokenBeg, end - currTokenBeg));
            queries.Add(new NpgsqlStatement(currentSql.ToString(), currentParameters));
        }

        static bool IsLetter(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z';
        }

        static bool IsIdentifierStart(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_' || 128 <= ch && ch <= 255;
        }

        static bool IsDollarTagIdentifier(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || 128 <= ch && ch <= 255;
        }

        static bool IsIdentifier(char ch)
        {
            return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || ch == '$' || 128 <= ch && ch <= 255;
        }

        static bool IsParamNameChar(char ch)
        {
            if (ch < '.' || ch > 'z') {
                return false;
            }
            return ((byte)ParamNameCharTable.GetValue(ch) != 0);
        }

        static Array BuildParameterNameCharacterTable()
        {
            // Table has lower bound of (int)'.';
            var paramNameCharTable = Array.CreateInstance(typeof(byte), new[] { 'z' - '.' + 1 }, new int[] { '.' });

            paramNameCharTable.SetValue((byte)'.', '.');

            for (int i = '0'; i <= '9'; i++) {
                paramNameCharTable.SetValue((byte)i, i);
            }

            for (int i = 'A'; i <= 'Z'; i++) {
                paramNameCharTable.SetValue((byte)i, i);
            }

            paramNameCharTable.SetValue((byte)'_', '_');

            for (int i = 'a'; i <= 'z'; i++) {
                paramNameCharTable.SetValue((byte)i, i);
            }

            return paramNameCharTable;
        }
    }
}
