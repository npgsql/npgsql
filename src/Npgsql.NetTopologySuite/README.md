Npgsql is the open source .NET data provider for PostgreSQL. It allows you to connect and interact with PostgreSQL server using .NET.

This package is an Npgsql plugin which allows you to interact with spatial data provided by the PostgreSQL [PostGIS extension](https://postgis.net); PostGIS is a mature, standard extension considered to provide top-of-the-line database spatial features. On the .NET side, the plugin adds support for the types from the [NetTopologySuite library](https://github.com/NetTopologySuite/NetTopologySuite), allowing you to read and write them directly to PostgreSQL. 

To use the NetTopologySuite plugin, simply add a dependency on this package and set it up at program startup:

```csharp
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();
```

Once this is done, you can simply use NetTopologySuite types when interacting with PostgreSQL:

```csharp
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

For more information, [visit the NetTopologySuite plugin documentation page](https://www.npgsql.org/doc/types/nts.html).
