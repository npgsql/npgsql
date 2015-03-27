// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;

namespace Npgsql.EntityFramework7.Extensions
{
    public class NpgsqlDbContextOptionsBuilder : RelationalDbContextOptionsBuilder
    {
        public NpgsqlDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        {
        }

        public virtual NpgsqlDbContextOptionsBuilder MaxBatchSize(int maxBatchSize)
        {
            var extension = new NpgsqlOptionsExtension(OptionsBuilder.Options.FindExtension<NpgsqlOptionsExtension>());
            Debug.Assert(extension != null);

            extension.MaxBatchSize = maxBatchSize;

            ((IOptionsBuilderExtender)OptionsBuilder).AddOrUpdateExtension(extension);

            return this;
        }

        public virtual NpgsqlDbContextOptionsBuilder CommandTimeout(int? commandTimeout)
        {
            var extension = new NpgsqlOptionsExtension(OptionsBuilder.Options.FindExtension<NpgsqlOptionsExtension>());
            Debug.Assert(extension != null);

            extension.CommandTimeout = commandTimeout;

            ((IOptionsBuilderExtender)OptionsBuilder).AddOrUpdateExtension(extension);

            return this;
        }

        public virtual NpgsqlDbContextOptionsBuilder MigrationsAssembly([NotNull] string assemblyName)
        {
            var extension = new NpgsqlOptionsExtension(OptionsBuilder.Options.FindExtension<NpgsqlOptionsExtension>());
            Debug.Assert(extension != null);

            extension.MigrationsAssembly = assemblyName;

            ((IOptionsBuilderExtender)OptionsBuilder).AddOrUpdateExtension(extension);

            return this;
        }
    }
}
