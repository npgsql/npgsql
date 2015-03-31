// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace Npgsql.EntityFramework7.Metadata
{
    public class ReadOnlyNpgsqlIndexExtensions : ReadOnlyRelationalIndexExtensions, INpgsqlIndexExtensions
    {
        protected const string NpgsqlNameAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Name;
        protected const string NpgsqlClusteredAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Clustered;

        public ReadOnlyNpgsqlIndexExtensions([NotNull] IIndex index)
            : base(index)
        {
        }

        public override string Name 
            => Index[NpgsqlNameAnnotation] as string 
                ?? base.Name;

        public virtual bool? IsClustered
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Index[NpgsqlClusteredAnnotation] as string;
                return value == null ? null : (bool?)bool.Parse(value);
            }
        }
    }
}
