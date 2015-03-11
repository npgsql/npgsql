using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlPropertyExtensions : ReadOnlyNpgsqlPropertyExtensions
    {
        public NpgsqlPropertyExtensions([NotNull] Property property)
            : base(property)
        {
        }

        public new virtual string Column
        {
            get { return base.Column; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

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
                Check.NullButNotEmpty(value, "value");

                ((Property)Property)[NpgsqlColumnTypeAnnotation] = value;
            }
        }

        [CanBeNull]
        public new virtual string DefaultExpression
        {
            get { return base.DefaultExpression; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

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

				( (Property)Property )[NpgsqlComputedExpressionAnnotation] = value;
			}
		}

		[CanBeNull]
        public new virtual string SequenceName
        {
            get { return base.SequenceName; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

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
                Check.NullButNotEmpty(value, "value");

                ((Property)Property)[NpgsqlSequenceSchemaAnnotation] = value;
            }
        }

        [CanBeNull]
        public new virtual NpgsqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get { return base.ValueGenerationStrategy; }
            [param: CanBeNull]
            set
            {
                var property = ((Property)Property);

                if (value == null)
                {
                    property[NpgsqlValueGenerationAnnotation] = null;
                    property.GenerateValueOnAdd = null;
                }
                else
                {
                    var propertyType = Property.PropertyType;

                    if (value == NpgsqlValueGenerationStrategy.Identity
                        && (!propertyType.IsInteger()
                            || propertyType == typeof(byte)
                            || propertyType == typeof(byte?)))
                    {
                        throw new ArgumentException(String.Format("Identity value generation cannot be used for the property '{0}' on entity type '{1}' because the property type is '{2}'.Identity value generation can only be used with signed integer properties.", Property.Name, Property.EntityType.Name, propertyType.Name));
                    }

                    if (value == NpgsqlValueGenerationStrategy.Sequence
                        && !propertyType.IsInteger())
                    {
                        throw new ArgumentException(String.Format("PostgreSQL sequences cannot be used to generate values for the property '{0}' on entity type '{1}' because the property type is '{2}'. Sequences can only be used with integer properties.", Property.Name, Property.EntityType.Name, propertyType.Name));
                    }

                    // TODO: Issue #777: Non-string annotations
                    property[NpgsqlValueGenerationAnnotation] = value.ToString();
                    property.GenerateValueOnAdd = true;
                }
            }
        }
    }
}