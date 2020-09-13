using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Represents the data transmitted for a tuple in a Logical Replication Protocol message
    /// </summary>
    [PublicAPI]
    public interface ITupleData
    {
        /// <summary>
        /// The kind of data in the tuple
        /// </summary>
        [PublicAPI]
        TupleDataKind Kind { get; }

        /// <summary>
        /// The <see cref="System.Type"/> of <see cref="Value"/>.
        /// </summary>
        [PublicAPI]
        Type Type { get; }

        /// <summary>
        /// The value of the tuple, in text format if <see cref="Kind" /> is <see cref="TupleDataKind.TextValue"/>
        /// or in binary format if <see cref="Kind" /> is <see cref="TupleDataKind.BinaryValue"/>.
        /// Otherwise <see langword="null" />.
        /// </summary>
        [PublicAPI]
        object? Value { get; }
    }

    /// <inheritdoc />
    [PublicAPI]
    public class TupleData<T> : ITupleData
    {
        internal TupleData(TupleDataKind kind)
        {
            Debug.Assert(kind == TupleDataKind.Null || kind == TupleDataKind.UnchangedToastedValue);
            Kind = kind;
            Value = default!;
        }

        internal TupleData(TupleDataKind kind, T value)
        {
            Debug.Assert(kind == TupleDataKind.TextValue || kind == TupleDataKind.BinaryValue);
            Kind = kind;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <inheritdoc />
        [PublicAPI]
        public TupleDataKind Kind { get; }

        Type ITupleData.Type => typeof(T);

        /// <summary>
        /// The value of the tuple, in text format if <see cref="Kind" /> is <see cref="TupleDataKind.TextValue"/>
        /// or in binary format if <see cref="Kind" /> is <see cref="TupleDataKind.BinaryValue"/>.
        /// Otherwise <see langword="null" />.
        /// </summary>
        [MaybeNull]
        [PublicAPI]
        public T Value { get; }

        object? ITupleData.Value => Value;
    }
}
