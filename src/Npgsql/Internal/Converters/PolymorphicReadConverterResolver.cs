using System;
using Npgsql.Internal.Descriptors;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.Converters;

abstract class PolymorphicReadConverterResolver : PgConverterResolver<object>
{
    protected PolymorphicReadConverterResolver(PgTypeId pgTypeId) => PgTypeId = pgTypeId;

    protected PgTypeId PgTypeId { get; }

    protected abstract PgConverter Get(Field? field);

    public sealed override PgConverterResolution GetDefault(PgTypeId pgTypeId)
    {
        if (pgTypeId != PgTypeId)
            throw CreateUnsupportedPgTypeIdException(pgTypeId);

        var converter = Get(null);
        return new(converter, PgTypeId);
    }

    public sealed override PgConverterResolution Get(object? value, PgTypeId? expectedPgTypeId)
        => throw new NotSupportedException("Polymorphic writing is not supported, try to resolve a converter by the type of an actual value instead.");

    public sealed override PgConverterResolution Get(Field field)
    {
        if (field.PgTypeId != PgTypeId)
            throw CreateUnsupportedPgTypeIdException(field.PgTypeId);

        var converter = Get(field);
        return new(converter, PgTypeId);
    }
}
