using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;

namespace Npgsql.EntityFramework7.Update
{
    using RelationalStrings = Microsoft.Data.Entity.Relational.Strings;

    public class NpgsqlModificationCommandBatch : ReaderModificationCommandBatch
    {
        public NpgsqlModificationCommandBatch([NotNull] ISqlGenerator sqlGenerator)
            : base(sqlGenerator)
        {}

        protected override bool CanAddCommand(ModificationCommand modificationCommand)
            => ModificationCommands.Count < NpgsqlCommand.MaxStatements;

        protected override bool IsCommandTextValid()
            => true;

        public override int Execute(IRelationalTransaction transaction, IRelationalTypeMapper typeMapper, DbContext context, ILogger logger)
        {
            Check.NotNull(transaction, nameof(transaction));
            Check.NotNull(typeMapper, nameof(typeMapper));
            Check.NotNull(context, nameof(context));
            Check.NotNull(logger, nameof(logger));

            var commandText = GetCommandText();

            var commandIndex = 0;
            using (var storeCommand = CreateStoreCommand(commandText, transaction.DbTransaction, typeMapper, transaction.Connection?.CommandTimeout))
            {
                /* LogCommand is internal...
                if (logger.IsEnabled(LogLevel.Verbose))
                {
                    logger.LogCommand(storeCommand);
                }
                */

                try
                {
                    using (var reader = (NpgsqlDataReader)storeCommand.ExecuteReader())
                    {
                        Debug.Assert(reader.Statements.Count == ModificationCommands.Count, $"Reader has {reader.Statements.Count} statements, expected {ModificationCommands.Count}");
                        var firstResultSet = true;
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
                                if (reader.Statements[commandIndex].Rows == 0)
                                {
                                    throw new DbUpdateConcurrencyException(
                                        RelationalStrings.UpdateConcurrencyException(1, 0),
                                        context,
                                        ModificationCommands[nextPropagating].Entries
                                    );
                                }
                            }

                            if (nextPropagating == ModificationCommands.Count)
                            {
                                Debug.Assert(!reader.NextResult(), "Expected less resultsets");
                                break;
                            }

                            if (firstResultSet)
                            {
                                firstResultSet = false;
                            }
                            else
                            {
                                var hasNextResultSet = reader.NextResult();
                                Debug.Assert(hasNextResultSet, "Expected more resultsets");
                            }

                            // Extract result from the command and propagate it

                            var modificationCommand = ModificationCommands[commandIndex++];

                            if (!reader.Read())
                            {
                                // Should never happen? Aren't result-propagating modifications only INSERTs,
                                // which either succeed or throw an exception (e.g. constraint violation)?
                                throw new DbUpdateConcurrencyException(
                                    RelationalStrings.UpdateConcurrencyException(1, 0),
                                    context,
                                    modificationCommand.Entries
                                );
                            }

                            modificationCommand.PropagateResults(modificationCommand.ValueBufferFactory.CreateValueBuffer(reader));
                        }
                    }
                }
                catch (DbUpdateException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw commandIndex < ModificationCommands.Count
                        ? new DbUpdateException(RelationalStrings.UpdateStoreException, context, ex, ModificationCommands[commandIndex].Entries)
                        : new DbUpdateException(RelationalStrings.UpdateStoreException, context, ex);
                }
            }

            return ModificationCommands.Count;
        }

        public override async Task<int> ExecuteAsync(IRelationalTransaction transaction, IRelationalTypeMapper typeMapper, DbContext context, ILogger logger,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Check.NotNull(transaction, nameof(transaction));
            Check.NotNull(typeMapper, nameof(typeMapper));
            Check.NotNull(context, nameof(context));
            Check.NotNull(logger, nameof(logger));

            var commandText = GetCommandText();

            var commandIndex = 0;
            using (var storeCommand = CreateStoreCommand(commandText, transaction.DbTransaction, typeMapper, transaction.Connection?.CommandTimeout))
            {
                /* LogCommand is internal...
                if (logger.IsEnabled(LogLevel.Verbose))
                {
                    logger.LogCommand(storeCommand);
                }
                */

                try
                {
                    using (var reader = (NpgsqlDataReader)await storeCommand.ExecuteReaderAsync(cancellationToken))
                    {
                        Debug.Assert(reader.Statements.Count == ModificationCommands.Count, $"Reader has {reader.Statements.Count} statements, expected {ModificationCommands.Count}");
                        var firstResultSet = true;
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
                                if (reader.Statements[commandIndex].Rows == 0)
                                {
                                    throw new DbUpdateConcurrencyException(
                                        RelationalStrings.UpdateConcurrencyException(1, 0),
                                        context,
                                        ModificationCommands[nextPropagating].Entries
                                    );
                                }
                            }

                            if (nextPropagating == ModificationCommands.Count)
                            {
                                Debug.Assert(!(await reader.NextResultAsync(cancellationToken)), "Expected less resultsets");
                                break;
                            }

                            if (firstResultSet)
                            {
                                firstResultSet = false;
                            }
                            else
                            {
                                var hasNextResultSet = await reader.NextResultAsync(cancellationToken);
                                Debug.Assert(hasNextResultSet, "Expected more resultsets");
                            }

                            // Extract result from the command and propagate it

                            var modificationCommand = ModificationCommands[commandIndex++];

                            if (!(await reader.ReadAsync(cancellationToken)))
                            {
                                // Should never happen? Aren't result-propagating modifications only INSERTs,
                                // which either succeed or throw an exception (e.g. constraint violation)?
                                throw new DbUpdateConcurrencyException(
                                    RelationalStrings.UpdateConcurrencyException(1, 0),
                                    context,
                                    modificationCommand.Entries
                                );
                            }

                            modificationCommand.PropagateResults(modificationCommand.ValueBufferFactory.CreateValueBuffer(reader));
                        }
                    }
                }
                catch (DbUpdateException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw commandIndex < ModificationCommands.Count
                        ? new DbUpdateException(RelationalStrings.UpdateStoreException, context, ex, ModificationCommands[commandIndex].Entries)
                        : new DbUpdateException(RelationalStrings.UpdateStoreException, context, ex);
                }
            }

            return ModificationCommands.Count;
        }
    }
}
