using System;
using System.Collections.Generic;
using System.Text;


// ReSharper disable once CheckNamespace
namespace NpgsqlTypes;

/// <summary>
/// Represents a PostgreSQL tsquery. This is the base class for the
/// lexeme, not, or, and, and "followed by" nodes.
/// </summary>
public abstract class NpgsqlTsQuery : IEquatable<NpgsqlTsQuery>
{
    /// <summary>
    /// Node kind
    /// </summary>
    public NodeKind Kind { get; }

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

    /// <summary>
    /// Constructs an <see cref="NpgsqlTsQuery"/>.
    /// </summary>
    /// <param name="kind"></param>
    protected NpgsqlTsQuery(NodeKind kind) => Kind = kind;

    /// <summary>
    /// Writes the tsquery in PostgreSQL's text format.
    /// </summary>
    public void Write(StringBuilder stringBuilder) => WriteCore(stringBuilder, true);

    internal abstract void WriteCore(StringBuilder sb, bool first = false);

    /// <summary>
    /// Writes the tsquery in PostgreSQL's text format.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        Write(sb);
        return sb.ToString();
    }

    /// <summary>
    /// Parses a tsquery in PostgreSQL's text format.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [Obsolete("Client-side parsing of NpgsqlTsQuery is unreliable and cannot fully duplicate the PostgreSQL logic. Use PG functions instead (e.g. to_tsquery)")]
    public static NpgsqlTsQuery Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var valStack = new Stack<NpgsqlTsQuery>();
        var opStack = new Stack<NpgsqlTsQueryOperator>();

        var sb = new StringBuilder();
        var pos = 0;
        var expectingBinOp = false;

        short lastFollowedByOpDistance = -1;

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
            else if (!short.TryParse(followedByOpDistanceString, out lastFollowedByOpDistance)
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
                valStack.Push((char)tsOp switch
                {
                    '&' => new NpgsqlTsQueryAnd(left, right),
                    '|' => new NpgsqlTsQueryOr(left, right),
                    '<' => new NpgsqlTsQueryFollowedBy(left, tsOp.FollowedByDistance, right),
                    _   => throw new FormatException("Syntax error in tsquery")
                });
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
        switch (ch)
        {
        case '*':
            ((NpgsqlTsQueryLexeme)valStack.Peek()).IsPrefixSearch = true;
            break;
        case 'a' or 'A':
            ((NpgsqlTsQueryLexeme)valStack.Peek()).Weights |= NpgsqlTsQueryLexeme.Weight.A;
            break;
        case 'b' or 'B':
            ((NpgsqlTsQueryLexeme)valStack.Peek()).Weights |= NpgsqlTsQueryLexeme.Weight.B;
            break;
        case 'c' or 'C':
            ((NpgsqlTsQueryLexeme)valStack.Peek()).Weights |= NpgsqlTsQueryLexeme.Weight.C;
            break;
        case 'd' or 'D':
            ((NpgsqlTsQueryLexeme)valStack.Peek()).Weights |= NpgsqlTsQueryLexeme.Weight.D;
            break;
        default:
            goto PushedVal;
        }

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

            var tsOp = opStack.Pop();
            var query = (char)tsOp switch
            {
                '&' => (NpgsqlTsQuery)new NpgsqlTsQueryAnd(left, right),
                '|' => new NpgsqlTsQueryOr(left, right),
                '<' => new NpgsqlTsQueryFollowedBy(left, tsOp.FollowedByDistance, right),
                _   => throw new FormatException("Syntax error in tsquery")
            };
            valStack.Push(query);
        }
        if (valStack.Count != 1)
            throw new FormatException("Syntax error in tsquery");
        return valStack.Pop();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => throw new NotSupportedException("Must be overridden");

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is NpgsqlTsQuery query && query.Equals(this);

    /// <summary>
    /// Returns a value indicating whether this instance and a specified <see cref="NpgsqlTsQuery"/> object represent the same value.
    /// </summary>
    /// <param name="other">An object to compare to this instance.</param>
    /// <returns><see langword="true"/> if g is equal to this instance; otherwise, <see langword="false"/>.</returns>
    public abstract bool Equals(NpgsqlTsQuery? other);

    /// <summary>
    /// Indicates whether the values of two specified <see cref="NpgsqlTsQuery"/> objects are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(NpgsqlTsQuery? left, NpgsqlTsQuery? right)
        => left is null ? right is null : left.Equals(right);

    /// <summary>
    /// Indicates whether the values of two specified <see cref="NpgsqlTsQuery"/> objects are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(NpgsqlTsQuery? left, NpgsqlTsQuery? right)
        => left is null ? right is not null : !left.Equals(right);
}

