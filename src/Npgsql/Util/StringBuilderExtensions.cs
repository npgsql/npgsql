#if NETSTANDARD2_0 || NET452
using System;
using System.Text;

namespace Npgsql.Util
{
    /// <summary>
    /// A set of extension methods to <see cref="StringBuilder"/> to allow runtime compatibility.
    /// </summary>
    internal static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the provided <see cref="ReadOnlySpan{T}"/> to the <see cref="StringBuilder"/> by calling ToString on
        /// the span.
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append to.</param>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to append.</param>
        public static void Append(this StringBuilder stringBuilder, ReadOnlySpan<char> span)
            => stringBuilder.Append(span.ToString());
    }
}
#endif