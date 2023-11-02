using System;
using System.Buffers.Text;
using Npgsql.Internal;

namespace Npgsql.BackendMessages;

sealed class CommandCompleteMessage : IBackendMessage
{
    uint _oid;
    ulong _rows;
    internal StatementType StatementType { get; private set; }

    internal uint OID => _oid;
    internal ulong Rows => _rows;

    internal CommandCompleteMessage Load(NpgsqlReadBuffer buf, int len)
    {
        var bytes = buf.Span.Slice(0, len);
        buf.Skip(len);

        // PostgreSQL always writes these strings as ASCII, see https://github.com/postgres/postgres/blob/c8e1ba736b2b9e8c98d37a5b77c4ed31baf94147/src/backend/tcop/cmdtag.c#L130-L133
        (StatementType, var argumentsStart) = Convert.ToChar(bytes[0]) switch
        {
            'S' when bytes.StartsWith("SELECT "u8) => (StatementType.Select, "SELECT ".Length),
            'I' when bytes.StartsWith("INSERT "u8) => (StatementType.Insert, "INSERT ".Length),
            'U' when bytes.StartsWith("UPDATE "u8) => (StatementType.Update, "UPDATE ".Length),
            'D' when bytes.StartsWith("DELETE "u8) => (StatementType.Delete, "DELETE ".Length),
            'M' when bytes.StartsWith("MERGE "u8) => (StatementType.Merge, "MERGE ".Length),
            'C' when bytes.StartsWith("COPY "u8) => (StatementType.Copy, "COPY ".Length),
            'C' when bytes.StartsWith("CALL"u8) => (StatementType.Call, "CALL".Length),
            'M' when bytes.StartsWith("MOVE "u8) => (StatementType.Move, "MOVE ".Length),
            'F' when bytes.StartsWith("FETCH "u8) => (StatementType.Fetch, "FETCH ".Length),
            'C' when bytes.StartsWith("CREATE TABLE AS "u8) => (StatementType.CreateTableAs, "CREATE TABLE AS ".Length),
            _ => (StatementType.Other, 0)
        };

        _oid = 0;
        _rows = 0;

        // Slice away the null terminator.
        var arguments = bytes.Slice(argumentsStart, bytes.Length - argumentsStart - 1);
        switch (StatementType)
        {
        case StatementType.Other:
        case StatementType.Call:
            break;
        case StatementType.Insert:
            if (!Utf8Parser.TryParse(arguments, out _oid, out var nextArgumentOffset))
                throw new InvalidOperationException("Invalid bytes in command complete message.");
            arguments = arguments.Slice(nextArgumentOffset + 1);
            goto default;
        default:
            if (!Utf8Parser.TryParse(arguments, out _rows, out _))
                throw new InvalidOperationException("Invalid bytes in command complete message.");
            break;
        }

        return this;
    }

    public BackendMessageCode Code => BackendMessageCode.CommandComplete;
}
