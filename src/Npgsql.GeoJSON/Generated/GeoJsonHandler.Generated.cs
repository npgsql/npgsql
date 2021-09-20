
using System;

using System.Threading;

using System.Threading.Tasks;

using Npgsql.Internal;

using System.Collections.Concurrent;

using System.Collections.ObjectModel;

using GeoJSON.Net;

using GeoJSON.Net.CoordinateReferenceSystem;

using GeoJSON.Net.Geometry;

using Npgsql.BackendMessages;

using Npgsql.Internal.TypeHandling;

using Npgsql.PostgresTypes;


#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql.GeoJSON.Internal
{
    partial class GeoJsonHandler
    {
        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                
                Point converted => ((INpgsqlTypeHandler<Point>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                MultiPoint converted => ((INpgsqlTypeHandler<MultiPoint>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                Polygon converted => ((INpgsqlTypeHandler<Polygon>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                MultiPolygon converted => ((INpgsqlTypeHandler<MultiPolygon>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                LineString converted => ((INpgsqlTypeHandler<LineString>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                MultiLineString converted => ((INpgsqlTypeHandler<MultiLineString>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                
                GeometryCollection converted => ((INpgsqlTypeHandler<GeometryCollection>)this).ValidateAndGetLength(converted, ref lengthCache, parameter),
                

                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type GeoJsonHandler")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                
                Point converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                MultiPoint converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                Polygon converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                MultiPolygon converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                LineString converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                MultiLineString converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                
                GeometryCollection converted => WriteWithLength(converted, buf, lengthCache, parameter, async, cancellationToken),
                

                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type GeoJsonHandler")
            };
    }
}
