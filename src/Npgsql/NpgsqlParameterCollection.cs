using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal;
using NpgsqlTypes;

namespace Npgsql;

/// <summary>
/// Represents a collection of parameters relevant to a <see cref="NpgsqlCommand"/> as well as their respective mappings to columns in
/// a <see cref="DataSet"/>.
/// </summary>
public sealed class NpgsqlParameterCollection : DbParameterCollection, IList<NpgsqlParameter>
{
    internal const int LookupThreshold = 5;

    internal List<NpgsqlParameter> InternalList { get; } = new(5);
#if DEBUG
    internal static bool TwoPassCompatMode;
#else
    internal static readonly bool TwoPassCompatMode;
#endif

    static NpgsqlParameterCollection()
        => TwoPassCompatMode = AppContext.TryGetSwitch("Npgsql.EnableLegacyCaseInsensitiveDbParameters", out var enabled)
                               && enabled;

    // Dictionary lookups for GetValue to improve performance. _caseSensitiveLookup is only ever used in legacy two-pass mode.
    Dictionary<string, int>? _caseInsensitiveLookup;
    Dictionary<string, int>? _caseSensitiveLookup;

    /// <summary>
    /// Initializes a new instance of the NpgsqlParameterCollection class.
    /// </summary>
    internal NpgsqlParameterCollection() { }

    bool LookupEnabled => InternalList.Count >= LookupThreshold;

    void LookupClear()
    {
        _caseInsensitiveLookup?.Clear();
        _caseSensitiveLookup?.Clear();
    }

    void LookupAdd(string name, int index)
    {
        if (_caseInsensitiveLookup is null)
            return;

        if (TwoPassCompatMode)
            _caseSensitiveLookup!.TryAdd(name, index);

        _caseInsensitiveLookup.TryAdd(name, index);
    }

    void LookupInsert(string name, int index)
    {
        if (_caseInsensitiveLookup is null)
            return;

        if (TwoPassCompatMode &&
            (!_caseSensitiveLookup!.TryGetValue(name, out var indexCs) || index < indexCs))
        {
            for (var i = index + 1; i < InternalList.Count; i++)
            {
                var parameterName = InternalList[i].TrimmedName;
                if (_caseSensitiveLookup.TryGetValue(parameterName, out var currentI) && currentI + 1 == i)
                    _caseSensitiveLookup[parameterName] = i;
            }

            _caseSensitiveLookup[name] = index;
        }

        if (!_caseInsensitiveLookup.TryGetValue(name, out var indexCi) || index < indexCi)
        {
            for (var i = index + 1; i < InternalList.Count; i++)
            {
                var parameterName = InternalList[i].TrimmedName;
                if (_caseInsensitiveLookup.TryGetValue(parameterName, out var currentI) && currentI + 1 == i)
                    _caseInsensitiveLookup[parameterName] = i;
            }

            _caseInsensitiveLookup[name] = index;
        }
    }

    void LookupRemove(string name, int index)
    {
        if (_caseInsensitiveLookup is null)
            return;

        if (TwoPassCompatMode && _caseSensitiveLookup!.Remove(name))
        {
            for (var i = index; i < InternalList.Count; i++)
            {
                var parameterName = InternalList[i].TrimmedName;
                if (_caseSensitiveLookup.TryGetValue(parameterName, out var currentI) && currentI - 1 == i)
                    _caseSensitiveLookup[parameterName] = i;
            }
        }

        if (_caseInsensitiveLookup.Remove(name))
        {
            for (var i = index; i < InternalList.Count; i++)
            {
                var parameterName = InternalList[i].TrimmedName;
                if (_caseInsensitiveLookup.TryGetValue(parameterName, out var currentI) && currentI - 1 == i)
                    _caseInsensitiveLookup[parameterName] = i;
            }

            // Fix-up the case-insensitive lookup to point to the next match, if any.
            for (var i = 0; i < InternalList.Count; i++)
            {
                var value = InternalList[i];
                if (string.Equals(name, value.TrimmedName, StringComparison.OrdinalIgnoreCase))
                {
                    _caseInsensitiveLookup[value.TrimmedName] = i;
                    break;
                }
            }
        }

    }

