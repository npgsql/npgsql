using System;
using System.Data;
using GeoAPI;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.NetTopologySuite;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Extension allowing adding the NetTopologySuite plugin to an Npgsql type mapper.
    /// </summary>
    public static class NpgsqlNetTopologySuiteExtensions
    {
        static readonly Type[] ClrTypes = new[]
        {
            typeof(IGeometry), typeof(Geometry),
            typeof(IPoint), typeof(Point),
            typeof(ILineString), typeof(LineString),
            typeof(IPolygon), typeof(Polygon),
            typeof(IMultiPoint), typeof(MultiPoint),
            typeof(IMultiLineString), typeof(MultiLineString),
            typeof(IMultiPolygon), typeof(MultiPolygon),
            typeof(IGeometryCollection), typeof(GeometryCollection)
        };

        /// <summary>
        /// Sets up NetTopologySuite mappings for the PostGIS types.
        /// </summary>
        /// <param name="mapper">The type mapper to set up (global or connection-specific).</param>
        /// <param name="coordinateSequenceFactory">The factory which knows how to build a particular implementation of ICoordinateSequence from an array of Coordinates.</param>
        /// <param name="precisionModel">Specifies the grid of allowable points.</param>
        /// <param name="handleOrdinates">Specifies the ordinates which will be handled. Not specified ordinates will be ignored.
        /// If <see cref="F:GeoAPI.Geometries.Ordiantes.None" /> is specified, an actual value will be taken from
        /// the <see cref="P:GeoAPI.Geometries.ICoordinateSequenceFactory.Ordinates"/> property of <paramref name="coordinateSequenceFactory"/>.</param>
        /// <param name="geographyAsDefault">Specifies that the geography type is used for mapping by default.</param>
        public static INpgsqlTypeMapper UseNetTopologySuite(
            this INpgsqlTypeMapper mapper,
            ICoordinateSequenceFactory coordinateSequenceFactory = null,
            IPrecisionModel precisionModel = null,
            Ordinates handleOrdinates = Ordinates.None,
            bool geographyAsDefault = false)
        {
            if (coordinateSequenceFactory == null)
                coordinateSequenceFactory = GeometryServiceProvider.Instance.DefaultCoordinateSequenceFactory;
            if (precisionModel == null)
                precisionModel = GeometryServiceProvider.Instance.DefaultPrecisionModel;
            if (handleOrdinates == Ordinates.None)
                handleOrdinates = coordinateSequenceFactory.Ordinates;

            NetTopologySuiteBootstrapper.Bootstrap();

            var typeHandlerFactory = new NetTopologySuiteHandlerFactory(
                new PostGisReader(coordinateSequenceFactory, precisionModel, handleOrdinates),
                new NpgsqlPostGisWriter());  // NOTE: We used our own patched-up version of PostGisWriter for now

            return mapper
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "geometry",
                    NpgsqlDbType = NpgsqlDbType.Geometry,
                    ClrTypes = geographyAsDefault ? Type.EmptyTypes : ClrTypes,
                    InferredDbType = DbType.Object,
                    TypeHandlerFactory = typeHandlerFactory
                }.Build())
                .AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "geography",
                    NpgsqlDbType = NpgsqlDbType.Geography,
                    ClrTypes = geographyAsDefault ? ClrTypes : Type.EmptyTypes,
                    InferredDbType = DbType.Object,
                    TypeHandlerFactory = typeHandlerFactory
                }.Build());
        }
    }
}
