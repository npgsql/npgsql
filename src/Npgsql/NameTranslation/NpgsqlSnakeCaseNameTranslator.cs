using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.NameTranslation
{
    /// <summary>
    /// A name translator which converts standard CLR names (e.g. SomeClass) to snake-case database
    /// names (some_class)
    /// </summary>
    public class NpgsqlSnakeCaseNameTranslator : INpgsqlNameTranslator
    {
        /// <summary>
        /// Given a CLR type name (e.g class, struct, enum), translates its name to a database type name.
        /// </summary>
        public string TranslateTypeName(string clrName)
        {
            return ClrToDatabaseName(clrName);
        }

        /// <summary>
        /// Given a CLR member name (property or field), translates its name to a database type name.
        /// </summary>
        public string TranslateMemberName(string clrName)
        {
            return ClrToDatabaseName(clrName);
        }

        static string ClrToDatabaseName(string clrName)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < clrName.Length; i++)
            {
                var c = clrName[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                        sb.Append('_');
                    sb.Append(char.ToLower(c));
                    continue;
                }

                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
