using System;
using System.Collections.Generic;
using System.Diagnostics;
using Npgsql.Util;

namespace Npgsql.Internal.Composites;

sealed class CompositeInfo<T>
{
    readonly int _lastConstructorFieldIndex;
    readonly CompositeFieldInfo[] _fields;

    public CompositeInfo(CompositeFieldInfo[] fields, int? constructorParameters, Func<StrongBox[], T>? constructor)
    {
        _lastConstructorFieldIndex = -1;
        for (var i = fields.Length - 1; i >= 0; i--)
            if (fields[i].ConstructorParameterIndex is not null)
            {
                _lastConstructorFieldIndex = i;
                break;
            }

        var parameterSum = 0;
        for(var i = constructorParameters - 1 ?? 0; i > 0; i--)
            parameterSum += i;

        var argumentsSum = 0;
        if (parameterSum > 0)
        {
            foreach (var field in fields)
                if (field.ConstructorParameterIndex is { } index)
                    argumentsSum += index;
        }

        if (parameterSum != argumentsSum)
            throw new InvalidOperationException($"Missing composite fields to map to the required {constructorParameters} constructor parameters.");

        _fields = fields;
        if (constructor is null)
            Constructor = _ => Activator.CreateInstance<T>();
        else
        {
            var arguments = new CompositeFieldInfo[constructorParameters.GetValueOrDefault()];
            foreach (var field in fields)
            {
                if (field.ConstructorParameterIndex is { } index)
                    arguments[index] = field;
            }
            Constructor = constructor;
        }

        ConstructorParameters = constructorParameters ?? 0;
    }

    public IReadOnlyList<CompositeFieldInfo> Fields => _fields;

    public int ConstructorParameters { get; }
    public Func<StrongBox[], T> Constructor { get; }

    /// <summary>
    /// Create temporary storage for all values that come before the constructor parameters can be saturated.
    /// </summary>
    /// <returns></returns>
    public StrongBox[] CreateTempBoxes()
    {
        var valueCache = _lastConstructorFieldIndex + 1 is 0 ? Array.Empty<StrongBox>() : new StrongBox[_lastConstructorFieldIndex + 1];
        var fields = _fields;

        for (var i = 0; i < valueCache.Length; i++)
            valueCache[i] = fields[i].CreateBox();

        return valueCache;
    }
}
