// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Linq;
using Microsoft.Data.Entity.Relational;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlTypeMapper : RelationalTypeMapper
    {
        // This dictionary is for invariant mappings from a sealed CLR type to a single
        // store type. If the CLR type is unsealed or if the mapping varies based on how the
        // type is used (e.g. in keys), then add custom mapping below.
        private readonly Tuple<Type, RelationalTypeMapping>[] _simpleMappings =
            {
                Tuple.Create(typeof(string), new RelationalTypeMapping("text", DbType.String)),
                Tuple.Create(typeof(DateTime), new RelationalTypeMapping("timestamp", DbType.DateTime2)),
                Tuple.Create(typeof(Guid), new RelationalTypeMapping("uuid", DbType.Guid)),
                Tuple.Create(typeof(byte), new RelationalTypeMapping("smallint", DbType.Byte)),
                //Tuple.Create(typeof(char), new RelationalTypeMapping("int", DbType.Int32)),
                Tuple.Create(typeof(sbyte), new RelationalTypeMapping("smallint", DbType.SByte)),
                Tuple.Create(typeof(ushort), new RelationalTypeMapping("int", DbType.UInt16)),
                Tuple.Create(typeof(uint), new RelationalTypeMapping("bigint", DbType.UInt32)),
                Tuple.Create(typeof(ulong), new RelationalTypeMapping("numeric(20, 0)", DbType.UInt64)),
                Tuple.Create(typeof(byte[]), new RelationalTypeMapping("bytea", DbType.Binary)),
            };

        public override RelationalTypeMapping GetTypeMapping(
            string specifiedType, string storageName, Type propertyType, bool isKey, bool isConcurrencyToken)
        {
            propertyType = propertyType.UnwrapNullableType();

            var mapping = _simpleMappings.FirstOrDefault(m => m.Item1 == propertyType);
            if (mapping != null)
            {
                return mapping.Item2;
            }

            // TODO: DateTime based on kind...

            return base.GetTypeMapping(specifiedType, storageName, propertyType, isKey, isConcurrencyToken);
        }
    }
}
