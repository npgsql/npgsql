using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Query.ExpressionVisitors;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.Internal
{
    public class NpgsqlQueryCompilationContext : RelationalQueryCompilationContext
    {
        public NpgsqlQueryCompilationContext(
            [NotNull] ISensitiveDataLogger logger,
            [NotNull] IEntityQueryModelVisitorFactory entityQueryModelVisitorFactory,
            [NotNull] IRequiresMaterializationExpressionVisitorFactory requiresMaterializationExpressionVisitorFactory,
            [NotNull] ILinqOperatorProvider linqOpeartorProvider,
            [NotNull] IQueryMethodProvider queryMethodProvider,
            [NotNull] Type contextType,
            bool trackQueryResults)
            : base(
                Check.NotNull(logger, nameof(logger)),
                Check.NotNull(entityQueryModelVisitorFactory, nameof(entityQueryModelVisitorFactory)),
                Check.NotNull(requiresMaterializationExpressionVisitorFactory, nameof(requiresMaterializationExpressionVisitorFactory)),
                Check.NotNull(linqOpeartorProvider, nameof(linqOpeartorProvider)),
                Check.NotNull(queryMethodProvider, nameof(queryMethodProvider)),
                Check.NotNull(contextType, nameof(contextType)),
                trackQueryResults)
        {
        }

        public override bool IsLateralJoinSupported => true;
    }
}
