using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Npgsql.Util;

// For logging batches we have to use a wrapper for parameters, otherwise they're logged as object[]. See https://github.com/npgsql/npgsql/issues/6078.
sealed class LoggingEnumerable<T>(IEnumerable<T> wrappedEnumerable) : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator() => wrappedEnumerable.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)wrappedEnumerable).GetEnumerator();

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append('[');

        var appended = false;

        foreach (var o in wrappedEnumerable)
        {
            if (appended)
                sb.Append(", ");
            else
                appended = true;

            sb.Append(o);
        }

        sb.Append(']');

        return sb.ToString();
    }
}
