using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Npgsql.NameTranslation
{
    /// <summary>
    /// A name translator which converts standard CLR names (e.g. SomeClass) to snake-case database
    /// names (some_class)
    /// </summary>
    public class NpgsqlSnakeCaseNameTranslator : INpgsqlNameTranslator
    {
        /// <summary>
        /// Creates a new <see cref="NpgsqlSnakeCaseNameTranslator"/>.
        /// </summary>
        public NpgsqlSnakeCaseNameTranslator()
            : this(false) { }

        /// <summary>
        /// Creates a new <see cref="NpgsqlSnakeCaseNameTranslator"/>.
        /// </summary>
        /// <param name="legacyMode">
        /// Uses the legacy naming convention if <see langword="true"/>, otherwise it uses the new naming convention.
        /// </param>
        public NpgsqlSnakeCaseNameTranslator(bool legacyMode)
            => LegacyMode = legacyMode;

        bool LegacyMode { get; }

        /// <summary>
        /// Given a CLR type name (e.g class, struct, enum), translates its name to a database type name.
        /// </summary>
        public string TranslateTypeName(string clrName) => TranslateMemberName(clrName);

        /// <summary>
        /// Given a CLR member name (property or field), translates its name to a database type name.
        /// </summary>
        public string TranslateMemberName(string clrName)
        {
            if (clrName == null)
                throw new ArgumentNullException(nameof(clrName));

            return LegacyMode
                ? string.Concat(clrName.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c.ToString() : c.ToString())).ToLower()
                : ConvertToSnakeCase(clrName);
        }

        /// <summary>
        /// Converts a string to its snake_case equivalent.
        /// </summary>
        /// <param name="name">The value to convert.</param>
        public static string ConvertToSnakeCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var builder = new StringBuilder(name.Length + Math.Min(2, name.Length / 5));
            var previousCategory = default(UnicodeCategory?);

            for (var currentIndex = 0; currentIndex < name.Length; currentIndex++)
            {
                var currentChar = name[currentIndex];
                if (currentChar == '_')
                {
                    builder.Append('_');
                    previousCategory = null;
                    continue;
                }

                var currentCategory = char.GetUnicodeCategory(currentChar);
                switch (currentCategory)
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                        if (previousCategory == UnicodeCategory.SpaceSeparator ||
                            previousCategory == UnicodeCategory.LowercaseLetter ||
                            previousCategory != UnicodeCategory.DecimalDigitNumber &&
                            previousCategory != null &&
                            currentIndex > 0 &&
                            currentIndex + 1 < name.Length &&
                            char.IsLower(name[currentIndex + 1]))
                        {
                            builder.Append('_');
                        }

                        currentChar = char.ToLower(currentChar);
                        break;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (previousCategory == UnicodeCategory.SpaceSeparator)
                            builder.Append('_');
                        break;

                    default:
                        if (previousCategory != null)
                            previousCategory = UnicodeCategory.SpaceSeparator;
                        continue;
                }

                builder.Append(currentChar);
                previousCategory = currentCategory;
            }

            return builder.ToString();
        }
    }
}
