// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Metadata.ModelConventions;
using Microsoft.Data.Entity.Utilities;
using Npgsql.EntityFramework7.Metadata.ModelConventions;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlConventionSetBuilder : IConventionSetBuilder
    {
        public virtual ConventionSet AddConventions(ConventionSet conventionSet)
        {
            Check.NotNull(conventionSet, nameof(conventionSet));

            conventionSet.ModelInitializedConventions.Add(new NpgsqlValueGenerationStrategyConvention());
            return conventionSet;
        }
    }
}
