using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class ReadOnlyNpgsqlModelExtensions : ReadOnlyRelationalModelExtensions, INpgsqlModelExtensions
    {
        protected const string NpgsqlValueGenerationAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ValueGeneration;
        protected const string NpgsqlSequenceAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Sequence;
        protected const string NpgsqlDefaultSequenceNameAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.DefaultSequenceName;
        protected const string NpgsqlDefaultSequenceSchemaAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.DefaultSequenceSchema;

        public ReadOnlyNpgsqlModelExtensions([NotNull] IModel model)
            : base(model)
        {
        }

        public virtual NpgsqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Model[NpgsqlValueGenerationAnnotation];
                return value == null ? null : (NpgsqlValueGenerationStrategy?)Enum.Parse(typeof(NpgsqlValueGenerationStrategy), value);
            }
        }

        public virtual string DefaultSequenceName
        {
            get { return Model[NpgsqlDefaultSequenceNameAnnotation]; }
        }

        public virtual string DefaultSequenceSchema
        {
            get { return Model[NpgsqlDefaultSequenceSchemaAnnotation]; }
        }

        public override Sequence TryGetSequence(string name, string schema = null)
        {
            Check.NotEmpty(name, "name");
            Check.NullButNotEmpty(schema, "schema");

            return FindSequence(NpgsqlSequenceAnnotation + schema + "." + name) ?? base.TryGetSequence(name, schema);
        }
    }
}