// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlSequenceBuilder
    {
        private Sequence _sequence;

        public NpgsqlSequenceBuilder([NotNull] Sequence sequence)
        {
            Check.NotNull(sequence, nameof(sequence));

            _sequence = sequence;
        }

        public virtual NpgsqlSequenceBuilder IncrementBy(int increment)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                increment,
                _sequence.MinValue,
                _sequence.MaxValue,
                _sequence.Type,
                _sequence.Cycle);

            model.Npgsql().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual NpgsqlSequenceBuilder Start(long startValue)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                startValue,
                _sequence.IncrementBy,
                _sequence.MinValue,
                _sequence.MaxValue,
                _sequence.Type,
                _sequence.Cycle);

            model.Npgsql().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual NpgsqlSequenceBuilder Type<T>()
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                _sequence.IncrementBy,
                _sequence.MinValue,
                _sequence.MaxValue,
                typeof(T),
                _sequence.Cycle);

            model.Npgsql().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual NpgsqlSequenceBuilder Max(long maximum)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                _sequence.IncrementBy,
                _sequence.MinValue,
                maximum,
                _sequence.Type,
                _sequence.Cycle);

            model.Npgsql().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual NpgsqlSequenceBuilder Min(long minimum)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                _sequence.IncrementBy,
                minimum,
                _sequence.MaxValue,
                _sequence.Type,
                _sequence.Cycle);

            model.Npgsql().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual NpgsqlSequenceBuilder Cycle(bool cycle = true)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                _sequence.IncrementBy,
                _sequence.MinValue,
                _sequence.MaxValue,
                _sequence.Type,
                cycle);

            model.Npgsql().AddOrReplaceSequence(_sequence);

            return this;
        }
    }
}
