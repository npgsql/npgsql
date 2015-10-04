// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class NpgsqlDbContextOptionsExtensions
    {
        public static NpgsqlDbContextOptionsBuilder UseNpgsql([NotNull] this DbContextOptionsBuilder optionsBuilder, [NotNull] string connectionString)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            var extension = GetOrCreateExtension(optionsBuilder);
            extension.ConnectionString = connectionString;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return new NpgsqlDbContextOptionsBuilder(optionsBuilder);
        }

        // Note: Decision made to use DbConnection not SqlConnection: Issue #772
        public static NpgsqlDbContextOptionsBuilder UseNpgsql([NotNull] this DbContextOptionsBuilder optionsBuilder, [NotNull] DbConnection connection)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            var extension = GetOrCreateExtension(optionsBuilder);
            extension.Connection = connection;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            return new NpgsqlDbContextOptionsBuilder(optionsBuilder);
        }

        private static NpgsqlOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        {
            var existing = optionsBuilder.Options.FindExtension<NpgsqlOptionsExtension>();
            return existing != null
                ? new NpgsqlOptionsExtension(existing)
                : new NpgsqlOptionsExtension();
        }
    }
}
