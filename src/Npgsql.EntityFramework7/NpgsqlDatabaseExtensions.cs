// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Npgsql.EntityFramework7;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class NpgsqlDatabaseExtensions
    {
        public static NpgsqlDatabase AsNpgsql([NotNull] this Database database)
        {
            Check.NotNull(database, nameof(database));

            var npgsqlDatabase = database as NpgsqlDatabase;

            if (npgsqlDatabase == null)
            {
                throw new InvalidOperationException(Strings.NpgsqlNotInUse);
            }

            return npgsqlDatabase;
        }
    }
}
