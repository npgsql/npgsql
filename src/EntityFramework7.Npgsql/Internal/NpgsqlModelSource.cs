// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata.Conventions.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Internal
{
    public class NpgsqlModelSource : ModelSource
    {
        public NpgsqlModelSource(
            [NotNull] IDbSetFinder setFinder,
            [NotNull] ICoreConventionSetBuilder coreConventionSetBuilder)
            : base(setFinder, coreConventionSetBuilder)
        {
        }
    }
}
