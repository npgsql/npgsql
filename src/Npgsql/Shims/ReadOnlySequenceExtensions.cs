namespace System.Buffers;

static class ReadOnlySequenceExtensions
{
    public static ReadOnlySpan<T> GetFirstSpan<T>(this ReadOnlySequence<T> sequence) => sequence.FirstSpan;
}
