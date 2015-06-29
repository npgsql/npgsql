// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Metadata.ModelConventions;

namespace EntityFramework7.Npgsql.Metadata.ModelConventions
{
    public class NpgsqlValueGenerationStrategyConvention : IModelConvention
    {
        public virtual InternalModelBuilder Apply(InternalModelBuilder modelBuilder)
        {
            modelBuilder.Annotation(
                NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ValueGeneration,
                NpgsqlValueGenerationStrategy.Identity.ToString(),
                ConfigurationSource.Convention);
            return modelBuilder;
        }
    }
}
