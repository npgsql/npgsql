using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    [TypeMapping("ltxtquery", NpgsqlDbType.Ltxtquery)]
    class LtxtqueryHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        public override NpgsqlTypeHandler<string> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new LtxtqueryHandler(postgresType, conn);
    }

    /// <summary>
    /// Ltxtquery binary encoding is a simple UTF8 string, but prepended with a version number.
    /// </summary>
    public class LtxtqueryHandler : TextHandler
    {
        /// <summary>
        /// Prepended to the string in the wire encoding
        /// </summary>
        const byte LtxtqueryProtocolVersion = 1;

        internal override bool PreferTextWrite => false;

        protected internal LtxtqueryHandler(PostgresType postgresType, NpgsqlConnection connection)
            : base(postgresType, connection) {}

        #region Write

        public override int ValidateAndGetLength(string value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get() + 1;

            // Add one byte for the prepended version number
            return base.ValidateAndGetLength(value, ref lengthCache, parameter) + 1;
        }

        public override int ValidateAndGetLength(char[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get() + 1;

            // Add one byte for the prepended version number
            return base.ValidateAndGetLength(value, ref lengthCache, parameter) + 1;
        }

        public override int ValidateAndGetLength(ArraySegment<char> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get() + 1;

            // Add one byte for the prepended version number
            return base.ValidateAndGetLength(value, ref lengthCache, parameter) + 1;
        }

        public override async Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte(LtxtqueryProtocolVersion);
            await base.Write(value, buf, lengthCache, parameter, async);
        }

        public override async Task Write(char[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte(LtxtqueryProtocolVersion);
            await base.Write(value, buf, lengthCache, parameter, async);
        }

        public override async Task Write(ArraySegment<char> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte(LtxtqueryProtocolVersion);
            await base.Write(value, buf, lengthCache, parameter, async);
        }

        #endregion

        #region Read

        public override async ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(1, async);
            var version = buf.ReadByte();
            if (version != LtxtqueryProtocolVersion)
                throw new NotSupportedException($"Don't know how to decode ltxtquery with wire format {version}, your connection is now broken");

            return await base.Read(buf, len - 1, async, fieldDescription);
        }

        #endregion

        public override TextReader GetTextReader(Stream stream)
        {
            var version = stream.ReadByte();
            if (version != LtxtqueryProtocolVersion)
                throw new NpgsqlException($"Don't know how to decode ltxtquery with wire format {version}, your connection is now broken");

            return base.GetTextReader(stream);
        }
    }
}
