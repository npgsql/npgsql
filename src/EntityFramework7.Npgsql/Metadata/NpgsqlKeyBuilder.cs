// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlKeyBuilder
    {
        private readonly Key _key;

        public NpgsqlKeyBuilder([NotNull] Key key)
        {
            Check.NotNull(key, nameof(key));

            _key = key;
        }

        public virtual NpgsqlKeyBuilder Name([CanBeNull] string name)
        {
            Check.NullButNotEmpty(name, "name");

            _key.Npgsql().Name = name;

            return this;
        }
    }
}
