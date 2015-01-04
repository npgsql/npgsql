using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Npgsql.FrontendMessages
{
    class BindMessage : ComplexFrontendMessage
    {
        /// <summary>
        /// The name of the destination portal (an empty string selects the unnamed portal).
        /// </summary>
        string Portal { get; set; }

        /// <summary>
        /// The name of the source prepared statement (an empty string selects the unnamed prepared statement).
        /// </summary>
        string Statement { get; set; }

        internal List<NpgsqlParameter> Parameters { get; private set; }
        internal List<FormatCode> ResultFormatCodes { get; private set; }

        State _state;
        int _paramIndex;

        const byte Code = (byte)'B';

        internal BindMessage(string portal="", string statement="")
        {
            Portal = portal;
            Statement = statement;
            Parameters = new List<NpgsqlParameter>();
            _state = State.WroteNothing;
            _paramIndex = 0;
        }

        internal override void Prepare()
        {
        }

        internal override bool Write(NpgsqlBuffer buf)
        {
            Contract.Requires(Statement != null && Statement.All(c => c < 128));
            Contract.Requires(Portal != null && Portal.All(c => c < 128));

            var formatCodeListLength = 0;  // TEMP
            var numInputParameters = 0;
            var parameterSizes = new List<int>() {0};
            int[] resultFormatCodes = null;

            switch (_state)
            {
                case State.WroteNothing:
                    var messageLengthBeforeParameters =
                        4 +                        // Message length
                        Portal.Length + 1 +
                        Statement.Length + 1 +
                        2 +                        // Number of parameter format codes that follow
                        2 * formatCodeListLength + // List of format codes
                        2;                         // Number of parameters

                    if (buf.WriteSpaceLeft < messageLengthBeforeParameters) {
                        if (buf.Size < messageLengthBeforeParameters) {
                            throw new Exception("Buffer too small for Bind header");
                        }
                        return false;
                    }

                    var messageLength = messageLengthBeforeParameters +
                        4 * numInputParameters +   // Parameter value lengths
                        parameterSizes.Sum() +     // Parameter values
                        2 +                        // Number of result format codes
                        2 * (resultFormatCodes == null ? 1 : resultFormatCodes.Length); // Use binary for everything that is received if unknown

                    buf.WriteByte(Code);
                    buf.WriteInt32(messageLength);
                    buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Portal));
                    buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Statement));
                    buf.WriteInt16(formatCodeListLength);
                    if (formatCodeListLength == 1)
                    {
                        buf.WriteInt16((short)FormatCode.Binary);
                    }
                    /*
                    else if (formatCodeListLength > 1)
                    {
                        if (2 * formatCodeListLength <= buf.Size)
                        {
                            buf.EnsureWrite(2 * formatCodeListLength);
                            foreach (var code in formatCodes)
                                buf.WriteInt16((short)code);
                        }
                        else
                        {
                            // Special case
                            foreach (var code in formatCodes)
                                buf.EnsuredWriteInt16((short)code);
                        }
                    }
                     */
                    buf.WriteInt16(numInputParameters);
                    goto case State.WroteHeader;

                case State.WroteHeader:
                    _state = State.WroteHeader;
                    if (!WriteParameters(buf)) {
                        return false;
                    }
                    goto case State.WroteParameters;

                case State.WroteParameters:
                    _state = State.WroteParameters;
                    // TODO: Implement for real and check length!
                    buf.WriteInt16(1);
                    buf.WriteInt16(0);
                    _state = State.WroteAll;
                    return true;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        bool WriteParameters(NpgsqlBuffer buf)
        {
            return true;
#if TODO
            // Traverse parameters
            int sizeForWriteBinary = 0;
            for (; _paramIndex < Parameters.Count; _paramIndex++)
            {
                var param = Parameters[_paramIndex];
                if (!param.IsInputDirection)
                {
                    continue;
                }

                if (param.IsNull)
                {
                    buf.WriteInt32(-1);
                    continue;
                }
                if (formatCodes[j] == FormatCode.Text)
                {
                    buf.EnsuredWriteInt32(parameterSizes[j]);
                    parameterTexts[j].WriteToOutputBuffer(buf);
                }
                else
                {
                    var handler = _connector.TypeHandlerRegistry[parameterTypeOIDs[j]];
                    var flushedBytesBeforeThisValue = buf.TotalBytesFlushed;
                    handler.WriteBinary(_connector.TypeHandlerRegistry, parameterTypeOIDs[j], _parameters[i].Value, buf, sizeArr, ref sizeForWriteBinary);
                }
            }
#endif
        }

        public override string ToString()
        {
            return String.Format("[Bind(Portal={0},Statement={1},NumParams={2}]", Portal, Statement, Parameters.Count);
        }

        private enum State
        {
            WroteNothing,
            WroteHeader,
            WroteParameters,
            WroteAll
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(Portal != null);
            Contract.Invariant(Statement != null);
            Contract.Invariant(Parameters != null);

        }
    }
}
