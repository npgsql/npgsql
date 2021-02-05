using System;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

// ReSharper disable once CheckNamespace
namespace Npgsql
{
    [Flags]
    public enum GeoJSONOptions
    {
        None = 0,
        BoundingBox = 1,
        ShortCRS = 2,
        LongCRS = 4
    }
}
