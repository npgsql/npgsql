using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework.Npgsql
{
    public interface INpgsqlValueGeneratorCache : IValueGeneratorCache
    {
        NpgsqlSequenceValueGeneratorState GetOrAddSequenceState([NotNull] IProperty property);
    }
}
