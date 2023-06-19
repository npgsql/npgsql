using System;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql.PostgresTypes;

public readonly record struct PgTypeId
{
    readonly DataTypeName _dataTypeName;
    readonly Oid _oid;

    public PgTypeId(DataTypeName name) => _dataTypeName = name;
    public PgTypeId(Oid oid) => _oid = oid;

    [MemberNotNullWhen(true, nameof(_dataTypeName))]
    public bool IsDataTypeName => !_dataTypeName.IsDefault;
    public bool IsOid => _dataTypeName.IsDefault;

    public DataTypeName DataTypeName
    {
        get
        {
            if (!IsDataTypeName)
                throw new InvalidOperationException("This value does not describe a DataTypeName.");

            return _dataTypeName;
        }
    }

    public Oid Oid
    {
        get
        {
            if (!IsOid)
                throw new InvalidOperationException("This value does not describe an Oid.");

            return _oid;
        }
    }

    public static implicit operator PgTypeId(DataTypeName name) => new(name);
    public static implicit operator PgTypeId(Oid id) => new(id);

    public bool Equals(PgTypeId other)
        => IsOid ? _oid.Value == other._oid.Value : _dataTypeName.Value == other._dataTypeName.Value;

    public override int GetHashCode() => IsOid ? _oid.GetHashCode() : _dataTypeName.GetHashCode();

    public override string ToString() => IsOid ? _oid.ToString() : _dataTypeName.Value;
}
