using System;
using System.Collections.Generic;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.TypeMapping;

sealed class UserMappedTypeHandlerResolverFactory : TypeHandlerResolverFactory
{
    public IUserTypeMapping Mapping { get; }

    public UserMappedTypeHandlerResolverFactory(IUserTypeMapping mapping) => Mapping = mapping;

    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new UserMappedTypeHandlerResolver(connector, Mapping);

    public override string? GetDataTypeNameByClrType(Type clrType)
        => null;

    public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
        => null;
}
