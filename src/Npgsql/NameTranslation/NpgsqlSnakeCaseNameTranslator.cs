using System.Linq;
using Npgsql.Util;

namespace Npgsql.NameTranslation
{
    /// <summary>
    /// A name translator which converts standard CLR names (e.g. SomeClass) to snake-case database
    /// names (some_class)
    /// </summary>
    public class NpgsqlSnakeCaseNameTranslator : INpgsqlNameTranslator
    {
        public NpgsqlSnakeCaseNameTranslator() : this(compatibilityMode: true)
        {
        }

        public NpgsqlSnakeCaseNameTranslator(bool compatibilityMode)
        {
            CompatibilityMode = compatibilityMode;
        }

        bool CompatibilityMode { get; }

        /// <summary>
        /// Given a CLR type name (e.g class, struct, enum), translates its name to a database type name.
        /// </summary>
        public string TranslateTypeName(string clrName) => ClrToDatabaseName(clrName);

        /// <summary>
        /// Given a CLR member name (property or field), translates its name to a database type name.
        /// </summary>
        public string TranslateMemberName(string clrName) => ClrToDatabaseName(clrName);

        string ClrToDatabaseName(string clrName) => CompatibilityMode
            ? string.Concat(clrName.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c.ToString() : c.ToString())).ToLower()
            : clrName.ToSnakeCase();
    }
}
