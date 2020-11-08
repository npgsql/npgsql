using System;
using System.Runtime.InteropServices;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Represents the data transmitted for a tuple in a Logical Replication Protocol message
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct TupleData
    {
        internal TupleData(TupleDataKind kind)
        {
            Kind = kind;
            _textValue = null;
            // _binaryValue = null;
        }

        internal TupleData(string textValue)
        {
            Kind = TupleDataKind.TextValue;
            _textValue = textValue;
            // _binaryValue = null;
        }

        internal TupleData(byte[] binaryValue)
        {
            Kind = TupleDataKind.TextValue;
            _textValue = null;
            // _binaryValue = binaryValue;
        }

        /// <summary>
        /// The kind of data in the tuple
        /// </summary>
        [field: FieldOffset(0)]
        public TupleDataKind Kind { get; }

        [FieldOffset(8)] readonly string? _textValue;

        /// <summary>
        /// The value of the tuple, if <see cref="Kind" /> is <see cref="TupleDataKind.TextValue"/>. Otherwise throws.
        /// </summary>
        public string TextValue => Kind == TupleDataKind.TextValue
            ? _textValue!
            : throw new InvalidOperationException("Tuple kind is " + Kind);

#if PG14
        [FieldOffset(8)] readonly byte[]? _binaryValue;

        /// <summary>
        /// The value of the tuple, if <see cref="Kind" /> is <see cref="TupleDataKind.BinaryValue"/>. Otherwise throws.
        /// </summary>
        public byte[] BinaryValue => Kind == TupleDataKind.BinaryValue
            ? _binaryValue!
            : throw new InvalidOperationException("Tuple kind is " + Kind);
#endif

        /// <summary>
        /// The value of the tuple, in text format if <see cref="Kind" /> is <see cref="TupleDataKind.TextValue"/>, or other formats
        /// as may be added. Otherwise <see langword="null" />.
        /// </summary>
        public object? Value => Kind switch
        {
            TupleDataKind.Null => null,
            TupleDataKind.UnchangedToastedValue => null,
            TupleDataKind.TextValue => _textValue,
            // TupleDataKind.BinaryValue => _binaryValue,
            _ => throw new ArgumentOutOfRangeException($"Unhandled {nameof(TupleDataKind)}: {Kind}")
        };

        /// <inheritdoc />
        public override string ToString() => Kind switch
        {
            TupleDataKind.Null => "<NULL>",
            TupleDataKind.UnchangedToastedValue => "<UnchangedToastedValue>",
            TupleDataKind.TextValue => TextValue,
            // TupleDataKind.BinaryValue => ,
            _ => throw new ArgumentOutOfRangeException($"Unhandled {nameof(TupleDataKind)}: {Kind}")
        };
    }

    /// <summary>
    /// The kind of data transmitted for a tuple in a Logical Replication Protocol message
    /// </summary>
    public enum TupleDataKind : byte
    {
        /// <summary>
        /// Identifies the data as NULL value.
        /// </summary>
        Null = (byte)'n',

        /// <summary>
        /// Identifies unchanged TOASTed value (the actual value is not sent).
        /// </summary>
        UnchangedToastedValue = (byte)'u',

        /// <summary>
        /// Identifies the data as text formatted value.
        /// </summary>
        TextValue = (byte)'t',

#if PG14
        /// <summary>
        /// Identifies the data as binary value.
        /// </summary>
        /// <remarks>Added in PG14</remarks>
        BinaryValue = (byte)'b'
#endif
    }
}
