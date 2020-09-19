using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Util;
// ReSharper disable VariableHidesOuterVariable

namespace Npgsql
{
    partial class NpgsqlConnector
    {
        internal Task WriteDescribe(StatementOrPortal statementOrPortal, string name, bool async, CancellationToken cancellationToken = default)
        {
            Debug.Assert(name.All(c => c < 128));

            var len = sizeof(byte) +       // Message code
                      sizeof(int)  +       // Length
                      sizeof(byte) +       // Statement or portal
                      (name.Length + 1);   // Statement/portal name

            if (WriteBuffer.WriteSpaceLeft < len)
                return FlushAndWrite(len, statementOrPortal, name, async);

            Write(len, statementOrPortal, name);
            return Task.CompletedTask;

            async Task FlushAndWrite(int len, StatementOrPortal statementOrPortal, string name, bool async)
            {
                await Flush(async, cancellationToken);
                Debug.Assert(len <= WriteBuffer.WriteSpaceLeft, $"Message of type {GetType().Name} has length {len} which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
                Write(len, statementOrPortal, name);
            }

            void Write(int len, StatementOrPortal statementOrPortal, string name)
            {
                WriteBuffer.WriteByte(FrontendMessageCode.Describe);
                WriteBuffer.WriteInt32(len - 1);
                WriteBuffer.WriteByte((byte)statementOrPortal);
                WriteBuffer.WriteNullTerminatedString(name);
            }
        }

        internal Task WriteSync(bool async, CancellationToken cancellationToken = default)
        {
            const int len = sizeof(byte) +  // Message code
                            sizeof(int);    // Length

            if (WriteBuffer.WriteSpaceLeft < len)
                return FlushAndWrite(async);

            Write();
            return Task.CompletedTask;

            async Task FlushAndWrite(bool async)
            {
                await Flush(async, cancellationToken);
                Debug.Assert(len <= WriteBuffer.WriteSpaceLeft, $"Message of type {GetType().Name} has length {len} which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
                Write();
            }

            void Write()
            {
                WriteBuffer.WriteByte(FrontendMessageCode.Sync);
                WriteBuffer.WriteInt32(len - 1);
            }
        }

        internal Task WriteExecute(int maxRows, bool async, CancellationToken cancellationToken = default)
        {
            // Note: non-empty portal currently not supported

            const int len = sizeof(byte) +       // Message code
                            sizeof(int)  +       // Length
                            sizeof(byte) +       // Null-terminated portal name (always empty for now)
                            sizeof(int);         // Max number of rows

            if (WriteBuffer.WriteSpaceLeft < len)
                return FlushAndWrite(maxRows, async);

            Write(maxRows);
            return Task.CompletedTask;

            async Task FlushAndWrite(int maxRows, bool async)
            {
                await Flush(async, cancellationToken);
                Debug.Assert(10 <= WriteBuffer.WriteSpaceLeft, $"Message of type {GetType().Name} has length 10 which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
                Write(maxRows);
            }

            void Write(int maxRows)
            {
                WriteBuffer.WriteByte(FrontendMessageCode.Execute);
                WriteBuffer.WriteInt32(len - 1);
                WriteBuffer.WriteByte(0);   // Portal is always empty for now
                WriteBuffer.WriteInt32(maxRows);
            }
        }

        internal async Task WriteParse(string sql, string statementName, List<NpgsqlParameter> inputParameters, bool async, CancellationToken cancellationToken = default)
        {
            Debug.Assert(statementName.All(c => c < 128));

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

            if (WriteBuffer.WriteSpaceLeft < 1 + 4 + statementName.Length + 1)
                await Flush(async, cancellationToken);

            var messageLength =
                sizeof(byte)                +         // Message code
                sizeof(int)                 +         // Length
                statementName.Length        +         // Statement name
                sizeof(byte)                +         // Null terminator for the statement name
                queryByteLen + sizeof(byte) +         // SQL query length plus null terminator
                sizeof(ushort)              +         // Number of parameters
                inputParameters.Count * sizeof(int);  // Parameter OIDs

            WriteBuffer.WriteByte(FrontendMessageCode.Parse);
            WriteBuffer.WriteInt32(messageLength - 1);
            WriteBuffer.WriteNullTerminatedString(statementName);

            await WriteBuffer.WriteString(sql, queryByteLen, async, cancellationToken);

            if (WriteBuffer.WriteSpaceLeft < 1 + 2)
                await Flush(async, cancellationToken);
            WriteBuffer.WriteByte(0); // Null terminator for the query
            WriteBuffer.WriteUInt16((ushort)inputParameters.Count);

            foreach (var p in inputParameters)
            {
                if (WriteBuffer.WriteSpaceLeft < 4)
                    await Flush(async, cancellationToken);

                WriteBuffer.WriteInt32((int)p.Handler!.PostgresType.OID);
            }
        }

        internal async Task WriteBind(
            List<NpgsqlParameter> inputParameters,
            string portal,
            string statement,
            bool allResultTypesAreUnknown,
            bool[]? unknownResultTypeList,
            bool async,
            CancellationToken cancellationToken = default)
        {
            Debug.Assert(statement.All(c => c < 128));
            Debug.Assert(portal.All(c => c < 128));

            var headerLength =
                sizeof(byte)                    +     // Message code
                sizeof(int)                     +     // Message length
                sizeof(byte)                    +     // Portal is always empty (only a null terminator)
                statement.Length + sizeof(byte) +     // Statement name plus null terminator
                sizeof(ushort);                       // Number of parameter format codes that follow

            if (WriteBuffer.WriteSpaceLeft < headerLength)
            {
                Debug.Assert(WriteBuffer.Size >= headerLength, "Write buffer too small for Bind header");
                await Flush(async, cancellationToken);
            }

            var formatCodesSum = 0;
            var paramsLength = 0;
            foreach (var p in inputParameters)
            {
                formatCodesSum += (int)p.FormatCode;
                p.LengthCache?.Rewind();
                paramsLength += p.ValidateAndGetLength();
            }

            var formatCodeListLength = formatCodesSum == 0 ? 0 : formatCodesSum == inputParameters.Count ? 1 : inputParameters.Count;

            var messageLength = headerLength         +
                sizeof(short) * formatCodeListLength +                  // List of format codes
                sizeof(short)                        +                  // Number of parameters
                sizeof(int) * inputParameters.Count  +                  // Parameter lengths
                paramsLength                         +                  // Parameter values
                sizeof(short)                        +                  // Number of result format codes
                sizeof(short) * (unknownResultTypeList?.Length ?? 1);   // Result format codes

            WriteBuffer.WriteByte(FrontendMessageCode.Bind);
            WriteBuffer.WriteInt32(messageLength - 1);
            Debug.Assert(portal == string.Empty);
            WriteBuffer.WriteByte(0);  // Portal is always empty

            WriteBuffer.WriteNullTerminatedString(statement);
            WriteBuffer.WriteInt16(formatCodeListLength);

            // 0 length implicitly means all-text, 1 means all-binary, >1 means mix-and-match
            if (formatCodeListLength == 1)
            {
                if (WriteBuffer.WriteSpaceLeft < 2)
                    await Flush(async, cancellationToken);
                WriteBuffer.WriteInt16((short)FormatCode.Binary);
            }
            else if (formatCodeListLength > 1)
            {
                foreach (var p in inputParameters)
                {
                    if (WriteBuffer.WriteSpaceLeft < 2)
                        await Flush(async, cancellationToken);
                    WriteBuffer.WriteInt16((short)p.FormatCode);
                }
            }

            if (WriteBuffer.WriteSpaceLeft < 2)
                await Flush(async, cancellationToken);

            WriteBuffer.WriteUInt16((ushort)inputParameters.Count);

            foreach (var param in inputParameters)
            {
                param.LengthCache?.Rewind();
                await param.WriteWithLength(WriteBuffer, async, cancellationToken);
            }

            if (unknownResultTypeList != null)
            {
                if (WriteBuffer.WriteSpaceLeft < 2 + unknownResultTypeList.Length * 2)
                    await Flush(async, cancellationToken);
                WriteBuffer.WriteInt16(unknownResultTypeList.Length);
                foreach (var t in unknownResultTypeList)
                    WriteBuffer.WriteInt16(t ? 0 : 1);
            }
            else
            {
                if (WriteBuffer.WriteSpaceLeft < 4)
                    await Flush(async, cancellationToken);
                WriteBuffer.WriteInt16(1);
                WriteBuffer.WriteInt16(allResultTypesAreUnknown ? 0 : 1);
            }
        }

        internal Task WriteClose(StatementOrPortal type, string name, bool async, CancellationToken cancellationToken = default)
        {
            var len = sizeof(byte) +               // Message code
                      sizeof(int)  +               // Length
                      sizeof(byte) +               // Statement or portal
                      name.Length + sizeof(byte);  // Statement or portal name plus null terminator

            if (WriteBuffer.WriteSpaceLeft < 10)
                return FlushAndWrite(len, type, name, async);

            Write(len, type, name);
            return Task.CompletedTask;

            async Task FlushAndWrite(int len, StatementOrPortal type, string name, bool async)
            {
                await Flush(async, cancellationToken);
                Debug.Assert(len <= WriteBuffer.WriteSpaceLeft, $"Message of type {GetType().Name} has length {len} which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
                Write(len, type, name);
            }

            void Write(int len, StatementOrPortal type, string name)
            {
                WriteBuffer.WriteByte(FrontendMessageCode.Close);
                WriteBuffer.WriteInt32(len - 1);
                WriteBuffer.WriteByte((byte)type);
                WriteBuffer.WriteNullTerminatedString(name);
            }
        }

        internal void WriteQuery(string sql) => WriteQuery(sql, false).GetAwaiter().GetResult();

        internal async Task WriteQuery(string sql, bool async, CancellationToken cancellationToken = default)
        {
            var queryByteLen = TextEncoding.GetByteCount(sql);

            if (WriteBuffer.WriteSpaceLeft < 1 + 4)
                await Flush(async, cancellationToken);

            WriteBuffer.WriteByte(FrontendMessageCode.Query);
            WriteBuffer.WriteInt32(
                sizeof(int)  +        // Message length (including self excluding code)
                queryByteLen +        // Query byte length
                sizeof(byte));        // Null terminator

            await WriteBuffer.WriteString(sql, queryByteLen, async, cancellationToken);
            if (WriteBuffer.WriteSpaceLeft < 1)
                await Flush(async, cancellationToken);
            WriteBuffer.WriteByte(0);  // Null terminator
        }

        internal void WriteCopyDone() => WriteCopyDone(false).GetAwaiter().GetResult();

        internal async Task WriteCopyDone(bool async, CancellationToken cancellationToken = default)
        {
            const int len = sizeof(byte) +   // Message code
                            sizeof(int);     // Length

            if (WriteBuffer.WriteSpaceLeft < len)
                await Flush(async, cancellationToken);

            WriteBuffer.WriteByte(FrontendMessageCode.CopyDone);
            WriteBuffer.WriteInt32(len - 1);
        }

        internal async Task WriteCopyFail(bool async, CancellationToken cancellationToken = default)
        {
            // Note: error message not supported for now

            const int len = sizeof(byte) +  // Message code
                            sizeof(int) +   // Length
                            sizeof(byte);   // Error message is always empty (only a null terminator)

            if (WriteBuffer.WriteSpaceLeft < len)
                await Flush(async, cancellationToken);

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

            if (WriteBuffer.WriteSpaceLeft < len)
                Flush(false).GetAwaiter().GetResult();

            WriteBuffer.WriteByte(FrontendMessageCode.Terminate);
            WriteBuffer.WriteInt32(len - 1);
        }

        internal void WriteSslRequest()
        {
            const int len = sizeof(int) +  // Length
                            sizeof(int);   // SSL request code

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
                len += PGUtil.UTF8Encoding.GetByteCount(kvp.Key) + 1 +
                       PGUtil.UTF8Encoding.GetByteCount(kvp.Value) + 1;

            // Should really never happen, just in case
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
            if (WriteBuffer.WriteSpaceLeft < sizeof(byte) + sizeof(int))
                await WriteBuffer.Flush(async, cancellationToken);
            WriteBuffer.WriteByte(FrontendMessageCode.Password);
            WriteBuffer.WriteInt32(sizeof(int) + count);

            if (count <= WriteBuffer.WriteSpaceLeft)
            {
                // The entire array fits in our WriteBuffer, copy it into the WriteBuffer as usual.
                WriteBuffer.WriteBytes(payload, offset, count);
                return;
            }

            await WriteBuffer.Flush(async, cancellationToken);
            await WriteBuffer.DirectWrite(new ReadOnlyMemory<byte>(payload, offset, count), async, cancellationToken);
        }

        internal async Task WriteSASLInitialResponse(string mechanism, byte[] initialResponse, bool async, CancellationToken cancellationToken = default)
        {
            var len = sizeof(byte)                                               +  // Message code
                      sizeof(int)                                                +  // Length
                      PGUtil.UTF8Encoding.GetByteCount(mechanism) + sizeof(byte) +  // Mechanism plus null terminator
                      sizeof(int)                                                +  // Initial response length
                      (initialResponse?.Length ?? 0);                               // Initial response payload

            if (WriteBuffer.WriteSpaceLeft < len)
                await WriteBuffer.Flush(async, cancellationToken);

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
            if (WriteBuffer.WriteSpaceLeft < data.Length)
                return FlushAndWrite(data, async);

            WriteBuffer.WriteBytes(data, 0, data.Length);
            return Task.CompletedTask;

            async Task FlushAndWrite(byte[] data, bool async)
            {
                await Flush(async, cancellationToken);
                Debug.Assert(data.Length <= WriteBuffer.WriteSpaceLeft, $"Pregenerated message has length {data.Length} which is bigger than the buffer ({WriteBuffer.WriteSpaceLeft})");
                WriteBuffer.WriteBytes(data, 0, data.Length);
            }
        }

        internal void Flush() => WriteBuffer.Flush(false).GetAwaiter().GetResult();

        internal Task Flush(bool async, CancellationToken cancellationToken = default) => WriteBuffer.Flush(async, cancellationToken);
    }
}
