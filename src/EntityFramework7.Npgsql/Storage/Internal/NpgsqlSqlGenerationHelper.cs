// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Storage.Internal
{
    public class NpgsqlSqlGenerationHelper : RelationalSqlGenerationHelper
    {
        public override string EscapeIdentifier([NotNull] string identifier)
            => Check.NotEmpty(identifier, nameof(identifier)).Replace("\"", "\"\"");

        public override string DelimitIdentifier([NotNull] string identifier)
            => $"\"{EscapeIdentifier(Check.NotEmpty(identifier, nameof(identifier)))}\"";

        protected override string GenerateLiteralValue(byte[] literal)
        {
            Check.NotNull(literal, nameof(literal));

            var builder = new StringBuilder(literal.Length * 2 + 6);

            builder.Append("E'\\\\x");
            foreach (var b in literal) {
                builder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
            }
            builder.Append('\'');

            return builder.ToString();
        }

        protected override string GenerateLiteralValue(bool literal) => literal ? "TRUE" : "FALSE";
        protected override string GenerateLiteralValue(DateTime literal) => "'" + literal.ToString(@"yyyy-MM-dd HH\:mm\:ss.fffffff") + "'";
        protected override string GenerateLiteralValue(DateTimeOffset literal) => "'" + literal.ToString(@"yyyy-MM-dd HH\:mm\:ss.fffffffzzz") + "'";
    }
}
