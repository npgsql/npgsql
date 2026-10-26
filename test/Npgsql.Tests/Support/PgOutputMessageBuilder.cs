using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using Npgsql.Replication;
using NpgsqlTypes;

namespace Npgsql.Tests.Support;

/// <summary>
/// Builds pgoutput messages (logical replication protocol), i.e. the payloads of the XLogData messages a walsender sends,
/// for tests against <see cref="PgServerMock"/>.
/// </summary>
sealed class PgOutputMessageBuilder
{
    readonly MemoryStream _stream = new();

    internal PgOutputMessageBuilder(char messageCode) => Byte((byte)messageCode);

    internal PgOutputMessageBuilder Byte(byte value)
    {
        _stream.WriteByte(value);
        return this;
    }

    internal PgOutputMessageBuilder Int16(short value)
    {
        Span<byte> bytes = stackalloc byte[2];
        BinaryPrimitives.WriteInt16BigEndian(bytes, value);
        _stream.Write(bytes);
        return this;
    }

    internal PgOutputMessageBuilder UInt32(uint value)
    {
        Span<byte> bytes = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(bytes, value);
        _stream.Write(bytes);
        return this;
    }

    internal PgOutputMessageBuilder Int32(int value)
    {
        Span<byte> bytes = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(bytes, value);
        _stream.Write(bytes);
        return this;
    }

    internal PgOutputMessageBuilder Int64(long value)
    {
        Span<byte> bytes = stackalloc byte[8];
        BinaryPrimitives.WriteInt64BigEndian(bytes, value);
        _stream.Write(bytes);
        return this;
    }

    internal PgOutputMessageBuilder Lsn(NpgsqlLogSequenceNumber value) => Int64(unchecked((long)(ulong)value));

    internal PgOutputMessageBuilder Timestamp(DateTime value) => Int64(PgDateTime.EncodeTimestamp(value));

    internal PgOutputMessageBuilder String(string value)
    {
        _stream.Write(Encoding.UTF8.GetBytes(value));
        _stream.WriteByte(0);
        return this;
    }

    internal PgOutputMessageBuilder Bytes(ReadOnlySpan<byte> value)
    {
        _stream.Write(value);
        return this;
    }

    internal byte[] ToArray() => _stream.ToArray();

    internal static byte[] Begin(NpgsqlLogSequenceNumber finalLsn, DateTime commitTimestamp, uint xid)
        => new PgOutputMessageBuilder('B').Lsn(finalLsn).Timestamp(commitTimestamp).UInt32(xid).ToArray();

    internal static byte[] Commit(NpgsqlLogSequenceNumber commitLsn, NpgsqlLogSequenceNumber endLsn, DateTime commitTimestamp)
        => new PgOutputMessageBuilder('C').Byte(0).Lsn(commitLsn).Lsn(endLsn).Timestamp(commitTimestamp).ToArray();

    /// <summary>
    /// A Relation message for a table whose columns are all part of the replica identity key.
    /// </summary>
    internal static byte[] Relation(uint relationId, string ns, string name, params (string Name, uint TypeOid)[] columns)
    {
        var builder = new PgOutputMessageBuilder('R').UInt32(relationId).String(ns).String(name).Byte((byte)'d')
            .Int16((short)columns.Length);
        foreach (var (columnName, typeOid) in columns)
            builder.Byte(1).String(columnName).UInt32(typeOid).Int32(-1);
        return builder.ToArray();
    }

    /// <summary>
    /// An Insert message with text values; <see langword="null"/> values are sent as SQL NULL.
    /// </summary>
    internal static byte[] Insert(uint relationId, params string?[] values)
    {
        var builder = new PgOutputMessageBuilder('I').UInt32(relationId).Byte((byte)'N').Int16((short)values.Length);
        foreach (var value in values)
        {
            if (value is null)
            {
                builder.Byte((byte)'n');
                continue;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            builder.Byte((byte)'t').Int32(bytes.Length).Bytes(bytes);
        }

        return builder.ToArray();
    }
}
