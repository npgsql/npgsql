using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using Npgsql.Util;

namespace Npgsql.Internal.Composites;

abstract class CompositeFieldInfo
{
    private protected PgConverter _converter;
    private protected BufferRequirements _binaryBufferRequirements;

    protected CompositeFieldInfo(string name, PgConverterResolution resolution)
    {
        Name = name;
        _converter = resolution.Converter;
        PgTypeId = resolution.PgTypeId;

        if (!_converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements))
            throw new InvalidOperationException("Converter must support binary format to participate in composite types.");
    }

    public string Name { get; }
    public PgTypeId PgTypeId { get; }
    public Size BinaryReadRequirement => _binaryBufferRequirements.Read;
    public Size BinaryWriteRequirement => _binaryBufferRequirements.Write;

    public abstract Type Type { get; }

    public abstract StrongBox CreateBox();
    public abstract void Set(object instance, StrongBox value);
    public abstract int? ConstructorParameterIndex { get; }

    public abstract void ReadDbNull(CompositeBuilder builder);
    public abstract ValueTask Read(bool async, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken = default);
    public abstract bool IsDbNull(object instance);
    public abstract Size? GetSizeOrDbNull(DataFormat format, object instance, ref object? writeState);
    public abstract ValueTask Write(bool async, PgWriter writer, object instance, CancellationToken cancellationToken);
}

sealed class CompositeFieldInfo<T> : CompositeFieldInfo
{
    readonly Action<object, T>? _setter;
    readonly int _parameterIndex;
    readonly Func<object, T> _getter;

    public CompositeFieldInfo(string name, PgConverterResolution resolution, Func<object, T> getter, int parameterIndex)
        : base(name, resolution)
    {
        _getter = getter;
        _parameterIndex = parameterIndex;
    }

    public CompositeFieldInfo(string name, PgConverterResolution resolution, Func<object, T> getter, Action<object, T> setter)
        : base(name, resolution)
    {
        _getter = getter;
        _setter = setter;
    }

    PgConverter<T> Converter
    {
        get
        {
            Debug.Assert(_converter is PgConverter<T>);
            return Unsafe.As<PgConverter, PgConverter<T>>(ref _converter);
        }
    }

    public override Type Type => typeof(T);

    public override int? ConstructorParameterIndex => _setter is not null ? null : _parameterIndex;

    public T Get(object instance) => _getter(instance);

    public override StrongBox CreateBox() => new Util.StrongBox<T>();

    public void Set(object instance, T value)
    {
        if (_setter is null)
            throw new InvalidOperationException("Not a composite field for a clr field.");

        _setter(instance, value);
    }

    public override void Set(object instance, StrongBox value)
    {
        if (_setter is null)
            throw new InvalidOperationException("Not a composite field for a clr field.");

        _setter(instance, ((Util.StrongBox<T>)value).TypedValue!);
    }

    public override void ReadDbNull(CompositeBuilder builder)
    {
        if (default(T) != null)
            throw new InvalidCastException($"Type {typeof(T).FullName} does not have null as a possible value.");

        builder.AddValue((T?)default);
    }

    public override ValueTask Read(bool async, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken = default)
    {
        if (async)
        {
            var task = Converter.ReadAsync(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return Core(builder, task);

            builder.AddValue(task.Result);
        }
        else
            builder.AddValue(Converter.Read(reader));
        return new();

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask Core(CompositeBuilder builder, ValueTask<T> task)
        {
            builder.AddValue(await task);
        }
    }

    public override bool IsDbNull(object instance) => Converter.IsDbNull(_getter(instance));

    public override Size? GetSizeOrDbNull(DataFormat format, object instance, ref object? writeState)
    {
        var value = _getter(instance);
        return Converter.GetSizeOrDbNull(format, _binaryBufferRequirements.Write, value, ref writeState);
    }

    public override ValueTask Write(bool async, PgWriter writer, object instance, CancellationToken cancellationToken)
    {
        var value = _getter(instance);
        if (async)
            return Converter.WriteAsync(writer, value!, cancellationToken);

        Converter.Write(writer, value!);
        return new();
    }
}
