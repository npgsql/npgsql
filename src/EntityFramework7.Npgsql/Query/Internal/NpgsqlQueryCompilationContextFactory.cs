using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.ExpressionVisitors;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.Internal
{
    public class NpgsqlQueryCompilationContextFactory : RelationalQueryCompilationContextFactory
    {
        public NpgsqlQueryCompilationContextFactory(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IEntityQueryModelVisitorFactory entityQueryModelVisitorFactory,
            [NotNull] IRequiresMaterializationExpressionVisitorFactory requiresMaterializationExpressionVisitorFactory,
            [NotNull] DbContext context)
            : base(Check.NotNull(loggerFactory, nameof(loggerFactory)),
                Check.NotNull(entityQueryModelVisitorFactory, nameof(entityQueryModelVisitorFactory)),
                Check.NotNull(requiresMaterializationExpressionVisitorFactory, nameof(requiresMaterializationExpressionVisitorFactory)),
                Check.NotNull(context, nameof(context)))
        {
        }

        public override QueryCompilationContext Create(bool async)
            => async
                ? new NpgsqlQueryCompilationContext(
                    CreateLogger(),
                    EntityQueryModelVisitorFactory,
                    RequiresMaterializationExpressionVisitorFactory,
                    new AsyncLinqOperatorProvider(),
                    new AsyncQueryMethodProvider(),
                    ContextType)
                : new NpgsqlQueryCompilationContext(
                    CreateLogger(),
                    EntityQueryModelVisitorFactory,
                    RequiresMaterializationExpressionVisitorFactory,
                    new LinqOperatorProvider(),
                    new QueryMethodProvider(),
                    ContextType);
    }
}
