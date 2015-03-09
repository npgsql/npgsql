using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlModelSource : ModelSourceBase
	{
		public NpgsqlModelSource([NotNull] DbSetFinder setFinder, [NotNull] ModelValidator modelValidator)
			: base(setFinder, modelValidator)
		{ }
	}
}