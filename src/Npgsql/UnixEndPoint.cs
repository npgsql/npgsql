// Copied and adapted from https://github.com/mono/mono/blob/master/mcs/class/Mono.Posix/Mono.Unix/UnixEndPoint.cs

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

using System.Net.Sockets;
using System.Text;

// ReSharper disable once CheckNamespace
namespace System.Net
{
    class UnixEndPoint : EndPoint
    {
        string _filename;

        public UnixEndPoint (string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (filename == "")
                throw new ArgumentException ("Cannot be empty.", nameof(filename));
            _filename = filename;
        }

        public string Filename {
            get => _filename;
            set => _filename = value;
        }

        public override AddressFamily AddressFamily => AddressFamily.Unix;

        public override EndPoint Create(SocketAddress socketAddress)
        {
            /*
             * Should also check this
             *
            int addr = (int) AddressFamily.Unix;
            if (socketAddress [0] != (addr & 0xFF))
                throw new ArgumentException ("socketAddress is not a unix socket address.");
            if (socketAddress [1] != ((addr & 0xFF00) >> 8))
                throw new ArgumentException ("socketAddress is not a unix socket address.");
             */

            if (socketAddress.Size == 2) {
                // Empty filename.
                // Probably from RemoteEndPoint which on linux does not return the file name.
                return new UnixEndPoint("a") { _filename = "" };
            }
            var size = socketAddress.Size - 2;
            var bytes = new byte[size];
            for (var i = 0; i < bytes.Length; i++) {
                bytes[i] = socketAddress[i + 2];
                // There may be junk after the null terminator, so ignore it all.
                if (bytes[i] == 0) {
                    size = i;
                    break;
                }
            }

            var name = Encoding.UTF8.GetString(bytes, 0, size);
            return new UnixEndPoint(name);
        }

        public override SocketAddress Serialize()
        {
            var bytes = Encoding.UTF8.GetBytes(_filename);
            var sa = new SocketAddress(AddressFamily, 2 + bytes.Length + 1);
            // sa [0] -> family low byte, sa [1] -> family high byte
            for (var i = 0; i < bytes.Length; i++)
                sa[2 + i] = bytes[i];

            //NULL suffix for non-abstract path
            sa[2 + bytes.Length] = 0;

            return sa;
        }

        public override string ToString() => _filename;

        public override int GetHashCode () => _filename.GetHashCode();

        public override bool Equals (object o)
        {
            var other = o as UnixEndPoint;
            if (other == null)
                return false;

            return (other._filename == _filename);
        }
    }
}