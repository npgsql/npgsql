using System.Diagnostics.CodeAnalysis;

namespace Npgsql.Util;

abstract class StrongBox
{
    private protected StrongBox() { }
    public abstract bool HasValue { get; }
    public abstract object? Value { get; set; }
    public abstract void Clear();
}

sealed class StrongBox<T> : StrongBox
{
    bool _hasValue;

    [MaybeNull] T _typedValue;
    [MaybeNull]
    public T TypedValue {
        get => _typedValue;
        set
        {
            _hasValue = true;
            _typedValue = value;
        }
    }

    public override bool HasValue => _hasValue;

    public override object? Value
    {
        get => TypedValue;
        set => TypedValue = (T)value!;
    }

    public override void Clear()
    {
        _hasValue = false;
        TypedValue = default!;
    }
}