    void LookupChangeName(NpgsqlParameter parameter, string oldName, string oldTrimmedName, int index)
    {
        if (string.Equals(oldTrimmedName, parameter.TrimmedName, StringComparison.OrdinalIgnoreCase))
            return;

        if (oldName.Length != 0)
            LookupRemove(oldTrimmedName, index);
        if (!parameter.IsPositional)
            LookupAdd(parameter.TrimmedName, index);
    }

    internal void ChangeParameterName(NpgsqlParameter parameter, string? value)
    {
        var oldName = parameter.ParameterName;
        var oldTrimmedName = parameter.TrimmedName;
        parameter.ChangeParameterName(value);

        if (_caseInsensitiveLookup is null)
            return;

        var index = IndexOf(parameter);
        if (index == -1) // This would be weird.
            return;

        LookupChangeName(parameter, oldName, oldTrimmedName, index);
    }

    #region NpgsqlParameterCollection Member

    /// <summary>
    /// Gets the <see cref="NpgsqlParameter"/> with the specified name.
    /// </summary>
    /// <param name="parameterName">The name of the <see cref="NpgsqlParameter"/> to retrieve.</param>
    /// <value>
    /// The <see cref="NpgsqlParameter"/> with the specified name, or a <see langword="null"/> reference if the parameter is not found.
    /// </value>
    public new NpgsqlParameter this[string parameterName]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(parameterName);

            var index = IndexOf(parameterName);
            if (index == -1)
                ThrowHelper.ThrowArgumentException("Parameter not found");

