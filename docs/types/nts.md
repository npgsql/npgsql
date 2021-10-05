# PostGIS/NetTopologySuite Type Plugin

PostgreSQL supports spatial data and operations via [the PostGIS extension](https://postgis.net/), which is a mature and feature-rich database spatial implementation. .NET doesn't provide a standard spatial library, but [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) is a leading spatial library. Npgsql has a plugin which allows which allows you to map the NTS types PostGIS columns, and even translate many useful spatial operations to SQL. This is the recommended way to interact with spatial types in Npgsql.

PostgreSQL provides support for spatial types (geometry/geography) via the powerful [PostGIS](https://postgis.net/) extension; this allows you to store points and other spatial constructs in the database, and efficiently perform operations and searches on them. Npgsql supports the PostGIS types via [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite), which is the leading spatial library in the .NET world: the NTS types can be read and written directly to their corresponding PostGIS types. This is the recommended way to work with spatial types in Npgsql.

## Setup

To avoid forcing a dependency on the NetTopologySuite library for users not using spatial, NTS support is delivered as a separate plugin. To use the plugin, simply add a dependency on [Npgsql.NetTopologySuite](https://www.nuget.org/packages/Npgsql.NetTopologySuite) and set it up:

```c#
using Npgsql;

// Place this at the beginning of your program to use NetTopologySuite everywhere (recommended)
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();

// Or to temporarily use NetTopologySuite on a single connection only
conn.TypeMapper.UseNetTopologySuite();
```

By default the plugin handles only ordinates provided by the `DefaultCoordinateSequenceFactory` of `GeometryServiceProvider.Instance`. If `GeometryServiceProvider` is initialized automatically the X and Y ordinates are handled. To change the behavior specify the `handleOrdinates` parameter like in the following example:

```c#
conn.TypeMapper.UseNetTopologySuite(handleOrdinates: Ordinates.XYZ);
```

To process the M ordinate, you must initialize `GeometryServiceProvider.Instance` to a new `NtsGeometryServices` instance with `coordinateSequenceFactory` set to a `DotSpatialAffineCoordinateSequenceFactory`. Or you can specify the factory when calling `UseNetTopologySuite`.

```c#
// Place this at the beginning of your program to use the specified settings everywhere (recommended)
GeometryServiceProvider.Instance = new NtsGeometryServices(
    new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XYM),
    new PrecisionModel(PrecisionModels.Floating),
    -1);

// Or specify settings for Npgsql only
conn.TypeMapper.UseNetTopologySuite(
    new DotSpatialAffineCoordinateSequenceFactory(Ordinates.XYM));
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
