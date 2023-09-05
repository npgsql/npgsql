using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
    /// <summary>
    /// Gets or sets the strongly-typed value of the parameter.
    /// </summary>
    public T? TypedValue { get; set; }

    /// <summary>
    /// Gets or sets the value of the parameter. This delegates to <see cref="TypedValue"/>.
    /// </summary>
    public override object? Value
    {
        get => TypedValue;
        set => TypedValue = (T)value!;
    }

    internal override Type? ValueType => typeof(T);

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

    private protected override PgConverterResolution ResolveConverter(PgTypeInfo typeInfo) => typeInfo.GetResolution(TypedValue);

    private protected override void BindCore(bool allowNullReference = false)
    {
        if (AsObject)
        {
            base.BindCore(_useSubStream || allowNullReference);
            return;
        }

        var value = TypedValue;
        Debug.Assert(Converter is PgConverter<T>);
        if (TypeInfo!.Bind(Unsafe.As<PgConverter<T>>(Converter), value, out var size, out _writeState, out var dataFormat) is { } info)
        {
            WriteSize = size;
            _bufferRequirement = info.BufferRequirement;
        }
        else
        {
            WriteSize = -1;
            _bufferRequirement = default;
        }
        Format = dataFormat;
    }

    private protected override ValueTask WriteValue(bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        if (AsObject)
            return base.WriteValue(async, writer, cancellationToken);

        Debug.Assert(Converter is PgConverter<T>);
        if (async)
            return Unsafe.As<PgConverter<T>>(Converter!).WriteAsync(writer, TypedValue!, cancellationToken);

        Unsafe.As<PgConverter<T>>(Converter!).Write(writer, TypedValue!);
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
