// created on 29/6/2003 at 13:28

// Npgsql.NpgsqlBind.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.IO;

namespace Npgsql
{
    /// <summary>
    /// This class represents the Bind message sent to PostgreSQL
    /// server.
    /// </summary>
    ///
    internal sealed class NpgsqlBind : ClientMessage
    {
        private readonly String _portalName;
        private readonly byte[] _bPortalName;
        private readonly String _preparedStatementName;
        private readonly byte[] _bPreparedStatementName;
        private Int16[] _parameterFormatCodes;
        private byte[][] _parameterValues;
        private Int16[] _resultFormatCodes;
        private int _messageLength = 0;

        public NpgsqlBind(String portalName, String preparedStatementName, Int16[] parameterFormatCodes,
                          byte[][] parameterValues, Int16[] resultFormatCodes)
        {
            _portalName = portalName;
            _bPortalName = BackendEncoding.UTF8Encoding.GetBytes(_portalName);

            _preparedStatementName = preparedStatementName;
            _bPreparedStatementName = BackendEncoding.UTF8Encoding.GetBytes(_preparedStatementName);

            _parameterFormatCodes = parameterFormatCodes;
            _parameterValues = parameterValues;
            _resultFormatCodes = resultFormatCodes;
        }

        public String PortalName
        {
            get { return _portalName; }
        }

        public String PreparedStatementName
        {
            get { return _preparedStatementName; }
        }

        public Int16[] ResultFormatCodes
        {
            get { return _resultFormatCodes; }

            set
            {
                _resultFormatCodes = value;
                _messageLength = 0;
            }
        }

        public Int16[] ParameterFormatCodes
        {
            get { return _parameterFormatCodes; }

            set
            {
                _parameterFormatCodes = value;
                _messageLength = 0;
            }
        }

        public byte[][] ParameterValues
        {
            get { return _parameterValues; }

            set
            {
                _parameterValues = value;
                _messageLength = 0;
            }
        }

        public override void WriteToStream(Stream outputStream)
        {
            if (_messageLength == 0)
            {
                _messageLength =
                    4 + // Message length (32 bits)
                    _bPortalName.Length + 1 + // Portal name + null terminator
                    _bPreparedStatementName.Length + 1 + // Statement name + null terminator
                    2 + // Parameter format code array length (16 bits)
                    _parameterFormatCodes.Length * 2 + // Parameter format code array (16 bits per code)
                    2; // Parameter va;ue array length (16 bits)

                if (_parameterValues != null)
                {
                    for (int i = 0; i < _parameterValues.Length; i++)
                    {
                        _messageLength += 4; // Parameter value length (32 bits)

                        if (_parameterValues[i] != null)
                        {
                            _messageLength += _parameterValues[i].Length; // Parameter value
                        }
                    }
                }

                _messageLength +=
                    2 + // Result format code array length (16 bits)
                    _resultFormatCodes.Length * 2; // Result format code array (16 bits per code)
            }

            outputStream
                .WriteBytes((byte)FrontEndMessageCode.Bind)
                .WriteInt32(_messageLength)
                .WriteBytesNullTerminated(_bPortalName)
                .WriteBytesNullTerminated(_bPreparedStatementName)
                .WriteInt16((Int16)_parameterFormatCodes.Length);

            foreach (short code in _parameterFormatCodes)
            {
                PGUtil.WriteInt16(outputStream, code);
            }

            if (_parameterValues != null)
            {
                PGUtil.WriteInt16(outputStream, (Int16)_parameterValues.Length);

                for (int i = 0 ; i < _parameterValues.Length ; i++)
                {
                    Byte[] parameterValue = _parameterValues[i];

                    if (parameterValue == null)
                    {
                        PGUtil.WriteInt32(outputStream, -1);
                    }
                    else
                    {
                        outputStream
                            .WriteInt32(parameterValue.Length)
                            .WriteBytes(parameterValue);
                    }
                }
            }
            else
            {
                PGUtil.WriteInt16(outputStream, 0);
            }

            PGUtil.WriteInt16(outputStream, (Int16)_resultFormatCodes.Length);

            foreach (short code in  _resultFormatCodes)
            {
                PGUtil.WriteInt16(outputStream, code);
            }
        }
    }
}
