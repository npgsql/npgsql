// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Framework.Logging;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlDatabase : RelationalDatabase
    {
        public NpgsqlDatabase(
            [NotNull] DbContext context,
            [NotNull] INpgsqlDataStoreCreator dataStoreCreator,
            [NotNull] INpgsqlConnection connection,
            [NotNull] Migrator migrator,
            [NotNull] ILoggerFactory loggerFactory)
            : base(context, dataStoreCreator, connection, migrator, loggerFactory)
        {
        }

        public new virtual INpgsqlConnection Connection => (INpgsqlConnection)base.Connection;
    }
}
