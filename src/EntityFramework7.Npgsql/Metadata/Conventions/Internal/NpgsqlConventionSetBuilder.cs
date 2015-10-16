// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// ReSharper disable once CheckNamespace

using JetBrains.Annotations;
using Microsoft.Data.Entity.Storage;

namespace Microsoft.Data.Entity.Metadata.Conventions.Internal
{
    public class NpgsqlConventionSetBuilder : RelationalConventionSetBuilder
    {
        public NpgsqlConventionSetBuilder([NotNull] IRelationalTypeMapper typeMapper)
            : base(typeMapper)
        {
        }

        // TODO: SqlServer has identity here, do we need something?
    }
}
