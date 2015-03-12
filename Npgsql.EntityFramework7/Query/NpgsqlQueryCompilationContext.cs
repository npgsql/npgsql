using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Relational.Query;
using Microsoft.Data.Entity.Relational.Query.Methods;
using Microsoft.Data.Entity.Relational.Query.Sql;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlQueryCompilationContext : RelationalQueryCompilationContext
    {
        public NpgsqlQueryCompilationContext(
            [NotNull] IModel model,
            [NotNull] ILogger logger,
            [NotNull] ILinqOperatorProvider linqOperatorProvider,
            [NotNull] IResultOperatorHandler resultOperatorHandler,
            [NotNull] EntityMaterializerSource entityMaterializerSource,
			[NotNull] EntityKeyFactorySource entityKeyFactorySource,
			[NotNull] IQueryMethodProvider queryMethodProvider,
            [NotNull] IMethodCallTranslator methodCallTranslator)
            : base(
                Check.NotNull(model, "model"),
                Check.NotNull(logger, "logger"),
                Check.NotNull(linqOperatorProvider, "linqOperatorProvider"),
                Check.NotNull(resultOperatorHandler, "resultOperatorHandler"),
                Check.NotNull(entityMaterializerSource, "entityMaterializerSource"),
				Check.NotNull(entityKeyFactorySource, "entityKeyFactorySource"),
				Check.NotNull(queryMethodProvider, "queryMethodProvider"),
                Check.NotNull(methodCallTranslator, "methodCallTranslator"))
        {
        }

        public override ISqlQueryGenerator CreateSqlQueryGenerator()
        {
            return new NpgsqlQueryGenerator();
        }

        public override string GetTableName(IEntityType entityType)
        {
            Check.NotNull(entityType, "entityType");

            return entityType.Npgsql().Table;
        }

        public override string GetSchema(IEntityType entityType)
        {
            Check.NotNull(entityType, "entityType");

            return entityType.Npgsql().Schema;
        }

        public override string GetColumnName(IProperty property)
        {
            Check.NotNull(property, "property");

            return property.Npgsql().Column;
        }
    }
}