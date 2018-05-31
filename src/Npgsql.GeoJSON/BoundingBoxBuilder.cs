using GeoJSON.Net.Geometry;

namespace Npgsql.GeoJSON
{
    sealed class BoundingBoxBuilder
    {
        bool _hasAltitude;
        double _minLongitude, _maxLongitude;
        double _minLatitude, _maxLatitude;
        double _minAltitude, _maxAltitude;

        internal BoundingBoxBuilder()
        {
            _hasAltitude = false;

            _minLongitude = double.PositiveInfinity;
            _minLatitude = double.PositiveInfinity;
            _minAltitude = double.PositiveInfinity;

            _maxLongitude = double.NegativeInfinity;
            _maxLatitude = double.NegativeInfinity;
            _maxAltitude = double.NegativeInfinity;
        }

        internal void Accumulate(Position position)
        {
            if (_minLongitude > position.Longitude)
                _minLongitude = position.Longitude;
            if (_maxLongitude < position.Longitude)
                _maxLongitude = position.Longitude;

            if (_minLatitude > position.Latitude)
                _minLatitude = position.Latitude;
            if (_maxLatitude < position.Latitude)
                _maxLatitude = position.Latitude;

            if (position.Altitude.HasValue)
            {
                var altitude = position.Altitude.Value;
                if (_minAltitude > altitude)
                    _minAltitude = altitude;
                if (_maxAltitude < altitude)
                    _maxAltitude = altitude;

                _hasAltitude = true;
            }
        }

        internal double[] Build()
            => _hasAltitude
                ? new[] { _minLongitude, _minLatitude, _minAltitude, _maxLongitude, _maxLatitude, _maxAltitude }
                : new[] { _minLongitude, _minLatitude, _maxLongitude, _maxLatitude };
    }
}
