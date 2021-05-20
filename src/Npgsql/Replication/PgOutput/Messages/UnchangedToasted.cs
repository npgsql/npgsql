using System;
using System.Collections.Generic;
using System.Text;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Represents an unchanged toasted value in a replication message. This class cannot be inherited.
    /// </summary>
    public sealed class UnchangedToasted
    {
        /// <summary>
        /// Represents the sole instance of the <see cref="UnchangedToasted"/> class.
        /// </summary>
        public static readonly UnchangedToasted Value = new();

        UnchangedToasted() { }
    }
}
