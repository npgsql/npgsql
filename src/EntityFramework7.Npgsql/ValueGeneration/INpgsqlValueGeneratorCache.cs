// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework7.Npgsql.ValueGeneration
{
    public interface INpgsqlValueGeneratorCache : IValueGeneratorCache
    {
        NpgsqlSequenceValueGeneratorState GetOrAddSequenceState([NotNull] IProperty property);
    }
}
