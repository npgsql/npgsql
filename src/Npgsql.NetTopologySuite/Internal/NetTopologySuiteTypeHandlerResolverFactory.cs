using NetTopologySuite;
using NetTopologySuite.Geometries;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;

namespace Npgsql.NetTopologySuite.Internal;

public class NetTopologySuiteTypeHandlerResolverFactory : TypeHandlerResolverFactory
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

    public override TypeHandlerResolver Create(TypeMapper typeMapper, NpgsqlConnector connector)
        => new NetTopologySuiteTypeHandlerResolver(connector, _coordinateSequenceFactory, _precisionModel, _handleOrdinates,
            _geographyAsDefault);

    public override TypeMappingResolver CreateMappingResolver() => new NetTopologySuiteTypeMappingResolver(_geographyAsDefault);
}
