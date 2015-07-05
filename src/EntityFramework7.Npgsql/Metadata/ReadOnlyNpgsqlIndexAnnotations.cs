// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;

namespace EntityFramework7.Npgsql.Metadata
{
    public class ReadOnlyNpgsqlIndexAnnotations : ReadOnlyRelationalIndexAnnotations, INpgsqlIndexAnnotations
    {
        protected const string NpgsqlNameAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Name;

        public ReadOnlyNpgsqlIndexAnnotations([NotNull] IIndex index)
            : base(index)
        {
        }

        public override string Name
            => Index[NpgsqlNameAnnotation] as string
                ?? base.Name;
    }
}
