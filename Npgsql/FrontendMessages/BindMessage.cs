using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Npgsql.FrontendMessages
{
    class BindMessage : ChunkingFrontendMessage
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
        internal bool AllResultTypesAreUnknown { get; set; }
        internal bool[] UnknownResultTypeList { get; set; }

        TypeHandlerRegistry _typeHandlerRegistry;
        State _state;
        int _paramIndex;
        bool _wroteParamLen;

        const byte Code = (byte)'B';

        internal BindMessage Populate(TypeHandlerRegistry typeHandlerRegistry, List<NpgsqlParameter> inputParameters,
                             string portal="", string statement="")
        {
            Contract.Requires(typeHandlerRegistry != null);
            Contract.Requires(inputParameters != null && inputParameters.All(p => p.IsInputDirection));
            Contract.Requires(portal != null);
            Contract.Requires(statement != null);

            AllResultTypesAreUnknown = false;
            UnknownResultTypeList = null;
            _typeHandlerRegistry = typeHandlerRegistry;
            Portal = portal;
            Statement = statement;
            InputParameters = inputParameters;
            _state = State.WroteNothing;
            _paramIndex = 0;
            _wroteParamLen = false;
            return this;
        }

        internal override bool Write(NpgsqlBuffer buf, ref DirectBuffer directBuf)
        {
            Contract.Requires(Statement != null && Statement.All(c => c < 128));
            Contract.Requires(Portal != null && Portal.All(c => c < 128));

            switch (_state)
            {
                case State.WroteNothing:
                    var formatCodesSum = InputParameters.Select(p => p.FormatCode).Sum(c => (int)c);
                    var formatCodeListLength = formatCodesSum == 0 ? 0 : formatCodesSum == InputParameters.Count ? 1 : InputParameters.Count;

                    var headerLength =
                        4 +                        // Message length
                        Portal.Length + 1 +
                        Statement.Length + 1 +
                        2 +                        // Number of parameter format codes that follow
                        2 * formatCodeListLength + // List of format codes
                        2;                         // Number of parameters

                    if (buf.WriteSpaceLeft < headerLength)
                    {
                        Contract.Assume(buf.Size >= headerLength, "Buffer too small for Bind header");
                        return false;
                    }

                    foreach (var p in InputParameters) { p.RewindLengthCache(); }
                    var messageLength = headerLength +
                        4 * InputParameters.Count +                                     // Parameter lengths
                        InputParameters.Select(p => p.ValidateAndGetLength()).Sum() +   // Parameter values
                        2 +                                                             // Number of result format codes
                        2 * (UnknownResultTypeList == null ? 1 : UnknownResultTypeList.Length);  // Result format codes

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
                        foreach (var code in InputParameters.Select(p => p.FormatCode))
                            buf.WriteInt16((short)code);
                    }

                    buf.WriteInt16(InputParameters.Count);

                    _state = State.WroteHeader;
                    goto case State.WroteHeader;

                case State.WroteHeader:
                    if (!WriteParameters(buf, ref directBuf)) { return false; }
                    _state = State.WroteParameters;
                    goto case State.WroteParameters;

                case State.WroteParameters:
                    if (UnknownResultTypeList != null)
                    {
                        if (buf.WriteSpaceLeft < 2 + UnknownResultTypeList.Length * 2) { return false; }
                        buf.WriteInt16(UnknownResultTypeList.Length);
                        foreach (var t in UnknownResultTypeList) {
                            buf.WriteInt16(t ? 0 : 1);
                        }
                    }
                    else
                    {
                        if (buf.WriteSpaceLeft < 4) { return false; }
                        buf.WriteInt16(1);
                        buf.WriteInt16(AllResultTypesAreUnknown ? 0 : 1);                        
                    }

                    _state = State.Done;
                    return true;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        bool WriteParameters(NpgsqlBuffer buf, ref DirectBuffer directBuf)
        {
            for (; _paramIndex < InputParameters.Count; _paramIndex++)
            {
                var param = InputParameters[_paramIndex];

                if (param.IsNull)
                {
                    if (buf.WriteSpaceLeft < 4) { return false; }
                    buf.WriteInt32(-1);
                    continue;
                }

                var handler = param.Handler;

                param.RewindLengthCache();
                var len = param.ValidateAndGetLength();

                var asChunkingWriter = handler as IChunkingTypeWriter;
                if (asChunkingWriter != null)
                {
                    if (!_wroteParamLen)
                    {
                        if (buf.WriteSpaceLeft < 4) { return false; }
                        buf.WriteInt32(len);
                        asChunkingWriter.PrepareWrite(param.Value, buf, param);
                        _wroteParamLen = true;
                    }
                    if (!asChunkingWriter.Write(ref directBuf)) {
                        return false;
                    }
                    _wroteParamLen = false;
                    continue;
                }

                var asSimpleWriter = (ISimpleTypeWriter)handler;
                if (buf.WriteSpaceLeft < len + 4)
                {
                    Contract.Assume(buf.Size >= len + 4);
                    return false;
                }
                buf.WriteInt32(len);
                asSimpleWriter.Write(param.Value, buf);                    
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
            Done
        }
    }
}
