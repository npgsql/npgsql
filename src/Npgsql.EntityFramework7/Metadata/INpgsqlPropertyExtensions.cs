using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Extensions
{
    public interface INpgsqlPropertyExtensions : IRelationalPropertyExtensions
    {
        [CanBeNull]
        NpgsqlValueGenerationStrategy? ValueGenerationStrategy { get; }

		[CanBeNull]
		string ComputedExpression { get; }

		[CanBeNull]
        string SequenceName { get; }

        [CanBeNull]
        string SequenceSchema { get; }

        [CanBeNull]
        Sequence TryGetSequence();
    }
}