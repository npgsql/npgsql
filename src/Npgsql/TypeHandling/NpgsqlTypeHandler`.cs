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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;

namespace Npgsql.TypeHandling
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
        delegate int NonGenericValidateAndGetLength(NpgsqlTypeHandler handler, object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter);

        readonly NonGenericValidateAndGetLength _nonGenericValidateAndGetLength;
        readonly NonGenericWriteWithLength _nonGenericWriteWithLength;

#pragma warning disable CA1823
        static readonly ConcurrentDictionary<Type, (NonGenericValidateAndGetLength, NonGenericWriteWithLength)>
            NonGenericDelegateCache = new ConcurrentDictionary<Type, (NonGenericValidateAndGetLength, NonGenericWriteWithLength)>();
#pragma warning restore CA1823

        /// <summary>
        /// Constructs an <see cref="NpgsqlTypeHandler{TDefault}"/>.
        /// </summary>
        protected NpgsqlTypeHandler()
            // Get code-generated delegates for non-generic ValidateAndGetLength/WriteWithLengthInternal
            =>
            (_nonGenericValidateAndGetLength, _nonGenericWriteWithLength) =
                NonGenericDelegateCache.GetOrAdd(GetType(), t => (
                    GenerateNonGenericValidationMethod(GetType()),
                    GenerateNonGenericWriteMethod(GetType(), typeof(INpgsqlTypeHandler<>)))
                );

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
        public abstract ValueTask<TDefault> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null);

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// using either sync or async I/O. Type handlers typically don't need to override this -
        /// override <see cref="Read(NpgsqlReadBuffer, int, bool, FieldDescription)"/> - but may do
        /// so in exceptional cases where reading of arbitrary types is required.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        protected internal override ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            var asTypedHandler = this as INpgsqlTypeHandler<TAny>;
            if (asTypedHandler == null)
            {
                buf.Skip(len);  // Perform this in sync for performance
                throw new NpgsqlSafeReadException(new InvalidCastException(fieldDescription == null
                    ? $"Can't cast database type to {typeof(TAny).Name}"
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TAny).Name}"
                ));
            }

            return asTypedHandler.Read(buf, len, async, fieldDescription);
        }

        internal override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<TAny>(buf, len, false, fieldDescription).Result;

        internal override async ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => await Read<TDefault>(buf, len, async, fieldDescription);

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<TDefault>(buf, len, fieldDescription);

        #endregion Read

        #region Write

        /// <summary>
        /// Called to validate and get the length of a value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        public abstract int ValidateAndGetLength(TDefault value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter);

        /// <summary>
        /// Called to write the value of a generic <see cref="NpgsqlParameter{T}"/>.
        /// </summary>
        public abstract Task Write(TDefault value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async);

        /// <summary>
        /// Called to validate and get the length of a value of an arbitrary type.
        /// Checks that the current handler supports that type and throws an exception otherwise.
        /// </summary>
        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => this is INpgsqlTypeHandler<TAny> typedHandler
                ? typedHandler.ValidateAndGetLength(value, ref lengthCache, parameter)
                : throw new InvalidCastException($"Can't write CLR type {typeof(TAny)} to database type {PgDisplayName}");

        /// <summary>
        /// In the vast majority of cases writing a parameter to the buffer won't need to perform I/O.
        /// </summary>
        internal override Task WriteWithLengthInternal<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong();

            if (value == null || typeof(TAny) == typeof(DBNull))
            {
                buf.WriteInt32(-1);
                return PGUtil.CompletedTask;
            }

            return WriteWithLength(value, buf, lengthCache, parameter, async);

            async Task WriteWithLengthLong()
            {
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);

                if (value == null || typeof(TAny) == typeof(DBNull))
                {
                    buf.WriteInt32(-1);
                    return;
                }

                await WriteWithLength(value, buf, lengthCache, parameter, async);
            }
        }

        /// <summary>
        /// Typically does not need to be overridden by type handlers, but may be needed in some
        /// cases (e.g. <see cref="ArrayHandler"/>.
        /// Note that this method assumes it can write 4 bytes of length (already verified by
        /// <see cref="WriteWithLengthInternal{TAny}"/>).
        /// </summary>
        protected virtual Task WriteWithLength<TAny>([CanBeNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            Debug.Assert(this is INpgsqlTypeHandler<TAny>);

            var typedHandler = (INpgsqlTypeHandler<TAny>)this;
            buf.WriteInt32(typedHandler.ValidateAndGetLength(value, ref lengthCache, parameter));
            return typedHandler.Write(value, buf, lengthCache, parameter, async);
        }

        // Object overloads for non-generic NpgsqlParameter

        /// <summary>
        /// Called to validate and get the length of a value of a non-generic <see cref="NpgsqlParameter"/>.
        /// Type handlers generally don't need to override this.
        /// </summary>
        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache,NpgsqlParameter parameter)
            => value == null || value is DBNull
                ? -1
                : _nonGenericValidateAndGetLength(this, value, ref lengthCache, parameter);

        /// <summary>
        /// Called to write the value of a non-generic <see cref="NpgsqlParameter"/>.
        /// Type handlers generally don't need to override this.
        /// </summary>
        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => value == null || value is DBNull
                ? WriteWithLengthInternal<DBNull>(null, buf, lengthCache, parameter, async)
                : _nonGenericWriteWithLength(this, value, buf, lengthCache, parameter, async);

        #endregion Write

        #region Code generation for non-generic writing

        // We need to support writing via non-generic NpgsqlParameter, which means we get requests
        // to write some object with no generic typing information.
        // We need to find out which INpgsqlTypeHandler interfaces our handler implements, and call
        // the ValidateAndGetLength/WriteWithLengthInternal methods on the interface which corresponds to the
        // value type.
        // Since doing this with reflection every time is slow, we generate delegates to do this for us
        // for each type handler.

        static NonGenericValidateAndGetLength GenerateNonGenericValidationMethod(Type handlerType)
        {
            var interfaces = handlerType.GetInterfaces().Where(i =>
                i.GetTypeInfo().IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(INpgsqlTypeHandler<>)
            ).Reverse().ToList();

            Expression ifElseExpression = null;

            var handlerParam = Expression.Parameter(typeof(NpgsqlTypeHandler), "handler");
            var valueParam = Expression.Parameter(typeof(object), "value");
            var lengthCacheParam = Expression.Parameter(typeof(NpgsqlLengthCache).MakeByRefType(), "lengthCache");
            var parameterParam = Expression.Parameter(typeof(NpgsqlParameter), "parameter");

            var resultVariable = Expression.Variable(typeof(int), "result");

            foreach (var i in interfaces)
            {
                var handledType = i.GenericTypeArguments[0];

                ifElseExpression = Expression.IfThenElse(
                    // Test whether the type of the value given to the delegate corresponds
                    // to our current interface's handled type (i.e. the T in INpgsqlTypeHandler<T>)
                    Expression.TypeEqual(valueParam, handledType),
                    // If it corresponds, cast the handler type (this) to INpgsqlTypeHandler<T>
                    // and call its ValidateAndGetLength method
                    Expression.Assign(
                        resultVariable,
                        Expression.Call(
                            Expression.Convert(handlerParam, i),
                            i.GetMethod(nameof(INpgsqlTypeHandler<TDefault>.ValidateAndGetLength)),
                            // Cast the value from object down to the interface's T
                            Expression.Convert(valueParam, handledType),
                            lengthCacheParam,
                            parameterParam
                        )
                    ),
                    // If this is the first interface we're looking at, the else clause throws.
                    // Otherwise we stick the previous interface's IfThenElse in our else clause
                    ifElseExpression ?? Expression.Throw(
                        Expression.New(
                            typeof(InvalidCastException).GetConstructor(new[] { typeof(string) }),
                            Expression.Call(  // Call string.Format to generate a nice informative exception message
                                typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object) }),
                                new Expression[]
                                {
                                    Expression.Constant($"Can't write CLR type {{0}} with handler type {handlerType.Name}"),
                                    Expression.Call(  // GetType() on the value
                                        valueParam,
                                        typeof(object).GetMethod(nameof(string.GetType), new Type[0])
                                    )
                                }
                            )
                        )
                    )
                );
            }

            return Expression.Lambda<NonGenericValidateAndGetLength>(
                Expression.Block(
                    new[] { resultVariable },
                    ifElseExpression, resultVariable
                ),
                handlerParam, valueParam, lengthCacheParam, parameterParam
            ).Compile();
        }

        #endregion Code generation for non-generic writing

        #region Misc

        internal override Type GetFieldType(FieldDescription fieldDescription = null) => typeof(TDefault);
        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null) => typeof(TDefault);

        /// <inheritdoc />
        public override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandler<TDefault>(this) { PostgresType = arrayBackendType };

        /// <inheritdoc />
        public override RangeHandler CreateRangeHandler(PostgresType rangeBackendType)
            => new RangeHandler<TDefault>(this) { PostgresType = rangeBackendType };

        #endregion Misc
    }
}
