// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations.Operations;
using Npgsql.EntityFramework7.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace Npgsql.EntityFramework7.Migrations
{
    public class NpgsqlModelDiffer : ModelDiffer, INpgsqlModelDiffer
    {
        public NpgsqlModelDiffer([NotNull] INpgsqlTypeMapper typeMapper)
            : base(typeMapper)
        {
        }

        #region IModel

        private static readonly LazyRef<Sequence> _defaultSequence =
            new LazyRef<Sequence>(() => new Sequence(Sequence.DefaultName));

        protected override IEnumerable<MigrationOperation> Diff([CanBeNull] IModel source, [CanBeNull] IModel target)
        {
            var operations = base.Diff(source, target);

            // TODO: Remove when the default sequence is added to the model (See #1568)
            var sourceUsesDefaultSequence = DefaultSequenceUsed(source);
            var targetUsesDefaultSequence = DefaultSequenceUsed(target);
            if (sourceUsesDefaultSequence == false && targetUsesDefaultSequence)
            {
                operations = operations.Concat(Add(_defaultSequence.Value));
            }
            else if (sourceUsesDefaultSequence && targetUsesDefaultSequence == false)
            {
                operations = operations.Concat(Remove(_defaultSequence.Value));
            }

            return operations;
        }

        private bool DefaultSequenceUsed(IModel model) =>
            model != null
            && model.Npgsql().DefaultSequenceName == null
            && (model.Npgsql().ValueGenerationStrategy == NpgsqlValueGenerationStrategy.Sequence
                || model.EntityTypes.SelectMany(t => t.GetProperties()).Any(
                    p => p.Npgsql().ValueGenerationStrategy == NpgsqlValueGenerationStrategy.Sequence
                         && p.Npgsql().SequenceName == null));

        #endregion

        #region IProperty

        protected override IEnumerable<MigrationOperation> Diff(IProperty source, IProperty target)
        {
            var operations = base.Diff(source, target).ToList();

            var sourceValueGenerationStrategy = GetValueGenerationStrategy(source);
            var targetValueGenerationStrategy = GetValueGenerationStrategy(target);

            var alterColumnOperation = operations.OfType<AlterColumnOperation>().SingleOrDefault();
            if (alterColumnOperation == null
                && (source.Npgsql().ComputedExpression != target.Npgsql().ComputedExpression
                    || sourceValueGenerationStrategy != targetValueGenerationStrategy))
            {
                alterColumnOperation = new AlterColumnOperation
                {
                    Schema = source.EntityType.Relational().Schema,
                    Table = source.EntityType.Relational().Table,
                    Name = source.Relational().Column,
                    Type = TypeMapper.GetTypeMapping(target).StoreTypeName,
                    IsNullable = target.IsNullable,
                    DefaultValue = target.Relational().DefaultValue,
                    DefaultExpression = target.Relational().DefaultExpression
                };
                operations.Add(alterColumnOperation);
            }

            if (alterColumnOperation != null)
            {
                if (targetValueGenerationStrategy == NpgsqlValueGenerationStrategy.Identity)
                {
                    alterColumnOperation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ValueGeneration] =
                        targetValueGenerationStrategy.ToString();
                }

                if (target.Npgsql().ComputedExpression != null)
                {
                    alterColumnOperation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ColumnComputedExpression] =
                        target.Npgsql().ComputedExpression;
                }
            }

            return operations;
        }

        protected override IEnumerable<MigrationOperation> Add(IProperty target)
        {
            var operation = base.Add(target).Cast<AddColumnOperation>().Single();

            var targetValueGenerationStrategy = GetValueGenerationStrategy(target);

            if (targetValueGenerationStrategy == NpgsqlValueGenerationStrategy.Identity)
            {
                operation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ValueGeneration] =
                    targetValueGenerationStrategy.ToString();
            }

            if (target.Npgsql().ComputedExpression != null)
            {
                operation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ColumnComputedExpression] =
                    target.Npgsql().ComputedExpression;
            }

            yield return operation;
        }

        // TODO: Move to metadata API?
        private NpgsqlValueGenerationStrategy? GetValueGenerationStrategy(IProperty property) => property.Npgsql().ValueGenerationStrategy;

        #endregion

        #region IKey

        protected override IEnumerable<MigrationOperation> Diff(IKey source, IKey target)
        {
            var operations = base.Diff(source, target).ToList();

            var addOperation = operations.SingleOrDefault(o => o is AddPrimaryKeyOperation || o is AddUniqueConstraintOperation);
            if (addOperation == null)
            {
                if (source.Npgsql().IsClustered != target.Npgsql().IsClustered)
                {
                    if (source != null
                        && !operations.Any(o => o is DropPrimaryKeyOperation || o is DropUniqueConstraintOperation))
                    {
                        operations.AddRange(Remove(source));
                    }

                    operations.AddRange(Add(target));
                }
            }
            else if (target.Npgsql().IsClustered != null)
            {
                addOperation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Clustered] =
                    target.Npgsql().IsClustered.ToString();
            }

            return operations;
        }

        protected override IEnumerable<MigrationOperation> Add(IKey target)
        {
            var operation = base.Add(target)
                .Single(o => o is AddPrimaryKeyOperation || o is AddUniqueConstraintOperation);

            if (target.Npgsql().IsClustered != null)
            {
                operation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Clustered] =
                    target.Npgsql().IsClustered.ToString();
            }

            yield return operation;
        }

        #endregion

        #region IIndex

        protected override IEnumerable<MigrationOperation> Diff(IIndex source, IIndex target)
        {
            var operations = base.Diff(source, target).ToList();

            var createIndexOperation = operations.OfType<CreateIndexOperation>().SingleOrDefault();
            if (createIndexOperation == null
                && source.Npgsql().IsClustered != target.Npgsql().IsClustered)
            {
                operations.AddRange(Remove(source));

                createIndexOperation = new CreateIndexOperation
                {
                    Name = target.Relational().Name,
                    Schema = target.EntityType.Relational().Schema,
                    Table = target.EntityType.Relational().Table,
                    Columns = target.Properties.Select(p => p.Relational().Column).ToArray(),
                    IsUnique = target.IsUnique
                };
                operations.Add(createIndexOperation);
            }

            if (createIndexOperation != null
                && target.Npgsql().IsClustered != null)
            {
                createIndexOperation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Clustered] =
                    target.Npgsql().IsClustered.ToString();
            }

            return operations;
        }

        protected override IEnumerable<MigrationOperation> Add(IIndex target)
        {
            var operation = base.Add(target).Cast<CreateIndexOperation>().Single();

            if (target.Npgsql().IsClustered != null)
            {
                operation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Clustered] =
                    target.Npgsql().IsClustered.ToString();
            }

            return base.Add(target);
        }

        #endregion
    }
}
