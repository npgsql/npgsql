#region License
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

#pragma warning disable CA1040, CA1034

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a PostgreSQL tsvector.
    /// </summary>
    public sealed class NpgsqlTsVector : IEnumerable<NpgsqlTsVector.Lexeme>
    {
        List<Lexeme> _lexemes;

        internal NpgsqlTsVector(List<Lexeme> lexemes, bool noCheck)
        {
            if (noCheck)
                _lexemes = lexemes;
            else
                Load(lexemes);
        }

        /// <summary>
        /// Constructs an NpgsqlTsVector from a list of lexemes. This also sorts and remove duplicates.
        /// </summary>
        /// <param name="lexemes"></param>
        public NpgsqlTsVector(List<Lexeme> lexemes)
        {
            Load(lexemes);
        }

        void Load(List<Lexeme> lexemes)
        {
            _lexemes = new List<Lexeme>(lexemes);

            if (_lexemes.Count == 0)
                return;

            // Culture-specific comparisons doesn't really matter for the backend. It's sorting on its own if it detects an unsorted collection.
            // Only when a .NET user wants to print the sort order.
            _lexemes.Sort((a, b) => a.Text.CompareTo(b.Text));

            var res = 0;
            var pos = 1;
            while (pos < _lexemes.Count)
            {
                if (_lexemes[pos].Text != _lexemes[res].Text)
                {
                    // We're done with this lexeme. First make sure the word pos list is sorted and contains unique elements.
                    _lexemes[res] = new Lexeme(_lexemes[res].Text, Lexeme.UniquePos(_lexemes[res]._wordEntryPositions), true);
                    res++;
                    if (res != pos)
                        _lexemes[res] = _lexemes[pos];
                }
                else
                {
                    // Just concatenate the word pos lists
                    if (_lexemes[res]._wordEntryPositions != null)
                    {
                        if (_lexemes[pos].Count > 0)
                            _lexemes[res]._wordEntryPositions.AddRange(_lexemes[pos]._wordEntryPositions);
                    }
                    else
                    {
                        _lexemes[res] = _lexemes[pos];
                    }
                }
                pos++;
            }

            // Last element
            _lexemes[res] = new Lexeme(_lexemes[res].Text, Lexeme.UniquePos(_lexemes[res]._wordEntryPositions), true);
            if (res != pos - 1)
            {
                _lexemes.RemoveRange(res, pos - 1 - res);
            }
        }

        /// <summary>
        /// Parses a tsvector in PostgreSQL's text format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NpgsqlTsVector Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var lexemes = new List<Lexeme>();
            var pos = 0;
            var wordPos = 0;
            var sb = new StringBuilder();
            List<Lexeme.WordEntryPos> wordEntryPositions;

            WaitWord:
            if (pos >= value.Length)
                goto Finish;
            if (char.IsWhiteSpace(value[pos]))
            {
                pos++;
                goto WaitWord;
            }
            sb.Clear();
            if (value[pos] == '\'')
            {
                pos++;
                goto WaitEndComplex;
            }
            if (value[pos] == '\\')
            {
                pos++;
                goto WaitNextChar;
            }
            sb.Append(value[pos++]);
            goto WaitEndWord;

            WaitNextChar:
            if (pos >= value.Length)
                throw new FormatException("Missing escaped character after \\ at end of value");
            sb.Append(value[pos++]);
            goto WaitEndWord;

            WaitEndWord:
            if (pos >= value.Length || char.IsWhiteSpace(value[pos]))
            {
                lexemes.Add(new Lexeme(sb.ToString()));
                if (pos >= value.Length)
                    goto Finish;
                pos++;
                goto WaitWord;
            }
            if (value[pos] == '\\')
            {
                pos++;
                goto WaitNextChar;
            }
            if (value[pos] == ':')
            {
                pos++;
                goto StartPosInfo;
            }
            sb.Append(value[pos++]);
            goto WaitEndWord;

            WaitEndComplex:
            if (pos >= value.Length)
                throw new FormatException("Unexpected end of value");
            if (value[pos] == '\'')
            {
                pos++;
                goto WaitCharComplex;
            }
            if (value[pos] == '\\')
            {
                pos++;
                if (pos >= value.Length)
                    throw new FormatException("Missing escaped character after \\ at end of value");
            }
            sb.Append(value[pos++]);
            goto WaitEndComplex;

            WaitCharComplex:
            if (pos < value.Length && value[pos] == '\'')
            {
                sb.Append('\'');
                pos++;
                goto WaitEndComplex;
            }
            if (pos < value.Length && value[pos] == ':')
            {
                pos++;
                goto StartPosInfo;
            }
            lexemes.Add(new Lexeme(sb.ToString()));
            goto WaitWord;

            StartPosInfo:
            wordEntryPositions = new List<Lexeme.WordEntryPos>();
            goto InPosInfo;

            InPosInfo:
            var digitPos = pos;
            while (pos < value.Length && value[pos] >= '0' && value[pos] <= '9')
                pos++;
            if (digitPos == pos)
                throw new FormatException("Missing length after :");
            wordPos = int.Parse(value.Substring(digitPos, pos - digitPos));
            goto WaitPosWeightOrDelim;

            // Note: PostgreSQL backend parser matches also for example 1DD2A, which is parsed into 1A, but not 1AA2D ...
            WaitPosWeightOrDelim:
            if (pos < value.Length)
            {
                if (value[pos] == 'A' || value[pos] == 'a' || value[pos] == '*') // Why * ?
                {
                    wordEntryPositions.Add(new Lexeme.WordEntryPos(wordPos, Lexeme.Weight.A));
                    pos++;
                    goto WaitPosDelim;
                }
                if (value[pos] >= 'B' && value[pos] <= 'D' || value[pos] >= 'b' && value[pos] <= 'd')
                {
                    var weight = value[pos];
                    if (weight >= 'b' && weight <= 'd')
                        weight = (char)(weight - ('b' - 'B'));
                    wordEntryPositions.Add(new Lexeme.WordEntryPos(wordPos, Lexeme.Weight.D + ('D' - weight)));
                    pos++;
                    goto WaitPosDelim;
                }
            }
            wordEntryPositions.Add(new Lexeme.WordEntryPos(wordPos));
            goto WaitPosDelim;

            WaitPosDelim:
            if (pos >= value.Length || char.IsWhiteSpace(value[pos]))
            {
                if (pos < value.Length)
                    pos++;
                lexemes.Add(new Lexeme(sb.ToString(), wordEntryPositions));
                goto WaitWord;
            }
            if (value[pos] == ',')
            {
                pos++;
                goto InPosInfo;
            }
            throw new FormatException("Missing comma, whitespace or end of value after lexeme pos info");

            Finish:
            return new NpgsqlTsVector(lexemes);
        }

        /// <summary>
        /// Returns the lexeme at a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Lexeme this[int index]
        {
            get
            {
                if (index < 0 || index >= _lexemes.Count)
                    throw new ArgumentException(nameof(index));

                return _lexemes[index];
            }
        }

        /// <summary>
        /// Gets the number of lexemes.
        /// </summary>
        public int Count => _lexemes.Count;

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Lexeme> GetEnumerator() => _lexemes.GetEnumerator();

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => _lexemes.GetEnumerator();

        /// <summary>
        /// Gets a string representation in PostgreSQL's format.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Join(" ", _lexemes);

        /// <summary>
        /// Represents a lexeme. A lexeme consists of a text string and optional word entry positions.
        /// </summary>
        public struct Lexeme : IEquatable<Lexeme>
        {
            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }

            [CanBeNull]
            internal List<WordEntryPos> _wordEntryPositions;

            /// <summary>
            /// Creates a lexeme with no word entry positions.
            /// </summary>
            /// <param name="text"></param>
            public Lexeme(string text)
            {
                Text = text;
                _wordEntryPositions = null;
            }

            /// <summary>
            /// Creates a lexeme with word entry positions.
            /// </summary>
            /// <param name="text"></param>
            /// <param name="wordEntryPositions"></param>
            public Lexeme(string text, [CanBeNull]List<WordEntryPos> wordEntryPositions)
                : this(text, wordEntryPositions, false) {}

            internal Lexeme(string text, [CanBeNull] List<WordEntryPos> wordEntryPositions, bool noCopy)
            {
                Text = text;
                if (wordEntryPositions != null)
                    _wordEntryPositions = noCopy ? wordEntryPositions : new List<WordEntryPos>(wordEntryPositions);
                else
                    _wordEntryPositions = null;
            }

            [CanBeNull]
            internal static List<WordEntryPos> UniquePos([CanBeNull] List<WordEntryPos> list)
            {
                if (list == null)
                    return null;
                var needsProcessing = false;
                for (var i = 1; i < list.Count; i++)
                {
                    if (list[i - 1].Pos >= list[i].Pos)
                    {
                        needsProcessing = true;
                        break;
                    }
                }
                if (!needsProcessing)
                    return list;

                // Don't change the original list, as the user might inspect it later if he holds a reference to the lexeme's list
                list = new List<WordEntryPos>(list);

                list.Sort((x, y) => x.Pos.CompareTo(y.Pos));

                var a = 0;
                for (var b = 1; b < list.Count; b++)
                {
                    if (list[a].Pos != list[b].Pos)
                    {
                        a++;
                        if (a != b)
                            list[a] = list[b];
                    }
                    else if (list[b].Weight > list[a].Weight)
                        list[a] = list[b];
                }
                if (a != list.Count - 1)
                {
                    list.RemoveRange(a, list.Count - 1 - a);
                }
                return list;
            }

            /// <summary>
            /// Gets a word entry position.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public WordEntryPos this[int index]
            {
                get
                {
                    if (index < 0 || _wordEntryPositions == null || index >= _wordEntryPositions.Count)
                        throw new ArgumentException(nameof(index));

                    return _wordEntryPositions[index];
                }
                internal set
                {
                    if (index < 0 || _wordEntryPositions == null || index >= _wordEntryPositions.Count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    _wordEntryPositions[index] = value;
                }
            }

            /// <summary>
            /// Gets the number of word entry positions.
            /// </summary>
            public int Count => _wordEntryPositions?.Count ?? 0;

            /// <summary>
            /// Creates a string representation in PostgreSQL's format.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                var str = '\'' + (Text ?? "").Replace(@"\", @"\\").Replace("'", "''") + '\'';
                if (Count > 0)
                    str += ":" + string.Join(",", _wordEntryPositions);
                return str;
            }

            /// <summary>
            /// Represents a word entry position and an optional weight.
            /// </summary>
            public struct WordEntryPos : IEquatable<WordEntryPos>
            {
                internal short Value { get; }

                internal WordEntryPos(short value)
                {
                    Value = value;
                }

                /// <summary>
                /// Creates a WordEntryPos with a given position and weight.
                /// </summary>
                /// <param name="pos">Position values can range from 1 to 16383; larger numbers are silently set to 16383.</param>
                /// <param name="weight">A weight labeled between A and D.</param>
                public WordEntryPos(int pos, Weight weight = Weight.D)
                {
                    if (pos == 0)
                        throw new ArgumentOutOfRangeException(nameof(pos), "Lexeme position is out of range. Min value is 1, max value is 2^14-1. Value was: " + pos);
                    if (weight < Weight.D || weight > Weight.A)
                        throw new ArgumentOutOfRangeException(nameof(weight));

                    // Per documentation: "Position values can range from 1 to 16383; larger numbers are silently set to 16383."
                    if (pos >> 14 != 0)
                        pos = (1 << 14) - 1;

                    Value = (short)(((int)weight << 14) | pos);
                }

                /// <summary>
                /// The weight is labeled from A to D. D is the default, and not printed.
                /// </summary>
                public Weight Weight => (Weight)((Value >> 14) & 3);

                /// <summary>
                /// The position is a 14-bit unsigned integer indicating the position in the text this lexeme occurs. Cannot be 0.
                /// </summary>
                public int Pos => Value & ((1 << 14) - 1);

                /// <summary>
                /// Prints this lexeme in PostgreSQL's format, i.e. position is followed by weight (weight is only printed if A, B or C).
                /// </summary>
                /// <returns></returns>
                public override string ToString()
                {
                    if (Weight != Weight.D)
                        return Pos + Weight.ToString();
                    return Pos.ToString();
                }

                /// <summary>
                /// Determines whether the specified object is equal to the current object.
                /// </summary>
                public bool Equals(WordEntryPos o) => Value == o.Value;

                /// <summary>
                /// Determines whether the specified object is equal to the current object.
                /// </summary>
                public override bool Equals([CanBeNull] object o)
                    => o is WordEntryPos && Equals((WordEntryPos)o);

                /// <summary>
                /// Gets a hash code for the current object.
                /// </summary>
                public override int GetHashCode() => Value.GetHashCode();

                /// <summary>
                /// Determines whether the specified object is equal to the current object.
                /// </summary>
                public static bool operator ==(WordEntryPos left, WordEntryPos right) => left.Equals(right);

                /// <summary>
                /// Determines whether the specified object is unequal to the current object.
                /// </summary>
                public static bool operator !=(WordEntryPos left, WordEntryPos right) => !left.Equals(right);
            }

            /// <summary>
            /// The weight is labeled from A to D. D is the default, and not printed.
            /// </summary>
            public enum Weight
            {
                /// <summary>
                /// D, the default
                /// </summary>
                D = 0,

                /// <summary>
                /// C
                /// </summary>
                C = 1,

                /// <summary>
                /// B
                /// </summary>
                B = 2,

                /// <summary>
                /// A
                /// </summary>
                A = 3
            }

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            public bool Equals(Lexeme o)
                => Text == o.Text &&
                    ((_wordEntryPositions == null && o._wordEntryPositions == null) ||
                    (_wordEntryPositions != null && _wordEntryPositions.Equals(o._wordEntryPositions)));

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            public override bool Equals([CanBeNull] object o)
                => o is Lexeme && Equals((Lexeme)o);

            /// <summary>
            /// Gets a hash code for the current object.
            /// </summary>
            public override int GetHashCode() => Text.GetHashCode();

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            public static bool operator ==(Lexeme left, Lexeme right) => left.Equals(right);

            /// <summary>
            /// Determines whether the specified object is unequal to the current object.
            /// </summary>
            public static bool operator !=(Lexeme left, Lexeme right) => !left.Equals(right);
        }
    }
}
