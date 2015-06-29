// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace Npgsql.EntityFramework7.Metadata
{
    public class NpgsqlPropertyBuilder
    {
        private readonly Property _property;

        public NpgsqlPropertyBuilder([NotNull] Property property)
        {
            Check.NotNull(property, nameof(property));

            _property = property;
        }

        public virtual NpgsqlPropertyBuilder Column([CanBeNull] string columnName)
        {
            Check.NullButNotEmpty(columnName, "columnName");

            _property.Npgsql().Column = columnName;

            return this;
        }

        public virtual NpgsqlPropertyBuilder ColumnType([CanBeNull] string columnType)
        {
            Check.NullButNotEmpty(columnType, "columnType");

            _property.Npgsql().ColumnType = columnType;

            return this;
        }

        public virtual NpgsqlPropertyBuilder DefaultExpression([CanBeNull] string expression)
        {
            Check.NullButNotEmpty(expression, "expression");

            _property.Npgsql().DefaultExpression = expression;

            return this;
        }

        public virtual NpgsqlPropertyBuilder DefaultValue([CanBeNull] object value)
        {
            _property.Npgsql().DefaultValue = value;

            return this;
        }

        public virtual NpgsqlPropertyBuilder ComputedExpression([CanBeNull] string expression)
        {
            Check.NullButNotEmpty(expression, nameof(expression));

            _property.Npgsql().ComputedExpression = expression;

            return this;
        }

        public virtual NpgsqlPropertyBuilder UseSequence()
        {
            _property.Npgsql().ValueGenerationStrategy = NpgsqlValueGenerationStrategy.Sequence;
            _property.Npgsql().SequenceName = null;
            _property.Npgsql().SequenceSchema = null;

            return this;
        }

        public virtual NpgsqlPropertyBuilder UseSequence([NotNull] string name, [CanBeNull] string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, "schema");

            var sequence = _property.EntityType.Model.Npgsql().GetOrAddSequence(name, schema);

            _property.Npgsql().ValueGenerationStrategy = NpgsqlValueGenerationStrategy.Sequence;
            _property.Npgsql().SequenceName = sequence.Name;
            _property.Npgsql().SequenceSchema = sequence.Schema;

            return this;
        }

        public virtual NpgsqlPropertyBuilder UseIdentity()
        {
            _property.Npgsql().ValueGenerationStrategy = NpgsqlValueGenerationStrategy.Identity;
            _property.Npgsql().SequenceName = null;
            _property.Npgsql().SequenceSchema = null;

            return this;
        }

        public virtual NpgsqlPropertyBuilder UseDefaultValueGeneration()
        {
            _property.Npgsql().ValueGenerationStrategy = NpgsqlValueGenerationStrategy.Default;
            _property.Npgsql().SequenceName = null;
            _property.Npgsql().SequenceSchema = null;

            return this;
        }

        public virtual NpgsqlPropertyBuilder UseNoValueGeneration()
        {
            _property.Npgsql().ValueGenerationStrategy = null;
            _property.Npgsql().SequenceName = null;
            _property.Npgsql().SequenceSchema = null;

            return this;
        }
    }
}
