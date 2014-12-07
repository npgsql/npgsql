#if ENTITIES7
// NpgsqlBatchPreparer.cs
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

using System;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Framework.Logging;

namespace Npgsql
{
    public class NpgsqlBatchPreparer : CommandBatchPreparer
	{
		
		public NpgsqlBatchPreparer(NpgsqlModificationCommandBatchFactory modificationCommandBatchFactory, ParameterNameGeneratorFactory parameterNameGeneratorFactory, ModificationCommandComparer modificationCommandComparer) : base(modificationCommandBatchFactory, parameterNameGeneratorFactory, modificationCommandComparer) {
		}
		
		public override IRelationalPropertyExtensions GetPropertyExtensions(IProperty property) {
			throw new NotImplementedException();
		}
		
		public override IRelationalEntityTypeExtensions GetEntityTypeExtensions(IEntityType entityType) {
			throw new NotImplementedException();
		}

	}
}

#endif