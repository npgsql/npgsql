// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.ValueGeneration;

namespace Npgsql.EntityFramework7.ValueGeneration
{
    public interface INpgsqlSequenceValueGeneratorFactory
    {
        ValueGenerator Create(
            [NotNull] IProperty property,
            [NotNull] NpgsqlSequenceValueGeneratorState generatorState,
            [NotNull] NpgsqlDatabaseConnection connection);
    }
}
