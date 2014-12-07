#if ENTITIES7
// NpgsqlTypeMapper.cs
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
using System.Data;
using System.Linq;
using Microsoft.Data.Entity.Relational;

namespace Npgsql
{
    public class NpgsqlTypeMapper : RelationalTypeMapper
	{
		
		private readonly Tuple<Type, RelationalTypeMapping>[] _simpleMappings =
		{
			Tuple.Create(typeof(int), new RelationalTypeMapping("int4", DbType.Int32)),
			Tuple.Create(typeof(DateTime), new RelationalTypeMapping("timestamp", DbType.DateTime)),
			Tuple.Create(typeof(Guid), new RelationalTypeMapping("uuid", DbType.Guid)),
			Tuple.Create(typeof(bool), new RelationalTypeMapping("bool", DbType.Boolean)),
			Tuple.Create(typeof(byte), new RelationalTypeMapping("int2", DbType.Byte)),
			Tuple.Create(typeof(double), new RelationalTypeMapping("float8", DbType.Double)),
			Tuple.Create(typeof(DateTimeOffset), new RelationalTypeMapping("timestamptz", DbType.DateTimeOffset)),
			Tuple.Create(typeof(char), new RelationalTypeMapping("int2", DbType.Int32)),
			Tuple.Create(typeof(sbyte), new RelationalTypeMapping("int2", DbType.SByte)),
			Tuple.Create(typeof(ushort), new RelationalTypeMapping("int2", DbType.UInt16)),
			Tuple.Create(typeof(uint), new RelationalTypeMapping("int4", DbType.UInt32)),
			Tuple.Create(typeof(ulong), new RelationalTypeMapping("int8", DbType.UInt64))
		};
		
		private readonly RelationalTypeMapping _nonKeyStringMapping = new RelationalTypeMapping("text", DbType.String);
		private readonly RelationalTypeMapping _nonKeyByteArrayMapping = new RelationalTypeMapping("bytea", DbType.Binary);
		private readonly RelationalTypeMapping _rowVersionMapping = new RelationalSizedTypeMapping("rowversion", DbType.Binary, 8);
		
		public override RelationalTypeMapping GetTypeMapping(string specifiedType, string storageName, Type propertyType, bool isKey, bool isConcurrencyToken) {
			propertyType = Nullable.GetUnderlyingType (propertyType) ?? propertyType;
			var mapping = _simpleMappings.FirstOrDefault(m => m.Item1 == propertyType);
			if (mapping != null) {
				return mapping.Item2;
			}
			if (propertyType == typeof(string)) {
				return _nonKeyStringMapping;
			}
			if (propertyType == typeof(byte[])) {
				if (isConcurrencyToken) {
					return _rowVersionMapping;
				}
				return _nonKeyByteArrayMapping;
			}
			return base.GetTypeMapping(specifiedType, storageName, propertyType, isKey, isConcurrencyToken);
		}

	}
}

#endif