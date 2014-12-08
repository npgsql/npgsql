#if ENTITIES7
// NpgsqlDataStore.cs
//
// Author:
//    Dylan Borg (borgdylan@hotmail.com)
//
//    Copyright (C) 2014 Dylan Borg
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query;
using Microsoft.Framework.Logging;
using Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Storage;

namespace Npgsql
{
    public class NpgsqlDataStore : DataStore
	{
		public NpgsqlDataStore (StateManager stateManager, DbContextService<IModel> model, EntityKeyFactorySource entityKeyFactorySource, EntityMaterializerSource entityMaterializerSource, ClrCollectionAccessorSource collectionAccessorSource, ClrPropertySetterSource propertySetterSource, ILoggerFactory loggerFactory) : base(stateManager, model, entityKeyFactorySource, entityMaterializerSource, collectionAccessorSource, propertySetterSource, loggerFactory) {
		}

		public NpgsqlDataStore() {
		}

		//
		// Methods
		//
		public override IAsyncEnumerable<TResult> AsyncQuery<TResult> (QueryModel queryModel, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
		
		public override IEnumerable<TResult> Query<TResult> (QueryModel queryModel) {
			throw new NotImplementedException();
		}

		public override int SaveChanges (IReadOnlyList<StateEntry> stateEntries) {
			throw new NotImplementedException();
		}

		public override Task<int> SaveChangesAsync ( IReadOnlyList<StateEntry> stateEntries, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
		
		public virtual Task<int> SaveChangesAsync ( IReadOnlyList<StateEntry> stateEntries) {
			return SaveChangesAsync(stateEntries, default(CancellationToken));
		}
	}
}

#endif