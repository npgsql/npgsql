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

using Npgsql.BackendMessages;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// Base class for all simple type handlers, which read and write short, non-arbitrary lengthed
    /// values to PostgreSQL. Provides a simpler API to implement when compared to <see cref="NpgsqlTypeHandler"/> -
    /// Npgsql takes care of all I/O before calling into this type, so no I/O needs to be performed by it.
    /// </summary>
    /// <typeparam name="TDefault">
    /// The default CLR type that this handler will read and write. For example, calling <see cref="DbDataReader.GetValue"/>
    /// on a column with this handler will return a value with type <typeparamref name="TDefault"/>.
    /// Type handlers can support additional types by implementing <see cref="INpgsqlTypeHandler{T}"/>.
    /// </typeparam>
    public abstract class NpgsqlSimpleTypeHandler<TDefault> : NpgsqlTypeHandler<TDefault>, INpgsqlSimpleTypeHandler<TDefault>
    {
        delegate int NonGenericValidateAndGetLength(NpgsqlTypeHandler handler, object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter);

        readonly NonGenericValidateAndGetLength _nonGenericValidateAndGetLength;
        readonly NonGenericWriteWithLength _nonGenericWriteWithLength;

        static readonly ConcurrentDictionary<Type, (NonGenericValidateAndGetLength, NonGenericWriteWithLength)>
            NonGenericDelegateCache = new ConcurrentDictionary<Type, (NonGenericValidateAndGetLength, NonGenericWriteWithLength)>();

        /// <summary>
        /// Constructs an <see cref="NpgsqlSimpleTypeHandler{TDefault}"/>.
        /// </summary>
        protected NpgsqlSimpleTypeHandler()
        {
            // Get code-generated delegates for non-generic ValidateAndGetLength/WriteWithLengthInternal
            (_nonGenericValidateAndGetLength, _nonGenericWriteWithLength) =
                NonGenericDelegateCache.GetOrAdd(GetType(), t => (
                    GenerateNonGenericValidationMethod(GetType()),
                    GenerateNonGenericWriteMethod(GetType(), typeof(INpgsqlSimpleTypeHandler<>)))
                );
        }

        #region Read

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public abstract TDefault Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// using either sync or async I/O. This method is sealed for <see cref="NpgsqlSimpleTypeHandler{T}"/>,
        /// override <see cref="Read(Npgsql.NpgsqlReadBuffer,int,Npgsql.BackendMessages.FieldDescription)"/>.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        public sealed override ValueTask<TDefault> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => Read<TDefault>(buf, len, async, fieldDescription);

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer,
        /// using either sync or async I/O. This method is sealed for <see cref="NpgsqlSimpleTypeHandler{T}"/>.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="async">If I/O is required to read the full length of the value, whether it should be performed synchronously or asynchronously.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        protected internal sealed override async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(len, async);
            return Read<TAny>(buf, len, fieldDescription);
        }

        /// <summary>
        /// Reads a value of type <typeparamref name="TDefault"/> with the given length from the provided buffer.
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required. Type handlers typically don't need to override this - override
        /// <see cref="Read(Npgsql.NpgsqlReadBuffer,int,Npgsql.BackendMessages.FieldDescription)"/> - but may do
        /// so in exceptional cases where reading of arbitrary types is required.
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        internal override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(len <= buf.ReadBytesLeft);

            var asTypedHandler = this as INpgsqlSimpleTypeHandler<TAny>;
            if (asTypedHandler == null)
            {
                buf.Skip(len);  // Perform this in sync for performance
                throw new NpgsqlSafeReadException(new InvalidCastException(fieldDescription == null
                    ? $"Can't cast database type to {typeof(TAny).Name}"
                    : $"Can't cast database type {fieldDescription.Handler.PgDisplayName} to {typeof(TAny).Name}"
                ));
            }

            return asTypedHandler.Read(buf, len, fieldDescription);
        }

        #endregion Read

        #region Write

        /// <summary>
        /// Responsible for validating that a value represents a value of the correct and which can be
        /// written for PostgreSQL - if the value cannot be written for any reason, an exception shold be thrown.
        /// Also returns the byte length needed to write the value.
        /// </summary>
        /// <param name="value">The value to be written to PostgreSQL</param>
        /// <param name="parameter">
        /// The <see cref="NpgsqlParameter"/> instance where this value resides. Can be used to access additional
        /// information relevant to the write process (e.g. <see cref="NpgsqlParameter.Size"/>).
        /// </param>
        /// <returns>The number of bytes required to write the value.</returns>
        public abstract int ValidateAndGetLength(TDefault value, NpgsqlParameter parameter);

        /// <summary>
        /// Writes a value to the provided buffer, with the assumption that there is enough space in the buffer
        /// (no I/O will occur). The Npgsql core will have taken care of that.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="buf">The buffer to which to write.</param>
        /// <param name="parameter">
        /// The <see cref="NpgsqlParameter"/> instance where this value resides. Can be used to access additional
        /// information relevant to the write process (e.g. <see cref="NpgsqlParameter.Size"/>).
        /// </param>
        public abstract void Write(TDefault value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter);

        /// <summary>
        /// This method is sealed, override <see cref="ValidateAndGetLength(TDefault,NpgsqlParameter)"/>.
        /// </summary>
        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => this is INpgsqlSimpleTypeHandler<TAny> typedHandler
                ? typedHandler.ValidateAndGetLength(value, parameter)
                : throw new InvalidCastException($"Can't write CLR type {typeof(TAny)} to database type {PgDisplayName}");

        /// <summary>
        /// In the vast majority of cases writing a parameter to the buffer won't need to perform I/O.
        /// </summary>
        internal sealed override Task WriteWithLengthInternal<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (value == null || typeof(TAny) == typeof(DBNull))
            {
                if (buf.WriteSpaceLeft < 4)
                    return WriteWithLengthLong();
                buf.WriteInt32(-1);
                return PGUtil.CompletedTask;
            }

            Debug.Assert(this is INpgsqlSimpleTypeHandler<TAny>);
            var typedHandler = (INpgsqlSimpleTypeHandler<TAny>)this;

            var elementLen = typedHandler.ValidateAndGetLength(value, parameter);
            if (buf.WriteSpaceLeft < 4 + elementLen)
                return WriteWithLengthLong();
            buf.WriteInt32(elementLen);
            typedHandler.Write(value, buf, parameter);
            return PGUtil.CompletedTask;

            async Task WriteWithLengthLong()
            {
                if (value == null || typeof(TAny) == typeof(DBNull))
                {
                    if (buf.WriteSpaceLeft < 4)
                        await buf.Flush(async);
                    buf.WriteInt32(-1);
                    return;
                }

                typedHandler = (INpgsqlSimpleTypeHandler<TAny>)this;
                elementLen = typedHandler.ValidateAndGetLength(value, parameter);
                if (buf.WriteSpaceLeft < 4 + elementLen)
                    await buf.Flush(async);
                buf.WriteInt32(elementLen);
                typedHandler.Write(value, buf, parameter);
            }
        }

        /// <summary>
        /// Simple type handlers override <see cref="Write(TDefault,NpgsqlWriteBuffer,NpgsqlParameter)"/> instead of this.
        /// </summary>
        public sealed override Task Write(TDefault value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => throw new NotSupportedException();

        /// <summary>
        /// Simple type handlers override <see cref="ValidateAndGetLength(TDefault,NpgsqlParameter)"/> instead of this.
        /// </summary>
        public sealed override int ValidateAndGetLength(TDefault value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => throw new NotSupportedException();

        // Object overloads for non-generic NpgsqlParameter

        /// <summary>
        /// Called to validate and get the length of a value of a non-generic <see cref="NpgsqlParameter"/>.
        /// Type handlers generally don't need to override this.
        /// </summary>
        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value == null || value is DBNull
                ? -1
                : _nonGenericValidateAndGetLength(this, value, ref lengthCache, parameter);

        /// <summary>
        /// Called to write the value of a non-generic <see cref="NpgsqlParameter"/>.
        /// Type handlers generally don't need to override this.
        /// </summary>
        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => value == null || value is DBNull  // For null just go through the default WriteWithLengthInternal
                ? WriteWithLengthInternal<DBNull>(null, buf, lengthCache, parameter, async)
                : _nonGenericWriteWithLength(this, value, buf, lengthCache, parameter, async);

        #endregion

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
                i.GetGenericTypeDefinition() == typeof(INpgsqlSimpleTypeHandler<>)
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
                            i.GetMethod(nameof(INpgsqlSimpleTypeHandler<TDefault>.ValidateAndGetLength)),
                            // Cast the value from object down to the interface's T
                            Expression.Convert(valueParam, handledType),
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
    }
}
