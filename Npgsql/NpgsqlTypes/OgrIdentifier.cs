using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents the identifier of the Well Known Binary representation of a geographical feature specified by the OGC.
    /// http://portal.opengeospatial.org/files/?artifact_id=13227 Chapter 6.3.2.7 
    /// </summary>
    internal enum OgrIdentifier 
    {
        Point               = 1,
        LineString          = 2,
        Polygon             = 3,
        MultiPoint          = 4,
        MultiLineString     = 5,
        MultiPolygon        = 6,
        GeometryCollection  = 7        
    }
}
