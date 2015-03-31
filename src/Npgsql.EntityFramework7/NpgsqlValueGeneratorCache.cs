// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlValueGeneratorCache : ValueGeneratorCache, INpgsqlValueGeneratorCache
    {
        private readonly ThreadSafeDictionaryCache<string, NpgsqlSequenceValueGeneratorState> _sequenceGeneratorCache
            = new ThreadSafeDictionaryCache<string, NpgsqlSequenceValueGeneratorState>();

        public virtual NpgsqlSequenceValueGeneratorState GetOrAddSequenceState(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            return _sequenceGeneratorCache.GetOrAdd(
                GetSequenceName(property),
                sequenceName => new NpgsqlSequenceValueGeneratorState(sequenceName, GetBlockSize(property), GetPoolSize(property)));
        }

        public virtual int GetBlockSize([NotNull] IProperty property)
        {
            Check.NotNull(property, nameof(property));

            var incrementBy = property.Npgsql().TryGetSequence().IncrementBy;

            if (incrementBy <= 0)
            {
                throw new NotSupportedException(Strings.SequenceBadBlockSize(incrementBy, GetSequenceName(property)));
            }

            return incrementBy;
        }

        public virtual string GetSequenceName([NotNull] IProperty property)
        {
            Check.NotNull(property, nameof(property));

            var sequence = property.Npgsql().TryGetSequence();

            return (sequence.Schema == null ? "" : (sequence.Schema + ".")) + sequence.Name;
        }

        public virtual int GetPoolSize([NotNull] IProperty property)
        {
            Check.NotNull(property, nameof(property));

            // TODO: Allow configuration without creation of derived factory type
            // Issue #778
            return 5;
        }
    }
}
