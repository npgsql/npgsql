// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlKeyAnnotations : ReadOnlyNpgsqlKeyAnnotations
    {
        public NpgsqlKeyAnnotations([NotNull] Key key)
            : base(key)
        {
        }

        [CanBeNull]
        public new virtual string Name
        {
            get { return base.Name; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

                ((Key)Key)[NpgsqlNameAnnotation] = value;
            }
        }
    }
}
