using System;
using System.Threading.Tasks;
using Npgsql.GeoJSON.Internal;

namespace Npgsql.GeoJSON;

/// <summary>
/// Extensions for getting a CrsMap from a database.
/// </summary>
public static class CrsMapExtensions
{
    /// <summary>
    /// Gets the full crs details from the database.
    /// </summary>
    /// <param name="dataSource"></param>
    public static async Task<CrsMap> GetCrsMapAsync(this NpgsqlDataSource dataSource)
    {
        var builder = new CrsMapBuilder();
        using var cmd = GetCsrCommand(dataSource);
        using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

        while (await reader.ReadAsync().ConfigureAwait(false))
            builder.Add(new CrsMapEntry(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));

        return builder.Build();
    }

    /// <summary>
    /// Gets the full crs details from the database.
    /// </summary>
    /// <param name="dataSource"></param>
    public static CrsMap GetCrsMap(this NpgsqlDataSource dataSource)
    {
        var builder = new CrsMapBuilder();
        using var cmd = GetCsrCommand(dataSource);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
            builder.Add(new CrsMapEntry(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));

        return builder.Build();
    }

    static NpgsqlCommand GetCsrCommand(NpgsqlDataSource dataSource)
        => dataSource.CreateCommand("""
        SELECT min(srid), max(srid), auth_name
        FROM(SELECT srid, auth_name, srid - rank() OVER(PARTITION BY auth_name ORDER BY srid) AS range FROM spatial_ref_sys) AS s
        GROUP BY range, auth_name
        ORDER BY 1;
        """);
}
