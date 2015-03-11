using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Query;
using Microsoft.Data.Entity.Relational.Query.Methods;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlDataStore : RelationalDataStore
    {
        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected NpgsqlDataStore()
        {
        }

        public NpgsqlDataStore(
            [NotNull] StateManager stateManager,
            [NotNull] IModel model,
            [NotNull] EntityKeyFactorySource entityKeyFactorySource,
            [NotNull] EntityMaterializerSource entityMaterializerSource,
            [NotNull] ClrCollectionAccessorSource collectionAccessorSource,
            [NotNull] ClrPropertySetterSource propertySetterSource,
            [NotNull] NpgsqlEntityFrameworkConnection connection,
            [NotNull] NpgsqlCommandBatchPreparer batchPreparer,
            [NotNull] NpgsqlBatchExecutor batchExecutor,
            [NotNull] IDbContextOptions options,
            [NotNull] ILoggerFactory loggerFactory)
            : base(stateManager, model, entityKeyFactorySource, entityMaterializerSource,
                collectionAccessorSource, propertySetterSource, connection, batchPreparer, batchExecutor, options, loggerFactory)
        {
        }

        protected RelationalValueReaderFactory ValueReaderFactory
        {
            get { return new RelationalObjectArrayValueReaderFactory(); }
        }

        protected override RelationalQueryCompilationContext CreateQueryCompilationContext(
            ILinqOperatorProvider linqOperatorProvider,
            IResultOperatorHandler resultOperatorHandler,
            IQueryMethodProvider enumerableMethodProvider,
            IMethodCallTranslator methodCallTranslator)
        {
            Check.NotNull(linqOperatorProvider, "linqOperatorProvider");
            Check.NotNull(resultOperatorHandler, "resultOperatorHandler");
            Check.NotNull(enumerableMethodProvider, "enumerableMethodProvider");
            Check.NotNull(methodCallTranslator, "methodCallTranslator");

            return new NpgsqlQueryCompilationContext(
                Model,
                Logger,
                linqOperatorProvider,
                resultOperatorHandler,
                EntityMaterializerSource,
				EntityKeyFactorySource,
                enumerableMethodProvider,
                methodCallTranslator);
        }
    }
}
