//﻿using System;
//using JetBrains.Annotations;
//using Microsoft.Data.Entity.Metadata;
//using Microsoft.Data.Entity.Utilities;
//using Microsoft.Data.Entity.ValueGeneration;

//namespace EntityFramework.Npgsql.Extensions
//{
//	public class NpgsqlValueGeneratorFactorySelector : ValueGeneratorFactorySelector
//    {
//		private readonly NpgsqlSequenceValueGeneratorFactory _sequenceFactory;
//		private readonly SimpleValueGeneratorFactory<SequentialGuidValueGenerator> _sequentialGuidFactory;

//		public NpgsqlValueGeneratorFactorySelector(
//			[NotNull] SimpleValueGeneratorFactory<GuidValueGenerator> guidFactory,
//			[NotNull] TemporaryIntegerValueGeneratorFactory integerFactory,
//			[NotNull] SimpleValueGeneratorFactory<TemporaryStringValueGenerator> stringFactory,
//			[NotNull] SimpleValueGeneratorFactory<TemporaryBinaryValueGenerator> binaryFactory,
//			[NotNull] NpgsqlSequenceValueGeneratorFactory sequenceFactory,
//			[NotNull] SimpleValueGeneratorFactory<SequentialGuidValueGenerator> sequentialGuidFactory)
//			: base(guidFactory, integerFactory, stringFactory, binaryFactory)
//		{
//			Check.NotNull(sequenceFactory, nameof(sequenceFactory));
//			Check.NotNull(sequentialGuidFactory, nameof(sequentialGuidFactory));

//			_sequenceFactory = sequenceFactory;
//			_sequentialGuidFactory = sequentialGuidFactory;
//		}

//		public override ValueGeneratorFactory Select(IProperty property)
//		{
//			Check.NotNull(property, nameof(property));

//			var strategy = property.Npgsql().ValueGenerationStrategy
//							?? property.EntityType.Model.Npgsql().ValueGenerationStrategy;

//			if ( property.PropertyType.IsInteger()
//				&& strategy == NpgsqlValueGenerationStrategy.Sequence )
//			{
//				return _sequenceFactory;
//			}

//			if ( property.PropertyType == typeof(Guid) )
//			{
//				return _sequentialGuidFactory;
//			}

//			return base.Select(property);
//		}
//	}
//}
