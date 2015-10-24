// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.Data.Entity.Metadata.Internal
{
    public static class NpgsqlInternalMetadataBuilderExtensions
    {
        public static RelationalEntityTypeBuilderAnnotations Npgsql(
            [NotNull] this InternalEntityTypeBuilder builder,
            ConfigurationSource configurationSource)
            => new RelationalEntityTypeBuilderAnnotations(builder, configurationSource, NpgsqlAnnotationNames.Prefix);
    }
}
