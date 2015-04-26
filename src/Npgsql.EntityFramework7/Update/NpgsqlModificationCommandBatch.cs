// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Framework.Logging;
using RelationalStrings = Microsoft.Data.Entity.Relational.Strings;

namespace Npgsql.EntityFramework7.Update
{
    // We heavily override ModificationCommandBatch.
    // This is probably a temporary situation; we do this because ReaderModificationCommandBatch
    // is geared towards the SqlServer method of retrieving affected rows (i.e. by appending
    // SELECT @@ROWCOUNT).
    // Specifically missing from NpgsqlModificationCommandBatch at the moment is propagated results
    // handling
    public class NpgsqlModificationCommandBatch : ReaderModificationCommandBatch
    {
        public NpgsqlModificationCommandBatch([NotNull] INpgsqlSqlGenerator sqlGenerator) : base(sqlGenerator) { }

        protected override bool CanAddCommand([NotNull] ModificationCommand modificationCommand)
        { return true; }
        protected override bool IsCommandTextValid()
        { return true; }

        protected override string GetCommandText()
        {
            var sb = new StringBuilder();
            foreach (var cmd in ModificationCommands)
            {
                switch (cmd.EntityState)
                {
                    case EntityState.Added:
                        SqlGenerator.AppendInsertOperation(sb, cmd);
                        break;
                    case EntityState.Modified:
                        SqlGenerator.AppendUpdateOperation(sb, cmd);
                        break;
                    case EntityState.Deleted:
                        SqlGenerator.AppendDeleteOperation(sb, cmd);
                        break;
                }
            }
            return sb.ToString();
        }

        public override int Execute(
            [NotNull] RelationalTransaction transaction,
            [NotNull] IRelationalTypeMapper typeMapper,
            [NotNull] DbContext context,
            [NotNull] ILogger logger)
        {
            Check.NotNull(transaction, nameof(transaction));
            Check.NotNull(typeMapper, nameof(typeMapper));
            Check.NotNull(context, nameof(context));
            Check.NotNull(logger, nameof(logger));

            var commandText = GetCommandText();

            using (var storeCommand = CreateStoreCommand(commandText, transaction.DbTransaction, typeMapper, transaction.Connection?.CommandTimeout))
            {
                if (logger.IsEnabled(LogLevel.Verbose))
                {
                    // RelationalLoggerExtensions.LogCommand can't be called because the class
                    // is internal...
                    //logger.LogCommand(storeCommand);
                }

                try
                {
                    using (var reader = storeCommand.ExecuteReader())
                    {
                        while (reader.NextResult()) ;

                        if (reader.RecordsAffected != ModificationCommands.Count)
                        {
                             throw new DbUpdateConcurrencyException(
                                 Microsoft.Data.Entity.Relational.Strings.UpdateConcurrencyException(ModificationCommands.Count, reader.RecordsAffected),
                                 context,
                                 new List<InternalEntityEntry>()  // TODO
                             );
                        }
                    }
                }
                catch (DbUpdateException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DbUpdateException(
                        Microsoft.Data.Entity.Relational.Strings.UpdateStoreException,
                        context,
                        ex,
                        new InternalEntityEntry[0]);
                }
            }

            return ModificationCommands.Count;
        }

        public override Task<int> ExecuteAsync(
            [NotNull] RelationalTransaction transaction,
            [NotNull] IRelationalTypeMapper typeMapper,
            [NotNull] DbContext context,
            [NotNull] ILogger logger,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException("No async here yet");
        }

        public override IRelationalPropertyExtensions GetPropertyExtensions(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            return property.Npgsql();
        }
    }
}
