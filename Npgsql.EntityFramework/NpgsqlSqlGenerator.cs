#if ENTITIES7
// NpgsqlSqlGenerator.cs
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

using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Utilities;

namespace Npgsql
{
	public class NpgsqlSqlGenerator : SqlGenerator {

		public override void AppendInsertOperation(StringBuilder commandStringBuilder, ModificationCommand command) {
			throw new NotImplementedException();
		}
		
		public override void AppendUpdateOperation(StringBuilder commandStringBuilder, ModificationCommand command) {
			throw new NotImplementedException();
		}
		
		public override void AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, SchemaQualifiedName schemaQualifiedName) {
			throw new NotImplementedException();
		}
		
		public override void AppendBatchHeader(StringBuilder commandStringBuilder) {
			throw new NotImplementedException();
		}
		
		protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification) {
			throw new NotImplementedException();
		}
		
		protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected) {
			throw new NotImplementedException();
		}
		
		public override string DelimitIdentifier(string identifier) {
			throw new NotImplementedException();
		}
		
		public override string EscapeIdentifier(string identifier) {
			throw new NotImplementedException();
		}
		
	}
}

#endif