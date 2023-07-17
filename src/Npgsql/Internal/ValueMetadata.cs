namespace Npgsql.Internal;

public struct ValueMetadata
{
    public DataFormat Format { get; init; }
    // TODO writer still uses the mutability of this.
    public Size Size { get; set; }
    public object? WriteState { get; set; }
}
