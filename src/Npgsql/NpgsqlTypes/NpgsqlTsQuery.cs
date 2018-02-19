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
using System.Text;
using JetBrains.Annotations;

#pragma warning disable CA1034

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a PostgreSQL tsquery. This is the base class for the
    /// lexeme, not, or, and, and "followed by" nodes.
    /// </summary>
    public abstract class NpgsqlTsQuery
    {
        /// <summary>
        /// Node kind
        /// </summary>
        public NodeKind Kind { get; protected set; }

        /// <summary>
        /// NodeKind
        /// </summary>
        public enum NodeKind
        {
            /// <summary>
            /// Represents the empty tsquery. Should only be used at top level.
            /// </summary>
            Empty = -1,
            /// <summary>
            /// Lexeme
            /// </summary>
            Lexeme = 0,
            /// <summary>
            /// Not operator
            /// </summary>
            Not = 1,
            /// <summary>
            /// And operator
            /// </summary>
            And = 2,
            /// <summary>
            /// Or operator
            /// </summary>
            Or = 3,
            /// <summary>
            /// "Followed by" operator
            /// </summary>
            Phrase = 4
        }

        internal abstract void Write(StringBuilder sb, bool first = false);

        /// <summary>
        /// Writes the tsquery in PostgreSQL's text format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            Write(sb, true);
            return sb.ToString();
        }

        /// <summary>
        /// Parses a tsquery in PostgreSQL's text format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NpgsqlTsQuery Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var valStack = new Stack<NpgsqlTsQuery>();
            var opStack = new Stack<NpgsqlTsQueryOperator>();

            var sb = new StringBuilder();
            var pos = 0;
            var expectingBinOp = false;

            var lastFollowedByOpDistance = -1;

            NextToken:
            if (pos >= value.Length)
                goto Finish;
            var ch = value[pos++];
            if (ch == '\'')
                goto WaitEndComplex;
            if ((ch == ')' || ch == '|' || ch == '&') && !expectingBinOp || (ch == '(' || ch == '!') && expectingBinOp)
                throw new FormatException("Syntax error in tsquery. Unexpected token.");

            if (ch == '<')
            {
                var endOfOperatorConsumed = false;
                var sbCurrentLength = sb.Length;

                while (pos < value.Length)
                {
                    var c = value[pos++];
                    if (c == '>')
                    {
                        endOfOperatorConsumed = true;
                        break;
                    }

                    sb.Append(c);
                }

                if (sb.Length == sbCurrentLength || !endOfOperatorConsumed)
                    throw new FormatException("Syntax error in tsquery. Malformed 'followed by' operator.");

                var followedByOpDistanceString = sb.ToString(sbCurrentLength, sb.Length - sbCurrentLength);
                if (followedByOpDistanceString == "-")
                {
                    lastFollowedByOpDistance = 1;
                }
                else if (!int.TryParse(followedByOpDistanceString, out lastFollowedByOpDistance)
                         || lastFollowedByOpDistance < 0)
                {
                    throw new FormatException("Syntax error in tsquery. Malformed distance in 'followed by' operator.");
                }

                sb.Length -= followedByOpDistanceString.Length;
            }

            if (ch == '(' || ch == '!' || ch == '&' || ch == '<')
            {
                opStack.Push(new NpgsqlTsQueryOperator(ch, lastFollowedByOpDistance));
                expectingBinOp = false;
                lastFollowedByOpDistance = 0;
                goto NextToken;
            }

            if (ch == '|')
            {
                if (opStack.Count > 0 && opStack.Peek() == '|')
                {
                    if (valStack.Count < 2)
                        throw new FormatException("Syntax error in tsquery");
                    var right = valStack.Pop();
                    var left = valStack.Pop();
                    valStack.Push(new NpgsqlTsQueryOr(left, right));
                    // Implicit pop and repush |
                }
                else
                    opStack.Push('|');
                expectingBinOp = false;
                goto NextToken;
            }

            if (ch == ')')
            {
                while (opStack.Count > 0 && opStack.Peek() != '(')
                {
                    if (valStack.Count < 2 || opStack.Peek() == '!')
                        throw new FormatException("Syntax error in tsquery");

                    var right = valStack.Pop();
                    var left = valStack.Pop();

                    var tsOp = opStack.Pop();
                    switch (tsOp)
                    {
                    case '&':
                        valStack.Push(new NpgsqlTsQueryAnd(left, right));
                        break;

                    case '|':
                        valStack.Push(new NpgsqlTsQueryOr(left, right));
                        break;

                    case '<':
                        valStack.Push(new NpgsqlTsQueryFollowedBy(left, tsOp.FollowedByDistance, right));
                        break;

                    default:
                        throw new FormatException("Syntax error in tsquery");
                    }
                }
                if (opStack.Count == 0)
                    throw new FormatException("Syntax error in tsquery: closing parenthesis without an opening parenthesis");
                opStack.Pop();
                goto PushedVal;
            }

            if (ch == ':')
                throw new FormatException("Unexpected : while parsing tsquery");

            if (char.IsWhiteSpace(ch))
                goto NextToken;

            pos--;
            if (expectingBinOp)
                throw new FormatException("Unexpected lexeme while parsing tsquery");
            // Proceed to WaitEnd

            WaitEnd:
            if (pos >= value.Length || char.IsWhiteSpace(ch = value[pos]) || ch == '!' || ch == '&' || ch == '|' || ch == '(' || ch == ')')
            {
                valStack.Push(new NpgsqlTsQueryLexeme(sb.ToString()));
                goto PushedVal;
            }
            pos++;
            if (ch == ':')
            {
                valStack.Push(new NpgsqlTsQueryLexeme(sb.ToString()));
                sb.Clear();
                goto InWeightInfo;
            }
            if (ch == '\\')
            {
                if (pos >= value.Length)
                    throw new FormatException(@"Unexpected \ in end of value");
                ch = value[pos++];
            }
            sb.Append(ch);
            goto WaitEnd;

            WaitEndComplex:
            if (pos >= value.Length)
                throw new FormatException("Missing terminating ' in string literal");
            ch = value[pos++];
            if (ch == '\'')
            {
                if (pos < value.Length && value[pos] == '\'')
                {
                    ch = '\'';
                    pos++;
                }
                else
                {
                    valStack.Push(new NpgsqlTsQueryLexeme(sb.ToString()));
                    if (pos < value.Length && value[pos] == ':')
                    {
                        pos++;
                        goto InWeightInfo;
                    }
                    goto PushedVal;
                }
            }
            if (ch == '\\')
            {
                if (pos >= value.Length)
                    throw new FormatException(@"Unexpected \ in end of value");
                ch = value[pos++];
            }
            sb.Append(ch);
            goto WaitEndComplex;


            InWeightInfo:
            if (pos >= value.Length)
                goto Finish;
            ch = value[pos];
            if (ch == '*')
                ((NpgsqlTsQueryLexeme)valStack.Peek()).IsPrefixSearch = true;
            else if (ch == 'a' || ch == 'A')
                ((NpgsqlTsQueryLexeme)valStack.Peek()).Weights |= NpgsqlTsQueryLexeme.Weight.A;
            else if (ch == 'b' || ch == 'B')
                ((NpgsqlTsQueryLexeme)valStack.Peek()).Weights |= NpgsqlTsQueryLexeme.Weight.B;
            else if (ch == 'c' || ch == 'C')
                ((NpgsqlTsQueryLexeme)valStack.Peek()).Weights |= NpgsqlTsQueryLexeme.Weight.C;
            else if (ch == 'd' || ch == 'D')
                ((NpgsqlTsQueryLexeme)valStack.Peek()).Weights |= NpgsqlTsQueryLexeme.Weight.D;
            else
                goto PushedVal;
            pos++;
            goto InWeightInfo;

            PushedVal:
            sb.Clear();
            var processTightBindingOperator = true;
            while (opStack.Count > 0 && processTightBindingOperator)
            {
                var tsOp = opStack.Peek();
                switch (tsOp)
                {
                case '&':
                    if (valStack.Count < 2)
                        throw new FormatException("Syntax error in tsquery");
                    var andRight = valStack.Pop();
                    var andLeft = valStack.Pop();
                    valStack.Push(new NpgsqlTsQueryAnd(andLeft, andRight));
                    opStack.Pop();
                    break;

                case '!':
                    if (valStack.Count == 0)
                        throw new FormatException("Syntax error in tsquery");
                    valStack.Push(new NpgsqlTsQueryNot(valStack.Pop()));
                    opStack.Pop();
                    break;

                case '<':
                    if (valStack.Count < 2)
                        throw new FormatException("Syntax error in tsquery");
                    var followedByRight = valStack.Pop();
                    var followedByLeft = valStack.Pop();
                    valStack.Push(
                        new NpgsqlTsQueryFollowedBy(
                            followedByLeft,
                            tsOp.FollowedByDistance,
                            followedByRight));
                    opStack.Pop();
                    break;

                default:
                    processTightBindingOperator = false;
                    break;
                }
            }
            expectingBinOp = true;
            goto NextToken;

            Finish:
            while (opStack.Count > 0)
            {
                if (valStack.Count < 2)
                    throw new FormatException("Syntax error in tsquery");

                var right = valStack.Pop();
                var left = valStack.Pop();

                NpgsqlTsQuery query;
                var tsOp = opStack.Pop();
                switch (tsOp)
                {
                case '&':
                    query = new NpgsqlTsQueryAnd(left, right);
                    break;

                case '|':
                    query = new NpgsqlTsQueryOr(left, right);
                    break;

                case '<':
                    query = new NpgsqlTsQueryFollowedBy(left, tsOp.FollowedByDistance, right);
                    break;

                default:
                    throw new FormatException("Syntax error in tsquery");
                }

                valStack.Push(query);
            }
            if (valStack.Count != 1)
                throw new FormatException("Syntax error in tsquery");
            return valStack.Pop();
        }
    }

    readonly struct NpgsqlTsQueryOperator
    {
        public readonly char Char;
        public readonly int FollowedByDistance;

        public NpgsqlTsQueryOperator(char character, int followedByDistance)
        {
            Char = character;
            FollowedByDistance = followedByDistance;
        }

        public static implicit operator NpgsqlTsQueryOperator(char c) => new NpgsqlTsQueryOperator(c, 0);
        public static implicit operator char(NpgsqlTsQueryOperator o) => o.Char;
    }

    /// <summary>
    /// TsQuery Lexeme node.
    /// </summary>
    public sealed class NpgsqlTsQueryLexeme : NpgsqlTsQuery
    {
        string _text;

        /// <summary>
        /// Lexeme text.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Text is null or empty string", nameof(value));

                _text = value;
            }
        }

        Weight _weights;

        /// <summary>
        /// Weights is a bitmask of the Weight enum.
        /// </summary>
        public Weight Weights
        {
            get => _weights;
            set
            {
                if (((byte)value >> 4) != 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Illegal weights");

                _weights = value;
            }
        }

        /// <summary>
        /// Prefix search.
        /// </summary>
        public bool IsPrefixSearch { get; set; }

        /// <summary>
        /// Creates a tsquery lexeme with only lexeme text.
        /// </summary>
        /// <param name="text">Lexeme text.</param>
        public NpgsqlTsQueryLexeme(string text) : this(text, Weight.None, false) { }

        /// <summary>
        /// Creates a tsquery lexeme with lexeme text and weights.
        /// </summary>
        /// <param name="text">Lexeme text.</param>
        /// <param name="weights">Bitmask of enum Weight.</param>
        public NpgsqlTsQueryLexeme(string text, Weight weights) : this(text, weights, false) { }

        /// <summary>
        /// Creates a tsquery lexeme with lexeme text, weights and prefix search flag.
        /// </summary>
        /// <param name="text">Lexeme text.</param>
        /// <param name="weights">Bitmask of enum Weight.</param>
        /// <param name="isPrefixSearch">Is prefix search?</param>
        public NpgsqlTsQueryLexeme(string text, Weight weights, bool isPrefixSearch)
        {
            Kind = NodeKind.Lexeme;
            Text = text;
            Weights = weights;
            IsPrefixSearch = isPrefixSearch;
        }

        /// <summary>
        /// Weight enum, can be OR'ed together.
        /// </summary>
#pragma warning disable CA1714
        [Flags]
        public enum Weight
#pragma warning restore CA1714
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// D
            /// </summary>
            D = 1,
            /// <summary>
            /// C
            /// </summary>
            C = 2,
            /// <summary>
            /// B
            /// </summary>
            B = 4,
            /// <summary>
            /// A
            /// </summary>
            A = 8
        }

        internal override void Write(StringBuilder sb, bool first = false)
        {
            sb.Append('\'').Append(Text.Replace(@"\", @"\\").Replace("'", "''")).Append('\'');
            if (IsPrefixSearch || Weights != Weight.None)
                sb.Append(':');
            if (IsPrefixSearch)
                sb.Append('*');
            if ((Weights & Weight.A) != Weight.None)
                sb.Append('A');
            if ((Weights & Weight.B) != Weight.None)
                sb.Append('B');
            if ((Weights & Weight.C) != Weight.None)
                sb.Append('C');
            if ((Weights & Weight.D) != Weight.None)
                sb.Append('D');
        }
    }

    /// <summary>
    /// TsQuery Not node.
    /// </summary>
    public sealed class NpgsqlTsQueryNot : NpgsqlTsQuery
    {
        /// <summary>
        /// Child node
        /// </summary>
        public NpgsqlTsQuery Child { get; set; }

        /// <summary>
        /// Creates a not operator, with a given child node.
        /// </summary>
        /// <param name="child"></param>
        public NpgsqlTsQueryNot([CanBeNull] NpgsqlTsQuery child)
        {
            Kind = NodeKind.Not;
            Child = child;
        }

        internal override void Write(StringBuilder sb, bool first = false)
        {
            sb.Append('!');
            if (Child == null)
            {
                sb.Append("''");
            }
            else
            {
                if (Child.Kind != NodeKind.Lexeme)
                    sb.Append("( ");
                Child.Write(sb, true);
                if (Child.Kind != NodeKind.Lexeme)
                    sb.Append(" )");
            }
        }
    }

    /// <summary>
    /// Base class for TsQuery binary operators (&amp; and |).
    /// </summary>
    public abstract class NpgsqlTsQueryBinOp : NpgsqlTsQuery
    {
        /// <summary>
        /// Left child
        /// </summary>
        public NpgsqlTsQuery Left { get; set; }

        /// <summary>
        /// Right child
        /// </summary>
        public NpgsqlTsQuery Right { get; set; }
    }

    /// <summary>
    /// TsQuery And node.
    /// </summary>
    public sealed class NpgsqlTsQueryAnd : NpgsqlTsQueryBinOp
    {
        /// <summary>
        /// Creates an and operator, with two given child nodes.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public NpgsqlTsQueryAnd([CanBeNull] NpgsqlTsQuery left, [CanBeNull] NpgsqlTsQuery right)
        {
            Kind = NodeKind.And;
            Left = left;
            Right = right;
        }

        internal override void Write(StringBuilder sb, bool first = false)
        {
            Left.Write(sb);
            sb.Append(" & ");
            Right.Write(sb);
        }
    }

    /// <summary>
    /// TsQuery Or Node.
    /// </summary>
    public sealed class NpgsqlTsQueryOr : NpgsqlTsQueryBinOp
    {
        /// <summary>
        /// Creates an or operator, with two given child nodes.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public NpgsqlTsQueryOr([CanBeNull] NpgsqlTsQuery left, [CanBeNull] NpgsqlTsQuery right)
        {
            Kind = NodeKind.Or;
            Left = left;
            Right = right;
        }

        internal override void Write(StringBuilder sb, bool first = false)
        {
            if (!first)
                sb.Append("( ");

            Left.Write(sb);
            sb.Append(" | ");
            Right.Write(sb);

            if (!first)
                sb.Append(" )");
        }
    }

    /// <summary>
    /// TsQuery "Followed by" Node.
    /// </summary>
    public sealed class NpgsqlTsQueryFollowedBy : NpgsqlTsQueryBinOp
    {
        /// <summary>
        /// The distance between the 2 nodes, in lexemes.
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// Creates a "followed by" operator, specifying 2 child nodes and the 
        /// distance between them in lexemes.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="distance"></param>
        /// <param name="right"></param>
        public NpgsqlTsQueryFollowedBy(
            [CanBeNull] NpgsqlTsQuery left,
            int distance,
            [CanBeNull] NpgsqlTsQuery right)
        {
            if (distance < 0)
                throw new ArgumentOutOfRangeException(nameof(distance));

            Left = left;
            Distance = distance;
            Right = right;
            Kind = NodeKind.Phrase;
        }

        internal override void Write(StringBuilder sb, bool first = false)
        {
            if (!first)
                sb.Append("( ");

            Left.Write(sb);

            sb.Append(" <");
            if (Distance == 1) sb.Append("-");
            else sb.Append(Distance);
            sb.Append("> ");

            Right.Write(sb);

            if (!first)
                sb.Append(" )");
        }
    }

    /// <summary>
    /// Represents an empty tsquery. Shold only be used as top node.
    /// </summary>
    public sealed class NpgsqlTsQueryEmpty : NpgsqlTsQuery
    {
        /// <summary>
        /// Creates a tsquery that represents an empty query. Should not be used as child node.
        /// </summary>
        public NpgsqlTsQueryEmpty()
        {
            Kind = NodeKind.Empty;
        }

        internal override void Write(StringBuilder sb, bool first = false)
        {
        }
    }
}
