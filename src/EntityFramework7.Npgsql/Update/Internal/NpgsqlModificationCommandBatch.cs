using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Npgsql;

namespace Microsoft.Data.Entity.Update.Internal
{
    using RelationalStrings = Microsoft.Data.Entity.Internal.RelationalStrings;

    /// <remarks>
    /// The usual ModificationCommandBatch implementation is <see cref="AffectedCountModificationCommandBatch"/>,
    /// which relies on <see cref="SqlGenerator.AppendSelectAffectedCountCommand"/> to fetch the number of
    /// rows modified via SQL.
    ///
    /// PostgreSQL actually has no way of selecting the modified row count.
    /// SQL defines GET DIAGNOSTICS which should provide this, but in PostgreSQL it's only available
    /// in PL/pgSQL. See http://www.postgresql.org/docs/9.4/static/unsupported-features-sql-standard.html,
    /// identifier F121-01.
    ///
    /// Instead, the affected row count can be accessed in the PostgreSQL protocol itself, which seems
    /// cleaner and more efficient anyway (no additional query).
    /// </remarks>
    public class NpgsqlModificationCommandBatch : ReaderModificationCommandBatch
    {
        public NpgsqlModificationCommandBatch(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerator sqlGenerator,
            [NotNull] IUpdateSqlGenerator updateSqlGenerator,
            [NotNull] IRelationalValueBufferFactoryFactory valueBufferFactoryFactory)
            : base(commandBuilderFactory, sqlGenerator, updateSqlGenerator, valueBufferFactoryFactory)
        {
        }

        protected override bool CanAddCommand(ModificationCommand modificationCommand)
            => ModificationCommands.Count < NpgsqlCommand.MaxStatements;

        protected override bool IsCommandTextValid()
            => true;

        protected override void Consume(DbDataReader reader)
        {
            var npgsqlReader = (NpgsqlDataReader)reader;
            Debug.Assert(npgsqlReader.Statements.Count == ModificationCommands.Count, $"Reader has {npgsqlReader.Statements.Count} statements, expected {ModificationCommands.Count}");
            var commandIndex = 0;

            try
            {
                while (true)
                {
                    // Find the next propagating command, if any
                    int nextPropagating;
                    for (nextPropagating = commandIndex;
                        nextPropagating < ModificationCommands.Count &&
                        !ModificationCommands[nextPropagating].RequiresResultPropagation;
                        nextPropagating++) ;

                    // Go over all non-propagating commands before the next propagating one,
                    // make sure they executed
                    for (; commandIndex < nextPropagating; commandIndex++)
                    {
                        if (npgsqlReader.Statements[commandIndex].Rows == 0)
                        {
                            throw new DbUpdateConcurrencyException(
                                RelationalStrings.UpdateConcurrencyException(1, 0),
                                ModificationCommands[commandIndex].Entries
                            );
                        }
                    }

                    if (nextPropagating == ModificationCommands.Count)
                    {
                        Debug.Assert(!reader.NextResult(), "Expected less resultsets");
                        break;
                    }

                    // Propagate to results from the reader to the ModificationCommand

                    var modificationCommand = ModificationCommands[commandIndex++];

                    if (!reader.Read())
                    {
                        throw new DbUpdateConcurrencyException(
                            RelationalStrings.UpdateConcurrencyException(1, 0),
                            modificationCommand.Entries);
                    }

                    var valueBufferFactory = CreateValueBufferFactory(modificationCommand.ColumnModifications);
                    modificationCommand.PropagateResults(valueBufferFactory.Create(reader));

                    reader.NextResult();
                }
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException(
                    RelationalStrings.UpdateStoreException,
                    ex,
                    ModificationCommands[commandIndex].Entries);
            }
        }

        protected override async Task ConsumeAsync(
            DbDataReader reader,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var npgsqlReader = (NpgsqlDataReader)reader;
            Debug.Assert(npgsqlReader.Statements.Count == ModificationCommands.Count, $"Reader has {npgsqlReader.Statements.Count} statements, expected {ModificationCommands.Count}");
            var commandIndex = 0;

            try
            {
                while (true)
                {
                    // Find the next propagating command, if any
                    int nextPropagating;
                    for (nextPropagating = commandIndex;
                        nextPropagating < ModificationCommands.Count &&
                        !ModificationCommands[nextPropagating].RequiresResultPropagation;
                        nextPropagating++)
                        ;

                    // Go over all non-propagating commands before the next propagating one,
                    // make sure they executed
                    for (; commandIndex < nextPropagating; commandIndex++)
                    {
                        if (npgsqlReader.Statements[commandIndex].Rows == 0)
                        {
                            throw new DbUpdateConcurrencyException(
                                RelationalStrings.UpdateConcurrencyException(1, 0),
                                ModificationCommands[commandIndex].Entries
                            );
                        }
                    }

                    if (nextPropagating == ModificationCommands.Count)
                    {
                        Debug.Assert(!(await reader.NextResultAsync(cancellationToken)), "Expected less resultsets");
                        break;
                    }

                    // Extract result from the command and propagate it

                    var modificationCommand = ModificationCommands[commandIndex++];

                    if (!(await reader.ReadAsync(cancellationToken)))
                    {
                        throw new DbUpdateConcurrencyException(
                            RelationalStrings.UpdateConcurrencyException(1, 0),
                            modificationCommand.Entries
                        );
                    }

                    var valueBufferFactory = CreateValueBufferFactory(modificationCommand.ColumnModifications);
                    modificationCommand.PropagateResults(valueBufferFactory.Create(reader));

                    await reader.NextResultAsync(cancellationToken);
                }
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException(
                    RelationalStrings.UpdateStoreException,
                    ex,
                    ModificationCommands[commandIndex].Entries);
            }
        }
    }
}
