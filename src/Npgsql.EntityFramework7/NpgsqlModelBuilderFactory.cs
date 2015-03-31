// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Metadata.ModelConventions;
using Npgsql.EntityFramework7.Metadata.ModelConventions;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlModelBuilderFactory : ModelBuilderFactory, INpgsqlModelBuilderFactory
    {
        protected override ConventionSet CreateConventionSet()
        {
            var conventions = base.CreateConventionSet();

            var sqlServerValueGenerationStrategyConvention = new NpgsqlValueGenerationStrategyConvention();
            conventions.KeyAddedConventions.Add(sqlServerValueGenerationStrategyConvention);

            conventions.ForeignKeyAddedConventions.Add(sqlServerValueGenerationStrategyConvention);

            conventions.ForeignKeyRemovedConventions.Add(sqlServerValueGenerationStrategyConvention);

            conventions.ModelConventions.Add(sqlServerValueGenerationStrategyConvention);

            return conventions;
        }
    }
}
