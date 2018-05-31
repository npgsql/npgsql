# PostGIS Legacy Type Plugin

Since 4.0, Npgsql supports *type plugins*, which are external nuget packages that modify how Npgsql maps PostgreSQL values to CLR types. The previous support for [PostGIS spatial types](https://postgis.net/) has been moved out of Npgsql and into the plugin Npgsql.LegacyPostgis. The recommended way to read and write spatial types is now [Npgsql.NetTopologySuite](nts.md), which maps PostGIS types to [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite), a more complete library dedicated to spatial. The NetTopologySuite plugin is now the recommended way to do spatial in Npgsql, but the Npgsql.LegacyPostgis still exists to provide support for the previous types.

At this time, the legacy types only support geometry, not geography, and only XY (not XYZ, XYM or XYZM).

## Setup

To use the PostGIS legacy plugin, simply add a dependency on [Npgsql.LegacyPostgis](https://www.nuget.org/packages/Npgsql.LegacyPostgis) and set it up:

```c#
using Npgsql;

// Place this at the beginning of your program to use legacy PostGIS everywhere (recommended):
NpgsqlConnection.GlobalTypeMapper.UseLegacyPostgis();

// Or to temporarily use legacy PostGIS on a single connection only:
conn.TypeMapper.UseLegacyPostgis();
```

## Usage

If you've used the internal PostGIS types in Npgsql 3.2 or earlier, the plugin works in the same way:

```c#
NpgsqlConnection.GlobalTypeMapper.UseLegacyPostgis();

// Write
var cmd = new NpgsqlCommand("INSERT INTO table (pg_point, pg_polygon) VALUES (@point, @polygon)", conn);
cmd.Parameters.AddWithValue("point", new PostgisPoint(3.5, 4.5));
cmd.ExecuteNonQuery();

// Read
var cmd = new NpgsqlCommand("SELECT * FROM table", conn);
var reader = cmd.ExecuteReader();
while (reader.Read()) {
    var point = reader.GetFieldValue<PostgisPoint>(0);
    var polygon = reader.GetFieldValue<PostgisPolygon>(1);
}
```
