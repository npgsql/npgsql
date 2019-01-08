using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.TypeMapping;

namespace Npgsql.FrontendMessages
{
    class ParseMessage : FrontendMessage
    {
        /// <summary>
        /// The query string to be parsed.
        /// </summary>
        string Query { get; set; }

        /// <summary>
        /// The name of the destination prepared statement (an empty string selects the unnamed prepared statement).
        /// </summary>
        string Statement { get; set; }

        // ReSharper disable once InconsistentNaming
        List<uint> ParameterTypeOIDs { get; }

        readonly Encoding _encoding;

        const byte Code = (byte)'P';

        internal ParseMessage(Encoding encoding)
        {
            _encoding = encoding;
            ParameterTypeOIDs = new List<uint>();
        }

        internal ParseMessage Populate(string sql, string statementName, List<NpgsqlParameter> inputParameters, ConnectorTypeMapper typeMapper)
        {
            Populate(sql, statementName);
            foreach (var inputParam in inputParameters)
            {
                Debug.Assert(inputParam.Handler != null, "Input parameter doesn't have a resolved handler when populating Parse message");
                ParameterTypeOIDs.Add(inputParam.Handler.PostgresType.OID);
            }
            return this;
        }

        internal ParseMessage Populate(string sql, string statementName)
        {
            ParameterTypeOIDs.Clear();
            Query = sql;
            Statement = statementName;
            return this;
        }

        internal override async Task Write(NpgsqlWriteBuffer buf, bool async)
        {
            Debug.Assert(Statement != null && Statement.All(c => c < 128));

            var queryByteLen = _encoding.GetByteCount(Query);
            if (buf.WriteSpaceLeft < 1 + 4 + Statement.Length + 1)
                await buf.Flush(async);

            var messageLength =
                1 +                         // Message code
                4 +                         // Length
                Statement.Length +
                1 +                         // Null terminator
                queryByteLen +
                1 +                         // Null terminator
                2 +                         // Number of parameters
                ParameterTypeOIDs.Count * 4;

            buf.WriteByte(Code);
            buf.WriteInt32(messageLength - 1);
            buf.WriteNullTerminatedString(Statement);

            await buf.WriteString(Query, queryByteLen, async);

            if (buf.WriteSpaceLeft < 1 + 2)
                await buf.Flush(async);
            buf.WriteByte(0); // Null terminator for the query
            buf.WriteInt16((short)ParameterTypeOIDs.Count);

            foreach (var t in ParameterTypeOIDs)
            {
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);
                buf.WriteInt32((int)t);
            }
        }

        public override string ToString()
            => $"[Parse(Statement={Statement},NumParams={ParameterTypeOIDs.Count}]";
    }
}
