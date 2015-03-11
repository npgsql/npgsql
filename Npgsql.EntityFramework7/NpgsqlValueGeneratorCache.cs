using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlValueGeneratorCache : ValueGeneratorCache
    {
        public virtual int GetPoolSize(IProperty property)
		{
			Check.NotNull(property, nameof(property));

			// TODO: Allow configuration without creation of derived factory type
			// Issue #778
			return 5;
		}
		
		public virtual int GetBlockSize([NotNull] IProperty property)
		{
			Check.NotNull(property, nameof(property));

			var incrementBy = property.Npgsql().TryGetSequence().IncrementBy;

			if ( incrementBy <= 0 )
			{
				throw new NotSupportedException(Strings.SequenceBadBlockSize(incrementBy, GetSequenceName(property)));
			}

			return incrementBy;
		}

		public virtual string GetSequenceName([NotNull] IProperty property)
		{
			Check.NotNull(property, nameof(property));

			var sequence = property.Npgsql().TryGetSequence();

			return ( sequence.Schema == null ? "" : ( sequence.Schema + "." ) ) + sequence.Name;
		}
    }
}
