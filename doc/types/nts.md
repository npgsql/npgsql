# PostGIS/NetTopologySuite Type Plugin

Before 4.0, Npgsql has supported reading and writing PostGIS types via some bundled .NET classes: `PostgisPoint`, `PostgisLineString`, etc. While this model provided some basic support, a proper representation of spatial types is a complicated task that's beyond Npgsql's scope, and should be handled by a specialized spatial library instead. The leading spatial library in the .NET world is currently [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite), and with the introduction of *type plugins* in Npgsql 4.0, it is now possible to map PostGIS types directly to NetTopologySuite types. This is now the recommended way to store and load PostGIS types.

If you prefer to work with the pre-4.0 types, you can still do so by using the [Npgsql.LegacyPostgis plugin](legacy-postgis.md).

## Adding the NetTopologySuite MyGet feed

Unfortunately, the version of NetTopologySuite currently required by Npgsql (1.15.0) is still in preview. As a result, you will have to add the NetTopologySuite MyGet feed by dropping the following `NuGet.Config` file at the root of your project:

```xml
<configuration>
  <packageSources>
    <add key="NuGet" value="https://api.nuget.org/v3/index.json" />
    <add key="NetTopologySuite" value="https://www.myget.org/F/nettopologysuite/api/v3/index.json" />
  </packageSources>
</configuration>
```

You will be able to remove this when NetTopologySuite make a final release.

## Setup

To use the NetTopologySuite plugin, simply add a dependency on [Npgsql.NetTopologySuite](https://www.nuget.org/packages/Npgsql.NetTopologySuite) and set it up:

```c#
using Npgsql;

// Place this at the beginning of your program to use NetTopologySuite everywhere (recommended)
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();

// Or to temporarily use NetTopologySuite on a single connection only:
conn.TypeMapper.UseNetTopologySuite();
```

## Reading and Writing Geometry Values

When reading PostGIS values from the database, Npgsql will automatically return the appropriate NetTopologySuite types: `Point`, `LineString`, and so on. Npgsql will also automatically recognize NetTopologySuite's types in parameters, and will automatically send the corresponding PostGIS type to the database. The following code demonstrates a roundtrip of a NetTopologySuite `Point` to the database:

```c#
var point = new Point(new Coordinate(1d, 1d));
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

Npgsql uses the same NetTopologySuite types to represent both `geometry` and `geography` - the `Point` type represents a point in either Cartesian or geodetic space. You usually don't need to worry about this distinction because PostgreSQL will usually cast types back and forth as needed. However, it's worth noting that Npgsql sends Cartesian `geometry` by default, because that's the usual requirement. You have the option of telling Npgsql to send `geography` instead by specifying `NpgsqlDbType.Geography`:

```c#
using (var cmd = new NpgsqlCommand("INSERT INTO data (geog) VALUES (@p)", conn))
{
    cmd.Parameters.AddWithValue("@p", NpgsqlDbType.Geography, point);
    cmd.ExecuteNonQuery();
}
```

If you prefer to use `geography` everywhere by default, you can also specify that when setting up the plugin:

```c#
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite(geographyAsDefault: true);
```
