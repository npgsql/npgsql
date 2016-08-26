#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class BindMessage : FrontendMessage
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
        int _formatCodeListLength;
        bool _wroteParamLen;

        const byte Code = (byte)'B';

        internal BindMessage Populate(List<NpgsqlParameter> inputParameters, string portal = "", string statement = "")
        {
            Debug.Assert(inputParameters != null && inputParameters.All(p => p.IsInputDirection));
            Debug.Assert(portal != null);
            Debug.Assert(statement != null);

            AllResultTypesAreUnknown = false;
            UnknownResultTypeList = null;
            Portal = portal;
            Statement = statement;
            InputParameters = inputParameters;
            _state = State.Header;
            _paramIndex = 0;
            _wroteParamLen = false;
            return this;
        }

        /// <summary>
        /// Bind is a special message in that it supports the "direct buffer" optimization, which allows us to write
        /// user byte[] data directly to the stream rather than copying it into our buffer. It therefore has its own
        /// special overload of Write below.
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        internal override bool Write(WriteBuffer buf)
        {
            throw new NotSupportedException($"Internal error, call the overload of {nameof(Write)} which accepts a {nameof(DirectBuffer)}");
        }

        internal bool Write(WriteBuffer buf, ref DirectBuffer directBuf)
        {
            Debug.Assert(Statement != null && Statement.All(c => c < 128));
            Debug.Assert(Portal != null && Portal.All(c => c < 128));

            switch (_state)
            {
            case State.Header:
                var formatCodesSum = InputParameters.Select(p => p.FormatCode).Sum(c => (int)c);
                _formatCodeListLength = formatCodesSum == 0 ? 0 : formatCodesSum == InputParameters.Count ? 1 : InputParameters.Count;
                var headerLength =
                    1 +                        // Message code
                    4 +                        // Message length
                    Portal.Length + 1 +
                    Statement.Length + 1 +
                    2;                         // Number of parameter format codes that follow

                if (buf.WriteSpaceLeft < headerLength)
                {
                    Debug.Assert(buf.Size >= headerLength, "Buffer too small for Bind header");
                    return false;
                }

                foreach (var c in InputParameters.Select(p => p.LengthCache).Where(c => c != null))
                    c.Rewind();
                var messageLength = headerLength +
                    2 * _formatCodeListLength + // List of format codes
                    2 +                         // Number of parameters
                    4 * InputParameters.Count +                                     // Parameter lengths
                    InputParameters.Select(p => p.ValidateAndGetLength()).Sum() +   // Parameter values
                    2 +                                                             // Number of result format codes
                    2 * (UnknownResultTypeList?.Length ?? 1);                       // Result format codes

                buf.WriteByte(Code);
                buf.WriteInt32(messageLength-1);
                buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Portal));
                buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Statement));
                buf.WriteInt16(_formatCodeListLength);
                _paramIndex = 0;

                _state = State.ParameterFormatCodes;
                goto case State.ParameterFormatCodes;

            case State.ParameterFormatCodes:
                // 0 length implicitly means all-text, 1 means all-binary, >1 means mix-and-match
                if (_formatCodeListLength == 1)
                {
                    if (buf.WriteSpaceLeft < 2)
                        return false;
                    buf.WriteInt16((short)FormatCode.Binary);
                }
                else if (_formatCodeListLength > 1)
                    for (; _paramIndex < InputParameters.Count; _paramIndex++)
                    {
                        if (buf.WriteSpaceLeft < 2)
                            return false;
                        buf.WriteInt16((short)InputParameters[_paramIndex].FormatCode);
                    }

                if (buf.WriteSpaceLeft < 2)
                    return false;

                buf.WriteInt16(InputParameters.Count);
                _paramIndex = 0;

                _state = State.ParameterValues;
                goto case State.ParameterValues;

            case State.ParameterValues:
                if (!WriteParameters(buf, ref directBuf))
                    return false;
                _state = State.ResultFormatCodes;
                goto case State.ResultFormatCodes;

            case State.ResultFormatCodes:
                if (UnknownResultTypeList != null)
                {
                    if (buf.WriteSpaceLeft < 2 + UnknownResultTypeList.Length * 2)
                        return false;
                    buf.WriteInt16(UnknownResultTypeList.Length);
                    foreach (var t in UnknownResultTypeList)
                        buf.WriteInt16(t ? 0 : 1);
                }
                else
                {
                    if (buf.WriteSpaceLeft < 4)
                        return false;
                    buf.WriteInt16(1);
                    buf.WriteInt16(AllResultTypesAreUnknown ? 0 : 1);
                }

                _state = State.Done;
                return true;

            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_state} of enum {nameof(BindMessage)}.{nameof(State)}. Please file a bug.");
            }
        }

        bool WriteParameters(WriteBuffer buf, ref DirectBuffer directBuf)
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
                    Debug.Assert(buf.Size >= len + 4);
                    return false;
                }
                buf.WriteInt32(len);
                asSimpleWriter.Write(param.Value, buf, param);
            }
            return true;
        }

        public override string ToString()
            => $"[Bind(Portal={Portal},Statement={Statement},NumParams={InputParameters.Count}]";

        enum State
        {
            Header,
            ParameterFormatCodes,
            ParameterValues,
            ResultFormatCodes,
            Done
        }
    }
}
