using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlModelExtensions : ReadOnlyNpgsqlModelExtensions
    {
        public NpgsqlModelExtensions([NotNull] Model model)
            : base(model)
        {
        }

        [CanBeNull]
        public new virtual NpgsqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get { return base.ValueGenerationStrategy; }
            [param: CanBeNull]
            set
            {
                // TODO: Issue #777: Non-string annotations
                ((Model)Model)[NpgsqlValueGenerationAnnotation] = value == null ? null : value.ToString();
            }
        }

        public new virtual string DefaultSequenceName
        {
            get { return base.DefaultSequenceName; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

                ((Model)Model)[NpgsqlDefaultSequenceNameAnnotation] = value;
            }
        }

        public new virtual string DefaultSequenceSchema
        {
            get { return base.DefaultSequenceSchema; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

                ((Model)Model)[NpgsqlDefaultSequenceSchemaAnnotation] = value;
            }
        }

        public virtual Sequence AddOrReplaceSequence([NotNull] Sequence sequence)
        {
            Check.NotNull(sequence, "sequence");

            var model = (Model)Model;
            sequence.Model = model;
            model[NpgsqlSequenceAnnotation + sequence.Schema + "." + sequence.Name] = sequence.Serialize();

            return sequence;
        }

        public virtual Sequence GetOrAddSequence([CanBeNull] string name = null, [CanBeNull] string schema = null)
        {
            Check.NullButNotEmpty(name, "name");
            Check.NullButNotEmpty(schema, "schema");

            name = name ?? Sequence.DefaultName;

            return ((Model)Model).Npgsql().TryGetSequence(name, schema) ?? AddOrReplaceSequence(new Sequence(name, schema));
        }
    }
}