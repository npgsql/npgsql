# PostGIS/GeoJSON Type Plugin

The [Npgsql.GeoJSON](https://nuget.org/packages/Npgsql.GeoJSON) plugin makes Npgsql read and write PostGIS spatial types as [GeoJSON (RFC7946) types](http://geojson.org/), via the [GeoJSON.NET](https://github.com/GeoJSON-Net/GeoJSON.Net) library.

As an alternative, you can use [Npgsql.NetTopologySuite](nts.md), which is a full-fledged .NET spatial library with many features.

## Setup

To avoid forcing a dependency on the GeoJSON library for users not using spatial, GeoJSON support is delivered as a separate plugin. To use the plugin, simply add a dependency on [Npgsql.GeoJSON](https://www.nuget.org/packages/Npgsql.GeoJSON) and set it up in one of the following ways:

### [NpgsqlDataSource](#tab/datasource)

> [!NOTE]
> `NpgsqlDataSource` was introduced in Npgsql 7.0, and is the recommended way to manage type mapping. If you're using an older version, see the other methods.

```c#
var dataSourceBuilder = new NpgsqlDataSourceBuilder(...);
dataSourceBuilder.UseGeoJson();
await using var dataSource = dataSourceBuilder.Build();
```

### [Global mapping](#tab/global)

If you're using an older version of Npgsql which doesn't yet support `NpgsqlDataSource`, you can configure mappings globally for all connections in your application:

```c#
NpgsqlConnection.GlobalTypeMapper.UseGeoJson();
```

For this to work, you must place this code at the beginning of your application, before any other Npgsql API is called. Note that in Npgsql 7.0, global type mappings are obsolete (but still supported) - `NpgsqlDataSource` is the recommended way to manage type mappings.

### [Connection mapping](#tab/connection)

> [!NOTE]
> This mapping method has been removed in Npgsql 7.0.

Older versions of Npgsql supported configuring a type mapping on an individual connection, as follows:

```c#
var conn = new NpgsqlConnection(...);
conn.TypeMapper.UseGeoJson();
```

***

## Reading and Writing Geometry Values

When reading PostGIS values from the database, Npgsql will automatically return the appropriate GeoJSON types: `Point`, `LineString`, and so on. Npgsql will also automatically recognize GeoJSON's types in parameters, and will automatically send the corresponding PostGIS type to the database. The following code demonstrates a roundtrip of a GeoJSON `Point` to the database:

```c#
conn.ExecuteNonQuery("CREATE TEMP TABLE data (geom GEOMETRY)");

await using (var cmd = new NpgsqlCommand("INSERT INTO data (geom) VALUES ($1)", conn))
{
    cmd.Parameters.Add(new() { Value = new Point(new Position(51.899523, -2.124156)) });
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = new NpgsqlCommand("SELECT geom FROM data", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    await reader.ReadAsync();
    var point2 = reader.GetFieldValue<Point>(0);;
}
```

You may also explicitly specify a parameter's type by setting `NpgsqlDbType.Geometry`.

## Geography (geodetic) Support

PostGIS has two types: `geometry` (for Cartesian coordinates) and `geography` (for geodetic or spherical coordinates). You can read about the geometry/geography distinction [in the PostGIS docs](https://postgis.net/docs/manual-2.4/using_postgis_dbmanagement.html#PostGIS_Geography) or in [this blog post](http://workshops.boundlessgeo.com/postgis-intro/geography.html). In a nutshell, `geography` is much more accurate when doing calculations over long distances, but is more expensive computationally and supports only a small subset of the spatial operations supported by `geometry`.

Npgsql uses the same GeoJSON types to represent both `geometry` and `geography` - the `Point` type represents a point in either Cartesian or geodetic space. You usually don't need to worry about this distinction because PostgreSQL will usually cast types back and forth as needed. However, it's worth noting that Npgsql sends Cartesian `geometry` by default, because that's the usual requirement. You have the option of telling Npgsql to send `geography` instead by specifying `NpgsqlDbType.Geography`:

```c#
using (var cmd = new NpgsqlCommand("INSERT INTO data (geog) VALUES ($1)", conn))
{
    cmd.Parameters.Add(new() { Value = point, NpgsqlDbType = NpgsqlDbType.Geography });
    await cmd.ExecuteNonQueryAsync();
}
```

If you prefer to use `geography` everywhere by default, you can also specify that when setting up the plugin:

```c#
dataSourceBuilder.UseGeoJson(geographyAsDefault: true);
```
