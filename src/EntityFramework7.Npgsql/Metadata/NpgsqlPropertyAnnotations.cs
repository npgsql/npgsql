// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using EntityFramework7.Npgsql.Properties;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlPropertyAnnotations : ReadOnlyNpgsqlPropertyAnnotations
    {
        public NpgsqlPropertyAnnotations([NotNull] Property property)
            : base(property)
        {
        }

        public new virtual string Column
        {
            get { return base.Column; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, nameof(value));

                ((Property)Property)[NpgsqlNameAnnotation] = value;
            }
        }

        [CanBeNull]
        public new virtual string ColumnType
        {
            get { return base.ColumnType; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, nameof(value));

                ((Property)Property)[NpgsqlColumnTypeAnnotation] = value;
            }
        }

        [CanBeNull]
        public new virtual string DefaultValueSql
        {
            get { return base.DefaultValueSql; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, nameof(value));

                ((Property)Property)[NpgsqlDefaultExpressionAnnotation] = value;
            }
        }

        public new virtual object DefaultValue
        {
            get { return base.DefaultValue; }
            [param: CanBeNull]
            set
            {
                var typedAnnotation = new TypedAnnotation(value);

                ((Property)Property)[NpgsqlDefaultValueTypeAnnotation] = typedAnnotation.TypeString;
                ((Property)Property)[NpgsqlDefaultValueAnnotation] = typedAnnotation.ValueString;
            }
        }

        [CanBeNull]
        public new virtual string ComputedExpression
        {
            get { return base.ComputedExpression; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, nameof(value));

                ((Property)Property)[NpgsqlComputedExpressionAnnotation] = value;
            }
        }

        [CanBeNull]
        public new virtual string SequenceName
        {
            get { return base.SequenceName; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, nameof(value));

                ((Property)Property)[NpgsqlSequenceNameAnnotation] = value;
            }
        }

        [CanBeNull]
        public new virtual string SequenceSchema
        {
            get { return base.SequenceSchema; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, nameof(value));

                ((Property)Property)[NpgsqlSequenceSchemaAnnotation] = value;
            }
        }
    }
}
