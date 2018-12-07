namespace Npgsql.NetTopologySuite
{
    // This is copied from NetTopologySuite.IO.PostGis, fixing some bugs until the fixes are merged upstream
    // https://github.com/NetTopologySuite/NetTopologySuite.IO.PostGis/blob/master/NetTopologySuite.IO.PostGis/PostGisGeometryType.cs
    internal enum PostGisGeometryType
    {

        /// <summary>
        /// The OGIS geometry type number for points.
        /// </summary>
        Point = 1,

        /// <summary>
        /// The OGIS geometry type number for lines.
        /// </summary>
        LineString = 2,

        /// <summary>
        /// The OGIS geometry type number for polygons.
        /// </summary>
        Polygon = 3,

        /// <summary>
        /// The OGIS geometry type number for aggregate points.
        /// </summary>
        MultiPoint = 4,

        /// <summary>
        /// The OGIS geometry type number for aggregate lines.
        /// </summary>
        MultiLineString = 5,

        /// <summary>
        /// The OGIS geometry type number for aggregate polygons.
        /// </summary>
        MultiPolygon = 6,

        /// <summary>
        /// The OGIS geometry type number for feature collections.
        /// </summary>
        GeometryCollection = 7,

    }
}
