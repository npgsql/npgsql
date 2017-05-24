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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    public struct NpgsqlLsn : IEquatable<NpgsqlLsn>
    {
        public ulong Value { get; }

        public uint Upper => (uint)(Value / ((ulong)uint.MaxValue + 1));

        public uint Lower => (uint)(Value % ((ulong)uint.MaxValue + 1));

        public NpgsqlLsn(uint lower, uint upper)
        {
            Value = upper * ((ulong)uint.MaxValue + 1) + lower;
        }

        public static NpgsqlLsn Parse(string str)
        {
            if (!TryParse(str, out NpgsqlLsn value))
                throw new InvalidCastException($"The string \"{str}\" is not a valid log sequence number.");

            return value;
        }

        public static bool TryParse(string str, out NpgsqlLsn value)
        {
            if (string.IsNullOrEmpty(str))
            {
                value = default(NpgsqlLsn);
                return false;
            }

            uint lower = 0, upper = 0;
            var counter = 0;
            var readUpper = false;
            foreach (var ch in str)
            {
                uint hex;
                if (ch >= '0' && ch <= '9')
                {
                    hex = (uint)(ch - '0');
                }
                else if (ch >= 'a' && ch <= 'f')
                {
                    hex = (uint)(ch - 'a' + 10);
                }
                else if (ch >= 'A' && ch <= 'F')
                {
                    hex = (uint)(ch - 'A' + 10);
                }
                else if (ch == '/' && counter > 0)
                {
                    readUpper = true;
                    counter = 0;
                    continue;
                }
                else
                {
                    readUpper = false;
                    break;
                }

                if (counter >= 8)
                {
                    readUpper = false;
                    break;
                }

                if (readUpper)
                {
                    if (counter > 0)
                        upper <<= 4;
                    upper += hex;
                }
                else
                {
                    if (counter > 0)
                        lower <<= 4;
                    lower += hex;
                }

                ++counter;
            }

            if (readUpper && counter > 0)
            {
                value = new NpgsqlLsn(lower, upper);
                return true;
            }

            value = default(NpgsqlLsn);
            return false;
        }

        public bool Equals(NpgsqlLsn other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is NpgsqlLsn && Equals((NpgsqlLsn)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(NpgsqlLsn x, NpgsqlLsn y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(NpgsqlLsn x, NpgsqlLsn y)
        {
            return x.Value != y.Value;
        }
    }
}
