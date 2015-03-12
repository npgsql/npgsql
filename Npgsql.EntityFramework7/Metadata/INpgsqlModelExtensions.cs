using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Extensions
{
    public interface INpgsqlModelExtensions : IRelationalModelExtensions
    {
        [CanBeNull]
        NpgsqlValueGenerationStrategy? ValueGenerationStrategy { get; }

        [CanBeNull]
        string DefaultSequenceName { get; }

        [CanBeNull]
        string DefaultSequenceSchema { get; }
    }
}