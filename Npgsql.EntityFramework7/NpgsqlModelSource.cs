using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlModelSource : ModelSource
	{
		public NpgsqlModelSource([NotNull] DbSetFinder setFinder, [NotNull] ModelValidator modelValidator)
			: base(setFinder, modelValidator)
		{ }
	}
}
