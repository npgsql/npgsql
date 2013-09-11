// created on 29/6/2003 at 13:28

// Npgsql.NpgsqlBind.cs
//
// Author:
//	Francisco Jr. (fxjrlists@yahoo.com.br)
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
			set { _resultFormatCodes = value; }
		}

		public Int16[] ParameterFormatCodes
		{
			get { return _parameterFormatCodes; }

			set { _parameterFormatCodes = value; }
		}

		public byte[][] ParameterValues
		{
			get { return _parameterValues; }

			set { _parameterValues = value; }
		}


        public override void WriteToStream(Stream outputStream)
        {
            Int32 messageLength = 4 + _bPortalName.Length + 1 +
                                  _bPreparedStatementName.Length + 1 + 2 + (_parameterFormatCodes.Length * 2) +
                                  2;

            // Get size of parameter values.
            Int32 i;

            if (_parameterValues != null)
            {
                for (i = 0; i < _parameterValues.Length; i++)
                {
                    messageLength += 4;
                    if (_parameterValues[i] != null)
                    {
                        messageLength += _parameterValues[i].Length;
                    }
                }
            }

            messageLength += 2 + (_resultFormatCodes.Length * 2);

            outputStream
                .WriteBytes((byte)FrontEndMessageCode.Bind)
                .WriteInt32(messageLength)
                .WriteBytesNullTerminated(_bPortalName)
                .WriteBytesNullTerminated(_bPreparedStatementName)
                .WriteInt16((Int16)_parameterFormatCodes.Length);

            for (i = 0 ; i < _parameterFormatCodes.Length ; i++)
            {
                PGUtil.WriteInt16(outputStream, _parameterFormatCodes[i]);
            }

            if (_parameterValues != null)
            {
                PGUtil.WriteInt16(outputStream, (Int16)_parameterValues.Length);

                for (i = 0 ; i < _parameterValues.Length ; i++)
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

            for (i = 0 ; i < _resultFormatCodes.Length ; i++)
            {
                PGUtil.WriteInt16(outputStream, _resultFormatCodes[i]);
            }
        }
    }
}
