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
        public NpgsqlSnakeCaseNameTranslator() : this(false) {}

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
        public string TranslateMemberName(string clrName) => LegacyMode
            ? string.Concat(clrName.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c.ToString() : c.ToString())).ToLower()
            : ConvertToSnakeCase(clrName);

        /// <summary>
        /// Converts a string to its snake_case equivalent.
        /// </summary>
        /// <remarks>
        /// Code borrowed from Newtonsoft.Json.
        /// See https://github.com/JamesNK/Newtonsoft.Json/blob/f012ba857f36fe75b1294a210b9104130a4db4d5/Src/Newtonsoft.Json/Utilities/StringUtils.cs#L200-L276.
        /// </remarks>
        /// <param name="value">The value to convert.</param>
        public static string ConvertToSnakeCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var sb = new StringBuilder();
            var state = SnakeCaseState.Start;

            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == ' ')
                {
                    if (state != SnakeCaseState.Start)
                        state = SnakeCaseState.NewWord;
                }
                else if (char.IsUpper(value[i]))
                {
                    switch (state)
                    {
                    case SnakeCaseState.Upper:
                        var hasNext = (i + 1 < value.Length);
                        if (i > 0 && hasNext)
                        {
                            var nextChar = value[i + 1];
                            if (!char.IsUpper(nextChar) && nextChar != '_')
                            {
                                sb.Append('_');
                            }
                        }
                        break;
                    case SnakeCaseState.Lower:
                    case SnakeCaseState.NewWord:
                        sb.Append('_');
                        break;
                    }

                    sb.Append(char.ToLowerInvariant(value[i]));
                    state = SnakeCaseState.Upper;
                }
                else if (value[i] == '_')
                {
                    sb.Append('_');
                    state = SnakeCaseState.Start;
                }
                else
                {
                    if (state == SnakeCaseState.NewWord)
                        sb.Append('_');

                    sb.Append(value[i]);
                    state = SnakeCaseState.Lower;
                }
            }

            return sb.ToString();
        }

        enum SnakeCaseState
        {
            Start,
            Lower,
            Upper,
            NewWord
        }
    }
}
