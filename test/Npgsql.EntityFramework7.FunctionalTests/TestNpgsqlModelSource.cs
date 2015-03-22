using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Framework.DependencyInjection;
using EntityFramework.Npgsql;
using EntityFramework.Npgsql.Extensions;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class TestNpgsqlModelSource : NpgsqlModelSource
    {
        private readonly TestModelSource _testModelSource;

        public TestNpgsqlModelSource(Action<ModelBuilder> onModelCreating, IDbSetFinder setFinder, IModelValidator modelValidator)
            : base(setFinder, modelValidator)
        {
            _testModelSource = new TestModelSource(onModelCreating, setFinder);
        }

        public override IModel GetModel(DbContext context, IModelBuilderFactory modelBuilderFactory)
            => _testModelSource.GetModel(context, modelBuilderFactory);

        public static Func<IServiceProvider, INpgsqlModelSource> GetFactory(Action<ModelBuilder> onModelCreating)
            => p => new TestNpgsqlModelSource(onModelCreating, p.GetRequiredService<IDbSetFinder>(), p.GetRequiredService<IModelValidator>());
    }
}