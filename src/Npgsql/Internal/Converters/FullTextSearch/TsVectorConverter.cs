using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class TsVectorConverter(Encoding encoding) : PgStreamingConverter<NpgsqlTsVector>
{
    public override NpgsqlTsVector Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<NpgsqlTsVector> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<NpgsqlTsVector> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);

        var numLexemes = reader.ReadInt32();
        var lexemes = new List<NpgsqlTsVector.Lexeme>(numLexemes);

        for (var i = 0; i < numLexemes; i++)
        {
            var lexemeString = async
                ? await reader.ReadNullTerminatedStringAsync(encoding, cancellationToken).ConfigureAwait(false)
                : reader.ReadNullTerminatedString(encoding);

            if (reader.ShouldBuffer(sizeof(short)))
                await reader.Buffer(async, sizeof(short), cancellationToken).ConfigureAwait(false);
            var numPositions = reader.ReadInt16();

            if (numPositions == 0)
            {
                lexemes.Add(new NpgsqlTsVector.Lexeme(lexemeString, wordEntryPositions: null, noCopy: true));
                continue;
            }

            // There can only be a maximum of 256 positions, so we just before them all (256 * sizeof(short) = 512)
            if (numPositions > 256)
                throw new NpgsqlException($"Got {numPositions} lexeme positions when reading tsvector");

            if (reader.ShouldBuffer(numPositions * sizeof(short)))
                await reader.Buffer(async, numPositions * sizeof(short), cancellationToken).ConfigureAwait(false);

            var positions = new List<NpgsqlTsVector.Lexeme.WordEntryPos>(numPositions);

            for (var j = 0; j < numPositions; j++)
            {
                var wordEntryPos = reader.ReadInt16();
                positions.Add(new NpgsqlTsVector.Lexeme.WordEntryPos(wordEntryPos));
            }

            lexemes.Add(new NpgsqlTsVector.Lexeme(lexemeString, positions, noCopy: true));
        }

        return new NpgsqlTsVector(lexemes, noCheck: true);
    }

    public override Size GetSize(SizeContext context, NpgsqlTsVector value, ref object? writeState)
    {
        var size = 4;
        foreach (var l in value)
            size += encoding.GetByteCount(l.Text) + 1 + 2 + l.Count * 2;

        return size;
    }

    public override void Write(PgWriter writer, NpgsqlTsVector value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, NpgsqlTsVector value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, NpgsqlTsVector value, CancellationToken cancellationToken)
    {
        if (writer.ShouldFlush(sizeof(int)))
            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        writer.WriteInt32(value.Count);

        foreach (var lexeme in value)
        {
            if (async)
                await writer.WriteCharsAsync(lexeme.Text.AsMemory(), encoding, cancellationToken).ConfigureAwait(false);
            else
                writer.WriteChars(lexeme.Text.AsMemory().Span, encoding);

            if (writer.ShouldFlush(sizeof(byte) + sizeof(short)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);

            writer.WriteByte(0);
            writer.WriteInt16((short)lexeme.Count);

            for (var i = 0; i < lexeme.Count; i++)
            {
                if (writer.ShouldFlush(sizeof(short)))
                    await writer.Flush(async, cancellationToken).ConfigureAwait(false);

                writer.WriteInt16(lexeme[i].Value);
            }
        }
    }
}
