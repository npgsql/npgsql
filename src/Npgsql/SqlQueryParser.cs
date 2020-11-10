using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Npgsql
{
    class SqlQueryParser
    {
        readonly Dictionary<string, int> _paramIndexMap = new Dictionary<string, int>();
        readonly StringBuilder _rewrittenSql = new StringBuilder();

        /// <summary>
        /// Receives a raw SQL query as passed in by the user, and performs some processing necessary
        /// before sending to the backend.
        /// This includes doing parameter placeholder processing (@p => $1), and splitting the query
        /// up by semicolons if needed (SELECT 1; SELECT 2)
        /// </summary>
        /// <param name="sql">Raw user-provided query.</param>
        /// <param name="parameters">The parameters configured on the <see cref="NpgsqlCommand"/> of this query
        /// or an empty <see cref="NpgsqlParameterCollection"/> if deriveParameters is set to true.</param>
        /// <param name="statements">An empty list to be populated with the statements parsed by this method</param>
        /// <param name="deriveParameters">A bool indicating whether parameters contains a list of preconfigured parameters or an empty list to be filled with derived parameters.</param>
        internal void ParseRawQuery(
            string sql,
            NpgsqlParameterCollection parameters,
            List<NpgsqlStatement> statements,
            bool deriveParameters = false)
            => ParseRawQuery(sql.AsSpan(), parameters, statements, deriveParameters);

        void ParseRawQuery(
            ReadOnlySpan<char> sql,
            NpgsqlParameterCollection parameters,
            List<NpgsqlStatement> statements,
            bool deriveParameters)
        {
            Debug.Assert(deriveParameters == false || parameters.Count == 0);

            NpgsqlStatement statement = null!;
            var statementIndex = -1;
            MoveToNextStatement();

            var currCharOfs = 0;
            var end = sql.Length;
            var ch = '\0';
            int dollarTagStart;
            int dollarTagEnd;
            var currTokenBeg = 0;
            var blockCommentLevel = 0;
            var parenthesisLevel = 0;

        None:
            if (currCharOfs >= end)
                goto Finish;
            var lastChar = ch;
            ch = sql[currCharOfs++];
        NoneContinue:
            for (; ; lastChar = ch, ch = sql[currCharOfs++])
            {
                switch (ch)
                {
                case '/':
                    goto BlockCommentBegin;
                case '-':
                    goto LineCommentBegin;
                case '\'':
                    goto Quoted;
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
                    if (parenthesisLevel == 0)
                        goto SemiColon;
                    break;
                case '(':
                    parenthesisLevel++;
                    break;
                case ')':
                    parenthesisLevel--;
                    break;
                case 'e':
                case 'E':
                    if (!IsLetter(lastChar))
                        goto EscapedStart;
                    else
                        break;
                }

                if (currCharOfs >= end)
                    goto Finish;
            }

        ParamStart:
            if (currCharOfs < end)
            {
                lastChar = ch;
                ch = sql[currCharOfs];
                if (IsParamNameChar(ch))
                {
                    if (currCharOfs - 1 > currTokenBeg)
                        _rewrittenSql.Append(sql.Slice(currTokenBeg, currCharOfs - 1 - currTokenBeg));
                    currTokenBeg = currCharOfs++ - 1;
                    goto Param;
                }
                currCharOfs++;
                goto NoneContinue;
            }
            goto Finish;

        Param:
            // We have already at least one character of the param name
            for (;;)
            {
                lastChar = ch;
                if (currCharOfs >= end || !IsParamNameChar(ch = sql[currCharOfs]))
                {
                    var paramName = sql.Slice(currTokenBeg + 1, currCharOfs - (currTokenBeg + 1)).ToString();

                    if (!_paramIndexMap.TryGetValue(paramName, out var index))
                    {
                        // Parameter hasn't been seen before in this query
                        if (!parameters.TryGetValue(paramName, out var parameter))
                        {
                            if (deriveParameters)
                            {
                                parameter = new NpgsqlParameter { ParameterName = paramName };
                                parameters.Add(parameter);
                            }
                            else
                            {
                                // Parameter placeholder does not match a parameter on this command.
                                // Leave the text as it was in the SQL, it may not be a an actual placeholder
                                _rewrittenSql.Append(sql.Slice(currTokenBeg, currCharOfs - currTokenBeg));
                                currTokenBeg = currCharOfs;
                                if (currCharOfs >= end)
                                    goto Finish;

                                currCharOfs++;
                                goto NoneContinue;
                            }
                        }

                        if (!parameter.IsInputDirection)
                            throw new Exception($"Parameter '{paramName}' referenced in SQL but is an out-only parameter");

                        statement.InputParameters.Add(parameter);
                        index = _paramIndexMap[paramName] = statement.InputParameters.Count;
                    }
                    _rewrittenSql.Append('$');
                    _rewrittenSql.Append(index);
                    currTokenBeg = currCharOfs;

                    if (currCharOfs >= end)
                        goto Finish;

                    currCharOfs++;
                    goto NoneContinue;
                }

                currCharOfs++;
            }

        Quoted:
            while (currCharOfs < end)
            {
                if (sql[currCharOfs++] == '\'')
                {
                    ch = '\0';
                    goto None;
                }
            }
            goto Finish;

        DoubleQuoted:
            while (currCharOfs < end)
            {
                if (sql[currCharOfs++] == '"')
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
                ch = sql[currCharOfs++];
                if (ch == '\'')
                    goto Escaped;
                goto NoneContinue;
            }
            goto Finish;

        Escaped:
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                switch (ch)
                {
                case '\'':
                    goto MaybeConcatenatedEscaped;
                case '\\':
                {
                    if (currCharOfs >= end)
                        goto Finish;
                    currCharOfs++;
                    break;
                }
                }
            }
            goto Finish;

        MaybeConcatenatedEscaped:
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                switch (ch)
                {
                case '\r':
                case '\n':
                    goto MaybeConcatenatedEscaped2;
                case ' ':
                case '\t':
                case '\f':
                    continue;
                default:
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        MaybeConcatenatedEscaped2:
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                switch (ch)
                {
                case '\'':
                    goto Escaped;
                case '-':
                {
                    if (currCharOfs >= end)
                        goto Finish;
                    ch = sql[currCharOfs++];
                    if (ch == '-')
                        goto MaybeConcatenatedEscapeAfterComment;
                    lastChar = '\0';
                    goto NoneContinue;
                }
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                case '\f':
                    continue;
                default:
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        MaybeConcatenatedEscapeAfterComment:
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                if (ch == '\r' || ch == '\n')
                    goto MaybeConcatenatedEscaped2;
            }
            goto Finish;

        DollarQuotedStart:
            if (currCharOfs < end)
            {
                ch = sql[currCharOfs];
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
                ch = sql[currCharOfs++];
                if (ch == '$')
                {
                    dollarTagEnd = currCharOfs - 1;
                    goto DollarQuoted;
                }
                if (!IsDollarTagIdentifier(ch))
                    goto NoneContinue;
            }
            goto Finish;

        DollarQuoted:
            var tag = sql.Slice(dollarTagStart - 1, dollarTagEnd - dollarTagStart + 2);
            var pos = sql.Slice(dollarTagEnd + 1).IndexOf(tag);
            if (pos == -1)
            {
                currCharOfs = end;
                goto Finish;
            }
            pos += dollarTagEnd + 1; // If the substring is found adjust the position to be relative to the entire span
            currCharOfs = pos + dollarTagEnd - dollarTagStart + 2;
            ch = '\0';
            goto None;

        LineCommentBegin:
            if (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                if (ch == '-')
                    goto LineComment;
                lastChar = '\0';
                goto NoneContinue;
            }
            goto Finish;

        LineComment:
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                if (ch == '\r' || ch == '\n')
                    goto None;
            }
            goto Finish;

        BlockCommentBegin:
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                if (ch == '*')
                {
                    blockCommentLevel++;
                    goto BlockComment;
                }
                if (ch != '/')
                {
                    if (blockCommentLevel > 0)
                        goto BlockComment;
                    lastChar = '\0';
                    goto NoneContinue;
                }
            }
            goto Finish;

        BlockComment:
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                switch (ch)
                {
                case '*':
                    goto BlockCommentEnd;
                case '/':
                    goto BlockCommentBegin;
                }
            }
            goto Finish;

        BlockCommentEnd:
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs++];
                if (ch == '/')
                {
                    if (--blockCommentLevel > 0)
                        goto BlockComment;
                    goto None;
                }
                if (ch != '*')
                    goto BlockComment;
            }
            goto Finish;

        SemiColon:
            _rewrittenSql.Append(sql.Slice(currTokenBeg, currCharOfs - currTokenBeg - 1));
            statement.SQL = _rewrittenSql.ToString();
            while (currCharOfs < end)
            {
                ch = sql[currCharOfs];
                if (char.IsWhiteSpace(ch))
                {
                    currCharOfs++;
                    continue;
                }
                // TODO: Handle end of line comment? Although psql doesn't seem to handle them...

                currTokenBeg = currCharOfs;
                if (_rewrittenSql.Length > 0)
                    MoveToNextStatement();
                goto None;
            }
            if (statements.Count > statementIndex + 1)
                statements.RemoveRange(statementIndex + 1, statements.Count - (statementIndex + 1));
            return;

        Finish:
            _rewrittenSql.Append(sql.Slice(currTokenBeg, end - currTokenBeg));
            statement.SQL = _rewrittenSql.ToString();
            if (statements.Count > statementIndex + 1)
               statements.RemoveRange(statementIndex + 1, statements.Count - (statementIndex + 1));

            void MoveToNextStatement()
            {
                statementIndex++;
                if (statements.Count > statementIndex)
                {
                    statement = statements[statementIndex];
                    statement.Reset();
                }
                else
                {
                    statement = new NpgsqlStatement();
                    statements.Add(statement);
                }
                _paramIndexMap.Clear();
                _rewrittenSql.Clear();
            }
        }

        static bool IsLetter(char ch)
            => 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z';

        static bool IsIdentifierStart(char ch)
            => 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_' || 128 <= ch && ch <= 255;

        static bool IsDollarTagIdentifier(char ch)
            => 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || 128 <= ch && ch <= 255;

        static bool IsIdentifier(char ch)
            => 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || '0' <= ch && ch <= '9' || ch == '_' || ch == '$' || 128 <= ch && ch <= 255;

        static bool IsParamNameChar(char ch)
            => char.IsLetterOrDigit(ch) || ch == '_' || ch == '.';  // why dot??
    }
}
