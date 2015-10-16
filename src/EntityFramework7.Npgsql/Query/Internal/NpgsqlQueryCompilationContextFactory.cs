using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Query.ExpressionVisitors;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.Internal
{
    public class NpgsqlQueryCompilationContextFactory : RelationalQueryCompilationContextFactory
    {
        public NpgsqlQueryCompilationContextFactory(
            [NotNull] ISensitiveDataLogger<NpgsqlQueryCompilationContextFactory> logger,
            [NotNull] IEntityQueryModelVisitorFactory entityQueryModelVisitorFactory,
            [NotNull] IRequiresMaterializationExpressionVisitorFactory requiresMaterializationExpressionVisitorFactory,
            [NotNull] DbContext context)
            : base(Check.NotNull(logger, nameof(logger)),
                Check.NotNull(entityQueryModelVisitorFactory, nameof(entityQueryModelVisitorFactory)),
                Check.NotNull(requiresMaterializationExpressionVisitorFactory, nameof(requiresMaterializationExpressionVisitorFactory)),
                Check.NotNull(context, nameof(context)))
        {
        }

        public override QueryCompilationContext Create(bool async)
            => async
                ? new NpgsqlQueryCompilationContext(
                    (ISensitiveDataLogger)Logger,
                    EntityQueryModelVisitorFactory,
                    RequiresMaterializationExpressionVisitorFactory,
                    new AsyncLinqOperatorProvider(),
                    new AsyncQueryMethodProvider(),
                    ContextType,
                    TrackQueryResults)
                : new NpgsqlQueryCompilationContext(
                    (ISensitiveDataLogger)Logger,
                    EntityQueryModelVisitorFactory,
                    RequiresMaterializationExpressionVisitorFactory,
                    new LinqOperatorProvider(),
                    new QueryMethodProvider(),
                    ContextType,
                    TrackQueryResults);
    }
}
