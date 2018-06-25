#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers
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

        internal UnknownTypeHandler(NpgsqlConnection connection) : base(connection)
        {
            _connector = connection.Connector;
            PostgresType = UnknownBackendType.Instance;
        }

        #region Read

        public override ValueTask<string> Read(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription fieldDescription = null)
        {
            if (fieldDescription == null)
                throw new Exception($"Received an unknown field but {nameof(fieldDescription)} is null (i.e. COPY mode)");

            if (fieldDescription.IsBinaryFormat)
            {
                // We can't do anything with a binary representation of an unknown type - the user should have
                // requested text. Skip the data and throw.
                buf.Skip(byteLen);
                // At least get the name of the PostgreSQL type for the exception
                throw new NpgsqlSafeReadException(new NotSupportedException(
                    _connector.TypeMapper.DatabaseInfo.ByOID.TryGetValue(fieldDescription.TypeOID, out var pgType)
                        ? $"The field '{fieldDescription.Name}' has type '{pgType.DisplayName}', which is currently unknown to Npgsql. You can retrieve it as a string by marking it as unknown, please see the FAQ."
                        : $"The field '{fieldDescription.Name}' has a type currently unknown to Npgsql (OID {fieldDescription.TypeOID}). You can retrieve it as a string by marking it as unknown, please see the FAQ."
                ));
            }
            return base.Read(buf, byteLen, async, fieldDescription);
        }

        #endregion Read

        #region Write

        // Allow writing anything that is a string or can be converted to one via the unknown type handler

        protected internal override int ValidateAndGetLength<T2>(T2 value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateObjectAndGetLength(value, ref lengthCache, parameter);

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            if (value is string asString)
                return base.ValidateAndGetLength(asString, ref lengthCache, parameter);

            var converted = Convert.ToString(value);
            if (parameter == null)
                throw CreateConversionButNoParamException(value.GetType());
            parameter.ConvertedValue = converted;
            return base.ValidateAndGetLength(converted, ref lengthCache, parameter);
        }

        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (value == null || value is DBNull)
                return base.WriteObjectWithLength(value, buf, lengthCache, parameter, async);

            buf.WriteInt32(ValidateObjectAndGetLength(value, ref lengthCache, parameter));
            return base.Write(
                value is string asString
                    ? asString
                    : (string)parameter.ConvertedValue,
                buf, lengthCache, parameter, async);
        }

        #endregion Write
    }
}
