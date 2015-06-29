// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;

namespace EntityFramework7.Npgsql.Extensions
{
    public class NpgsqlDbContextOptionsBuilder : RelationalDbContextOptionsBuilder
    {
        public NpgsqlDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        {
        }

        public virtual NpgsqlDbContextOptionsBuilder MaxBatchSize(int maxBatchSize)
        {
            var extension = new NpgsqlOptionsExtension(OptionsBuilder.Options.GetExtension<NpgsqlOptionsExtension>())
            {
                MaxBatchSize = maxBatchSize
            };

            ((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(extension);

            return this;
        }

        public virtual NpgsqlDbContextOptionsBuilder CommandTimeout(int? commandTimeout)
        {
            var extension = new NpgsqlOptionsExtension(OptionsBuilder.Options.GetExtension<NpgsqlOptionsExtension>())
            {
                CommandTimeout = commandTimeout
            };

            ((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(extension);

            return this;
        }

        public virtual NpgsqlDbContextOptionsBuilder MigrationsAssembly([NotNull] string assemblyName)
        {
            var extension = new NpgsqlOptionsExtension(OptionsBuilder.Options.GetExtension<NpgsqlOptionsExtension>())
            {
                MigrationsAssembly = assemblyName
            };

            ((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(extension);

            return this;
        }

        public virtual NpgsqlDbContextOptionsBuilder SuppressAmbientTransactionWarning()
        {
            var extension = new NpgsqlOptionsExtension(OptionsBuilder.Options.GetExtension<NpgsqlOptionsExtension>())
            {
                ThrowOnAmbientTransaction = false
            };

            ((IDbContextOptionsBuilderInfrastructure)OptionsBuilder).AddOrUpdateExtension(extension);

            return this;
        }
    }
}
