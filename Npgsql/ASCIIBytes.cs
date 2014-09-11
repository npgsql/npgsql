// Npgsql.ASCIIBytes.cs
//
// Authors:
//    Glen Parker         <glenebob@gmail.com>
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
using System.Text;

namespace Npgsql
{
    internal enum ASCIIBytes : byte
    {
        LineFeed = (byte)'\n',
        CarriageReturn = (Byte)'\r',

        Space = (byte)' ',

        DoubleQuote = (byte)'"',
        SingleQuote = (byte)'\'',

        ParenLeft = (byte)'(',
        ParenRight = (byte)')',

        Comma = (byte)',',

        Dash = (byte)'-',

        b0 = (byte)'0',
        b1 = (byte)'1',
        b2 = (byte)'2',
        b3 = (byte)'3',
        b4 = (byte)'4',
        b5 = (byte)'5',
        b6 = (byte)'6',
        b7 = (byte)'7',
        b8 = (byte)'8',
        b9 = (byte)'9',

        Colon = (byte)':',
        SemiColon = (byte)';',

        A = (byte)'A',
        B = (byte)'B',
        C = (byte)'C',
        D = (byte)'D',
        E = (byte)'E',
        F = (byte)'F',
        N = (byte)'N',
        P = (byte)'P',
        R = (byte)'R',
        S = (byte)'S',
        T = (byte)'T',
        V = (byte)'V',
        X = (byte)'X',

        BraceSquareLeft = (byte)'[',

        BackSlash = (byte)'\\',

        BraceSquareRight = (byte)']',

        a = (byte)'a',
        b = (byte)'b',
        c = (byte)'c',
        d = (byte)'d',
        e = (byte)'e',
        f = (byte)'f',
        n = (byte)'n',
        p = (byte)'p',
        r = (byte)'r',
        s = (byte)'s',
        t = (byte)'t',
        v = (byte)'v',
        x = (byte)'x',

        BraceCurlyLeft = (byte)'{',
        BraceCurlyRight = (byte)'}'
    }

    internal class ASCIIByteArrays
    {
        internal static readonly byte[] Empty           = new byte[0];
        internal static readonly byte[] Byte_0          = new byte[] { 0 };
        internal static readonly byte[] Byte_1          = new byte[] { 1 };
        internal static readonly byte[] NULL            = BackendEncoding.UTF8Encoding.GetBytes("NULL");
        internal static readonly byte[] AsciiDigit_0    = BackendEncoding.UTF8Encoding.GetBytes("0");
        internal static readonly byte[] AsciiDigit_1    = BackendEncoding.UTF8Encoding.GetBytes("1");
        internal static readonly byte[] TRUE            = BackendEncoding.UTF8Encoding.GetBytes("TRUE");
        internal static readonly byte[] FALSE           = BackendEncoding.UTF8Encoding.GetBytes("FALSE");
        internal static readonly byte[] INFINITY        = BackendEncoding.UTF8Encoding.GetBytes("INFINITY");
        internal static readonly byte[] NEG_INFINITY    = BackendEncoding.UTF8Encoding.GetBytes("-INFINITY");
        internal static readonly byte[] INFINITY_QUOTED = BackendEncoding.UTF8Encoding.GetBytes("'INFINITY'");
        internal static readonly byte[] NEG_INFINITY_QUOTED = BackendEncoding.UTF8Encoding.GetBytes("'-INFINITY'");
        internal static readonly byte[] LineTerminator  = BackendEncoding.UTF8Encoding.GetBytes("\r\n");
        internal static readonly byte[] NAN_QUOTED      = BackendEncoding.UTF8Encoding.GetBytes("'NaN'");
    }
}
