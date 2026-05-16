using System;

namespace Npgsql.Util;

static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Determines whether this type and <paramref name="other"/> are in a subtype relationship,
        /// i.e. whether one is assignable to the other in either direction.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="true"/> when the types are identical, when one inherits from or implements the other,
        /// or more generally when an implicit reference or boxing conversion exists between them.
        /// </remarks>
        /// <param name="other">The type to check the relationship with.</param>
        /// <returns><see langword="true"/> if either type is assignable to the other; otherwise, <see langword="false"/>.</returns>
        public bool IsInSubtypeRelationshipWith(Type other) =>
            type.IsAssignableTo(other) || other.IsAssignableTo(type);

        /// <summary>
        /// Walks the inheritance chain returning the first base type matching <paramref name="baseType"/>.
        /// When <paramref name="baseType"/> is an open generic definition, returns the closed form encountered.
        /// </summary>
        public Type? GetBase(Type baseType)
        {
            var t = type;
            while (t is not null)
            {
                if (baseType.IsGenericTypeDefinition
                        ? t.IsGenericType && t.GetGenericTypeDefinition() == baseType
                        : t == baseType)
                    return t;
                t = t.BaseType;
            }
            return null;
        }
    }
}
