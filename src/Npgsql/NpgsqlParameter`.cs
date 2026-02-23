using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using NpgsqlTypes;

namespace Npgsql;

/// <summary>
/// A generic version of <see cref="NpgsqlParameter"/> which provides more type safety and
/// avoids boxing of value types. Use <see cref="TypedValue"/> instead of <see cref="NpgsqlParameter.Value"/>.
/// </summary>
/// <typeparam name="T">The type of the value that will be stored in the parameter.</typeparam>
public sealed class NpgsqlParameter<T> : NpgsqlParameter
{
    T? _typedValue;

    /// <summary>
    /// Gets or sets the strongly-typed value of the parameter.
    /// </summary>
    public T? TypedValue
    {
        get => _typedValue;
        set
        {
            if (typeof(T) == typeof(object) && ShouldResetObjectTypeInfo(value))
                ResetTypeInfo();
            else
                DisposeBindingState();
            _typedValue = value;
        }
    }

    /// <summary>
    /// Gets or sets the value of the parameter. This delegates to <see cref="TypedValue"/>.
    /// </summary>
    public override object? Value
    {
        get => TypedValue;
        set => TypedValue = (T)value!;
    }

    private protected override Type StaticValueType => typeof(T);

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="NpgsqlParameter{T}" />.
    /// </summary>
    public NpgsqlParameter() { }

    /// <summary>
    /// Initializes a new instance of <see cref="NpgsqlParameter{T}" /> with a parameter name and value.
    /// </summary>
    public NpgsqlParameter(string parameterName, T value)
    {
        ParameterName = parameterName;
        TypedValue = value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NpgsqlParameter{T}" /> with a parameter name and type.
    /// </summary>
    public NpgsqlParameter(string parameterName, NpgsqlDbType npgsqlDbType)
    {
        ParameterName = parameterName;
        NpgsqlDbType = npgsqlDbType;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NpgsqlParameter{T}" /> with a parameter name and type.
    /// </summary>
    public NpgsqlParameter(string parameterName, DbType dbType)
    {
        ParameterName = parameterName;
        DbType = dbType;
    }

    #endregion Constructors

    private protected override void SetOutputValueCore(NpgsqlDataReader reader, int ordinal)
        => TypedValue = reader.GetFieldValue<T>(ordinal);

    private protected override PgConcreteTypeInfo GetConcreteTypeInfo(PgTypeInfo typeInfo)
    {
        if (typeof(T) == typeof(object) || TypeInfo!.IsBoxing)
            return base.GetConcreteTypeInfo(typeInfo);

        return typeInfo.GetConcreteTypeInfo(TypedValue);
    }

    private protected override PgValueBindingContext BindGenericValue(PgConcreteTypeInfo typeInfo, DataFormat? formatPreference)
        => typeInfo.BindValue(TypedValue, formatPreference);

    private protected override ValueTask WriteGenericValue(bool async, PgConcreteTypeInfo typeInfo, PgWriter writer, CancellationToken cancellationToken)
    {
        Debug.Assert(TypedValue is not null);
        if (async)
            return typeInfo.ConverterWriteAsync(writer, TypedValue!, cancellationToken);

        typeInfo.ConverterWrite(writer, TypedValue!);
        return new();
    }

    private protected override NpgsqlParameter CloneCore() =>
        // use fields instead of properties
        // to avoid auto-initializing something like type_info
        new NpgsqlParameter<T>
        {
            _precision = _precision,
            _scale = _scale,
            _size = _size,
            _dbType = _dbType,
            _npgsqlDbType = _npgsqlDbType,
            _dataTypeName = _dataTypeName,
            Direction = Direction,
            IsNullable = IsNullable,
            _name = _name,
            TrimmedName = TrimmedName,
            SourceColumn = SourceColumn,
            SourceVersion = SourceVersion,
            TypedValue = TypedValue,
            SourceColumnNullMapping = SourceColumnNullMapping,
        };
}
