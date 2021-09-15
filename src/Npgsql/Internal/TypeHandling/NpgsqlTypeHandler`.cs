using System;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandlers;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandling
{
    /// <summary>
    /// Base class for all type handlers, which read and write CLR types into their PostgreSQL
    /// binary representation. Unless your type is arbitrary-length, consider inheriting from
    /// <see cref="NpgsqlSimpleTypeHandler{T}"/> instead.
    /// </summary>
    /// <typeparam name="TDefault">
    /// The default CLR type that this handler will read and write. For example, calling <see cref="DbDataReader.GetValue"/>
    /// on a column with this handler will return a value with type <typeparamref name="TDefault"/>.
    /// Type handlers can support additional types by implementing <see cref="INpgsqlTypeHandler{T}"/>.
    /// </typeparam>
    public abstract class NpgsqlTypeHandler<TDefault> : NpgsqlTypeHandler, INpgsqlTypeHandler<TDefault>
    {
        protected NpgsqlTypeHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// using either sync or async I/O.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public abstract ValueTask<TDefault> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null);

        // Since TAny isn't constrained to class? or struct (C# doesn't have a non-nullable constraint that doesn't limit us to either struct or class),
        // we must use the bang operator here to tell the compiler that a null value will never returned.
        public override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => (await Read<TDefault>(buf, len, async, fieldDescription))!;

        #endregion Read

        #region Write

        /// <summary>
        /// Called to validate and get the length of a value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        public abstract int ValidateAndGetLength(TDefault value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter);

        /// <summary>
        /// Called to write the value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        public abstract Task Write(TDefault value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default);

        #endregion Write

        #region Misc

        public override Type GetFieldType(FieldDescription? fieldDescription = null) => typeof(TDefault);
        public override Type GetProviderSpecificFieldType(FieldDescription? fieldDescription = null) => typeof(TDefault);

        /// <inheritdoc />
        public override NpgsqlTypeHandler CreateArrayHandler(PostgresArrayType pgArrayType, ArrayNullabilityMode arrayNullabilityMode)
            => new ArrayHandler<TDefault>(pgArrayType, this, arrayNullabilityMode);

        /// <inheritdoc />
        public override NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType)
            => new RangeHandler<TDefault>(pgRangeType, this);

        /// <inheritdoc />
        public override NpgsqlTypeHandler CreateMultirangeHandler(PostgresMultirangeType pgMultirangeType)
            => new MultirangeHandler<TDefault>(pgMultirangeType, (RangeHandler<TDefault>)CreateRangeHandler(pgMultirangeType.Subrange));

        #endregion Misc
    }
}
