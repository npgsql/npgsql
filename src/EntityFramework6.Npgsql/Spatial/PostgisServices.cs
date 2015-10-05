using System;
#if ENTITIES6
using System.Data.Entity.Spatial;
#else
using System.Data.Spatial;
#endif

namespace Npgsql.Spatial
{
    /// <summary>
    /// A class exposing spatial services.
    /// </summary>
    public class PostgisServices
        : DbSpatialServices
    {
        /// <summary>
        /// Returns the well known binary value of the geometry input.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override byte[] AsBinary(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_asbinary(:p1)";
                return (byte[])cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Returns the well known binary value of the geography input.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override byte[] AsBinary(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the geographical markup language representation of the geometry input.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override string AsGml(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_asgml(:p1)";
                return (string)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Returns the geographical markup language representation of the geography input.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override string AsGml(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the well known text representation of the geometry input.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override string AsText(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_astext(:p1)";
                return (string)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Returns the well known text representation of the geography input.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override string AsText(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents all points whose distance from this Geometry is less than or equal to distance.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public override DbGeometry Buffer(DbGeometry geometryValue, double distance)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Double, distance);
                cmd.CommandText =
                    "SELECT st_buffer(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Returns a geometry that represents all points whose distance from this Geometry is less than or equal to distance.
        /// Calculations are in the Spatial Reference System of this Geometry. Uses a planar transform wrapper.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public override DbGeography Buffer(DbGeography geographyValue, double distance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if and only if no points of B lie in the exterior of A, and at least one point of the interior of B lies in the interior of A
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override bool Contains(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_contains(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wellKnownValue"></param>
        /// <returns></returns>
        public override object CreateProviderValue(DbGeometryWellKnownValue wellKnownValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, wellKnownValue.WellKnownBinary);
                cmd.CommandText =
                    "SELECT st_geomfromwkb(:p1)";
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wellKnownValue"></param>
        /// <returns></returns>
        public override object CreateProviderValue(DbGeographyWellKnownValue wellKnownValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometryWellKnownValue CreateWellKnownValue(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText = "SELECT st_astext(:p1)";
                var d = new DbGeometryWellKnownValue();
                d.WellKnownText = (string)cmd.ExecuteScalar();
                cmd.CommandText = "SELECT st_asbinary(:p1)";
                d.WellKnownBinary = (byte[])cmd.ExecuteScalar();
                cmd.CommandText = "SELECT st_srid(:p1)";
                d.CoordinateSystemId = (int)cmd.ExecuteScalar();
                return d;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override DbGeographyWellKnownValue CreateWellKnownValue(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Returns TRUE if the supplied geometries have some, but not all, interior points in commo
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override bool Crosses(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_crosses(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// Returns a geometry that represents that part of geometry A that does not intersect with geometry B.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override DbGeometry Difference(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_difference(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Returns a geometry that represents that part of geometry A that does not intersect with geometry B.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="otherGeography"></param>
        /// <returns></returns>
        public override DbGeography Difference(DbGeography geographyValue, DbGeography otherGeography)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Returns TRUE if the Geometries do not "spatially intersect" - if they do not share any space together.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override bool Disjoint(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_disjoint(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        ///  Returns TRUE if the Geometries do not "spatially intersect" - if they do not share any space together.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="otherGeography"></param>
        /// <returns></returns>
        public override bool Disjoint(DbGeography geographyValue, DbGeography otherGeography)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Returns the 2-dimensional cartesian minimum distance (based on spatial ref) between two geometries in projected units.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override double Distance(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_distance(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetDouble(0);
                }
            }
        }

        /// <summary>
        ///  Returns the 2-dimensional cartesian minimum distance (based on spatial ref) between two geometries in projected units.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="otherGeography"></param>
        /// <returns></returns>
        public override double Distance(DbGeography geographyValue, DbGeography otherGeography)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given a geometry collection, returns the index-nth geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override DbGeometry ElementAt(DbGeometry geometryValue, int index)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, index);
                cmd.CommandText =
                    "SELECT st_geometryn(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        ///  Given a geography collection, returns the index-nth geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override DbGeography ElementAt(DbGeography geographyValue, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geographyCollectionWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyCollectionFromBinary(byte[] geographyCollectionWellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geographyCollectionWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyCollectionFromText(string geographyCollectionWellKnownText, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wellKnownBinary"></param>
        /// <returns></returns>
        public override DbGeography GeographyFromBinary(byte[] wellKnownBinary)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyFromBinary(byte[] wellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geographyMarkup"></param>
        /// <returns></returns>
        public override DbGeography GeographyFromGml(string geographyMarkup)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geographyMarkup"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyFromGml(string geographyMarkup, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="providerValue"></param>
        /// <returns></returns>
        public override DbGeography GeographyFromProviderValue(object providerValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wellKnownText"></param>
        /// <returns></returns>
        public override DbGeography GeographyFromText(string wellKnownText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="wellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyFromText(string wellKnownText, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyLineFromBinary(byte[] lineWellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lineWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyLineFromText(string lineWellKnownText, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="multiLineWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyMultiLineFromBinary(byte[] multiLineWellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="multiLineWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyMultiLineFromText(string multiLineWellKnownText, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="multiPointWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyMultiPointFromBinary(byte[] multiPointWellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="multiPointWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyMultiPointFromText(string multiPointWellKnownText, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="multiPolygonWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyMultiPolygonFromBinary(byte[] multiPolygonWellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="multiPolygonWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyMultiPolygonFromText(string multiPolygonWellKnownText, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pointWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyPointFromBinary(byte[] pointWellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pointWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyPointFromText(string pointWellKnownText, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="polygonWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyPolygonFromBinary(byte[] polygonWellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="polygonWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeography GeographyPolygonFromText(string polygonWellKnownText, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the geometry collection from a well know binary representation.
        /// </summary>
        /// <param name="geometryCollectionWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryCollectionFromBinary(byte[] geometryCollectionWellKnownBinary, int coordinateSystemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the geometry collection from a well know binary representation.
        /// </summary>
        /// <param name="geometryCollectionWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        public override DbGeometry GeometryCollectionFromText(string geometryCollectionWellKnownText, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, geometryCollectionWellKnownText);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_geomcollfromtext(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the geometry from its well known binary representation
        /// </summary>
        /// <param name="wellKnownBinary"></param>
        /// <returns></returns>
        public override DbGeometry GeometryFromBinary(byte[] wellKnownBinary)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, wellKnownBinary);
                cmd.CommandText =
                    "SELECT st_geomfromwkb(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the geometry from its well known binary representation
        /// </summary>
        /// <param name="wellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryFromBinary(byte[] wellKnownBinary, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, wellKnownBinary);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_geomfromwkb(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the geometry from a geometic markup language representation.
        /// </summary>
        /// <param name="geometryMarkup"></param>
        /// <returns></returns>
        public override DbGeometry GeometryFromGml(string geometryMarkup)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, geometryMarkup);
                cmd.CommandText =
                    "SELECT st_geomfromgml(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the geometry from a geometic markup language representation.
        /// </summary>
        /// <param name="geometryMarkup"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryFromGml(string geometryMarkup, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, geometryMarkup);
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_geomfromgml(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Wrap a npgsql geometry in a DbGeometry structure.
        /// </summary>
        /// <param name="providerValue"></param>
        /// <returns></returns>
        public override DbGeometry GeometryFromProviderValue(object providerValue)
        {
            return DbSpatialServices.CreateGeometry(this, providerValue);
        }

        /// <summary>
        /// Get the geometry from a well known text value.
        /// </summary>
        /// <param name="wellKnownText"></param>
        /// <returns></returns>
        public override DbGeometry GeometryFromText(string wellKnownText)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, wellKnownText);
                cmd.CommandText =
                    "SELECT st_geomfromtext(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the geometry from a well known text value.
        /// </summary>
        /// <param name="wellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryFromText(string wellKnownText, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, wellKnownText);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_geomfromtext(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a line from its well known binary value.
        /// </summary>
        /// <param name="lineWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryLineFromBinary(byte[] lineWellKnownBinary, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, lineWellKnownBinary);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_linefromwkb(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a line from its well known text value.
        /// </summary>
        /// <param name="lineWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryLineFromText(string lineWellKnownText, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, lineWellKnownText);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_linefromtext(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a multiline from its well known binary value.
        /// </summary>
        /// <param name="multiLineWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryMultiLineFromBinary(byte[] multiLineWellKnownBinary, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, multiLineWellKnownBinary);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_mlinefromwkb(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a multiline from a well known text value.
        /// </summary>
        /// <param name="multiLineWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryMultiLineFromText(string multiLineWellKnownText, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, multiLineWellKnownText);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_mlinefromtext(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a multipoint from its well known binaryrepresentation.
        /// </summary>
        /// <param name="multiPointWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryMultiPointFromBinary(byte[] multiPointWellKnownBinary, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, multiPointWellKnownBinary);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_mpointfromwkb(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a multipoint from its well known text representation.
        /// </summary>
        /// <param name="multiPointWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryMultiPointFromText(string multiPointWellKnownText, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, multiPointWellKnownText);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_mpointfromtext(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a multipolygon from its well known binary value.
        /// </summary>
        /// <param name="multiPolygonWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryMultiPolygonFromBinary(byte[] multiPolygonWellKnownBinary, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, multiPolygonWellKnownBinary);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_mpolyfromwkb(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a multipolygon from its well known text value.
        /// </summary>
        /// <param name="multiPolygonKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryMultiPolygonFromText(string multiPolygonKnownText, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, multiPolygonKnownText);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_mpolyfromtext(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a point from its well known binary value.
        /// </summary>
        /// <param name="pointWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryPointFromBinary(byte[] pointWellKnownBinary, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, pointWellKnownBinary);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_GeomFromWKB(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a point from its well known text value.
        /// </summary>
        /// <param name="pointWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryPointFromText(string pointWellKnownText, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, pointWellKnownText);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_pointfromtext(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a polygon from its well known binary value.
        /// </summary>
        /// <param name="polygonWellKnownBinary"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryPolygonFromBinary(byte[] polygonWellKnownBinary, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Bytea, polygonWellKnownBinary);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_geomfromwkb(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get a polygon from its well known text value.
        /// </summary>
        /// <param name="polygonWellKnownText"></param>
        /// <param name="coordinateSystemId"></param>
        /// <returns></returns>
        public override DbGeometry GeometryPolygonFromText(string polygonWellKnownText, int coordinateSystemId)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Text, polygonWellKnownText);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, coordinateSystemId);
                cmd.CommandText =
                    "SELECT st_geometryfromtext(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Returns the area of the surface if it is a polygon or multi-polygon.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override double? GetArea(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_area(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.IsDBNull(0) ? new double?() : new double?(rdr.GetDouble(0));
                }
            }
        }

        /// <summary>
        ///  Returns the area of the surface if it is a polygon or multi-polygon.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override double? GetArea(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Returns the closure of the combinatorial boundary of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometry GetBoundary(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_boundary(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Returns the centroid of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometry GetCentroid(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_centroid(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the convex hull of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometry GetConvexHull(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_convexhull(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the SRID of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override int GetCoordinateSystemId(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_srid(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetInt32(0);
                }

            }
        }

        /// <summary>
        /// Get the SRID of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override int GetCoordinateSystemId(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the geometry dimension.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override int GetDimension(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_dimension(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetInt32(0);
                }

            }
        }

        /// <summary>
        /// Get the geograpy dimension.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override int GetDimension(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the element count of the geometry collection.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override int? GetElementCount(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_numgeometries(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.IsDBNull(0) ? new int() : new int?(rdr.GetInt32(0));
                }
            }
        }

        /// <summary>
        /// Get the element count of the geometry collection.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override int? GetElementCount(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the elevation of the geometry
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override double? GetElevation(DbGeometry geometryValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the elevation of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override double? GetElevation(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the endpoint of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometry GetEndPoint(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_endpoint(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the endpoint of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override DbGeography GetEndPoint(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the envelope of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometry GetEnvelope(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_envelope(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the exterior ring of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometry GetExteriorRing(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_exteriorring(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Get the ring count of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override int? GetInteriorRingCount(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_numinteriorring(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetInt32(0);
                }
            }
        }

        /// <summary>
        /// Check if the geometry is closed.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override bool? GetIsClosed(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_isclosed(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// Check if the geography is closed;
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override bool? GetIsClosed(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Chekc if the geometry is empty.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override bool GetIsEmpty(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_isempty(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// Check if the geography is empty.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override bool GetIsEmpty(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if the geometry is a linestring, simple and closed.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override bool? GetIsRing(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_isring(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// Check if the geometry is simple.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override bool GetIsSimple(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_issimple(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// Check if the geometry is valid.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override bool GetIsValid(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_isvalid(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// Returns the latitude of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override double? GetLatitude(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the length of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override double? GetLength(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_length(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.IsDBNull(0) ? new double?() : new double?(rdr.GetDouble(0));
                }
            }
        }

        /// <summary>
        /// Returns the length of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override double? GetLength(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the longitutde of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override double? GetLongitude(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override double? GetMeasure(DbGeometry geometryValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override double? GetMeasure(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the point count of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override int? GetPointCount(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_npoints(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.IsDBNull(0) ? new int?() : new int?(rdr.GetInt32(0));
                }
            }
        }

        /// <summary>
        /// Returns the point count of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override int? GetPointCount(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a POINT guaranteed to lie on the geometry surface.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometry GetPointOnSurface(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_pointonsurface(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// returns the spatial type of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override string GetSpatialTypeName(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT geometrytype(:p1)";
                return (string)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Returns the spatial type of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override string GetSpatialTypeName(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the start point of the geometry.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override DbGeometry GetStartPoint(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_startpoint(:p1)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Returns the start point of the geography.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <returns></returns>
        public override DbGeography GetStartPoint(DbGeography geographyValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a point X coordinate.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override double? GetXCoordinate(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_X(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.IsDBNull(0) ? new double?() : new double?(rdr.GetDouble(0));
                }
            }
        }

        /// <summary>
        /// Returns a point Y coordinate.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <returns></returns>
        public override double? GetYCoordinate(DbGeometry geometryValue)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.CommandText =
                    "SELECT st_y(:p1)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.IsDBNull(0) ? new double?() : new double?(rdr.GetDouble(0));
                }
            }
        }

        /// <summary>
        /// Returns the index-nth interior ring of the geometry
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override DbGeometry InteriorRingAt(DbGeometry geometryValue, int index)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Integer, index);
                cmd.CommandText =
                    "SELECT st_interiorringn(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        ///Returns the intersection of two geometries.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override DbGeometry Intersection(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_intersection(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Returns the intersection of two geographies.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="otherGeography"></param>
        /// <returns></returns>
        public override DbGeography Intersection(DbGeography geographyValue, DbGeography otherGeography)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns TRUE if the Geometries/Geography "spatially intersect in 2D" - (share any portion of space) and FALSE if they don't (they are Disjoint).
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override bool Intersects(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry);
                cmd.CommandText =
                    "SELECT st_intersects(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// Returns TRUE if the Geometries/Geography "spatially intersect in 2D" - (share any portion of space) and FALSE if they don't (they are Disjoint).
        ///  For geography -- tolerance is 0.00001 meters (so any points that close are considered to intersect)
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="otherGeography"></param>
        /// <returns></returns>
        public override bool Intersects(DbGeography geographyValue, DbGeography otherGeography)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns TRUE if the Geometries share space, are of the same dimension, but are not completely contained by each other.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override bool Overlaps(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_overlaps(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override DbGeometry PointAt(DbGeometry geometryValue, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override DbGeography PointAt(DbGeography geographyValue, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if this Geometry is spatially related to anotherGeometry,
        /// by testing for intersections between the Interior, Boundary and Exterior of the two geometries
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public override bool Relate(DbGeometry geometryValue, DbGeometry otherGeometry, string matrix)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.Parameters.AddWithValue("p3", NpgsqlTypes.NpgsqlDbType.Text, matrix);
                cmd.CommandText =
                    "SELECT st_relate(:p1,:p2,:p3)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        ///  Returns true if the given geometries represent the same geometry. Directionality is ignored.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override bool SpatialEquals(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_equals(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        ///  Returns true if the given geometries represent the same geometry. Directionality is ignored.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="otherGeography"></param>
        /// <returns></returns>
        public override bool SpatialEquals(DbGeography geographyValue, DbGeography otherGeography)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Returns a geometry that represents the portions of A and B that do not intersect.
        /// It is called a symmetric difference because ST_SymDifference(A,B) = ST_SymDifference(B,A).
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override DbGeometry SymmetricDifference(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_symdifference(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        ///  Returns a geometry that represents the portions of A and B that do not intersect.
        /// It is called a symmetric difference because ST_SymDifference(A,B) = ST_SymDifference(B,A).
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="otherGeography"></param>
        /// <returns></returns>
        public override DbGeography SymmetricDifference(DbGeography geographyValue, DbGeography otherGeography)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns TRUE if the geometries have at least one point in common, but their interiors do not intersect.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override bool Touches(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_touches(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        /// <summary>
        /// Returns a geometry that represents the point set union of the Geometries.
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override DbGeometry Union(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_union(:p1,:p2)";
                return DbSpatialServices.CreateGeometry(this, cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Returns a geometry that represents the point set union of the Geometries.
        /// </summary>
        /// <param name="geographyValue"></param>
        /// <param name="otherGeography"></param>
        /// <returns></returns>
        public override DbGeography Union(DbGeography geographyValue, DbGeography otherGeography)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the geometry A is completely inside geometry B
        /// </summary>
        /// <param name="geometryValue"></param>
        /// <param name="otherGeometry"></param>
        /// <returns></returns>
        public override bool Within(DbGeometry geometryValue, DbGeometry otherGeometry)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlTypes.NpgsqlDbType.Geometry, geometryValue.ProviderValue);
                cmd.Parameters.AddWithValue("p2", NpgsqlTypes.NpgsqlDbType.Geometry, otherGeometry.ProviderValue);
                cmd.CommandText =
                    "SELECT st_within(:p1,:p2)";
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    return rdr.GetBoolean(0);
                }
            }
        }

        private NpgsqlConnection _connection;

        /// <summary>
        /// Set the provider connection
        /// </summary>
        /// <param name="c"></param>
        public void SetConnection(Npgsql.NpgsqlConnection c)
        {
            _connection = c;
        }
    }
}
