// NpgsqlTypes.NpgsqlTypeConvNativeToProvider.cs
//
// Author:
//    Glen Parker <glenebob@gmail.com>
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
    /// Provide event handlers to convert the basic native supported data types from
    /// native form to provider type.
    /// </summary>
    internal static class NativeToProviderTypeConverter
    {
        internal static object NativeToBitString(object frameworkValue)
        {
            if (frameworkValue is bool)
            {
                return new BitString((bool)frameworkValue);
            }
            else if (frameworkValue is Int32)
            {
                return new BitString((Int32)frameworkValue);
            }
            else if (frameworkValue is IEnumerable<bool>)
            {
                return new BitString((IEnumerable<bool>)frameworkValue);
            }
            else
            {
                throw new InvalidCastException(String.Format(Messages.GetString("Exception_ImpossibleToCast"), frameworkValue.GetType()));
            }
        }
    }
}