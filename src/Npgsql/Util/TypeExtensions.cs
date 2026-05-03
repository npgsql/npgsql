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
    }
}
