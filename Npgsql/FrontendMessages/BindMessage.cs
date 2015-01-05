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

        List<NpgsqlParameter> InputParameters { get; set; }
        internal List<FormatCode> ResultFormatCodes { get; private set; }

        TypeHandlerRegistry _typeHandlerRegistry;
        State _state;
        int _paramIndex;
        bool _wroteParamLen;

        const byte Code = (byte)'B';

        internal BindMessage(TypeHandlerRegistry typeHandlerRegistry, List<NpgsqlParameter> inputParameters,
                             string portal="", string statement="")
        {
            Contract.Requires(typeHandlerRegistry != null);
            Contract.Requires(inputParameters != null);
            Contract.Requires(portal != null);
            Contract.Requires(statement != null);

            _typeHandlerRegistry = typeHandlerRegistry;
            Portal = portal;
            Statement = statement;
            InputParameters = inputParameters;
            _state = State.WroteNothing;
            _paramIndex = 0;
            _wroteParamLen = false;
        }

        internal override void Prepare()
        {
            Contract.Requires(InputParameters.All(p => p.IsInputDirection));
            foreach (var inParam in InputParameters)
            {
                inParam.Bind(_typeHandlerRegistry);
            }
        }

        internal override bool Write(NpgsqlBuffer buf, out byte[] directBuf)
        {
            Contract.Requires(Statement != null && Statement.All(c => c < 128));
            Contract.Requires(Portal != null && Portal.All(c => c < 128));
            directBuf = null;

            int[] resultFormatCodes = null;

            switch (_state)
            {
                case State.WroteNothing:
                    var formatCodesSum = InputParameters.Select(p => p.BoundFormatCode).Sum(c => (int)c);
                    var formatCodeListLength = formatCodesSum == 0 ? 0 : formatCodesSum == InputParameters.Count ? 1 : InputParameters.Count;

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
                        4 * InputParameters.Count +                       // Parameter lengths
                        InputParameters.Select(p => p.BoundSize).Sum() +  // Parameter values
                        2 +                                                // Number of result format codes
                        2 * (resultFormatCodes == null ? 1 : resultFormatCodes.Length); // Use binary for everything that is received if unknown

                    buf.WriteByte(Code);
                    buf.WriteInt32(messageLength);
                    buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Portal));
                    buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Statement));

                    // 0 implicitly means all-text, 1 means all binary, >1 means mix-and-match
                    buf.WriteInt16(formatCodeListLength);
                    if (formatCodeListLength == 1)
                    {
                        buf.WriteInt16((short)FormatCode.Binary);
                    }
                    else if (formatCodeListLength > 1)
                    {
                        foreach (var code in InputParameters.Select(p => p.BoundFormatCode))
                            buf.WriteInt16((short)code);
                    }

                    buf.WriteInt16(InputParameters.Count);
                    goto case State.WroteHeader;

                case State.WroteHeader:
                    _state = State.WroteHeader;
                    if (!WriteParameters(buf, out directBuf)) {
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

        bool WriteParameters(NpgsqlBuffer buf, out byte[] directBuf)
        {
            directBuf = null;
            for (; _paramIndex < InputParameters.Count; _paramIndex++)
            {
                var param = InputParameters[_paramIndex];

                if (param.IsNull)
                {
                    if (buf.WriteSpaceLeft < 4) {
                        return false;
                    }

                    buf.WriteInt32(-1);
                    continue;
                }

                var handler = param.BoundHandler;
                if (param.BoundFormatCode == FormatCode.Text)
                {
                    throw new NotImplementedException();
                }

                if (handler.IsBufferManager)
                {
                    if (!_wroteParamLen)
                    {
                        if (buf.WriteSpaceLeft < 4) {
                            return false;
                        }
                        buf.WriteInt32(param.BoundSize);
                        _wroteParamLen = true;
                    }
                    if (!handler.WriteBinary(param.Value, buf, out directBuf)) {
                        return false;
                    }
                    _wroteParamLen = false;
                }
                else
                {
                    if (buf.WriteSpaceLeft < param.BoundSize + 4)
                    {
                        Contract.Assume(buf.Size < param.BoundSize + 4);
                        return false;
                    }
                    buf.WriteInt32(param.BoundSize);
                    param.BoundHandler.WriteBinary(param.Value, buf);                    
                }
            }
            return true;
        }

        public override string ToString()
        {
            return String.Format("[Bind(Portal={0},Statement={1},NumParams={2}]", Portal, Statement, InputParameters.Count);
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
            Contract.Invariant(InputParameters != null);

        }
    }
}
