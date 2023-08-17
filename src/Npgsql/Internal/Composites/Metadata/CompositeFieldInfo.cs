using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using Npgsql.Util;

namespace Npgsql.Internal.Composites;

abstract class CompositeFieldInfo
{
    protected PgConverter Converter { get; }
    protected BufferRequirements _binaryBufferRequirements;

    private protected CompositeFieldInfo(string name, PgConverterResolution resolution)
    {
        Name = name;
        Converter = resolution.Converter;
        PgTypeId = resolution.PgTypeId;

        if (!Converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements))
            throw new InvalidOperationException("Converter must support binary format to participate in composite types.");
    }

    protected PgConverter<T> GetConverter<T>() => (PgConverter<T>)Converter;

    protected ValueTask ReadAsObject(bool async, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken)
    {
        if (async)
        {
            var task = Converter.ReadAsObjectAsync(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return Core(builder, task);

            AddValue(builder, task.Result);
        }
        else
            AddValue(builder, Converter.ReadAsObject(reader));
        return new();
#if NET6_0_OR_GREATER
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
#endif
        async ValueTask Core(CompositeBuilder builder, ValueTask<object> task)
        {
            builder.AddValue(await task.ConfigureAwait(false));
        }
    }

    protected ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        if (async)
            return Converter.WriteAsObjectAsync(writer, value, cancellationToken);

        Converter.WriteAsObject(writer, value);
        return new();
    }

    public string Name { get; }
    public PgTypeId PgTypeId { get; }
    public Size BinaryReadRequirement => _binaryBufferRequirements.Read;
    public Size BinaryWriteRequirement => _binaryBufferRequirements.Write;

    public abstract Type Type { get; }

    protected abstract void AddValue(CompositeBuilder builder, object value);

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
    readonly bool _asObject;

    CompositeFieldInfo(string name, PgConverterResolution resolution, Func<object, T> getter)
        : base(name, resolution)
    {
        var typeToConvert = resolution.Converter.TypeToConvert;
        if (!typeToConvert.IsAssignableFrom(typeof(T)))
            throw new InvalidOperationException($"Converter type '{typeToConvert}' must be assignable from field type '{typeof(T)}'.");

        _getter = getter;
        _asObject = typeToConvert != typeof(T);
    }

    public CompositeFieldInfo(string name, PgConverterResolution resolution, Func<object, T> getter, int parameterIndex)
        : this(name, resolution, getter)
        => _parameterIndex = parameterIndex;

    public CompositeFieldInfo(string name, PgConverterResolution resolution, Func<object, T> getter, Action<object, T> setter)
        : this(name, resolution, getter)
        => _setter = setter;

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

    protected override void AddValue(CompositeBuilder builder, object value) => builder.AddValue((T)value);

    public override ValueTask Read(bool async, CompositeBuilder builder, PgReader reader, CancellationToken cancellationToken = default)
    {
        if (_asObject)
            return ReadAsObject(async, builder, reader, cancellationToken);

        if (async)
        {
            var task = GetConverter<T>().ReadAsync(reader, cancellationToken);
            if (!task.IsCompletedSuccessfully)
                return Core(builder, task);

            builder.AddValue(task.Result);
        }
        else
            builder.AddValue(GetConverter<T>().Read(reader));
        return new();
#if NET6_0_OR_GREATER
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
#endif
        async ValueTask Core(CompositeBuilder builder, ValueTask<T> task)
        {
            builder.AddValue(await task.ConfigureAwait(false));
        }
    }

    public override bool IsDbNull(object instance)
    {
        var value = _getter(instance);
        return _asObject ? Converter.IsDbNullAsObject(value) : GetConverter<T>().IsDbNull(_getter(instance));
    }

    public override Size? GetSizeOrDbNull(DataFormat format, object instance, ref object? writeState)
    {
        var value = _getter(instance);
        return _asObject
            ? Converter.GetSizeOrDbNullAsObject(format, _binaryBufferRequirements.Write, value, ref writeState)
            : GetConverter<T>().GetSizeOrDbNull(format, _binaryBufferRequirements.Write, value, ref writeState);
    }

    public override ValueTask Write(bool async, PgWriter writer, object instance, CancellationToken cancellationToken)
    {
        var value = _getter(instance);
        if (_asObject)
            return WriteAsObject(async, writer, value!, cancellationToken);

        if (async)
            return GetConverter<T>().WriteAsync(writer, value!, cancellationToken);

        GetConverter<T>().Write(writer, value!);
        return new();
    }
}
