// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace Npgsql.EntityFramework7.Metadata
{
    public class NpgsqlModelBuilder
    {
        private readonly Model _model;

        public NpgsqlModelBuilder([NotNull] Model model)
        {
            Check.NotNull(model, nameof(model));

            _model = model;
        }

        public virtual NpgsqlModelBuilder UseIdentity()
        {
            _model.Npgsql().ValueGenerationStrategy = NpgsqlValueGenerationStrategy.Identity;

            return this;
        }

        public virtual NpgsqlModelBuilder UseSequence()
        {
            var extensions = _model.Npgsql();

            extensions.ValueGenerationStrategy = NpgsqlValueGenerationStrategy.Sequence;
            extensions.DefaultSequenceName = null;
            extensions.DefaultSequenceSchema = null;

            return this;
        }

        public virtual NpgsqlModelBuilder UseSequence([NotNull] string name, [CanBeNull] string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, "schema");

            var extensions = _model.Npgsql();

            var sequence = extensions.GetOrAddSequence(name, schema);

            extensions.ValueGenerationStrategy = NpgsqlValueGenerationStrategy.Sequence;
            extensions.DefaultSequenceName = sequence.Name;
            extensions.DefaultSequenceSchema = sequence.Schema;

            return this;
        }

        public virtual NpgsqlSequenceBuilder Sequence([CanBeNull] string name = null, [CanBeNull] string schema = null)
        {
            Check.NullButNotEmpty(name, "name");
            Check.NullButNotEmpty(schema, "schema");

            return new NpgsqlSequenceBuilder(_model.Npgsql().GetOrAddSequence(name, schema));
        }
    }
}
