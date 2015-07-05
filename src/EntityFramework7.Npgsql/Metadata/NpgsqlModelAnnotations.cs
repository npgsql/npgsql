// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlModelAnnotations : ReadOnlyNpgsqlModelAnnotations
    {
        public NpgsqlModelAnnotations([NotNull] Model model)
            : base(model)
        {
        }

        public new virtual string DefaultSequenceName
        {
            get { return base.DefaultSequenceName; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

                ((Model)Model)[NpgsqlDefaultSequenceNameAnnotation] = value;
            }
        }

        public new virtual string DefaultSequenceSchema
        {
            get { return base.DefaultSequenceSchema; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

                ((Model)Model)[NpgsqlDefaultSequenceSchemaAnnotation] = value;
            }
        }

        public virtual Sequence AddOrReplaceSequence([NotNull] Sequence sequence)
        {
            Check.NotNull(sequence, nameof(sequence));

            var model = (Model)Model;
            sequence.Model = model;
            model[NpgsqlSequenceAnnotation + sequence.Schema + "." + sequence.Name] = sequence.Serialize();

            return sequence;
        }

        public virtual Sequence GetOrAddSequence([CanBeNull] string name = null, [CanBeNull] string schema = null)
        {
            Check.NullButNotEmpty(name, "name");
            Check.NullButNotEmpty(schema, "schema");

            name = name ?? Sequence.DefaultName;

            return ((Model)Model).Npgsql().TryGetSequence(name, schema)
                   ?? AddOrReplaceSequence(new Sequence(name, schema));
        }
    }
}
