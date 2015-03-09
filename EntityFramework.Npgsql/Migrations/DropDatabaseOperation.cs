using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Migrations.Operations;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Migrations
{
    public class DropDatabaseOperation : MigrationOperation
    {
        public DropDatabaseOperation(
            [NotNull] string name,
            [CanBeNull] IReadOnlyDictionary<string, string> annotations = null)
            : base(annotations)
        {
            Check.NotEmpty(name, nameof(name));

            Name = name;
        }

        public virtual string Name { get; }
    }
}
