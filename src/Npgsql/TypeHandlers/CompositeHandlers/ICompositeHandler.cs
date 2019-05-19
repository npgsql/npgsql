using System;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    interface ICompositeHandler
    {
        /// <summary>
        /// The CLR type mapped to the PostgreSQL composite type.
        /// </summary>
        Type CompositeType { get; }
    }
}