readonly struct NpgsqlTsQueryOperator(char character, short followedByDistance)
{
    public readonly char Char = character;
    public readonly short FollowedByDistance = followedByDistance;

    public static implicit operator NpgsqlTsQueryOperator(char c) => new(c, 0);
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
            ArgumentException.ThrowIfNullOrEmpty(value);

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
        : base(NodeKind.Lexeme)
    {
        _text = text;
        Weights = weights;
        IsPrefixSearch = isPrefixSearch;
    }

    /// <summary>
    /// Weight enum, can be OR'ed together.
    /// </summary>
    [Flags]
    public enum Weight
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

    internal override void WriteCore(StringBuilder sb, bool first = false)
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

    /// <inheritdoc/>
    public override bool Equals(NpgsqlTsQuery? other)
        => other is NpgsqlTsQueryLexeme lexeme &&
           lexeme.Text == Text &&
           lexeme.Weights == Weights &&
           lexeme.IsPrefixSearch == IsPrefixSearch;

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Text, Weights, IsPrefixSearch);
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
    public NpgsqlTsQueryNot(NpgsqlTsQuery child)
        : base(NodeKind.Not)
        => Child = child;

    internal override void WriteCore(StringBuilder sb, bool first = false)
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
            Child.WriteCore(sb, true);
            if (Child.Kind != NodeKind.Lexeme)
                sb.Append(" )");
        }
    }

    /// <inheritdoc/>
    public override bool Equals(NpgsqlTsQuery? other)
        => other is NpgsqlTsQueryNot not && not.Child == Child;

    /// <inheritdoc/>
    public override int GetHashCode()
        => Child?.GetHashCode() ?? 0;
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

    /// <summary>
    /// Constructs a <see cref="NpgsqlTsQueryBinOp"/>.
    /// </summary>
    protected NpgsqlTsQueryBinOp(NodeKind kind, NpgsqlTsQuery left, NpgsqlTsQuery right)
        : base(kind)
    {
        Left = left;
        Right = right;
    }
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
    public NpgsqlTsQueryAnd(NpgsqlTsQuery left, NpgsqlTsQuery right)
        : base(NodeKind.And, left, right) {}

    internal override void WriteCore(StringBuilder sb, bool first = false)
    {
        Left.WriteCore(sb);
        sb.Append(" & ");
        Right.WriteCore(sb);
    }

    /// <inheritdoc/>
    public override bool Equals(NpgsqlTsQuery? other)
        => other is NpgsqlTsQueryAnd and && and.Left == Left && and.Right == Right;

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Left, Right);
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
    public NpgsqlTsQueryOr(NpgsqlTsQuery left, NpgsqlTsQuery right)
        : base(NodeKind.Or, left, right) {}

    internal override void WriteCore(StringBuilder sb, bool first = false)
    {
        // TODO: Figure out the nullability strategy here
        if (!first)
            sb.Append("( ");

        Left.WriteCore(sb);
        sb.Append(" | ");
        Right.WriteCore(sb);

        if (!first)
            sb.Append(" )");
    }

    /// <inheritdoc/>
    public override bool Equals(NpgsqlTsQuery? other)
        => other is NpgsqlTsQueryOr or && or.Left == Left && or.Right == Right;

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Left, Right);
}

/// <summary>
/// TsQuery "Followed by" Node.
/// </summary>
public sealed class NpgsqlTsQueryFollowedBy : NpgsqlTsQueryBinOp
{
    /// <summary>
    /// The distance between the 2 nodes, in lexemes.
    /// </summary>
    public short Distance { get; set; }

    /// <summary>
    /// Creates a "followed by" operator, specifying 2 child nodes and the
    /// distance between them in lexemes.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="distance"></param>
    /// <param name="right"></param>
    public NpgsqlTsQueryFollowedBy(
        NpgsqlTsQuery left,
        short distance,
        NpgsqlTsQuery right)
        : base(NodeKind.Phrase, left, right)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(distance);

        Distance = distance;
    }

    internal override void WriteCore(StringBuilder sb, bool first = false)
    {
        // TODO: Figure out the nullability strategy here
        if (!first)
            sb.Append("( ");

        Left.WriteCore(sb);

        sb.Append(" <");
        if (Distance == 1) sb.Append("-");
        else sb.Append(Distance);
        sb.Append("> ");

        Right.WriteCore(sb);

        if (!first)
            sb.Append(" )");
    }

    /// <inheritdoc/>
    public override bool Equals(NpgsqlTsQuery? other)
        => other is NpgsqlTsQueryFollowedBy followedBy &&
           followedBy.Left == Left &&
           followedBy.Right == Right &&
           followedBy.Distance == Distance;

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Left, Right, Distance);
}

/// <summary>
/// Represents an empty tsquery. Should only be used as top node.
/// </summary>
public sealed class NpgsqlTsQueryEmpty : NpgsqlTsQuery
{
    /// <summary>
    /// Creates a tsquery that represents an empty query. Should not be used as child node.
    /// </summary>
    public NpgsqlTsQueryEmpty() : base(NodeKind.Empty) {}

    internal override void WriteCore(StringBuilder sb, bool first = false) { }

    /// <inheritdoc/>
    public override bool Equals(NpgsqlTsQuery? other)
        => other is NpgsqlTsQueryEmpty;

    /// <inheritdoc/>
    public override int GetHashCode()
        => Kind.GetHashCode();
}
