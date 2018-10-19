using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.TypeMapping;

// This file contains some pretty awful hacks to make current version of the EF Core provider
// compatible with the new type mapping/handling system introduced in Npgsql 4.0.
// The EF Core provider dynamically loads its type mappings from Npgsql, which allows it to
// automatically support any type supported by Npgsql. Unfortunately, the current loading
// system is very tightly coupled to pre-3.2 type mapping types (e.g. TypeHandlerRegistry),
// and so this shim is required.

// ReSharper disable once CheckNamespace
namespace Npgsql
{
    [Obsolete("Purely for EF Core backwards compatibility")]
    class TypeHandlerRegistry
    {
        internal static readonly Dictionary<string, TypeAndMapping> HandlerTypes;

        static TypeHandlerRegistry()
        {
            HandlerTypes = GlobalTypeMapper.Instance.Mappings.Values.ToDictionary(
                m => m.PgTypeName,
                m => new TypeAndMapping
                {
                    HandlerType = typeof(TypeHandler<>).MakeGenericType(m.TypeHandlerFactory.DefaultValueType),
                    Mapping = new TypeMappingAttribute(m.PgTypeName, m.NpgsqlDbType,
                        m.DbTypes, m.ClrTypes, m.InferredDbType)
                }
            );
        }
    }

    [Obsolete("Purely for EF Core backwards compatibility")]
    class TypeHandler<T> {}

    [Obsolete("Purely for EF Core backwards compatibility")]
    struct TypeAndMapping
    {
        internal Type HandlerType;
        internal TypeMappingAttribute Mapping;
    }
}
