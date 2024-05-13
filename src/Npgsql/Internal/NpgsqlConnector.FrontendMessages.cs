using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

partial class NpgsqlConnector
{
    internal Task WriteDescribe(StatementOrPortal statementOrPortal, byte[] asciiName, bool async, CancellationToken cancellationToken = default)
    {
        NpgsqlWriteBuffer.AssertASCIIOnly(asciiName);

        var len = sizeof(byte) +       // Message code
                  sizeof(int)  +       // Length
                  sizeof(byte) +       // Statement or portal
                  (asciiName.Length + 1);   // Statement/portal name

        var writeBuffer = WriteBuffer;
        writeBuffer.StartMessage(len);
        if (writeBuffer.WriteSpaceLeft < len)
            return FlushAndWrite(len, statementOrPortal, asciiName, async, cancellationToken);

        Write(writeBuffer, len, statementOrPortal, asciiName);
        return Task.CompletedTask;

        async Task FlushAndWrite(int len, StatementOrPortal statementOrPortal, byte[] name, bool async, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            Debug.Assert(len <= WriteBuffer.WriteSpaceLeft, $"Message of type {GetType().Name} has length {len} which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
            Write(WriteBuffer, len, statementOrPortal, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Write(NpgsqlWriteBuffer writeBuffer, int len, StatementOrPortal statementOrPortal, byte[] name)
        {
            writeBuffer.WriteByte(FrontendMessageCode.Describe);
            writeBuffer.WriteInt32(len - 1);
            writeBuffer.WriteByte((byte)statementOrPortal);
            writeBuffer.WriteNullTerminatedString(name);
        }
    }

    internal Task WriteSync(bool async, CancellationToken cancellationToken = default)
    {
        const int len = sizeof(byte) +  // Message code
                        sizeof(int);    // Length

        var writeBuffer = WriteBuffer;
        writeBuffer.StartMessage(len);
        if (writeBuffer.WriteSpaceLeft < len)
            return FlushAndWrite(async, cancellationToken);

        Write(writeBuffer);
        return Task.CompletedTask;

        async Task FlushAndWrite(bool async, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            Debug.Assert(len <= WriteBuffer.WriteSpaceLeft, $"Message of type {GetType().Name} has length {len} which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
            Write(WriteBuffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Write(NpgsqlWriteBuffer writeBuffer)
        {
            writeBuffer.WriteByte(FrontendMessageCode.Sync);
            writeBuffer.WriteInt32(len - 1);
        }
    }

    internal Task WriteExecute(int maxRows, bool async, CancellationToken cancellationToken = default)
    {
        // Note: non-empty portal currently not supported

        const int len = sizeof(byte) +       // Message code
                        sizeof(int)  +       // Length
                        sizeof(byte) +       // Null-terminated portal name (always empty for now)
                        sizeof(int);         // Max number of rows

        var writeBuffer = WriteBuffer;
        writeBuffer.StartMessage(len);
        if (writeBuffer.WriteSpaceLeft < len)
            return FlushAndWrite(maxRows, async, cancellationToken);

        Write(writeBuffer, maxRows);
        return Task.CompletedTask;

        async Task FlushAndWrite(int maxRows, bool async, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            Debug.Assert(10 <= WriteBuffer.WriteSpaceLeft, $"Message of type {GetType().Name} has length 10 which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
            Write(WriteBuffer, maxRows);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Write(NpgsqlWriteBuffer writeBuffer, int maxRows)
        {
            writeBuffer.WriteByte(FrontendMessageCode.Execute);
            writeBuffer.WriteInt32(len - 1);
            writeBuffer.WriteByte(0);   // Portal is always empty for now
            writeBuffer.WriteInt32(maxRows);
        }
    }

    internal async Task WriteParse(string sql, byte[] asciiName, List<NpgsqlParameter> inputParameters, bool async, CancellationToken cancellationToken = default)
    {
        NpgsqlWriteBuffer.AssertASCIIOnly(asciiName);

        int queryByteLen;
        try
        {
            queryByteLen = TextEncoding.GetByteCount(sql);
        }
        catch (Exception e)
        {
            Break(e);
            throw;
        }

        var writeBuffer = WriteBuffer;
        var messageLength =
            sizeof(byte)                +         // Message code
            sizeof(int)                 +         // Length
            asciiName.Length        +         // Statement name
            sizeof(byte)                +         // Null terminator for the statement name
            queryByteLen + sizeof(byte) +         // SQL query length plus null terminator
            sizeof(ushort)              +         // Number of parameters
            inputParameters.Count * sizeof(int);  // Parameter OIDs


        WriteBuffer.StartMessage(messageLength);
        if (WriteBuffer.WriteSpaceLeft < 1 + 4 + asciiName.Length + 1)
            await Flush(async, cancellationToken).ConfigureAwait(false);

        WriteBuffer.WriteByte(FrontendMessageCode.Parse);
        WriteBuffer.WriteInt32(messageLength - 1);
        WriteBuffer.WriteNullTerminatedString(asciiName);

        await writeBuffer.WriteString(sql, queryByteLen, async, cancellationToken).ConfigureAwait(false);

        if (writeBuffer.WriteSpaceLeft < 1 + 2)
            await Flush(async, cancellationToken).ConfigureAwait(false);
        writeBuffer.WriteByte(0); // Null terminator for the query
        writeBuffer.WriteUInt16((ushort)inputParameters.Count);

        var databaseInfo = DatabaseInfo;
        foreach (var p in inputParameters)
        {
            if (writeBuffer.WriteSpaceLeft < 4)
                await Flush(async, cancellationToken).ConfigureAwait(false);

            writeBuffer.WriteUInt32(databaseInfo.GetOid(p.PgTypeId).Value);
        }
    }

    internal async Task WriteBind(
        List<NpgsqlParameter> parameters,
        string portal,
        byte[] asciiName,
        bool allResultTypesAreUnknown,
        bool[]? unknownResultTypeList,
        bool async,
        CancellationToken cancellationToken = default)
    {
        NpgsqlWriteBuffer.AssertASCIIOnly(asciiName);
        NpgsqlWriteBuffer.AssertASCIIOnly(portal);

        var headerLength =
            sizeof(byte)                    +     // Message code
            sizeof(int)                     +     // Message length
            sizeof(byte)                    +     // Portal is always empty (only a null terminator)
            asciiName.Length + sizeof(byte) +     // Statement name plus null terminator
            sizeof(ushort);                       // Number of parameter format codes that follow

        var writeBuffer = WriteBuffer;
        var formatCodesSum = 0;
        var paramsLength = 0;
        for (var paramIndex = 0; paramIndex < parameters.Count; paramIndex++)
        {
            var param = parameters[paramIndex];
            param.Bind(out var format, out var size);
            paramsLength += size.Value > 0 ? size.Value : 0;
            formatCodesSum += format.ToFormatCode();
        }

        var formatCodeListLength = formatCodesSum == 0 ? 0 : formatCodesSum == parameters.Count ? 1 : parameters.Count;

        var messageLength = headerLength         +
                            sizeof(short) * formatCodeListLength +                  // List of format codes
                            sizeof(short)                        +                  // Number of parameters
                            sizeof(int) * parameters.Count  +                       // Parameter lengths
                            paramsLength                         +                  // Parameter values
                            sizeof(short)                        +                  // Number of result format codes
                            sizeof(short) * (unknownResultTypeList?.Length ?? 1);   // Result format codes

        WriteBuffer.StartMessage(messageLength);
        if (WriteBuffer.WriteSpaceLeft < headerLength)
        {
            Debug.Assert(WriteBuffer.Size >= headerLength, "Write buffer too small for Bind header");
            await Flush(async, cancellationToken).ConfigureAwait(false);
        }

        WriteBuffer.WriteByte(FrontendMessageCode.Bind);
        WriteBuffer.WriteInt32(messageLength - 1);
        Debug.Assert(portal == string.Empty);
        writeBuffer.WriteByte(0);  // Portal is always empty

        writeBuffer.WriteNullTerminatedString(asciiName);
        writeBuffer.WriteInt16((short)formatCodeListLength);

        // 0 length implicitly means all-text, 1 means all-binary, >1 means mix-and-match
        if (formatCodeListLength == 1)
        {
            if (writeBuffer.WriteSpaceLeft < sizeof(short))
                await Flush(async, cancellationToken).ConfigureAwait(false);
            writeBuffer.WriteInt16(DataFormat.Binary.ToFormatCode());
        }
        else if (formatCodeListLength > 1)
        {
            for (var paramIndex = 0; paramIndex < parameters.Count; paramIndex++)
            {
                if (writeBuffer.WriteSpaceLeft < sizeof(short))
                    await Flush(async, cancellationToken).ConfigureAwait(false);
                writeBuffer.WriteInt16(parameters[paramIndex].Format.ToFormatCode());
            }
        }

        if (writeBuffer.WriteSpaceLeft < sizeof(ushort))
            await Flush(async, cancellationToken).ConfigureAwait(false);

        writeBuffer.WriteUInt16((ushort)parameters.Count);
        if (parameters.Count > 0)
        {
            var writer = writeBuffer.GetWriter(DatabaseInfo, async ? FlushMode.NonBlocking : FlushMode.Blocking);
            try
            {
                for (var paramIndex = 0; paramIndex < parameters.Count; paramIndex++)
                {
                    var param = parameters[paramIndex];
                    await param.Write(async, writer, cancellationToken).ConfigureAwait(false);
                }
            }
            catch(Exception ex)
            {
                Break(ex);
                throw;
            }
        }

        if (unknownResultTypeList != null)
        {
            if (writeBuffer.WriteSpaceLeft < 2 + unknownResultTypeList.Length * 2)
                await Flush(async, cancellationToken).ConfigureAwait(false);
            writeBuffer.WriteInt16((short)unknownResultTypeList.Length);
            foreach (var t in unknownResultTypeList)
                writeBuffer.WriteInt16((short)(t ? 0 : 1));
        }
        else
        {
            if (writeBuffer.WriteSpaceLeft < 4)
                await Flush(async, cancellationToken).ConfigureAwait(false);
            writeBuffer.WriteInt16(1);
            writeBuffer.WriteInt16((short)(allResultTypesAreUnknown ? 0 : 1));
        }
    }

    internal Task WriteClose(StatementOrPortal type, byte[] asciiName, bool async, CancellationToken cancellationToken = default)
    {
        var len = sizeof(byte) +               // Message code
                  sizeof(int)  +               // Length
                  sizeof(byte) +               // Statement or portal
                  asciiName.Length + sizeof(byte);  // Statement or portal name plus null terminator

        var writeBuffer = WriteBuffer;
        writeBuffer.StartMessage(len);
        if (writeBuffer.WriteSpaceLeft < len)
            return FlushAndWrite(len, type, asciiName, async, cancellationToken);

        Write(writeBuffer, len, type, asciiName);
        return Task.CompletedTask;

        async Task FlushAndWrite(int len, StatementOrPortal type, byte[] name, bool async, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            Debug.Assert(len <= WriteBuffer.WriteSpaceLeft, $"Message of type {GetType().Name} has length {len} which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
            Write(WriteBuffer, len, type, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Write(NpgsqlWriteBuffer writeBuffer, int len, StatementOrPortal type, byte[] name)
        {
            writeBuffer.WriteByte(FrontendMessageCode.Close);
            writeBuffer.WriteInt32(len - 1);
            writeBuffer.WriteByte((byte)type);
            writeBuffer.WriteNullTerminatedString(name);
        }
    }

    internal async Task WriteQuery(string sql, bool async, CancellationToken cancellationToken = default)
    {
        var queryByteLen = TextEncoding.GetByteCount(sql);

        var len = sizeof(byte) +
                  sizeof(int) + // Message length (including self excluding code)
                  queryByteLen + // Query byte length
                  sizeof(byte);

        WriteBuffer.StartMessage(len);
        if (WriteBuffer.WriteSpaceLeft < 1 + 4)
            await Flush(async, cancellationToken).ConfigureAwait(false);

        WriteBuffer.WriteByte(FrontendMessageCode.Query);
        WriteBuffer.WriteInt32(len - 1);

        await WriteBuffer.WriteString(sql, queryByteLen, async, cancellationToken).ConfigureAwait(false);
        if (WriteBuffer.WriteSpaceLeft < 1)
            await Flush(async, cancellationToken).ConfigureAwait(false);
        WriteBuffer.WriteByte(0);  // Null terminator
    }

    internal async Task WriteCopyDone(bool async, CancellationToken cancellationToken = default)
    {
        const int len = sizeof(byte) +   // Message code
                        sizeof(int);     // Length

        WriteBuffer.StartMessage(len);
        if (WriteBuffer.WriteSpaceLeft < len)
            await Flush(async, cancellationToken).ConfigureAwait(false);

        WriteBuffer.WriteByte(FrontendMessageCode.CopyDone);
        WriteBuffer.WriteInt32(len - 1);
    }

    internal async Task WriteCopyFail(bool async, CancellationToken cancellationToken = default)
    {
        // Note: error message not supported for now

        const int len = sizeof(byte) +  // Message code
                        sizeof(int) +   // Length
                        sizeof(byte);   // Error message is always empty (only a null terminator)

        WriteBuffer.StartMessage(len);
        if (WriteBuffer.WriteSpaceLeft < len)
            await Flush(async, cancellationToken).ConfigureAwait(false);

        WriteBuffer.WriteByte(FrontendMessageCode.CopyFail);
        WriteBuffer.WriteInt32(len - 1);
        WriteBuffer.WriteByte(0);   // Error message is always empty (only a null terminator)
    }

    internal void WriteCancelRequest(int backendProcessId, int backendSecretKey)
    {
        const int len = sizeof(int) +  // Length
                        sizeof(int) +  // Cancel request code
                        sizeof(int) +  // Backend process id
                        sizeof(int);   // Backend secret key

        Debug.Assert(backendProcessId != 0);

        WriteBuffer.StartMessage(len);
        if (WriteBuffer.WriteSpaceLeft < len)
            Flush(false).GetAwaiter().GetResult();

        WriteBuffer.WriteInt32(len);
        WriteBuffer.WriteInt32(1234 << 16 | 5678);
        WriteBuffer.WriteInt32(backendProcessId);
        WriteBuffer.WriteInt32(backendSecretKey);
    }

    internal void WriteTerminate()
    {
        const int len = sizeof(byte) +  // Message code
                        sizeof(int);    // Length

        WriteBuffer.StartMessage(len);
        if (WriteBuffer.WriteSpaceLeft < len)
            Flush(false).GetAwaiter().GetResult();

        WriteBuffer.WriteByte(FrontendMessageCode.Terminate);
        WriteBuffer.WriteInt32(len - 1);
    }

    internal void WriteSslRequest()
    {
        const int len = sizeof(int) +  // Length
                        sizeof(int);   // SSL request code

        WriteBuffer.StartMessage(len);
        if (WriteBuffer.WriteSpaceLeft < len)
            Flush(false).GetAwaiter().GetResult();

        WriteBuffer.WriteInt32(len);
        WriteBuffer.WriteInt32(80877103);
    }

    internal void WriteStartup(Dictionary<string, string> parameters)
    {
        const int protocolVersion3 = 3 << 16; // 196608

        var len = sizeof(int) +  // Length
                  sizeof(int) +  // Protocol version
                  sizeof(byte);  // Trailing zero byte

        foreach (var kvp in parameters)
            len += NpgsqlWriteBuffer.UTF8Encoding.GetByteCount(kvp.Key) + 1 +
                   NpgsqlWriteBuffer.UTF8Encoding.GetByteCount(kvp.Value) + 1;

        // Should really never happen, just in case
        WriteBuffer.StartMessage(len);
        if (len > WriteBuffer.Size)
            throw new Exception("Startup message bigger than buffer");

        WriteBuffer.WriteInt32(len);
        WriteBuffer.WriteInt32(protocolVersion3);

        foreach (var kv in parameters)
        {
            WriteBuffer.WriteString(kv.Key);
            WriteBuffer.WriteByte(0);
            WriteBuffer.WriteString(kv.Value);
            WriteBuffer.WriteByte(0);
        }

        WriteBuffer.WriteByte(0);
    }

    #region Authentication

    internal Task WritePassword(byte[] payload, bool async, CancellationToken cancellationToken = default) => WritePassword(payload, 0, payload.Length, async, cancellationToken);

    internal async Task WritePassword(byte[] payload, int offset, int count, bool async, CancellationToken cancellationToken = default)
    {
        WriteBuffer.StartMessage(sizeof(byte) + sizeof(int) + count);
        if (WriteBuffer.WriteSpaceLeft < sizeof(byte) + sizeof(int))
            await WriteBuffer.Flush(async, cancellationToken).ConfigureAwait(false);

        WriteBuffer.WriteByte(FrontendMessageCode.Password);
        WriteBuffer.WriteInt32(sizeof(int) + count);

        if (count <= WriteBuffer.WriteSpaceLeft)
        {
            // The entire array fits in our WriteBuffer, copy it into the WriteBuffer as usual.
            WriteBuffer.WriteBytes(payload, offset, count);
            return;
        }

        await WriteBuffer.Flush(async, cancellationToken).ConfigureAwait(false);
        await WriteBuffer.DirectWrite(new ReadOnlyMemory<byte>(payload, offset, count), async, cancellationToken).ConfigureAwait(false);
    }

    internal async Task WriteSASLInitialResponse(string mechanism, byte[] initialResponse, bool async, CancellationToken cancellationToken = default)
    {
        var len = sizeof(byte)                                               +  // Message code
                  sizeof(int)                                                +  // Length
                  NpgsqlWriteBuffer.UTF8Encoding.GetByteCount(mechanism) + sizeof(byte) +  // Mechanism plus null terminator
                  sizeof(int)                                                +  // Initial response length
                  (initialResponse?.Length ?? 0);                               // Initial response payload

        WriteBuffer.StartMessage(len);
        if (WriteBuffer.WriteSpaceLeft < len)
            await WriteBuffer.Flush(async, cancellationToken).ConfigureAwait(false);

        WriteBuffer.WriteByte(FrontendMessageCode.Password);
        WriteBuffer.WriteInt32(len - 1);

        WriteBuffer.WriteString(mechanism);
        WriteBuffer.WriteByte(0);   // null terminator
        if (initialResponse == null)
            WriteBuffer.WriteInt32(-1);
        else
        {
            WriteBuffer.WriteInt32(initialResponse.Length);
            WriteBuffer.WriteBytes(initialResponse);
        }
    }

    internal Task WriteSASLResponse(byte[] payload, bool async, CancellationToken cancellationToken = default) => WritePassword(payload, async, cancellationToken);

    #endregion Authentication

    internal Task WritePregenerated(byte[] data, bool async = false, CancellationToken cancellationToken = default)
    {
        WriteBuffer.StartMessage(data.Length);
        if (WriteBuffer.WriteSpaceLeft < data.Length)
            return FlushAndWrite(data, async, cancellationToken);

        WriteBuffer.WriteBytes(data, 0, data.Length);
        return Task.CompletedTask;

        async Task FlushAndWrite(byte[] data, bool async, CancellationToken cancellationToken)
        {
            await Flush(async, cancellationToken).ConfigureAwait(false);
            Debug.Assert(data.Length <= WriteBuffer.WriteSpaceLeft, $"Pregenerated message has length {data.Length} which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
            WriteBuffer.WriteBytes(data, 0, data.Length);
        }
    }

    internal void Flush() => WriteBuffer.Flush(false).GetAwaiter().GetResult();

    internal Task Flush(bool async, CancellationToken cancellationToken = default) => WriteBuffer.Flush(async, cancellationToken);
}
