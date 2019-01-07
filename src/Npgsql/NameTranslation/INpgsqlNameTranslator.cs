namespace Npgsql
{
    /// <summary>
    /// A component which translates a CLR name (e.g. SomeClass) into a database name (e.g. some_class)
    /// according to some scheme.
    /// Used for mapping enum and composite types.
    /// </summary>
    public interface INpgsqlNameTranslator
    {
        /// <summary>
        /// Given a CLR type name (e.g class, struct, enum), translates its name to a database type name.
        /// </summary>
        string TranslateTypeName(string clrName);

        /// <summary>
        /// Given a CLR member name (property or field), translates its name to a database type name.
        /// </summary>
        string TranslateMemberName(string clrName);
    }
}
