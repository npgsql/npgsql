using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework.Npgsql
{
    public class NpgsqlSequenceValueGeneratorState : HiLoValueGeneratorState
    {
        public NpgsqlSequenceValueGeneratorState([NotNull] string sequenceName, int blockSize, int poolSize)
            : base(blockSize, poolSize)
        {
            Check.NotEmpty(sequenceName, nameof(sequenceName));

            SequenceName = sequenceName;
        }

        public virtual string SequenceName { get; }
    }
}