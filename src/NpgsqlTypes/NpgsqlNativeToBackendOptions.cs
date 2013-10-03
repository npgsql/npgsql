// NpgsqlTypes.NpgsqlNativeToBackendOptions.cs
//
// Author:
//    Glen Parker <glenebob@gmail.com>
//
//    Copyright (C) 2004 The Npgsql Development Team
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

// This file provides data type converters between PostgreSQL representations
// and .NET objects.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;

namespace NpgsqlTypes
{
    /// <summary>
    /// Options that control certain aspects of native to backend conversions that depend
    /// on backend version and status.
    /// </summary>
    internal class NativeToBackendTypeConverterOptions : ICloneable
    {
        internal static NativeToBackendTypeConverterOptions _default;

        private bool IsImmutable;
        private bool _UseConformantStrings;
        private bool _Supports_E_StringPrefix;
        private bool _SupportsHexByteFormat;
        private NpgsqlBackendTypeMapping _oidToNameMapping;

        static NativeToBackendTypeConverterOptions()
        {
            _default = new NativeToBackendTypeConverterOptions(true, false, true, false, null);
        }

        internal static NativeToBackendTypeConverterOptions Default
        {
            get
            {
                return _default;
            }
        }

        private NativeToBackendTypeConverterOptions(bool Immutable, bool useConformantStrings, bool supports_E_StringPrefix, bool supportsHexByteFormat, NpgsqlBackendTypeMapping oidToNameMapping)
        {
            this.IsImmutable = Immutable;
            this._UseConformantStrings = useConformantStrings;
            this._Supports_E_StringPrefix = supports_E_StringPrefix;
            this._SupportsHexByteFormat = supportsHexByteFormat;
            this._oidToNameMapping = oidToNameMapping;
        }

        internal NativeToBackendTypeConverterOptions(bool useConformantStrings, bool supports_E_StringPrefix, bool supportsHexByteFormat, NpgsqlBackendTypeMapping oidToNameMapping)
            : this(false, useConformantStrings, supports_E_StringPrefix, supportsHexByteFormat, oidToNameMapping)
        {
        }

        /// <summary>
        /// Clone the current object.
        /// </summary>
        /// <returns>A new NativeToBackendTypeConverterOptions object.</returns>
        Object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Clone the current object with a different OID/Name mapping.
        /// </summary>
        /// <param name="oidToNameMapping">OID/Name mapping object to use in the new instance.</param>
        /// <returns>A new NativeToBackendTypeConverterOptions object.</returns>
        internal NativeToBackendTypeConverterOptions Clone(NpgsqlBackendTypeMapping oidToNameMapping = null)
        {
            return new NativeToBackendTypeConverterOptions(_UseConformantStrings, _Supports_E_StringPrefix, _SupportsHexByteFormat, oidToNameMapping);
        }

        internal bool UseConformantStrings
        {
            get { return _UseConformantStrings; }

            set
            {
                if (IsImmutable)
                {
                    throw new InvalidOperationException("Object is immutable");
                }
                else
                {
                    _UseConformantStrings = value;
                }
            }
        }

        internal bool Supports_E_StringPrefix
        {
            get { return _Supports_E_StringPrefix; }

            set
            {
                if (IsImmutable)
                {
                    throw new InvalidOperationException("Object is immutable");
                }
                else
                {
                    _Supports_E_StringPrefix = value;
                }
            }
        }

        internal bool SupportsHexByteFormat
        {
            get { return _SupportsHexByteFormat; }

            set
            {
                if (IsImmutable)
                {
                    throw new InvalidOperationException("Object is immutable");
                }
                else
                {
                    _SupportsHexByteFormat = value;
                }
            }
        }

        internal NpgsqlBackendTypeMapping OidToNameMapping
        {
            get { return _oidToNameMapping; }

            set
            {
                if (IsImmutable)
                {
                    throw new InvalidOperationException("Object is immutable");
                }
                else
                {
                    _oidToNameMapping = value;
                }
            }
        }
    }
}