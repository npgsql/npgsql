# PostGIS/GeoJSON Type Plugin

Before 4.0, Npgsql has supported reading and writing PostGIS types via some bundled .NET classes: `PostgisPoint`, `PostgisLineString`, etc. While this model provided some basic support, a proper representation of spatial types is a complicated task that's beyond Npgsql's scope, and should be handled by a specialized spatial library instead.

The [Npgsql.GeoJSON](https://nuget.org/packages/Npgsql.GeoJSON) plugin makes Npgsql read and write PostGIS spatial types as [GeoJSON (RFC7946) types](http://geojson.org/), via the [GeoJSON.NET](https://github.com/GeoJSON-Net/GeoJSON.Net) library.

As an alternative, you can use [Npgsql.NetTopologySuite](nts.md), which is a full-fledged .NET spatial library with many features. If you prefer to work with the pre-4.0 types, you can still do so by using the [Npgsql.LegacyPostgis plugin](legacy-postgis.md).

## Setup

To use the GeoJSON plugin, simply add a dependency on [Npgsql.GeoJSON](https://www.nuget.org/packages/Npgsql.GeoJSON) and set it up:

```c#
using Npgsql;

// Place this at the beginning of your program to use NetTopologySuite everywhere (recommended)
NpgsqlConnection.GlobalTypeMapper.UseGeoJSON();

// Or to temporarily use GeoJSON on a single connection only:
conn.TypeMapper.UseGeoJSON();
```

## Reading and Writing Geometry Values

When reading PostGIS values from the database, Npgsql will automatically return the appropriate GeoJSON types: `Point`, `LineString`, and so on. Npgsql will also automatically recognize GeoJSON's types in parameters, and will automatically send the corresponding PostGIS type to the database. The following code demonstrates a roundtrip of a GeoJSON `Point` to the database:

```c#
var point = new Point(new Position(51.899523, -2.124156));
conn.ExecuteNonQuery("CREATE TEMP TABLE data (geom GEOMETRY)");
using (var cmd = new NpgsqlCommand("INSERT INTO data (geom) VALUES (@p)", conn))
{
    cmd.Parameters.AddWithValue("@p", point);
    cmd.ExecuteNonQuery();
}

using (var cmd = new NpgsqlCommand("SELECT geom FROM data", conn))
using (var reader = cmd.ExecuteReader())
{
    reader.Read();
    Assert.That(reader[0], Is.EqualTo(point));
}
```

You may also explicitly specify a parameter's type by setting `NpgsqlDbType.Geometry`.

## Geography (geodetic) Support

PostGIS has two types: `geometry` (for Cartesian coordinates) and `geography` (for geodetic or spherical coordinates). You can read about the geometry/geography distinction [in the PostGIS docs](https://postgis.net/docs/manual-2.4/using_postgis_dbmanagement.html#PostGIS_Geography) or in [this blog post](http://workshops.boundlessgeo.com/postgis-intro/geography.html). In a nutshell, `geography` is much more accurate when doing calculations over long distances, but is more expensive computationally and supports only a small subset of the spatial operations supported by `geometry`.

Npgsql uses the same GeoJSON types to represent both `geometry` and `geography` - the `Point` type represents a point in either Cartesian or geodetic space. You usually don't need to worry about this distinction because PostgreSQL will usually cast types back and forth as needed. However, it's worth noting that Npgsql sends Cartesian `geometry` by default, because that's the usual requirement. You have the option of telling Npgsql to send `geography` instead by specifying `NpgsqlDbType.Geography`:

```c#
using (var cmd = new NpgsqlCommand("INSERT INTO data (geog) VALUES (@p)", conn))
{
    cmd.Parameters.AddWithValue("@p", NpgsqlDbType.Geography, point);
    cmd.ExecuteNonQuery();
}
```

If you prefer to use `geography` everywhere by default, you can also specify that when setting up the plugin:

```c#
NpgsqlConnection.GlobalTypeMapper.UseGeoJSON(geographyAsDefault: true);
```

