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
        => IsDataTypeName ? _dataTypeName : throw new InvalidOperationException("This value does not describe a DataTypeName.");

    public Oid Oid
        => IsOid ? _oid : throw new InvalidOperationException("This value does not describe an Oid.");

    public static implicit operator PgTypeId(DataTypeName name) => new(name);
    public static implicit operator PgTypeId(Oid id) => new(id);

    public bool Equals(PgTypeId other)
        => (this, other) switch
        {
            ({ IsOid: true }, { IsOid: true }) => _oid == other._oid,
            ({ IsDataTypeName: true }, { IsDataTypeName: true }) => _dataTypeName.Equals(other._dataTypeName),
            _ => false
        };

    public override int GetHashCode() => IsOid ? _oid.GetHashCode() : _dataTypeName.GetHashCode();

    public override string ToString() => IsOid ? _oid.ToString() : _dataTypeName.Value;
}
