﻿#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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

using System.Text;

namespace Npgsql.Util
{
    /// <summary>
    /// This class contains extensions for the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to its snake_case equivalent.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <remarks>
        /// Code borrowed from Newtonsoft.Json.
        /// See https://github.com/JamesNK/Newtonsoft.Json/blob/f012ba857f36fe75b1294a210b9104130a4db4d5/Src/Newtonsoft.Json/Utilities/StringUtils.cs#L200-L276.
        /// </remarks>
        public static string ToSnakeCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var sb = new StringBuilder();
            var state = SnakeCaseState.Start;

            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == ' ')
                {
                    if (state != SnakeCaseState.Start)
                    {
                        state = SnakeCaseState.NewWord;
                    }
                }
                else if (char.IsUpper(value[i]))
                {
                    switch (state)
                    {
                        case SnakeCaseState.Upper:
                            var hasNext = (i + 1 < value.Length);
                            if (i > 0 && hasNext)
                            {
                                var nextChar = value[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != '_')
                                {
                                    sb.Append('_');
                                }
                            }
                            break;
                        case SnakeCaseState.Lower:
                        case SnakeCaseState.NewWord:
                            sb.Append('_');
                            break;
                    }

                    sb.Append(char.ToLowerInvariant(value[i]));
                    state = SnakeCaseState.Upper;
                }
                else if (value[i] == '_')
                {
                    sb.Append('_');
                    state = SnakeCaseState.Start;
                }
                else
                {
                    if (state == SnakeCaseState.NewWord)
                    {
                        sb.Append('_');
                    }

                    sb.Append(value[i]);
                    state = SnakeCaseState.Lower;
                }
            }

            return sb.ToString();
        }

        enum SnakeCaseState
        {
            Start,
            Lower,
            Upper,
            NewWord
        }
    }
}
