using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql;

/// <inheritdoc cref="DbBatchCommandCollection"/>
public class NpgsqlBatchCommandCollection : DbBatchCommandCollection, IList<NpgsqlBatchCommand>
{
    readonly List<NpgsqlBatchCommand> _list;

    internal NpgsqlBatchCommandCollection(List<NpgsqlBatchCommand> batchCommands)
        => _list = batchCommands;

    /// <inheritdoc/>
    public override int Count => _list.Count;

    /// <inheritdoc/>
    public override bool IsReadOnly => false;

    IEnumerator<NpgsqlBatchCommand> IEnumerable<NpgsqlBatchCommand>.GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc/>
    public override IEnumerator<DbBatchCommand> GetEnumerator() => _list.GetEnumerator();

    /// <inheritdoc/>
    public void Add(NpgsqlBatchCommand item) => _list.Add(item);

    /// <inheritdoc/>
    public override void Add(DbBatchCommand item) => Add(Cast(item));

    /// <inheritdoc/>
    public override void Clear() => _list.Clear();

    /// <inheritdoc/>
    public bool Contains(NpgsqlBatchCommand item) => _list.Contains(item);

    /// <inheritdoc/>
    public override bool Contains(DbBatchCommand item) => Contains(Cast(item));

    /// <inheritdoc/>
    public void CopyTo(NpgsqlBatchCommand[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public override void CopyTo(DbBatchCommand[] array, int arrayIndex)
    {
        if (array is NpgsqlBatchCommand[] typedArray)
        {
            CopyTo(typedArray, arrayIndex);
            return;
        }

        throw new InvalidCastException(
            $"{nameof(array)} is not of type {nameof(NpgsqlBatchCommand)} and cannot be used in this batch command collection.");
    }

    /// <inheritdoc/>
    public int IndexOf(NpgsqlBatchCommand item) => _list.IndexOf(item);

    /// <inheritdoc/>
    public override int IndexOf(DbBatchCommand item) => IndexOf(Cast(item));

    /// <inheritdoc/>
    public void Insert(int index, NpgsqlBatchCommand item) => _list.Insert(index, item);

    /// <inheritdoc/>
    public override void Insert(int index, DbBatchCommand item) => Insert(index, Cast(item));

    /// <inheritdoc/>
    public bool Remove(NpgsqlBatchCommand item) => _list.Remove(item);

    /// <inheritdoc/>
    public override bool Remove(DbBatchCommand item) => Remove(Cast(item));

    /// <inheritdoc/>
    public override void RemoveAt(int index) => _list.RemoveAt(index);

    NpgsqlBatchCommand IList<NpgsqlBatchCommand>.this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    /// <inheritdoc cref="IList{T}.this" />
    public new NpgsqlBatchCommand this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    /// <inheritdoc/>
    protected override DbBatchCommand GetBatchCommand(int index)
        => _list[index];

    /// <inheritdoc/>
    protected override void SetBatchCommand(int index, DbBatchCommand batchCommand)
        => _list[index] = Cast(batchCommand);

    static NpgsqlBatchCommand Cast(DbBatchCommand? value)
    {
        var castedValue = value as NpgsqlBatchCommand;
        if (castedValue is null)
            ThrowInvalidCastException(value);

        return castedValue;
    }

    [DoesNotReturn]
    static void ThrowInvalidCastException(DbBatchCommand? value) =>
        throw new InvalidCastException(
            $"The value \"{value}\" is not of type \"{nameof(NpgsqlBatchCommand)}\" and cannot be used in this batch command collection.");
}
