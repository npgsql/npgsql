using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers
{
    /// <summary>
    /// Handles "conversions" for columns sent by the database with unknown OIDs.
    /// This differs from TextHandler in that its a text-only handler (we don't want to receive binary
    /// representations of the types registered here).
    /// Note that this handler is also used in the very initial query that loads the OID mappings
    /// (chicken and egg problem).
    /// Also used for sending parameters with unknown types (OID=0)
    /// </summary>
    class UnknownTypeHandler : TextHandler
    {
        readonly NpgsqlConnector _connector;

        internal UnknownTypeHandler(NpgsqlConnector connector)
            : base(UnknownBackendType.Instance, connector.TextEncoding)
            => _connector = connector;

        #region Read

        public override ValueTask<string> Read(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription = null)
        {
            if (fieldDescription == null)
                throw new Exception($"Received an unknown field but {nameof(fieldDescription)} is null (i.e. COPY mode)");

            if (fieldDescription.IsBinaryFormat)
            {
                // At least get the name of the PostgreSQL type for the exception
                throw new NotSupportedException(
                    _connector.TypeMapper.DatabaseInfo.ByOID.TryGetValue(fieldDescription.TypeOID, out var pgType)
                        ? $"The field '{fieldDescription.Name}' has type '{pgType.DisplayName}', which is currently unknown to Npgsql. You can retrieve it as a string by marking it as unknown, please see the FAQ."
                        : $"The field '{fieldDescription.Name}' has a type currently unknown to Npgsql (OID {fieldDescription.TypeOID}). You can retrieve it as a string by marking it as unknown, please see the FAQ."
                );
            }

            return base.Read(buf, byteLen, async, fieldDescription);
        }

        #endregion Read

        #region Write

        // Allow writing anything that is a string or can be converted to one via the unknown type handler

        protected internal override int ValidateAndGetLengthCustom<TAny>(
            [DisallowNull] TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateObjectAndGetLength(value, ref lengthCache, parameter);

        public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (value is string asString)
                return base.ValidateAndGetLength(asString, ref lengthCache, parameter);

            if (parameter == null)
                throw CreateConversionButNoParamException(value.GetType());

            var converted = Convert.ToString(value)!;
            parameter.ConvertedValue = converted;

            return base.ValidateAndGetLength(converted, ref lengthCache, parameter);
        }

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (value is null or DBNull)
                return base.WriteObjectWithLength(value, buf, lengthCache, parameter, async, cancellationToken);

            var convertedValue = value is string asString
                ? asString
                : (string)parameter!.ConvertedValue!;

            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong(value, convertedValue, buf, lengthCache, parameter, async, cancellationToken);

            buf.WriteInt32(ValidateObjectAndGetLength(value, ref lengthCache, parameter));
            return base.Write(convertedValue, buf, lengthCache, parameter, async, cancellationToken);

            async Task WriteWithLengthLong(object value, string convertedValue, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            {
                await buf.Flush(async, cancellationToken);
                buf.WriteInt32(ValidateObjectAndGetLength(value!, ref lengthCache, parameter));
                await base.Write(convertedValue, buf, lengthCache, parameter, async, cancellationToken);
            }
        }

        #endregion Write
    }
}
