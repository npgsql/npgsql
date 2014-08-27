//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    https://github.com/npgsql/Npgsql
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
//
//    NpgsqlConnectorRequest.cs
// ------------------------------------------------------------------
//    Project
//        Npgsql
//    Status
//        0.00.0000 - 03/01/2014 - Sunny Ahuwanya<sunny at ahuwanya.net> - created

using System;
using System.Threading;

namespace Npgsql
{
    /// <summary>
    /// Represents a request for a Connector
    /// </summary>
    internal class NpgsqlConnectorRequest
    {
        readonly NpgsqlConnection _connection; 

        NpgsqlConnector _connector; // Connector that was obtained for this request
        Exception _exEncountered; //Exception encountered while acquiring connector
        int _cancelled = 0; // 0 for false, -1 for true.

        public NpgsqlConnectorRequest(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Gets the connection associated with this request.
        /// </summary>
        internal NpgsqlConnection RequestConnection
        {
            get { return _connection; }
        }

        /// <summary>
        /// Gets or sets a connector for this request.
        /// Throws an exception if an exception was encountered for this request.
        /// </summary>
        internal NpgsqlConnector Connector
        {
            get {

                if (_connector == null)
                {
                    //Ensure we are reading the latest value.
                    _connector = Interlocked.CompareExchange(ref _connector, null, null);

                    if (_connector == null)
                    {
                        //Check if an exception was set.
                        _exEncountered = Interlocked.CompareExchange(ref _exEncountered, null, null);

                        if (_exEncountered != null)
                        {
                            throw _exEncountered;
                        }
                    }
                }

                return _connector; 
            }
            set {

                if (Connector != null)
                {
                    throw new InvalidOperationException("Connector can only be set once!");
                }

                //Set connector
                Interlocked.CompareExchange(ref _connector, value, null);
            }
        }

        /// <summary>
        /// Sets an exception encountered while the connector was sought.
        /// </summary>
        /// <param name="exception"></param>
        internal void SetException(Exception exception)
        {
            if (_exEncountered != null)
            {
                throw new InvalidOperationException("Exception can only be set once!");
            }

            //Set exception
            Interlocked.CompareExchange(ref _exEncountered, exception, null);
        }

        /// <summary>
        /// Indicates that this item has been cancelled. i.e. requestor doesn't care for this request to be serviced anymore
        /// </summary>
        internal bool IsCancelled
        {
            get
            {
                if (_cancelled == 0)
                {
                    //Ensure we are reading the latest value
                    _cancelled = Interlocked.CompareExchange(ref _cancelled, 0, 0);
                }

                return _cancelled == -1;
            }
            set
            {
                if (IsCancelled && value == true)
                {
                    throw new InvalidOperationException("IsCancelled can only be set true once!");
                }

                //Set _cancelled
                Interlocked.CompareExchange(ref _cancelled, value == true ? -1 : 0, 0);
            }
        }
    }
}
