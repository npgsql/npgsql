#if ENTITIES7
// NpgsqlDataStoreServices.cs
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

using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using System;
using Microsoft.Data.Entity.Storage;

namespace Npgsql
{
    public class NpgsqlDataStoreServices : DataStoreServices
	{
		private readonly NpgsqlDataStore _store;
		private readonly NpgsqlDataStoreCreator _creator;
		private readonly NpgsqlServerConnection _connection;
		private readonly ModelBuilderFactory _modelBuilderFactory;
		
		public NpgsqlDataStoreServices(NpgsqlDataStore store, NpgsqlDataStoreCreator creator, NpgsqlServerConnection connection, ModelBuilderFactory modelBuilderFactory) {
			_store = store;
			_creator = creator;
			_connection = connection;
			_modelBuilderFactory = modelBuilderFactory;
		}
		
		public override DataStoreConnection Connection {
			get {
				return _connection;
			}
		}

		public override DataStoreCreator Creator {
			get {
				return _creator;
			}
		}

		public override Database Database {
			get {
				throw new NotImplementedException();
			}
		}

		public override IModelBuilderFactory ModelBuilderFactory {
			get {
				return _modelBuilderFactory;
			}
		}

		public override DataStore Store {
			get {
				return _store;
			}
		}

		public override ValueGeneratorCache ValueGeneratorCache {
			get {
				throw new NotImplementedException();
			}
		}
	}
}

#endif