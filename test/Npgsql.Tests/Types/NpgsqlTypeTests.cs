using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Npgsql.Tests.Types
{
    public abstract class NpgsqlTypeTests : TestBase
    {
        protected async Task BackendTrue(string query, params NpgsqlParameter[] parameters) =>
            Assert.True(await Execute<bool>(query, parameters));

        private async Task<TResult> Execute<TResult>(string query, IEnumerable<NpgsqlParameter> parameters)
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);

            foreach (var parameter in parameters)
                cmd.Parameters.Add(parameter);

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            return await reader.GetFieldValueAsync<TResult>(0);
        }

        // The attribute should work some day, but not now.
        protected NpgsqlParameter<T> Parameter<T>(T value, [CallerArgumentExpression("value")] string? name = null) =>
            new NpgsqlParameter<T>(name!, value);
    }
}
