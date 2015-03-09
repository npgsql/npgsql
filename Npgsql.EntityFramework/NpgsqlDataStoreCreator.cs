#if ENTITIES7
// NpgsqlDataStoreCreator.cs
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

using Microsoft.Data.Entity.Metadata;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Storage;

namespace Npgsql
{
    public class NpgsqlDataStoreCreator : DataStoreCreator
	{
		
		public NpgsqlDataStoreCreator() {
		}

		
		public override bool EnsureCreated (IModel model) {
			throw new NotImplementedException();
		}

		public override Task<bool> EnsureCreatedAsync (IModel model, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
		
		public virtual Task<bool> EnsureCreatedAsync (IModel model) {
			return EnsureCreatedAsync(model, default(CancellationToken));
		}

		public override bool EnsureDeleted (IModel model) {
			throw new NotImplementedException();
		}

		public override Task<bool> EnsureDeletedAsync (IModel model, CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
		
		public virtual Task<bool> EnsureDeletedAsync (IModel model) {
			return EnsureDeletedAsync(model, default(CancellationToken));
		}
		
	}
}

#endif