using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql
{
    internal class PersistentPreparedCommand
    {
        /// <summary>
        /// Initializes PersistentPreparedCommand
        /// </summary>
        /// <param name="commandSQL">Command SQL</param>
        /// <param name="statements">Prepared statements of the command</param>
        public PersistentPreparedCommand(string commandSQL, IReadOnlyList<PersistentPreparedStatement> statements)
        {
            if (commandSQL == null)
                throw new ArgumentNullException("commandSQL");

            if (statements == null)
                throw new ArgumentNullException("statements");

            CommandSQL = commandSQL;
            Statements = statements;
        }

        /// <summary>
        /// Command SQL
        /// </summary>
        public string CommandSQL { get; private set; }
        
        /// <summary>
        /// Prepared statements of the command
        /// </summary>
        public IReadOnlyList<PersistentPreparedStatement> Statements { get; private set; }
    }
}
