using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Extensions
{
    public interface INpgsqlIndexExtensions : IRelationalIndexExtensions
    {
        [CanBeNull]
        bool? IsClustered { get; }
    }
}