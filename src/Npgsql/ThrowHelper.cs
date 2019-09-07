using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Npgsql
{
    static class ThrowHelper
    {
        [DoesNotReturn]
        internal static void ThrowInvalidOperationException_NoPropertyGetter(Type type, MemberInfo property) =>
            throw new InvalidOperationException($"Composite type {type} cannot be written because the {property} property has no getter.");

        [DoesNotReturn]
        internal static void ThrowInvalidOperationException_NoPropertySetter(Type type, MemberInfo property) =>
            throw new InvalidOperationException($"Composite type {type} cannot be read because the {property} property has no setter.");
    }
}
