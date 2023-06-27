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

        return new(Get(null), PgTypeId);
    }

    public sealed override PgConverterResolution Get(object? value, PgTypeId? expectedPgTypeId)
        => new(Get(null), PgTypeId);

    public sealed override PgConverterResolution Get(Field field)
    {
        if (field.PgTypeId != PgTypeId)
            throw CreateUnsupportedPgTypeIdException(field.PgTypeId);

        var converter = Get(field);
        return new(converter, PgTypeId);
    }
}
