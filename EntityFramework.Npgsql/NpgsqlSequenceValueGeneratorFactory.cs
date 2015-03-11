using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlSequenceValueGeneratorFactory : ValueGeneratorFactory
    {
		private readonly SqlStatementExecutor _executor;

		public NpgsqlSequenceValueGeneratorFactory([NotNull] SqlStatementExecutor executor)
		{
			Check.NotNull(executor, nameof(executor));

			_executor = executor;
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

		public override ValueGenerator Create(IProperty property)
		{
			Check.NotNull(property, nameof(property));

			if ( property.PropertyType.UnwrapNullableType() == typeof(long) )
			{
				return new NpgsqlSequenceValueGenerator<long>(_executor, GetSequenceName(property), GetBlockSize(property));
			}

			if ( property.PropertyType.UnwrapNullableType() == typeof(int) )
			{
				return new NpgsqlSequenceValueGenerator<int>(_executor, GetSequenceName(property), GetBlockSize(property));
			}

			if ( property.PropertyType.UnwrapNullableType() == typeof(short) )
			{
				return new NpgsqlSequenceValueGenerator<short>(_executor, GetSequenceName(property), GetBlockSize(property));
			}

			if ( property.PropertyType.UnwrapNullableType() == typeof(byte) )
			{
				return new NpgsqlSequenceValueGenerator<byte>(_executor, GetSequenceName(property), GetBlockSize(property));
			}

			if ( property.PropertyType.UnwrapNullableType() == typeof(ulong) )
			{
				return new NpgsqlSequenceValueGenerator<ulong>(_executor, GetSequenceName(property), GetBlockSize(property));
			}

			if ( property.PropertyType.UnwrapNullableType() == typeof(uint) )
			{
				return new NpgsqlSequenceValueGenerator<uint>(_executor, GetSequenceName(property), GetBlockSize(property));
			}

			if ( property.PropertyType.UnwrapNullableType() == typeof(ushort) )
			{
				return new NpgsqlSequenceValueGenerator<ushort>(_executor, GetSequenceName(property), GetBlockSize(property));
			}

			if ( property.PropertyType.UnwrapNullableType() == typeof(sbyte) )
			{
				return new NpgsqlSequenceValueGenerator<sbyte>(_executor, GetSequenceName(property), GetBlockSize(property));
			}

			throw new ArgumentException(Microsoft.Data.Entity.Internal.Strings.InvalidValueGeneratorFactoryProperty(
				nameof(NpgsqlSequenceValueGeneratorFactory), property.Name, property.EntityType.SimpleName));
		}

		public virtual int GetPoolSize(IProperty property)
		{
			Check.NotNull(property, nameof(property));

			// TODO: Allow configuration without creation of derived factory type
			// Issue #778
			return 5;
		}

		public virtual string GetCacheKey(IProperty property)
		{
			Check.NotNull(property, nameof(property));

			return GetSequenceName(property);
		}
	}
}
