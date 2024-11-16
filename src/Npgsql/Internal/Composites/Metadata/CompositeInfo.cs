using System;
using System.Collections.Generic;
using Npgsql.Util;

namespace Npgsql.Internal.Composites;

sealed class CompositeInfo<T>
{
    readonly int _lastConstructorFieldIndex;
    readonly CompositeFieldInfo[] _fields;

    public CompositeInfo(CompositeFieldInfo[] fields, int constructorParameters, Func<StrongBox[], T> constructor)
    {
        _lastConstructorFieldIndex = -1;
        var constructorFields = 0;
        for (var i = 0; i < fields.Length; i++)
        {
            if (fields[i].ConstructorParameterIndex is not null)
            {
                _lastConstructorFieldIndex = i;
                constructorFields++;
            }
        }

        if (constructorParameters != constructorFields)
            throw new InvalidOperationException($"Missing composite fields to map to the required {constructorParameters} constructor parameters.");

        _fields = fields;
        Constructor = constructor;
        ConstructorParameters = constructorParameters;
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
        if (_lastConstructorFieldIndex is -1)
            return [];

        var boxes = new StrongBox[_lastConstructorFieldIndex + 1];
        var fields = _fields;
        for (var i = 0; i < boxes.Length; i++)
            boxes[i] = fields[i].CreateBox();

        return boxes;
    }
}
