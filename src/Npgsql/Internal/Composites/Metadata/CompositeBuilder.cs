using System;
using System.Buffers;
using Npgsql.Util;

namespace Npgsql.Internal.Composites;

abstract class CompositeBuilder(StrongBox[] tempBoxes)
{
    protected StrongBox[] _tempBoxes = tempBoxes;
    protected int _currentField;

    protected abstract void Construct();
    protected abstract void SetField<TValue>(TValue value);

    public void AddValue<TValue>(TValue value)
    {
        var tempBoxes = _tempBoxes;
        var currentField = _currentField;
        if (currentField >= tempBoxes.Length)
        {
            if (currentField == tempBoxes.Length)
                Construct();
            SetField(value);
        }
        else
        {
            ((StrongBox<TValue>)tempBoxes[currentField]).TypedValue = value;
            if (currentField + 1 == tempBoxes.Length)
                Construct();
        }

        _currentField++;
    }
}

sealed class CompositeBuilder<T>(CompositeInfo<T> compositeInfo) : CompositeBuilder(compositeInfo.CreateTempBoxes()), IDisposable
{
    T _instance = default!;
    object? _boxedInstance;

    public T Complete()
    {
        if (_currentField < compositeInfo.Fields.Count)
            throw new InvalidOperationException($"Missing values, expected: {compositeInfo.Fields.Count} got: {_currentField}");

        return (T)(_boxedInstance ?? _instance!);
    }

    public void Reset()
    {
        _instance = default!;
        _boxedInstance = null;
        _currentField = 0;
        foreach (var box in _tempBoxes)
            box.Clear();
    }

    public void Dispose() => Reset();

    protected override void Construct()
    {
        var tempBoxes = _tempBoxes;
        if (_currentField < tempBoxes.Length - 1)
            throw new InvalidOperationException($"Missing values, expected: {tempBoxes.Length} got: {_currentField + 1}");

        var fields = compositeInfo.Fields;
        var args = ArrayPool<StrongBox>.Shared.Rent(compositeInfo.ConstructorParameters);
        for (var i = 0; i < tempBoxes.Length; i++)
        {
            var field = fields[i];
            if (field.ConstructorParameterIndex is { } argIndex)
                args[argIndex] = tempBoxes[i];
        }
        _instance = compositeInfo.Constructor(args)!;
        ArrayPool<StrongBox>.Shared.Return(args);

        if (tempBoxes.Length == compositeInfo.Fields.Count)
            return;

        // We're expecting or already have stored more fields, so box the instance once here.
        _boxedInstance = _instance;
        for (var i = 0; i < tempBoxes.Length; i++)
        {
            var field = compositeInfo.Fields[i];
            if (field.ConstructorParameterIndex is null)
                field.Set(_boxedInstance, tempBoxes[i]);
        }
    }

    protected override void SetField<TValue>(TValue value)
    {
        if (_boxedInstance is null)
            ThrowHelper.ThrowInvalidOperationException("Not constructed yet, or no more fields were expected.");

        var currentField = _currentField;
        var fields = compositeInfo.Fields;
        if (currentField > fields.Count - 1)
            ThrowHelper.ThrowIndexOutOfRangeException($"Cannot set field {value} at position {currentField} - all fields have already been set");

        ((CompositeFieldInfo<TValue>)fields[currentField]).Set(_boxedInstance, value);
    }
}
