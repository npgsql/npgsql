using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// A name translator which converts standard CLR names (e.g. SomeClass) to snake-case database
    /// names (some_class)
    /// </summary>
    public class NpgsqlSnakeCaseNameTranslator : INpgsqlNameTranslator
    {
        const int LetterCasesGap = 'a' - 'A'; // 0x61 - 0x41 = 0x20

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
            if (clrName.IsNullOrWhiteSpace())
                return clrName;

            var sb = new StringBuilder();
            sb.Append(clrName[0]);
            for(var i = 1; i < clrName.Length; i++)
            {
                var c = clrName[i];
                if (c < 'A' || c > 'Z')
                {
                    sb.Append(c);
                    continue;
                }

                sb.Append('_');
                sb.Append((char)(c + LetterCasesGap));
            }
            return sb.ToString();
        }
    }
}
