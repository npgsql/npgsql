namespace Npgsql.Internal;

public struct ValueMetadata
{
    public DataFormat Format { get; init; }
    public Size Size { get; set; }
    public object? WriteState { get; set; }
}
