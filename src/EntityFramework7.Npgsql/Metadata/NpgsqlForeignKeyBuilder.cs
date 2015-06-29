// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlForeignKeyBuilder
    {
        private readonly ForeignKey _foreignKey;

        public NpgsqlForeignKeyBuilder([NotNull] ForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            _foreignKey = foreignKey;
        }

        public virtual NpgsqlForeignKeyBuilder Name([CanBeNull] string name)
        {
            Check.NullButNotEmpty(name, "name");

            _foreignKey.Npgsql().Name = name;

            return this;
        }
    }
}
