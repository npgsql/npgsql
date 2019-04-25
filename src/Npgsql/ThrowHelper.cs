using System;
using System.Reflection;

namespace Npgsql
{
    static class ThrowHelper
    {
        internal static void ThrowNotSupportedException_NoPropertyGetter(Type type, MemberInfo property) =>
            throw new NotSupportedException($"Composite type {type} cannot be written because the {property} property has no getter.");

        internal static void ThrowNotSupportedException_NoPropertySetter(Type type, MemberInfo property) =>
            throw new NotSupportedException($"Composite type {type} cannot be read because the {property} property has no setter.");
    }
}
