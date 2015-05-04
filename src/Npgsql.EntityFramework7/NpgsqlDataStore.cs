// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Query;
using Microsoft.Data.Entity.Relational.Query.Methods;
using Npgsql.EntityFramework7.Query;
using Npgsql.EntityFramework7.Update;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlDataStore : RelationalDataStore, INpgsqlDataStore
    {
        public NpgsqlDataStore(
            [NotNull] IModel model,
            [NotNull] IEntityKeyFactorySource entityKeyFactorySource,
            [NotNull] IEntityMaterializerSource entityMaterializerSource,
            [NotNull] IClrAccessorSource<IClrPropertyGetter> clrPropertyGetterSource,
            [NotNull] INpgsqlEFConnection connection,
            [NotNull] INpgsqlCommandBatchPreparer batchPreparer,
            [NotNull] INpgsqlBatchExecutor batchExecutor,
            [NotNull] IDbContextOptions options,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] INpgsqlValueBufferFactoryFactory valueBufferFactoryFactory)
            : base(
                Check.NotNull(model, nameof(model)),
                Check.NotNull(entityKeyFactorySource, nameof(entityKeyFactorySource)),
                Check.NotNull(entityMaterializerSource, nameof(entityMaterializerSource)),
                Check.NotNull(clrPropertyGetterSource, nameof(clrPropertyGetterSource)),
                Check.NotNull(connection, nameof(connection)),
                Check.NotNull(batchPreparer, nameof(batchPreparer)),
                Check.NotNull(batchExecutor, nameof(batchExecutor)),
                Check.NotNull(options, nameof(options)),
                Check.NotNull(loggerFactory, nameof(loggerFactory)),
                Check.NotNull(valueBufferFactoryFactory, nameof(valueBufferFactoryFactory)))
        {
        }

        protected override RelationalQueryCompilationContext CreateQueryCompilationContext(
            ILinqOperatorProvider linqOperatorProvider,
            IResultOperatorHandler resultOperatorHandler,
            IQueryMethodProvider enumerableMethodProvider,
            IMethodCallTranslator methodCallTranslator)
        {
            Check.NotNull(linqOperatorProvider, nameof(linqOperatorProvider));
            Check.NotNull(resultOperatorHandler, nameof(resultOperatorHandler));
            Check.NotNull(enumerableMethodProvider, nameof(enumerableMethodProvider));
            Check.NotNull(methodCallTranslator, nameof(methodCallTranslator));

            return new NpgsqlQueryCompilationContext(
                Model,
                Logger,
                linqOperatorProvider,
                resultOperatorHandler,
                EntityMaterializerSource,
                EntityKeyFactorySource,
                ClrPropertyGetterSource,
                enumerableMethodProvider,
                methodCallTranslator,
                (INpgsqlValueBufferFactoryFactory)ValueBufferFactoryFactory);
        }
    }
}
