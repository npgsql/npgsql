// NpgsqlTypes\ArrayHandling.cs
//
// Author:
//    Jon Hanna. (jon@hackcraft.net)
//
//    Copyright (C) 2008 The Npgsql Development Team
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Keep the xml comment warning quiet for this file.
#pragma warning disable 1591

namespace NpgsqlTypes
{
    /// <summary>
    /// <para>Implements a bit string; a collection of zero or more bits which can each be 1 or 0.</para>
    /// <para>BitString's behave as a list of bools, though like most strings and unlike most collections the position
    /// tends to be of as much significance as the value.</para>
    /// <para>BitStrings are often used as masks, and are commonly cast to and from other values.</para>
    /// </summary>
    public struct BitString : IList<bool>, IEquatable<BitString>, IComparable<BitString>, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        /// Represents the empty string.
        /// </summary>
        public static readonly BitString Empty = new BitString(new List<uint>(0), 0);
        private readonly List<uint> _chunks;
        private readonly int _lastChunkLen;
        /// <summary>
        /// Create a BitString from an enumeration of boolean values. The BitString will contain
        /// those booleans in the order they came in.
        /// </summary>
        /// <param name="bits">The boolean values.</param>
        public BitString(IEnumerable<bool> bits)
        {
            _chunks = new List<uint>();
            int curChunkLen = 0;
            uint curChunk = 0;
            foreach(bool bit in bits)
            {
                curChunk = (curChunk << 1) | (bit ? 1u : 0u);
                if(++curChunkLen == 32)
                {
                    _chunks.Add(curChunk);
                    curChunk = 0;
                    curChunkLen = 0;
                }
            }
            if(curChunkLen != 0)
                _chunks.Add(curChunk << -curChunkLen);
            _lastChunkLen = curChunkLen;
        }
        //Used for optimised internal creation. The last chunk must be zero'd at bits less significant than lastChunkLen or comparisons will fail.
        private BitString(List<uint> chunks, int lastChunkLen)
        {
            _chunks = chunks;
            _lastChunkLen = lastChunkLen;
        }
        /// <summary>
        /// Creates a BitString filled with a given number of true or false values.
        /// </summary>
        /// <param name="value">The value to fill the string with.</param>
        /// <param name="count">The number of bits to fill.</param>
        public BitString(bool value, int count)
        {
            if(value)
            {
                _chunks = new List<uint>((count + 31) / 32);
                for(int i = 0; i < count / 32; ++i)
                    _chunks.Add(0xFFFFFFFFu);
                if(count % 32 != 0)
                    _chunks.Add(0xFFFFFFFFu << - count);
            }
            else
                _chunks = new List<uint>(new uint[(count + 31) / 32]);
            _lastChunkLen = count % 32;
        }
        /// <summary>
        /// Creats a bitstring from a <see cref="System.String">string</see>.
        /// <param name="str">The <see cref="System.String">string to copy from</see>.</param>
        /// <seealso cref="NpgsqlTypes.BitString.Parse(System.String)"/>
        /// </summary>
        public BitString(string str)
        {
            BitString fromParse = Parse(str);
            _chunks = fromParse._chunks;
            _lastChunkLen = fromParse._lastChunkLen;
        }
        /// <summary>
        /// Creates a single-bit element from a boolean value.
        /// </summary>
        /// <param name="boolean">The <see cref="System.Boolean">bool</see> value which determines whether
        /// the bit is 1 or 0.</param>
        public BitString(bool boolean)
            :this(boolean, 1){}
        /// <summary>
        /// Creates a bitstring from an unsigned integer value. The string will be the shortest required to
        /// contain the integer (e.g. 1 bit for 0 or 1, 2 for 2 or 3, 3 for 4-7, and so on).
        /// </summary>
        /// <param name="integer">The <see cref="System.UInt32">integer</see>.</param>
        /// <remarks>This method is not CLS Compliant, and may not be available to some languages.</remarks>
        [CLSCompliant(false)]
        public BitString(uint integer)
        {
            int bitCount = 32;
            while(bitCount >= 1 && (integer & 0x80000000u) == 0)
            {
                integer <<= 1;
                --bitCount;
            }
            _chunks = new List<uint>(1);
            _chunks.Add(integer);
            _lastChunkLen = bitCount;
        }
        /// <summary>
        /// Creates a bitstring from an integer value. The string will be the shortest required to
        /// contain the integer (e.g. 1 bit for 0 or 1, 2 for 2 or 3, 3 for 4-7, and so on).
        /// </summary>
        /// <param name="integer">The <see cref="System.Int32">integer</see>.</param>
        public BitString(int integer)
            :this((uint)integer){}
        private IEnumerable<uint> AllChunksButLast
        {
            get
            {
                for(int i = 0; i < _chunks.Count - 1; ++i)
                    yield return _chunks[i];
            }
        }
        private IEnumerable<uint> EnumChunks(bool includeLast)
        {
            return includeLast ? _chunks : AllChunksButLast;
        }
        /// <summary>
        /// The length of the string.
        /// </summary>
        public int Length
        {
            get
            {
                return (_chunks.Count - (_lastChunkLen == 0 ? 0 : 1)) * 32 + _lastChunkLen;
            }
        }
        /// <summary>
        /// Retrieves the value of the bit at the given index.
        /// </summary>
        public bool this[int index]
        {
            get
            {
                if(index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException();
                return (_chunks[index / 32] & (1 << (31 - index % 32))) != 0;
            }
        }
        bool IList<bool>.this[int idx]
        {
            get
            {
                return this[idx];
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        int ICollection<bool>.Count
        {
            get
            {
                return Length;
            }
        }
        bool ICollection<bool>.IsReadOnly
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Finds the first instance of a given value
        /// </summary>
        /// <param name="item">The value - whether true or false - to search for.</param>
        /// <returns>The index of the value found, or -1 if none are present.</returns>
        public int IndexOf(bool item)
        {
            if(item)
            {
                for(int chunkCount = 0; chunkCount != _chunks.Count; ++chunkCount)
                {
                    if(_chunks[chunkCount] != 0)
                    {
                        uint chunk = _chunks[chunkCount];
                        for(int i = 0; i != 32; ++i)
                        {
                            if((chunkCount & (0x80000000u >> i)) != 0)
                            {
                                return chunkCount * 32 + i;
                            }
                        }
                    }
                }
            }
            else
            {
                for(int chunkCount = 0; chunkCount != _chunks.Count; ++chunkCount)
                {
                    if(_chunks[chunkCount] != 0xFFFFFFFFu)
                    {
                        uint chunk = _chunks[chunkCount];
                        for(int i = 0; i != 32; ++i)
                        {
                            if((~chunkCount & (0x80000000u >> i)) != 0)
                            {
                                int ret = chunkCount * 32 + i;
                                return ret < Length ? ret : -1;
                            }
                        }
                    }
                }
            }
            return -1;
        }
        void IList<bool>.Insert(int index, bool item)
        {
            throw new NotSupportedException();
        }
        void IList<bool>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
        void ICollection<bool>.Add(bool item)
        {
            throw new NotSupportedException();
        }
        void ICollection<bool>.Clear()
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// True if there is at least one bit with the value looked for.
        /// </summary>
        /// <param name="item">The value - true or false - to detect.</param>
        /// <returns>True if at least one bit was the same as item, false otherwise.</returns>
        public bool Contains(bool item)
        {
            foreach(uint chunk in EnumChunks(item))//because last chunk is zero-filled in unused portion, it is safe to check it if searching for true
                if(item && (chunk != 0) || !item && chunk != 0xFFFFFFFFu)
                    return true;
            return !item && (_chunks[_chunks.Count - 1] & (0xFFFFFFFFu >> _lastChunkLen)) != 0;
        }
        /// <summary>
        /// Copies the bitstring to an array of bools.
        /// </summary>
        /// <param name="array">The <see cref="System.Boolean">boolean</see> array to copy to.</param>
        /// <param name="arrayIndex">The index in the array to start copying from.</param>
        public void CopyTo(bool[] array, int arrayIndex)
        {
            if(array == null)
                throw new ArgumentNullException();
            if(arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            if(array.Rank != 1
               || arrayIndex >= array.Length
               || arrayIndex + Length > array.Length)
                throw new ArgumentException();
            foreach(bool bit in this)
                array[arrayIndex++] = bit;
        }
        bool ICollection<bool>.Remove(bool item)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Returns an enumerator that enumerates through the string.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<bool> GetEnumerator()
        {
            if(_chunks.Count != 0)
            {
                foreach(uint chunk in EnumChunks(_lastChunkLen == 0))
                    for(int i = 31; i != -1; --i)
                        yield return (chunk & (1u << i)) != 0;
                uint lastChunk = _chunks[_chunks.Count - 1];
                for(int i = 31; i != 31 - _lastChunkLen; --i)
                    yield return (lastChunk & (1u << i)) != 0;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Creats a bitstring by concatenating another onto this one.
        /// </summary>
        /// <param name="append">The string to append to this one.</param>
        /// <returns>The combined strings.</returns>
        public BitString Concat(BitString append)
        {
            if(Length == 0)
                return append;
            else if(append.Length == 0)
                return this;
            else if(_lastChunkLen == 0)//Not only more efficient case with special handling, but also reasonably likely to appear in real use quite often.
            {
                List<uint> chunks = new List<uint>(_chunks);//Note that this copies, doesn't share.
                chunks.AddRange(append._chunks);
                return new BitString(chunks, append._lastChunkLen);
            }
            else
            {
                List<uint> chunks = new List<uint>(AllChunksButLast);
                chunks.Add(_chunks[_chunks.Count - 1] | (append._chunks[0] >> _lastChunkLen));
                for(int idx = 1; idx != append._chunks.Count; ++idx)
                {
                    chunks.Add((append._chunks[idx - 1] << -_lastChunkLen) | (append._chunks[idx] >> _lastChunkLen));
                }
                chunks.Add(append._chunks[append._chunks.Count - 1] << -_lastChunkLen);
                while(chunks.Count > (Length + append.Length + 31) / 32)
                    chunks.RemoveAt(chunks.Count - 1);
                return new BitString(chunks, (_lastChunkLen + append._lastChunkLen) % 32);
            }
        }
        /// <summary>
        /// Returns a substring of this string.
        /// </summary>
        /// <param name="start">The position to start from, must be between 0 and the length of the string.</param>
        /// <param name="length">The length of the string to return, must be greater than zero, and may not be
        /// so large that the start + length exceeds the bounds of this instance.</param>
        /// <returns>The Bitstring identified</returns>
        public BitString Substring(int start, int length)
        {
            if(start < 0 || length < 0 || start + length > Length)
                throw new ArgumentOutOfRangeException();
            else if(length == 0)
                return Empty;
            else if(start == 0 && length == Length)
                return this;
            else if(start % 32 == 0)
            {
                List<uint> chunks = _chunks.GetRange(start / 32, (length + 31) / 32);
                if(length % 32 != 0)
                    chunks[chunks.Count - 1] = chunks[chunks.Count -1] & (0xFFFFFFFFu << -length);
                return new BitString(chunks, length % 32);
            }
            else
            {
                List<uint> chunks = new List<uint>();
                for(int i = start / 32 + 1; i < (start + length) / 32 + 1; ++i)
                    chunks.Add((_chunks[i - 1] << start) | (_chunks[i] >> -start));
                if(length % 32 != 0 && chunks.Count < length / 32 + 1)
                    chunks.Add((_chunks[(start + length - 1) / 32] << start) & (0xFFFFFFFFu << -length));
                return new BitString(chunks, length % 32);
            }
        }
        /// <summary>
        /// Returns a substring of this string.
        /// </summary>
        /// <param name="start">The position to start from, must be between 0 and the length of the string,
        /// the rest of the string is returned.</param>
        /// <returns>The Bitstring identified</returns>
        public BitString Substring(int start)
        {
            return Substring(start, Length - start);
        }
        /// <summary>
        /// A logical and between this string and another. The two strings must be the same length.
        /// </summary>
        /// <param name="operand">Another BitString to AND with this one.</param>
        /// <returns>A bitstring with 1 where both BitStrings had 1 and 0 otherwise.</returns>
        public BitString And(BitString operand)
        {
            if(_lastChunkLen != operand._lastChunkLen || _chunks.Count != operand._chunks.Count)
                throw new ArgumentException("Cannot AND bitstrings of different sizes");
            List<uint> chunks = new List<uint>(_chunks.Count);
            for(int i = 0; i != _chunks.Count; ++i)
                chunks.Add(_chunks[i] & operand._chunks[i]);
            return new BitString(chunks, _lastChunkLen);
        }
        /// <summary>
        /// A logical or between this string and another. The two strings must be the same length.
        /// </summary>
        /// <param name="operand">Another BitString to OR with this one.</param>
        /// <returns>A bitstring with 1 where either BitString had 1 and 0 otherwise.</returns>
        public BitString Or(BitString operand)
        {
            if(_lastChunkLen != operand._lastChunkLen || _chunks.Count != operand._chunks.Count)
                throw new ArgumentException("Cannot OR bitstrings of different sizes");
            List<uint> chunks = new List<uint>(_chunks.Count);
            for(int i = 0; i != _chunks.Count; ++i)
                chunks.Add(_chunks[i] | operand._chunks[i]);
            return new BitString(chunks, _lastChunkLen);
        }
        /// <summary>
        /// A logical xor between this string and another. The two strings must be the same length.
        /// </summary>
        /// <param name="operand">Another BitString to XOR with this one.</param>
        /// <returns>A bitstring with 1 where one BitStrings and the other had 0,
        /// and 0 where they both had 1 or both had 0.</returns>
        public BitString Xor(BitString operand)
        {
            if(_lastChunkLen != operand._lastChunkLen || _chunks.Count != operand._chunks.Count)
                throw new ArgumentException("Cannot XOR bitstrings of different sizes");
            List<uint> chunks = new List<uint>(_chunks.Count);
            for(int i = 0; i != _chunks.Count; ++i)
                chunks.Add(_chunks[i] ^ operand._chunks[i]);
            return new BitString(chunks, _lastChunkLen);
        }
        /// <summary>
        /// A bitstring that is the logical inverse of this one.
        /// </summary>
        /// <returns>A bitstring of the same length as this with 1 where this has 0 and vice-versa.</returns>
        public BitString Not()
        {
            List<uint> chunks = new List<uint>(_chunks.Count);
            foreach(uint chunk in _chunks)
                chunks.Add(~chunk);
            chunks[chunks.Count - 1] = chunks[chunks.Count - 1] & (0xFFFFFFFFu << -_lastChunkLen);
            return new BitString(chunks, _lastChunkLen);
        }
        /// <summary>
        /// Shifts the string operand bits to the left, filling with zeros to produce a
        /// string of the same length.
        /// </summary>
        /// <param name="operand">The number of bits to shift to the left.</param>
        /// <returns>A left-shifted bitstring.</returns>
        /// <remarks><para>The behaviour of LShift is closer to what one would expect from dealing
        /// with PostgreSQL bit-strings than in using the same operations on integers in .NET</para>
        /// <para>In particular, negative operands result in a right-shift, and operands greater than
        /// the length of the string will shift it entirely, resulting in a zero-filled string.</para>
        /// </remarks>
        public BitString LShift(int operand)
        {
            if(_chunks.Count == 0)
                return Empty;
            else if(operand < 0)
                return RShift(-operand);
            else if(operand == 0)
                return this;
            else if(operand >= Length)
                return new BitString(false, Length);
            else if(operand % 32 == 0)
            {
                List<uint> chunks = _chunks.GetRange(operand / 32, (Length - operand) / 32);
                chunks.AddRange(new uint[_chunks.Count - chunks.Count]);
                return new BitString(chunks, _lastChunkLen);
            }
            else
            {
                List<uint> chunks = new List<uint>();
                for(int idx = (operand + 31) / 32; idx != _chunks.Count; ++idx)
                    chunks.Add((_chunks[idx - 1] << operand) | (_chunks[idx] >> -operand));
                chunks.Add(_chunks[_chunks.Count - 1] << operand);
                chunks.AddRange(new uint[_chunks.Count - chunks.Count]);
                return new BitString(chunks, _lastChunkLen);
            }
        }
        /// <summary>
        /// Shifts the string operand bits to the right, filling with zeros to produce a
        /// string of the same length.
        /// </summary>
        /// <param name="operand">The number of bits to shift to the right.</param>
        /// <returns>A right-shifted bitstring.</returns>
        /// <remarks><para>The behaviour of RShift is closer to what one would expect from dealing
        /// with PostgreSQL bit-strings than in using the same operations on integers in .NET</para>
        /// <para>In particular, negative operands result in a left-shift, and operands greater than
        /// the length of the string will shift it entirely, resulting in a zero-filled string. It also performs
        /// a logical shift, rather than an arithmetic shift, so it always sets the vacated bit positions to zero
        /// (like PostgreSQL and like .NET for unsigned integers but not for signed integers).</para>
        /// </remarks>
        public BitString RShift(int operand)
        {
            if(_chunks.Count == 0)
                return Empty;
            else if(operand < 0)
                return LShift(-operand);
            else if(operand == 0)
                return this;
            else if(operand >= Length)
                return new BitString(false, Length);
            else if(operand % 32 == 0)
            {
                List<uint> chunks = _chunks.GetRange(0, _chunks.Count - operand / 32);
                chunks.InsertRange(0, new uint[_chunks.Count - chunks.Count]);
                return new BitString(chunks, _lastChunkLen);
            }
            else
            {
                List<uint> chunks = new List<uint>();
                chunks.Add(_chunks[0] >> operand);
                for(int idx = 0; idx != (_chunks.Count - operand /32) - 1; ++idx)
                    chunks.Add((_chunks[idx] << -operand) | (_chunks[idx + 1] >> operand));
                chunks.InsertRange(0, new uint[_chunks.Count - chunks.Count]);
                return new BitString(chunks, _lastChunkLen);
            }
        }
        /// <summary>
        /// Returns true if the this string is identical to the argument passed.
        /// </summary>
        public bool Equals(BitString other)
        {
            if(null == (object)other)
                return false;
            if(ReferenceEquals(_chunks, other._chunks))//short cut on shallow copies
                return true;
            if(_lastChunkLen != other._lastChunkLen || _chunks.Count != other._chunks.Count)
                return false;
            for(int i = 0; i != _chunks.Count; ++i)
                if(_chunks[i] != other._chunks[i])
                    return false;
            return true;
        }
        /// <summary>
        /// Compares two strings. Strings are compared as strings, so while 0 being less than 1 will
        /// mean a comparison between two strings of the same size is the same as treating them as numbers,
        /// in the case of two strings of differing lengths the comparison starts at the right-most (most significant)
        /// bit, and if all bits of the shorter string are exhausted without finding a comparison, then the larger
        /// string is deemed to be greater than the shorter (0010 is greater than 0001 but less than 00100).
        /// </summary>
        /// <param name="other">Another string to compare with this one.</param>
        /// <returns>A value if the two strings are identical, an integer less
        /// than zero if this is less than the argument, and an integer greater
        /// than zero otherwise.</returns>
        public int CompareTo(BitString other)
        {
            if(null == (object)other)
                return 1;
            int endAt = Math.Min(_chunks.Count, other._chunks.Count);
            int cmp = 0;
            for(int i = 0; i != endAt; ++i)
                if((cmp = _chunks[i].CompareTo(other._chunks[i])) != 0)
                   return cmp;
            return _lastChunkLen.CompareTo(other._lastChunkLen);
        }
        /// <summary>
        /// Compares the string with another object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>If the object is null then this string is considered greater. If the object is another BitString
        /// then they are compared as in <see cref="CompareTo(NpgsqlTypes.BitString)">the explicit comparison for BitStrings</see>
        /// in any other case a <see cref="System.ArgumentException"/> is thrown.</returns>
        public int CompareTo(object obj)
        {
            if(null == obj)
                return 1;
            if(!(obj is BitString))
                throw new ArgumentException();
            return CompareTo((BitString)obj);
        }
        /// <summary>
        /// Compares this BitString with an object for equality.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is BitString && Equals((BitString)obj);
        }
        /// <summary>
        /// Returns a code for use in hashing operations.
        /// </summary>
        public override int GetHashCode()
        {
            int ret = _lastChunkLen;
            //The ideal amount to shift each value is one that would evenly spread it throughout
            //the resultant bytes. Using the current result % 32 is essentially using a random value
            //but one that will be the same on subsequent calls.
            foreach(uint chunk in _chunks)
                ret ^= Npgsql.PGUtil.RotateShift((int)chunk, ret % 32);
            return ret;
        }
        private StringBuilder BFormatString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(bool bit in this)
                sb.Append(bit ? '1' : '0');
            return sb;
        }
        private StringBuilder XFormatString(bool upperCase, bool ignoreTrailingBits)
        {
            if(!ignoreTrailingBits && _lastChunkLen % 4 != 0)
                throw new FormatException();
            StringBuilder sb = new StringBuilder();
            foreach(int chunk in _chunks)
                sb.Append(chunk.ToString(upperCase ? "X8" : "x8"));
            if(_lastChunkLen != 0)
                sb.Length -= (32 - _lastChunkLen + 3) / 4;
            return sb;
        }
        private static StringBuilder ZeroPad(StringBuilder str, int padTo)
        {
            int padBy = padTo - str.Length;
            while(padBy-- > 0)
                str.Insert(0, '0');
            return str;
        }
        /// <summary>
        /// Returns a string representation of the BitString.
        /// </summary>
        /// <param name="format">
        /// A string which can contain a letter and optionally a number which sets a minimum size for the string
        /// returned. In each case using the lower-case form of the letter will result in a lower-case string
        /// being returned.
        /// <list type="table">
        /// <item>
        /// <term>B</term>
        /// <description>A string of 1s and 0s.</description>
        /// </item>
        /// <item>
        /// <term>X</term>
        /// <description>An hexadecimal string (will result in an error unless the string's length is divisible by 4).</description>
        /// </item>
        /// <item>
        /// <term>G</term>
        /// <description>A string of 1s and 0s in single-quotes preceded by 'B' (Postgres bit string literal syntax).</description>
        /// </item>
        /// <term>Y</term>
        /// <description>An hexadecimal string in single-quotes preceded by 'X' (Postgres bit literal syntax, will result in an error unless the string's length is divisible by 4.</description>
        /// </list>
        /// <term>C</term>
        /// <description>The format produced by format-string "Y" if legal, otherwise that produced by format-string "G".</description>
        /// <term>E</term>
        /// <description>The most compact safe representation for Postgres. If single bit will be either a 0 or a 1. Otherwise if it
        /// can be that produce by format string "Y" it will, otherwise if there are less than 9bits in length it will be that
        /// produced by format-string "G". For longer strings that cannot be represented in hexadecimal it will be a string
        /// representing the first part of the string in format "Y" followed by the PostgreSQL concatenation operator, followed
        /// by the final bits in the format "G". E.g. "X'13DCE'||B'110'"</description>
        /// If format is empty or null, it is treated as if "B" had been passed (the default repreesentation, and that
        /// generally used by PostgreSQL for display).
        /// </param>
        /// <returns>The formatted string.</returns>
        public string ToString(string format)
        {
            format = string.IsNullOrEmpty(format) ? "B" : format.Trim();
            int padTo = int.Parse("0" + format.Substring(1));
            switch(format[0])
            {
                case 'b': case 'B':
                    return ZeroPad(BFormatString(), padTo).ToString();
                case 'x':
                    return ZeroPad(XFormatString(false, false), padTo).ToString();
                case 'X':
                    return ZeroPad(XFormatString(true, false), padTo).ToString();
                case 'g':
                    return ZeroPad(BFormatString(), padTo).Insert(0, "b'").Append('\'').ToString();
                case 'G':
                    return ZeroPad(BFormatString(), padTo).Insert(0, "B'").Append('\'').ToString();
                case 'y':
                    return ZeroPad(XFormatString(false, false), padTo).Insert(0, "x'").Append('\'').ToString();
                case 'Y':
                    return ZeroPad(XFormatString(true, false), padTo).Insert(0, "X'").Append('\'').ToString();
                case 'c':
                    return ToString((_lastChunkLen %4 == 0 ? "y" : "g") + padTo.ToString());
                case 'C':
                    return ToString((_lastChunkLen %4 == 0 ? "Y" : "G") + padTo.ToString());
                case 'e':
                    return ToString("E" + padTo.ToString()).ToLowerInvariant();
                case 'E':
                    if(_lastChunkLen == 1 && _chunks.Count == 1)
                        return (_chunks[0] & 0x80000000u) == 0 ? "0" : "1";//both safe in this case for all lengths, and allows for some backwards compatibility from threating bit(1) as if it were boolean.
                    else if(_lastChunkLen % 4 == 0)
                        return ToString("Y" + padTo.ToString());
                    else if(Length < 9)
                        return ToString("G" + padTo.ToString());
                    else
                    {
                        StringBuilder sb = XFormatString(true, true).Insert(0, "X'");
                        sb.Append("\'||B\'");
                        uint lastNibble =  _chunks[_chunks.Count - 1] << (_lastChunkLen / 4 * 4);
                        for(int i = 0; i < _lastChunkLen % 4; ++i)
                        {
                            uint mask = 0x80000000u >> i;
                            sb.Append((lastNibble & mask) != 0 ? '1' : '0');
                        }
                        return sb.Append('\'').ToString();
                    }
                default:
                    throw new FormatException();
            }
        }
        /// <summary>
        /// Returns a string representation for the Bitstring
        /// </summary>
        /// <returns>A string containing '0' and '1' characters.</returns>
        public override string ToString()
        {
            return ToString("B");
        }
        /// <summary>
        /// Returns the same string as <see cref="ToString(System.String)"/>. formatProvider is ignored.
        /// </summary>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(format);
        }
        private static IEnumerable<bool> ReadBinary(TextReader tr)
        {
            for(;;)
                switch(tr.Peek())
                {
                    case -1: case (int)'\'':
                        yield break;
                    case (int)'0':
                        tr.Read();
                        yield return false;
                        break;
                    case (int)'1':
                        tr.Read();
                        yield return true;
                        break;
                    default:
                        throw new FormatException();
                }
        }
        private static IEnumerable<uint> ReadHexNibbles(TextReader tr)
        {
            for(;;)
                switch(tr.Peek())
                {
                    case -1: case (int)'\'':
                        yield break;
                    case (int)'0': case (int)'1': case (int)'2': case (int)'3':
                    case (int)'4': case (int)'5': case (int)'6': case (int)'7':
                    case (int)'8': case (int)'9':
                        yield return (uint)tr.Read() - '0';
                        break;
                    case (int)'a': case (int)'b': case (int)'c': case (int)'d':
                    case (int)'e': case (int)'f':
                        yield return (uint)tr.Read() - 'a' + 10;
                        break;
                    case (int)'A': case (int)'B': case (int)'C': case (int)'D':
                    case (int)'E': case (int)'F':
                        yield return (uint)tr.Read() - 'A' + 10;
                        break;
                    default:
                        throw new FormatException();
                }
        }
        private static BitString Parse(TextReader tr)
        {
            List<BitString> parts = new List<BitString>();
            for(;;)
                switch(tr.Peek())
                {
                    case (int)'0': case (int)'1':
                        parts.Add(new BitString(ReadBinary(tr)));
                        break;
                    case (int)'x': case (int)'X':
                        tr.Read();
                        if(tr.Read() != (int)'\'')
                            throw new FormatException();
                        int nibbleCount = 0;
                        uint currentChunk = 0;
                        List<uint> chunks = new List<uint>();
                        foreach(uint nibble in ReadHexNibbles(tr))
                        {
                            currentChunk = currentChunk << 4 | nibble;
                            if(++nibbleCount == 8)
                            {
                                chunks.Add(currentChunk);
                                nibbleCount = 0;
                            }
                        }
                        if(nibbleCount != 0)
                            chunks.Add(currentChunk << 32 - nibbleCount * 4);
                        parts.Add(new BitString(chunks, nibbleCount * 4));
                        break;
                    case (int)'\'': case (int)'b': case (int)'B': case (int)'|':
                    case (int)' ': case (int)'\n': case (int)'\r': case '\t':
                        tr.Read();
                        break;
                    case -1:
                        switch(parts.Count)
                        {
                            case 0:
                                return Empty;
                            case 1:
                                return parts[0];
                            default:
                                BitString accum = parts[0];
                                for(int i = 1; i != parts.Count; ++i)
                                    accum = accum.Concat(parts[i]);
                                return accum;
                        }
                    default:
                        throw new FormatException();
                }
        }
        /// <summary>
        /// Parses a string to produce a BitString. Most formats that can be produced by
        /// <see cref="ToString(System.String)"/> can be accepted, but hexadecimal
        /// can be interpreted with the preceding X' to mark the following characters as
        /// being hexadecimal rather than binary.
        /// </summary>
        public static BitString Parse(string text)
        {
            using(StringReader sr = new StringReader(text))
                return Parse(sr);
        }
        /// <summary>
        /// Performs a logical AND on the two operands.
        /// </summary>
        public static BitString operator&(BitString x, BitString y)
        {
            return x.And(y);
        }
        /// <summary>
        /// Performs a logcial OR on the two operands.
        /// </summary>
        public static BitString operator|(BitString x, BitString y)
        {
            return x.Or(y);
        }
        /// <summary>
        /// Perofrms a logical EXCLUSIVE-OR on the two operands
        /// </summary>
        public static BitString operator^(BitString x, BitString y)
        {
            return x.Xor(y);
        }
        /// <summary>
        /// Performs a logical NOT on the operand.
        /// </summary>
        public static BitString operator~(BitString x)
        {
            return x.Not();
        }
        /// <summary>
        /// Concatenates the operands.
        /// </summary>
        public static BitString operator+(BitString x, BitString y)
        {
            return x.Concat(y);
        }
        /// <summary>
        /// Left-shifts the string BitString.
        /// </summary>
        public static BitString operator<<(BitString bs, int shift)
        {
            return bs.LShift(shift);
        }
        /// <summary>
        /// Right-shifts the string BitString.
        /// </summary>
        public static BitString operator>>(BitString bs, int shift)
        {
            return bs.RShift(shift);
        }
        /// <summary>
        /// Compares the two operands.
        /// </summary>
        public static bool operator==(BitString x, BitString y)
        {
            return x.Equals(y);
        }
        /// <summary>
        /// Compares the two operands.
        /// </summary>
        public static bool operator!=(BitString x, BitString y)
        {
            return !x.Equals(y);
        }
        /// <summary>
        /// Compares the two operands.
        /// </summary>
        public static bool operator<(BitString x, BitString y)
        {
            return x.CompareTo(y) < 0;
        }
        /// <summary>
        /// Compares the two operands.
        /// </summary>
        public static bool operator>(BitString x, BitString y)
        {
            return x.CompareTo(y) > 0;
        }
        /// <summary>
        /// Compares the two operands.
        /// </summary>
        public static bool operator<=(BitString x, BitString y)
        {
            return x.CompareTo(y) <= 0;
        }
        /// <summary>
        /// Compares the two operands.
        /// </summary>
        public static bool operator>=(BitString x, BitString y)
        {
            return x.CompareTo(y) >= 0;
        }
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }
        public bool ToBoolean()
        {
            if(Length != 1)
                throw new InvalidCastException();
            return _chunks[0] != 0;
        }
        public bool ToBoolean(IFormatProvider provider)
        {
            return ToBoolean();
        }
        char IConvertible.ToChar(IFormatProvider provider)
        {
            //To a char in what character encoding?
            //If we insist on UTF-8 as a reasonable choice for most modern code, what
            //do we do with surrogate pairs?
            //
            //In all, there's no single reasonable approach to this.
            throw new NotSupportedException();
        }
        [CLSCompliant(false)]
        public sbyte ToSByte()
        {
            return (sbyte)ToByte();
        }
        [CLSCompliant(false)]
        public sbyte ToSByte(IFormatProvider provider)
        {
            return ToSByte();
        }
        public byte ToByte()
        {
            if(_lastChunkLen > 8)
                throw new InvalidCastException();
            return (byte)(ToUInt32() & 0x000000FFu);
        }
        public byte ToByte(IFormatProvider provider)
        {
            return ToByte();
        }
        public short ToInt16()
        {
            return (short)ToUInt16();
        }
        public short ToInt16(IFormatProvider provider)
        {
            return ToInt16();
        }
        [CLSCompliant(false)]
        public ushort ToUInt16()
        {
            if(_lastChunkLen > 16)
                throw new InvalidCastException();
            return (ushort)(ToUInt32() & 0x0000FFFFu);
        }
        [CLSCompliant(false)]
        public ushort ToUInt16(IFormatProvider provider)
        {
            return ToUInt16();
        }
        public int ToInt32()
        {
            return (int)ToUInt32();
        }
        public int ToInt32(IFormatProvider provider)
        {
            return ToInt32();
        }
        [CLSCompliant(false)]
        public uint ToUInt32()
        {
            switch(_chunks.Count)
            {
                case 0:
                    return 0;
                case 1:
                    return _chunks[0] >> - _lastChunkLen;
                default:
                    throw new InvalidCastException();
            }
        }
        [CLSCompliant(false)]
        public uint ToUInt32(IFormatProvider provider)
        {
            return ToUInt32();
        }
        public long ToInt64()
        {
            return (long)ToUInt64();
        }
        public long ToInt64(IFormatProvider provider)
        {
            return ToInt64();
        }
        [CLSCompliant(false)]
        public ulong ToUInt64()
        {
            switch(_chunks.Count)
            {
                case 0: case 1:
                    return ToUInt32();
                case 2:
                    return (((ulong)_chunks[0]) << 32 | (ulong)_chunks[1]) >> 32 -_lastChunkLen;
                default:
                    throw new InvalidCastException();
            }
        }
        [CLSCompliant(false)]
        public ulong ToUInt64(IFormatProvider provider)
        {
            return ToUInt64();
        }
        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }
        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }
        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }
        public string ToString(IFormatProvider provider)
        {
            return ToString();
        }
        /// <summary>
        /// Interprets the bitstring as a series of bits in an encoded character string,
        /// encoded according to the Encoding passed, and returns that string.
        /// The bitstring must contain a whole number of octets(bytes) and also be
        /// valid according to the Encoding passed.
        /// </summary>
        /// <param name="encoding">The <see cref="System.Text.Encoding"/> to use in producing the string.</param>
        /// <returns>The string that was encoded in the BitString.</returns>
        public string ToString(Encoding encoding)
        {
            return encoding.GetString(new List<byte>(ToByteEnumerable()).ToArray());
        }
        /// <summary>
        /// Interprets the bitstring as a series of octets (bytes) and returns those octets. Fails
        /// if the Bitstring does not contain a whole number of octets (its length is not evenly
        /// divisible by 8).
        /// </summary>
        public IEnumerable<byte> ToByteEnumerable()
        {
            if(_lastChunkLen % 8 != 0)
                throw new InvalidCastException();
            foreach(uint chunk in EnumChunks(_lastChunkLen == 0))
                for(int i = 24; i != -8; i -= 8)
                    yield return (byte)(chunk >> i & 0x000000FFu);
            for(int i = 24; i > 24 - _lastChunkLen; i -=8)
                yield return (byte)(_chunks[_chunks.Count - 1] >> i);
        }
        /// <summary>
        /// Interprets the bitstring as a series of signed octets (bytes) and returns those octets. Fails
        /// if the Bitstring does not contain a whole number of octets (its length is not evenly
        /// divisible by 8).
        /// <remarks>This method is not CLS-Compliant and may not be available to languages that cannot
        /// handle signed bytes.</remarks>
        /// </summary>
        [CLSCompliant(false)]
        public IEnumerable<sbyte> ToSByteEnumerable()
        {
            foreach(byte b in ToByteEnumerable())
                yield return (sbyte)b;
        }
        /// <summary>
        /// Interprets the bitstring as a series of unsigned 16-bit integers and returns those integers.
        /// Fails if the Bitstring's length is not evenly divisible by 16.
        /// <remarks>This method is not CLS-Compliant and may not be available to languages that cannot
        /// handle unsigned integers.</remarks>
        /// </summary>
        [CLSCompliant(false)]
        public IEnumerable<ushort> ToUInt16Enumerable()
        {
            if(_lastChunkLen % 16 != 0)
                throw new InvalidCastException();
            foreach(uint chunk in EnumChunks(_lastChunkLen == 0))
            {
                yield return (ushort)(chunk >> 16);
                yield return (ushort)(chunk & 0xFFFF);
            }
            if(_lastChunkLen == 16)
                yield return (ushort)(_chunks[_chunks.Count] >> 16);
        }
        /// <summary>
        /// Interprets the bitstring as a series of 16-bit integers and returns those integers.
        /// Fails if the Bitstring's length is not evenly divisible by 16.
        /// </summary>
        public IEnumerable<short> ToInt16Enumerable()
        {
            foreach(ushort us in ToUInt16Enumerable())
                yield return (short)us;
        }
        /// <summary>
        /// Interprets the bitstring as a series of unsigned 32-bit integers and returns those integers.
        /// Fails if the Bitstring's length is not evenly divisible by 32.
        /// <remarks>This method is not CLS-Compliant and may not be available to languages that cannot
        /// handle unsigned integers.</remarks>
        /// </summary>
        [CLSCompliant(false)]
        public IEnumerable<uint> ToUInt32Enumerable()
        {
            if(_lastChunkLen != 0)
                throw new InvalidCastException();
            return _chunks;
        }
        /// <summary>
        /// Interprets the bitstring as a series of signed 32-bit integers and returns those integers.
        /// Fails if the Bitstring's length is not evenly divisible by 32.
        /// </summary>
        public IEnumerable<int> ToInt32Enumerable()
        {
            foreach(uint ui in ToUInt32Enumerable())
                yield return (int)ui;
        }
        /// <summary>
        /// Interprets the bitstring as a series of unsigned 64-bit integers and returns those integers.
        /// Fails if the Bitstring's length is not evenly divisible by 64.
        /// <remarks>This method is not CLS-Compliant and may not be available to languages that cannot
        /// handle unsigned integers.</remarks>
        /// </summary>
        [CLSCompliant(false)]
        public IEnumerable<ulong> ToUInt64Enumerable()
        {
            if(_lastChunkLen != 0 || _chunks.Count % 2 != 0)
                throw new InvalidCastException();
            for(int i = 0; i != _chunks.Count; i += 2)
                yield return (ulong)_chunks[i] << 32 | (ulong)_chunks[i + 1];
        }
        /// <summary>
        /// Interprets the bitstring as a series of signed 64-bit integers and returns those integers.
        /// Fails if the Bitstring's length is not evenly divisible by 64.
        /// </summary>
        public IEnumerable<long> ToInt64Enumerable()
        {
            foreach(ulong ul in ToUInt64Enumerable())
                yield return (long)ul;
        }
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            switch(Type.GetTypeCode(conversionType))
            {
                case TypeCode.Boolean:
                    return ToBoolean();
                case TypeCode.Byte:
                    return ToByte();
                case TypeCode.DBNull:
                    return System.DBNull.Value;
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return ToInt16();
                case TypeCode.Int32:
                    return ToInt32();
                case TypeCode.Int64:
                    return ToInt64();
                case TypeCode.SByte:
                    return ToSByte();
                case TypeCode.String:
                    return ToString();
                case TypeCode.UInt16:
                    return ToUInt16();
                case TypeCode.UInt32:
                    return ToUInt32();
                case TypeCode.UInt64:
                    return ToUInt64();
                case TypeCode.Object:
                    if(conversionType == typeof(BitString))
                        return this;
                    //Should fill this out a bit to cover the IEnumerable<T> interfaces.
                    throw new InvalidCastException();
                default:
                    throw new InvalidCastException();
            }
        }
     }
}

#pragma warning restore 1591
