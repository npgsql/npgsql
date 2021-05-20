namespace Npgsql.Replication.PgOutput
{
    enum TupleType : byte
    {
        Key = (byte)'K',
        NewTuple = (byte)'N',
        OldTuple = (byte)'O',
    }
}
