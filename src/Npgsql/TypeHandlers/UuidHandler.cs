using System;
using System.Data;
using System.Runtime.InteropServices;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL uuid data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-uuid.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("uuid", NpgsqlDbType.Uuid, DbType.Guid, typeof(Guid))]
    public class UuidHandler : NpgsqlSimpleTypeHandler<Guid>
    {
        // The following table shows .NET GUID vs Postgres UUID (RFC 4122) layouts.
        //
        // Note that the first fields are converted from/to native endianness (handled by the Read*
        // and Write* methods), while the last field is always read/written in big-endian format.
        //
        // We're passing BitConverter.IsLittleEndian to prevent reversing endianness on little-endian systems.
        //
        // | Bits | Bytes | Name  | Endianness (GUID) | Endianness (RFC 4122) |
        // | ---- | ----- | ----- | ----------------- | --------------------- |
        // | 32   | 4     | Data1 | Native            | Big                   |
        // | 16   | 2     | Data2 | Native            | Big                   |
        // | 16   | 2     | Data3 | Native            | Big                   |
        // | 64   | 8     | Data4 | Big               | Big                   |

        /// <inheritdoc />
        public UuidHandler(PostgresType postgresType) : base(postgresType) {}

        /// <inheritdoc />
        public override Guid Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var raw = new GuidRaw
            {
                Data1 = buf.ReadInt32(),
                Data2 = buf.ReadInt16(),
                Data3 = buf.ReadInt16(),
                Data4 = buf.ReadInt64(BitConverter.IsLittleEndian)
            };

            return raw.Value;
        }

        /// <inheritdoc />
        public override int ValidateAndGetLength(Guid value, NpgsqlParameter? parameter)
            => 16;

        /// <inheritdoc />
        public override void Write(Guid value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            var raw = new GuidRaw(value);

            buf.WriteInt32(raw.Data1);
            buf.WriteInt16(raw.Data2);
            buf.WriteInt16(raw.Data3);
            buf.WriteInt64(raw.Data4, BitConverter.IsLittleEndian);
        }

        [StructLayout(LayoutKind.Explicit)]
        struct GuidRaw
        {
            [FieldOffset(00)] public Guid Value;
            [FieldOffset(00)] public int Data1;
            [FieldOffset(04)] public short Data2;
            [FieldOffset(06)] public short Data3;
            [FieldOffset(08)] public long Data4;
            public GuidRaw(Guid value) : this() => Value = value;
        }
    }
}
