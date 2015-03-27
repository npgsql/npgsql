// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace Npgsql.EntityFramework7.Metadata
{
    public class ReadOnlyNpgsqlPropertyExtensions : ReadOnlyRelationalPropertyExtensions, INpgsqlPropertyExtensions
    {
        protected const string NpgsqlNameAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.ColumnName;
        protected const string NpgsqlColumnTypeAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.ColumnType;
        protected const string NpgsqlDefaultExpressionAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.ColumnDefaultExpression;
        protected const string NpgsqlValueGenerationAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ValueGeneration;
        protected const string NpgsqlComputedExpressionAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ColumnComputedExpression;
        protected const string NpgsqlSequenceNameAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.SequenceName;
        protected const string NpgsqlSequenceSchemaAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.SequenceSchema;
        protected const string NpgsqlDefaultValueAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.ColumnDefaultValue;
        protected const string NpgsqlDefaultValueTypeAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.ColumnDefaultValueType;

        public ReadOnlyNpgsqlPropertyExtensions([NotNull] IProperty property)
            : base(property)
        {
        }

        public override string Column
            => Property[NpgsqlNameAnnotation] as string
               ?? base.Column;

        public override string ColumnType
            => Property[NpgsqlColumnTypeAnnotation] as string
               ?? base.ColumnType;

        public override string DefaultExpression
            => Property[NpgsqlDefaultExpressionAnnotation] as string
               ?? base.DefaultExpression;

        public override object DefaultValue
            => new TypedAnnotation(
                Property[NpgsqlDefaultValueTypeAnnotation] as string,
                Property[NpgsqlDefaultValueAnnotation] as string).Value
                         ?? base.DefaultValue;

        public virtual string ComputedExpression
            => Property[NpgsqlComputedExpressionAnnotation] as string;

        public virtual NpgsqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Property[NpgsqlValueGenerationAnnotation] as string;

                var strategy = value == null
                    ? null
                    : (NpgsqlValueGenerationStrategy?)Enum.Parse(typeof(NpgsqlValueGenerationStrategy), value);

                return strategy == NpgsqlValueGenerationStrategy.Default
                    ? Property.EntityType.Model.Npgsql().ValueGenerationStrategy
                    : strategy;
            }
        }

        public virtual string SequenceName => Property[NpgsqlSequenceNameAnnotation] as string;
        public virtual string SequenceSchema => Property[NpgsqlSequenceSchemaAnnotation] as string;

        public virtual Sequence TryGetSequence()
        {
            var modelExtensions = Property.EntityType.Model.Npgsql();

            if (ValueGenerationStrategy != NpgsqlValueGenerationStrategy.Sequence)
            {
                return null;
            }

            var sequenceName = SequenceName
                               ?? modelExtensions.DefaultSequenceName
                               ?? Sequence.DefaultName;

            var sequenceSchema = SequenceSchema
                                 ?? modelExtensions.DefaultSequenceSchema;

            return modelExtensions.TryGetSequence(sequenceName, sequenceSchema)
                   ?? new Sequence(Sequence.DefaultName);
        }
    }
}
