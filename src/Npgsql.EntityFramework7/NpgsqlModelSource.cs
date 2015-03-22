using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;

namespace EntityFramework.Npgsql
{
	public class NpgsqlModelSource : ModelSource, INpgsqlModelSource
    {
		public NpgsqlModelSource([NotNull] IDbSetFinder setFinder, [NotNull] IModelValidator modelValidator)
			: base(setFinder, modelValidator)
		{ }
	}
}
