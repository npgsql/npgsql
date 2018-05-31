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
using System.Data.Common;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// A simple type handler that supports a provider-specific value in addition to its default value.
    /// This is necessary mainly in cases where the CLR type cannot represent the full range of the
    /// PostgreSQL type, and a custom CLR type is needed (e.g. <see cref="DateTime"/> and
    /// <see cref="NpgsqlDateTime"/>). The provider-specific type <typeparamref name="TPsv"/> will be returned
    /// from calls to <see cref="DbDataReader.GetProviderSpecificValue"/>.
    /// </summary>
    /// <typeparam name="TDefault">
    /// The default CLR type that this handler will read and write. For example, calling <see cref="DbDataReader.GetValue"/>
    /// on a column with this handler will return a value with type <typeparamref name="TDefault"/>.
    /// Type handlers can support additional types by implementing <see cref="INpgsqlTypeHandler{T}"/>.
    /// </typeparam>
    /// <typeparam name="TPsv">The provider-specific CLR type that this handler will read and write.</typeparam>
    public abstract class NpgsqlSimpleTypeHandlerWithPsv<TDefault, TPsv> : NpgsqlSimpleTypeHandler<TDefault>, INpgsqlSimpleTypeHandler<TPsv>
    {
        #region Read

        /// <summary>
        /// Reads a value of type <typeparamref name="TPsv"/> with the given length from the provided buffer,
        /// with the assumption that it is entirely present in the provided memory buffer and no I/O will be
        /// required. 
        /// </summary>
        /// <param name="buf">The buffer from which to read.</param>
        /// <param name="len">The byte length of the value. The buffer might not contain the full length, requiring I/O to be performed.</param>
        /// <param name="fieldDescription">Additional PostgreSQL information about the type, such as the length in varchar(30).</param>
        /// <returns>The fully-read value.</returns>
        protected abstract TPsv ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null);

        TPsv INpgsqlSimpleTypeHandler<TPsv>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => ReadPsv(buf, len, fieldDescription);

        /// <summary>
        /// Reads a column as the type handler's provider-specific type, assuming that it is already entirely
        /// in memory (i.e. no I/O is necessary). Called by <see cref="NpgsqlDefaultDataReader"/>, which
        /// buffers entire rows in memory.
        /// </summary>
        internal override object ReadPsvAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read<TPsv>(buf, len, fieldDescription);

        /// <summary>
        /// Reads a column as the type handler's provider-specific type. If it is not already entirely in
        /// memory, sync or async I/O will be performed as specified by <paramref name="async"/>.
        /// </summary>
        internal override async ValueTask<object> ReadPsvAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => await Read<TPsv>(buf, len, async, fieldDescription);

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
        public abstract int ValidateAndGetLength(TPsv value, NpgsqlParameter parameter);

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
        public abstract void Write(TPsv value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter);

        #endregion Write

        #region Misc

        internal override Type GetProviderSpecificFieldType(FieldDescription fieldDescription = null)
            => typeof(TPsv);

        /// <summary>
        /// Creates a type handler for arrays of this handler's type.
        /// </summary>
        protected internal override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandlerWithPsv<TDefault, TPsv>(this) { PostgresType = arrayBackendType };

        #endregion Misc
    }
}
