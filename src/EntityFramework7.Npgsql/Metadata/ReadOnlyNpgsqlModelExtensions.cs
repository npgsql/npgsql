// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class ReadOnlyNpgsqlModelExtensions : ReadOnlyRelationalModelExtensions, INpgsqlModelExtensions
    {
        protected const string NpgsqlValueGenerationAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ValueGeneration;
        protected const string NpgsqlSequenceAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Sequence;
        protected const string NpgsqlDefaultSequenceNameAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.DefaultSequenceName;
        protected const string NpgsqlDefaultSequenceSchemaAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.DefaultSequenceSchema;

        public ReadOnlyNpgsqlModelExtensions([NotNull] IModel model)
            : base(model)
        {
        }

        public virtual NpgsqlValueGenerationStrategy? ValueGenerationStrategy
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Model[NpgsqlValueGenerationAnnotation] as string;

                return value == null
                    ? null
                    : (NpgsqlValueGenerationStrategy?)Enum.Parse(typeof(NpgsqlValueGenerationStrategy), value);
            }
        }

        public virtual string DefaultSequenceName => Model[NpgsqlDefaultSequenceNameAnnotation] as string;
        public virtual string DefaultSequenceSchema => Model[NpgsqlDefaultSequenceSchemaAnnotation] as string;

        public override IReadOnlyList<Sequence> Sequences
        {
            get
            {
                var sqlServerSequences = (
                    from a in Model.Annotations
                    where a.Name.StartsWith(NpgsqlSequenceAnnotation)
                    select Sequence.Deserialize((string)a.Value))
                    .ToList();

                return base.Sequences
                    .Where(rs => !sqlServerSequences.Any(ss => ss.Name == rs.Name && ss.Schema == rs.Schema))
                    .Concat(sqlServerSequences)
                    .ToList();
            }
        }

        public override Sequence TryGetSequence(string name, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            return FindSequence(NpgsqlSequenceAnnotation + schema + "." + name)
                   ?? base.TryGetSequence(name, schema);
        }
    }
}
