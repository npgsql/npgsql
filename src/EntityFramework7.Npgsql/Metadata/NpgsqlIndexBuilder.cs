// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlIndexBuilder
    {
        private readonly Index _index;

        public NpgsqlIndexBuilder([NotNull] Index index)
        {
            Check.NotNull(index, nameof(index));

            _index = index;
        }

        public virtual NpgsqlIndexBuilder Name([CanBeNull] string name)
        {
            Check.NullButNotEmpty(name, "name");

            _index.Npgsql().Name = name;

            return this;
        }
    }
}
