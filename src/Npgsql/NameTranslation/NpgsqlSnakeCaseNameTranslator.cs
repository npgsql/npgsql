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
        /// <param name="legacyMode">Uses the legacy naming convention if <c>true</c>, otherwise it uses the new naming convention.</param>
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
        /// <param name="value">The value to convert.</param>
        public static string ConvertToSnakeCase(string value)
        {
            const char underscore = '_';

            var builder = new StringBuilder();
            var previousCategory = default(UnicodeCategory?);

            for (var currentIndex = 0; currentIndex< value.Length; currentIndex++)
            {
                var currentChar = value[currentIndex];
                if (currentChar == underscore)
                {
                    builder.Append(underscore);
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
                            currentIndex + 1 < value.Length &&
                            char.IsLower(value[currentIndex + 1]))
                        {
                            builder.Append(underscore);
                        }

                        currentChar = char.ToLower(currentChar);
                        break;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (previousCategory == UnicodeCategory.SpaceSeparator)
                            builder.Append(underscore);
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
