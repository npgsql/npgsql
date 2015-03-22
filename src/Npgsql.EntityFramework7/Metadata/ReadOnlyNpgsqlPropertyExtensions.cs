using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Extensions
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
        {
            get { return Property[NpgsqlNameAnnotation] ?? base.Column; }
        }

        public override string ColumnType
        {
            get { return Property[NpgsqlColumnTypeAnnotation] ?? base.ColumnType; }
        }

        public override string DefaultExpression
        {
            get { return Property[NpgsqlDefaultExpressionAnnotation] ?? base.DefaultExpression; }
        }

        public override object DefaultValue
        {
            get
            {
                return new TypedAnnotation(Property[NpgsqlDefaultValueTypeAnnotation], Property[NpgsqlDefaultValueAnnotation]).Value
                       ?? base.DefaultValue;
            }
        }

		public virtual string ComputedExpression => Property[NpgsqlComputedExpressionAnnotation];

		public virtual NpgsqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Property[NpgsqlValueGenerationAnnotation];
                return value == null ? null : (NpgsqlValueGenerationStrategy?)Enum.Parse(typeof(NpgsqlValueGenerationStrategy), value);
            }
        }

        public virtual string SequenceName
        {
            get { return Property[NpgsqlSequenceNameAnnotation]; }
        }

        public virtual string SequenceSchema
        {
            get { return Property[NpgsqlSequenceSchemaAnnotation]; }
        }

        public virtual Sequence TryGetSequence()
        {
            var modelExtensions = Property.EntityType.Model.Npgsql();

            if (ValueGenerationStrategy != NpgsqlValueGenerationStrategy.Sequence
                && (ValueGenerationStrategy != null
                    || modelExtensions.ValueGenerationStrategy != NpgsqlValueGenerationStrategy.Sequence))
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