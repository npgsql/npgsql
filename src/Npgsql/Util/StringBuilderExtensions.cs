#if NETSTANDARD2_0 || NET461
using System;
using System.Text;

namespace Npgsql.Util
{
    /// <summary>
    /// A set of extension methods to <see cref="StringBuilder"/> to allow runtime compatibility.
    /// </summary>
    static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the provided <see cref="ReadOnlySpan{T}"/> to the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> to append to.</param>
        /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to append.</param>
        public static StringBuilder Append(this StringBuilder stringBuilder, ReadOnlySpan<char> span)
        {
            if (span.Length > 0)
            {
                unsafe
                {
                    fixed (char* value = &span.GetPinnableReference())
                    {
                        return stringBuilder.Append(value, span.Length);
                    }
                }
            }

            return stringBuilder;
        }
    }
}
#endif