            return InternalList[index];
        }
        set
        {
            ArgumentNullException.ThrowIfNull(parameterName);
            ArgumentNullException.ThrowIfNull(value);

            var index = IndexOf(parameterName);
            if (index == -1)
                ThrowHelper.ThrowArgumentException("Parameter not found");

            if (!string.Equals(parameterName, value.TrimmedName, StringComparison.OrdinalIgnoreCase))
                ThrowHelper.ThrowArgumentException("Parameter name must be a case-insensitive match with the property 'ParameterName' on the given NpgsqlParameter", nameof(parameterName));

            var oldValue = InternalList[index];
            LookupChangeName(value, oldValue.ParameterName, oldValue.TrimmedName, index);

            InternalList[index] = value;
        }
    }

    /// <summary>
    /// Gets the <see cref="NpgsqlParameter"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the <see cref="NpgsqlParameter"/> to retrieve.</param>
    /// <value>The <see cref="NpgsqlParameter"/> at the specified index.</value>
    public new NpgsqlParameter this[int index]
    {
        get => InternalList[index];
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.Collection is not null)
                ThrowHelper.ThrowInvalidOperationException("The parameter already belongs to a collection");

            var oldValue = InternalList[index];

            if (ReferenceEquals(oldValue, value))
                return;

            LookupChangeName(value, oldValue.ParameterName, oldValue.TrimmedName, index);

            InternalList[index] = value;
            value.Collection = this;
            oldValue.Collection = null;
        }
    }

    /// <summary>
    /// Adds the specified <see cref="NpgsqlParameter"/> object to the <see cref="NpgsqlParameterCollection"/>.
    /// </summary>
    /// <param name="value">The <see cref="NpgsqlParameter"/> to add to the collection.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter Add(NpgsqlParameter value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Collection is not null)
            ThrowHelper.ThrowInvalidOperationException("The parameter already belongs to a collection");

        InternalList.Add(value);
        value.Collection = this;
        if (!value.IsPositional)
            LookupAdd(value.TrimmedName, InternalList.Count - 1);
        return value;
    }

    /// <inheritdoc />
    void ICollection<NpgsqlParameter>.Add(NpgsqlParameter item)
        => Add(item);

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> given the specified parameter name and
    /// value.
    /// </summary>
    /// <param name="parameterName">The name of the <see cref="NpgsqlParameter"/>.</param>
    /// <param name="value">The value of the <see cref="NpgsqlParameter"/> to add to the collection.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter AddWithValue(string parameterName, object value)
        => Add(new NpgsqlParameter(parameterName, value));

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> given the specified parameter name,
    /// data type and value.
    /// </summary>
    /// <param name="parameterName">The name of the <see cref="NpgsqlParameter"/>.</param>
    /// <param name="parameterType">One of the NpgsqlDbType values.</param>
    /// <param name="value">The value of the <see cref="NpgsqlParameter"/> to add to the collection.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter AddWithValue(string parameterName, NpgsqlDbType parameterType, object value)
        => Add(new NpgsqlParameter(parameterName, parameterType) { Value = value });

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> given the specified parameter name and
    /// value.
    /// </summary>
    /// <param name="parameterName">The name of the <see cref="NpgsqlParameter"/>.</param>
    /// <param name="value">The value of the <see cref="NpgsqlParameter"/> to add to the collection.</param>
    /// <param name="parameterType">One of the <see cref="NpgsqlDbType"/> values.</param>
    /// <param name="size">The length of the column.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter AddWithValue(string parameterName, NpgsqlDbType parameterType, int size, object value)
        => Add(new NpgsqlParameter(parameterName, parameterType, size) { Value = value });

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> given the specified parameter name and
    /// value.
    /// </summary>
    /// <param name="parameterName">The name of the <see cref="NpgsqlParameter"/>.</param>
    /// <param name="value">The value of the <see cref="NpgsqlParameter"/> to add to the collection.</param>
    /// <param name="parameterType">One of the <see cref="NpgsqlDbType"/> values.</param>
    /// <param name="size">The length of the column.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter AddWithValue(string parameterName, NpgsqlDbType parameterType, int size, string? sourceColumn, object value)
        => Add(new NpgsqlParameter(parameterName, parameterType, size, sourceColumn) { Value = value });

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> given the specified value.
    /// </summary>
    /// <param name="value">The value of the <see cref="NpgsqlParameter"/> to add to the collection.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter AddWithValue(object value)
        => Add(new NpgsqlParameter { Value = value });

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> given the specified data type and value.
    /// </summary>
    /// <param name="parameterType">One of the <see cref="NpgsqlDbType"/> values.</param>
    /// <param name="value">The value of the <see cref="NpgsqlParameter"/> to add to the collection.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter AddWithValue(NpgsqlDbType parameterType, object value)
        => Add(new NpgsqlParameter { NpgsqlDbType = parameterType, Value = value });

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> given the parameter name and the data type.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="parameterType">One of the <see cref="DbType"/> values.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter Add(string parameterName, NpgsqlDbType parameterType)
        => Add(new NpgsqlParameter(parameterName, parameterType));

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> with the parameter name, the data type,
    /// and the column length.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="parameterType">One of the <see cref="DbType"/> values.</param>
    /// <param name="size">The length of the column.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter Add(string parameterName, NpgsqlDbType parameterType, int size)
        => Add(new NpgsqlParameter(parameterName, parameterType, size));

    /// <summary>
    /// Adds a <see cref="NpgsqlParameter"/> to the <see cref="NpgsqlParameterCollection"/> with the parameter name, the data type, the
    /// column length, and the source column name.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="parameterType">One of the <see cref="DbType"/> values.</param>
    /// <param name="size">The length of the column.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    /// <returns>The parameter that was added.</returns>
    public NpgsqlParameter Add(string parameterName, NpgsqlDbType parameterType, int size, string sourceColumn)
        => Add(new NpgsqlParameter(parameterName, parameterType, size, sourceColumn));

    #endregion

    #region IDataParameterCollection Member

    /// <inheritdoc />
    // ReSharper disable once ImplicitNotNullOverridesUnknownExternalMember
    public override void RemoveAt(string parameterName)
        => RemoveAt(IndexOf(parameterName ?? throw new ArgumentNullException(nameof(parameterName))));

    /// <inheritdoc />
    public override bool Contains(string parameterName)
        => IndexOf(parameterName ?? throw new ArgumentNullException(nameof(parameterName))) != -1;

    /// <inheritdoc />
    public override int IndexOf(string parameterName)
    {
        if (parameterName is null)
            return -1;

        if (parameterName.Length > 0 && (parameterName[0] == ':' || parameterName[0] == '@'))
            parameterName = parameterName.Remove(0, 1);

        // Using a dictionary is always faster after around 10 items when matched against reference equality.
        // For string equality this is the case after ~3 items so we take a decent compromise going with 5.
        if (LookupEnabled && parameterName.Length != 0)
        {
            if (_caseInsensitiveLookup is null)
                BuildLookup();

            if (TwoPassCompatMode && _caseSensitiveLookup!.TryGetValue(parameterName, out var indexCs))
                return indexCs;

            if (_caseInsensitiveLookup!.TryGetValue(parameterName, out var indexCi))
                return indexCi;

            return -1;
        }

        // Start with case-sensitive search in two pass mode.
        if (TwoPassCompatMode)
        {
            for (var i = 0; i < InternalList.Count; i++)
            {
                var name = InternalList[i].TrimmedName;
                if (string.Equals(parameterName, name))
                    return i;
            }
        }

        // Then do case-insensitive search.
        for (var i = 0; i < InternalList.Count; i++)
        {
            var name = InternalList[i].TrimmedName;
            if (ReferenceEquals(parameterName, name) || string.Equals(parameterName, name, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;

        void BuildLookup()
        {
            if (TwoPassCompatMode)
                _caseSensitiveLookup = new Dictionary<string, int>(InternalList.Count);

            _caseInsensitiveLookup = new Dictionary<string, int>(InternalList.Count, StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < InternalList.Count; i++)
            {
                var item = InternalList[i];
                if (!item.IsPositional)
                    LookupAdd(item.TrimmedName, i);
            }
        }
    }

    #endregion

    #region IList Member

    /// <inheritdoc />
    public override bool IsReadOnly => false;

    /// <summary>
    /// Removes the specified <see cref="NpgsqlParameter"/> from the collection using a specific index.
    /// </summary>
    /// <param name="index">The zero-based index of the parameter.</param>
    public override void RemoveAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, InternalList.Count);

        Remove(InternalList[index]);
    }

    /// <inheritdoc />
    public override void Insert(int index, object value)
        => Insert(index, Cast(value));

    /// <summary>
    /// Removes the specified <see cref="NpgsqlParameter"/> from the collection.
    /// </summary>
    /// <param name="parameterName">The name of the <see cref="NpgsqlParameter"/> to remove from the collection.</param>
    public void Remove(string parameterName)
    {
        ArgumentNullException.ThrowIfNull(parameterName);

        var index = IndexOf(parameterName);
        if (index < 0)
            ThrowHelper.ThrowInvalidOperationException("No parameter with the specified name exists in the collection");

        RemoveAt(index);
    }

    /// <summary>
    /// Removes the specified <see cref="NpgsqlParameter"/> from the collection.
    /// </summary>
    /// <param name="value">The <see cref="NpgsqlParameter"/> to remove from the collection.</param>
    public override void Remove(object value)
        => Remove(Cast(value));

    /// <inheritdoc />
    public override bool Contains(object value)
        => value is NpgsqlParameter param && InternalList.Contains(param);

    /// <summary>
    /// Gets a value indicating whether a <see cref="NpgsqlParameter"/> with the specified parameter name exists in the collection.
    /// </summary>
    /// <param name="parameterName">The name of the <see cref="NpgsqlParameter"/> object to find.</param>
    /// <param name="parameter">
    /// A reference to the requested parameter is returned in this out param if it is found in the list.
    /// This value is <see langword="null"/> if the parameter is not found.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the collection contains the parameter and param will contain the parameter;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryGetValue(string parameterName, [NotNullWhen(true)] out NpgsqlParameter? parameter)
    {
        ArgumentNullException.ThrowIfNull(parameterName);

        var index = IndexOf(parameterName);

        if (index != -1)
        {
            parameter = InternalList[index];
            return true;
        }

        parameter = null;
        return false;
    }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    public override void Clear()
    {
        // clean up parameters so they can be added to another command if required.
        foreach (var toRemove in InternalList)
            toRemove.Collection = null;

        InternalList.Clear();
        LookupClear();
    }

    /// <inheritdoc />
    public override int IndexOf(object value)
        => IndexOf(Cast(value));

    /// <inheritdoc />
    public override int Add(object value)
    {
        Add(Cast(value));
        return Count - 1;
    }

    /// <inheritdoc />
    public override bool IsFixedSize => false;

    #endregion

    #region ICollection Member

    /// <inheritdoc />
    public override bool IsSynchronized => (InternalList as ICollection).IsSynchronized;

    /// <summary>
    /// Gets the number of <see cref="NpgsqlParameter"/> objects in the collection.
    /// </summary>
    /// <value>The number of <see cref="NpgsqlParameter"/> objects in the collection.</value>
    public override int Count => InternalList.Count;

    /// <inheritdoc />
    public override void CopyTo(Array array, int index)
        => ((ICollection)InternalList).CopyTo(array, index);

    /// <inheritdoc />
    bool ICollection<NpgsqlParameter>.IsReadOnly => false;

    /// <inheritdoc />
    public override object SyncRoot => ((ICollection)InternalList).SyncRoot;

    #endregion

    #region IEnumerable Member

    IEnumerator<NpgsqlParameter> IEnumerable<NpgsqlParameter>.GetEnumerator()
        => InternalList.GetEnumerator();

    /// <inheritdoc />
    public override IEnumerator GetEnumerator() => InternalList.GetEnumerator();

    #endregion

    /// <inheritdoc />
    public override void AddRange(Array values)
    {
        ArgumentNullException.ThrowIfNull(values);

        foreach (var parameter in values)
            Add(Cast(parameter));
    }

    /// <inheritdoc />
    protected override DbParameter GetParameter(string parameterName)
        => this[parameterName];

    /// <inheritdoc />
    protected override DbParameter GetParameter(int index)
        => this[index];

    /// <inheritdoc />
    protected override void SetParameter(string parameterName, DbParameter value)
        => this[parameterName] = Cast(value);

    /// <inheritdoc />
    protected override void SetParameter(int index, DbParameter value)
        => this[index] = Cast(value);

    /// <summary>
    /// Report the offset within the collection of the given parameter.
    /// </summary>
    /// <param name="item">Parameter to find.</param>
    /// <returns>Index of the parameter, or -1 if the parameter is not present.</returns>
    public int IndexOf(NpgsqlParameter item)
        => InternalList.IndexOf(item);

    /// <summary>
    /// Insert the specified parameter into the collection.
    /// </summary>
    /// <param name="index">Index of the existing parameter before which to insert the new one.</param>
    /// <param name="item">Parameter to insert.</param>
    public void Insert(int index, NpgsqlParameter item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.Collection != null)
            throw new Exception("The parameter already belongs to a collection");

        InternalList.Insert(index, item);
        item.Collection = this;
        if (!item.IsPositional)
            LookupInsert(item.TrimmedName, index);
    }

    /// <summary>
    /// Report whether the specified parameter is present in the collection.
    /// </summary>
    /// <param name="item">Parameter to find.</param>
    /// <returns>True if the parameter was found, otherwise false.</returns>
    public bool Contains(NpgsqlParameter item) => InternalList.Contains(item);

    /// <summary>
    /// Remove the specified parameter from the collection.
    /// </summary>
    /// <param name="item">Parameter to remove.</param>
    /// <returns>True if the parameter was found and removed, otherwise false.</returns>
    public bool Remove(NpgsqlParameter item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.Collection != this)
            ThrowHelper.ThrowInvalidOperationException("The item does not belong to this collection");

        var index = IndexOf(item);
        if (index >= 0)
        {
            InternalList.RemoveAt(index);
            if (!LookupEnabled)
                LookupClear();
            if (!item.IsPositional)
                LookupRemove(item.TrimmedName, index);
            item.Collection = null;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Convert collection to a System.Array.
    /// </summary>
    /// <param name="array">Destination array.</param>
    /// <param name="arrayIndex">Starting index in destination array.</param>
    public void CopyTo(NpgsqlParameter[] array, int arrayIndex)
        => InternalList.CopyTo(array, arrayIndex);

    /// <summary>
    /// Convert collection to a System.Array.
    /// </summary>
    /// <returns>NpgsqlParameter[]</returns>
    public NpgsqlParameter[] ToArray() => InternalList.ToArray();

    internal void CloneTo(NpgsqlParameterCollection other)
    {
        other.InternalList.Clear();
        foreach (var param in InternalList)
        {
            var newParam = param.Clone();
            newParam.Collection = other;
            other.InternalList.Add(newParam);
        }

        if (LookupEnabled && _caseInsensitiveLookup is not null)
        {
            other._caseInsensitiveLookup = new Dictionary<string, int>(_caseInsensitiveLookup, StringComparer.OrdinalIgnoreCase);
            if (TwoPassCompatMode)
            {
                Debug.Assert(_caseSensitiveLookup is not null);
                other._caseSensitiveLookup = new Dictionary<string, int>(_caseSensitiveLookup);
            }
        }
    }

    internal void ProcessParameters(PgSerializerOptions options, bool validateValues, CommandType commandType)
    {
        HasOutputParameters = false;
        PlaceholderType = PlaceholderType.NoParameters;

        var list = InternalList;
        for (var i = 0; i < list.Count; i++)
        {
            var p = list[i];

            switch (PlaceholderType)
            {
            case PlaceholderType.NoParameters:
                PlaceholderType = p.IsPositional ? PlaceholderType.Positional : PlaceholderType.Named;
                break;
            case PlaceholderType.Named:
                if (p.IsPositional)
                    PlaceholderType = PlaceholderType.Mixed;
                break;
            case PlaceholderType.Positional:
                if (!p.IsPositional)
                    PlaceholderType = PlaceholderType.Mixed;
                break;
            case PlaceholderType.Mixed:
                break;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(PlaceholderType), $"Unknown {nameof(PlaceholderType)} value: {{0}}", PlaceholderType);
                break;
            }

            switch (p.Direction)
            {
            case ParameterDirection.Input:
                break;

            case ParameterDirection.InputOutput:
                if (PlaceholderType == PlaceholderType.Positional && commandType != CommandType.StoredProcedure)
                    ThrowHelper.ThrowNotSupportedException("Output parameters are not supported in positional mode (unless used with CommandType.StoredProcedure)");
                HasOutputParameters = true;
                break;

            case ParameterDirection.Output:
                if (PlaceholderType == PlaceholderType.Positional && commandType != CommandType.StoredProcedure)
                    ThrowHelper.ThrowNotSupportedException("Output parameters are not supported in positional mode (unless used with CommandType.StoredProcedure)");
                HasOutputParameters = true;
                continue;

            case ParameterDirection.ReturnValue:
                // Simply ignored
                continue;

            default:
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(ParameterDirection),
                    $"Unhandled {nameof(ParameterDirection)} value: {{0}}", p.Direction);
                break;
            }

            p.ResolveTypeInfo(options);

            if (validateValues)
            {
                p.Bind(out _, out _);
            }
        }
    }

    internal bool HasOutputParameters { get; set; }
    internal PlaceholderType PlaceholderType { get; set; }

    static NpgsqlParameter Cast(object? value)
    {
        var castedValue = value as NpgsqlParameter;
        if (castedValue is null)
            ThrowInvalidCastException(value);

        return castedValue;
    }

    [DoesNotReturn]
    static void ThrowInvalidCastException(object? value) =>
        throw new InvalidCastException(
            $"The value \"{value}\" is not of type \"{nameof(NpgsqlParameter)}\" and cannot be used in this parameter collection.");
}

enum PlaceholderType
{
    /// <summary>
    /// The parameter collection includes no parameters.
    /// </summary>
    NoParameters,

    /// <summary>
    /// The parameter collection includes only named parameters.
    /// </summary>
    Named,

    /// <summary>
    /// The parameter collection includes only positional parameters.
    /// </summary>
    Positional,

    /// <summary>
    /// The parameter collection includes both named and positional parameters.
    /// This is only supported when <see cref="NpgsqlCommand.CommandType" /> is set to <see cref="CommandType.StoredProcedure" />.
    /// </summary>
    Mixed
}
