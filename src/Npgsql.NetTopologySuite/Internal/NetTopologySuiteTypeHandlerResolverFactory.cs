using System;
using System.Data;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.NetTopologySuite.Internal
{
    public class NetTopologySuiteTypeHandlerResolverFactory : ITypeHandlerResolverFactory
    {
        readonly CoordinateSequenceFactory _coordinateSequenceFactory;
        readonly PrecisionModel _precisionModel;
        readonly Ordinates _handleOrdinates;
        readonly bool _geographyAsDefault;

        public NetTopologySuiteTypeHandlerResolverFactory(
            CoordinateSequenceFactory? coordinateSequenceFactory,
            PrecisionModel? precisionModel,
            Ordinates handleOrdinates,
            bool geographyAsDefault)
        {
            _coordinateSequenceFactory = coordinateSequenceFactory ?? NtsGeometryServices.Instance.DefaultCoordinateSequenceFactory;;
            _precisionModel = precisionModel ?? NtsGeometryServices.Instance.DefaultPrecisionModel;
            _handleOrdinates = handleOrdinates == Ordinates.None ? _coordinateSequenceFactory.Ordinates : handleOrdinates;
            _geographyAsDefault = geographyAsDefault;
        }

        public ITypeHandlerResolver Create(NpgsqlConnector connector)
            => new NetTopologySuiteTypeHandlerResolver(connector, _coordinateSequenceFactory, _precisionModel, _handleOrdinates,
                _geographyAsDefault);

        public string? GetDataTypeNameByClrType(Type type)
            => NetTopologySuiteTypeHandlerResolver.ClrTypeToDataTypeName(type, _geographyAsDefault);

        public TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName)
            => NetTopologySuiteTypeHandlerResolver.DoGetMappingByDataTypeName(dataTypeName);
    }
}
