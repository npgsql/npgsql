using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers;

public class CubeHandler : NpgsqlSimpleTypeHandler<NpgsqlCube>
{
    const uint PointBit = 0x80000000;
    const int DimMask = 0x7fffffff;
    const int HeaderSize = sizeof(int);
    const int CoordSize = sizeof(double);

    public CubeHandler(PostgresType postgresType) : base(postgresType) { }

    public override NpgsqlCube Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
    {
        var header = buf.ReadInt32();
        var dim = header & DimMask;
        var point = (header & PointBit) != 0;

        var lowerLeft = new double[dim];
        for (var i = 0; i < dim; i++)
            lowerLeft[i] = buf.ReadDouble();

        if (point) 
            return new NpgsqlCube(lowerLeft);

        var upperRight = new double[dim];
        for (var i = 0; i < dim; i++)
            upperRight[i] = buf.ReadDouble();
        
        return new NpgsqlCube(lowerLeft, upperRight);
    }

    public override int ValidateAndGetLength(NpgsqlCube value, NpgsqlParameter? parameter) => 
        HeaderSize + CoordSize * (value.Point ? value.Dimensions : value.Dimensions * 2);

    public override void Write(NpgsqlCube value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
    {
        var header = value.Dimensions;
        if (value.Point) 
            header |= 1 << 31;
        
        buf.WriteInt32(header);
        
        for (var i = 0; i < value.Dimensions; i++)
            buf.WriteDouble(value.LowerLeft[i]);
        
        if (value.Point) 
            return;
        
        for (var i = 0; i < value.Dimensions; i++)
            buf.WriteDouble(value.UpperRight[i]);
    }

    public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
     => value switch
    {
        NpgsqlCube cube => ValidateAndGetLength(cube, parameter),
        null => 0,
        _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type CubeHandler")
    };

    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, 
        NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => value switch
        {
            NpgsqlCube cube => WriteWithLength(cube, buf, lengthCache, parameter, async, cancellationToken),
            null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
            _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type CubeHandler")
        };
}
