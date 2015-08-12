// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;

namespace EntityFramework7.Npgsql.Extensions
{
    public class NpgsqlDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<NpgsqlDbContextOptionsBuilder, NpgsqlOptionsExtension>
    {
        public NpgsqlDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        {
        }

        protected override NpgsqlOptionsExtension CloneExtension()
            => new NpgsqlOptionsExtension(OptionsBuilder.Options.GetExtension<NpgsqlOptionsExtension>());
    }
}
