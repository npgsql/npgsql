// NpgsqlTypes.FastConverter.cs
//
// Author:
//    Maxim Rylov
//
//    Copyright (C) 2013 The Npgsql Development Team
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
using System.IO;

namespace NpgsqlTypes
{
    internal class FastConverter
    {
        private static readonly byte[] HexFormatLookupTableLow = new byte[] {
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
        };
        private static readonly byte[] HexFormatLookupTableHigh = new byte[] {
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
        };
        private static readonly byte[] EscapeFormatLookupTable = new byte[] {
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
          0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
          0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff
        };

        private static byte LookupLowHexFormat(char c)
        {
            var b = HexFormatLookupTableLow[c];
            if (b == 255)
                throw new IOException("Expected a hex character, got " + c);
            return b;
        }

        private static byte LookupHighHexFormat(char c)
        {
            var b = HexFormatLookupTableHigh[c];
            if (b == 255)
                throw new IOException("Expected a hex character, got " + c);
            return b;
        }

        public static byte ToByteHexFormat(string str, int index)
        {
            byte b1 = HexFormatLookupTableHigh[str[index++]];
            if (b1 == 255)
                throw new IOException("Expected a hex character, got " + 'c');

            byte b2 = HexFormatLookupTableLow[str[index]];
            if (b2 == 255)
                throw new IOException("Expected a hex character, got " + 'c');

            return (byte)(b1 | b2);
        }


        public static byte ToByteHexFormat(char[] str, int index)
        {
            byte b1 = HexFormatLookupTableHigh[str[index++]];
            if (b1 == 255)
                throw new IOException("Expected a hex character, got " + 'c');

            byte b2 = HexFormatLookupTableLow[str[index]];
            if (b2 == 255)
                throw new IOException("Expected a hex character, got " + 'c');

            return (byte)(b1 | b2);
        }

        public static byte ToByteHexFormat(byte[] str, int index)
        {
            byte b1 = HexFormatLookupTableHigh[str[index]];
            if (b1 == 255)
                throw new IOException(string.Format("Expected a hex character, got {0}", str[index]));

            byte b2 = HexFormatLookupTableLow[str[++index]];
            if (b2 == 255)
                throw new IOException(string.Format("Expected a hex character, got {0}", str[index]));

            return (byte)(b1 | b2);
        }

        public static byte ToByteEscapeFormat(byte[] str, int index)
        {
            byte b1 = (byte)(EscapeFormatLookupTable[str[index]] << 6);
            if (b1 == 255)
                throw new IOException(string.Format("Expected an octal character, got {0}", str[index]));

            byte b2 = (byte)(EscapeFormatLookupTable[str[++index]] << 3);
            if (b2 == 255)
                throw new IOException(string.Format("Expected an octal character, got {0}", str[index]));

            byte b3 = EscapeFormatLookupTable[str[++index]];
            if (b3 == 255)
                throw new IOException(string.Format("Expected an octal character, got {0}", str[index]));

            return (byte)(b1 | b2 | b3);
        }

#if UNSAFE
        public static unsafe byte ToByteHexFormat(char* ch)
        {
          char* tmp = ch;
          tmp++;

          byte b1 = HexFormatLookupTableHigh[*ch];
          if (b1 == 255)
            throw new IOException("Expected a hex character, got " + 'c');

          byte b2 = HexFormatLookupTableLow[*tmp];
          if (b2 == 255)
            throw new IOException("Expected a hex character, got " + 'c');

          return (byte)(b1 | b2);
        }

        public static unsafe byte ToByteHexFormat(byte* ch)
        {
          byte b1 = HexFormatLookupTableHigh[*ch];
          if (b1 == 255)
            throw new IOException(string.Format("Expected a hex character, got {0}", *ch));

          byte b2 = HexFormatLookupTableLow[*(ch + 1)];
          if (b2 == 255)
            throw new IOException(string.Format("Expected a hex character, got {0}", *(ch + 1)));

          return (byte)(b1 | b2);
        }
#endif
    }
}
