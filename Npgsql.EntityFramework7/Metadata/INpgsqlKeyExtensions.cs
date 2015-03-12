using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Extensions
{
    public interface INpgsqlKeyExtensions : IRelationalKeyExtensions
    {
        [CanBeNull]
        bool? IsClustered { get; }
    }
}