// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Migrations.Operations;

namespace EntityFramework7.Npgsql.Migrations
{
    public class CreateDatabaseOperation : MigrationOperation
    {
        public virtual string Name { get;[param: NotNull] set; }
    }
}
