#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

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

        State _state;
        int _paramIndex;
        bool _wroteParamLen;

        const byte Code = (byte)'B';

        internal BindMessage Populate(List<NpgsqlParameter> inputParameters, string portal = "", string statement = "")
        {
            Contract.Requires(inputParameters != null && inputParameters.All(p => p.IsInputDirection));
            Contract.Requires(portal != null);
            Contract.Requires(statement != null);

            AllResultTypesAreUnknown = false;
            UnknownResultTypeList = null;
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
                        1 +                        // Message code
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

                    foreach (var c in InputParameters.Select(p => p.LengthCache).Where(c => c != null)) { c.Rewind(); }
                    var messageLength = headerLength +
                        4 * InputParameters.Count +                                     // Parameter lengths
                        InputParameters.Select(p => p.ValidateAndGetLength()).Sum() +   // Parameter values
                        2 +                                                             // Number of result format codes
                        2 * (UnknownResultTypeList?.Length ?? 1);                       // Result format codes

                    buf.WriteByte(Code);
                    buf.WriteInt32(messageLength-1);
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

                if (!_wroteParamLen)
                {
                    if (param.Value is DBNull)
                    {
                        if (buf.WriteSpaceLeft < 4) { return false; }
                        buf.WriteInt32(-1);
                        continue;
                    }

                    param.LengthCache?.Rewind();
                }

                var handler = param.Handler;

                var asChunkingWriter = handler as IChunkingTypeHandler;
                if (asChunkingWriter != null)
                {
                    if (!_wroteParamLen)
                    {
                        if (buf.WriteSpaceLeft < 4) { return false; }
                        buf.WriteInt32(param.ValidateAndGetLength());
                        asChunkingWriter.PrepareWrite(param.Value, buf, param.LengthCache, param);
                        _wroteParamLen = true;
                    }
                    if (!asChunkingWriter.Write(ref directBuf)) {
                        return false;
                    }
                    _wroteParamLen = false;
                    continue;
                }

                var len = param.ValidateAndGetLength();
                var asSimpleWriter = (ISimpleTypeHandler)handler;
                if (buf.WriteSpaceLeft < len + 4)
                {
                    Contract.Assume(buf.Size >= len + 4);
                    return false;
                }
                buf.WriteInt32(len);
                asSimpleWriter.Write(param.Value, buf, param);
            }
            return true;
        }

        public override string ToString()
        {
            return $"[Bind(Portal={Portal},Statement={Statement},NumParams={InputParameters.Count}]";
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
