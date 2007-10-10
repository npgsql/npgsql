// Npgsql.NpgsqlCopyInState.cs
//
// Author:
// 	Kalle Hallivuori <kato@iki.fi>
//
//	Copyright (C) 2007 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
//  Copyright (c) 2002-2007, The Npgsql Development Team
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
    internal sealed class NpgsqlCopyInState : NpgsqlState
    {
        private static NpgsqlCopyInState _instance = null;

        private readonly String CLASSNAME = "NpgsqlCopyInState";

        private NpgsqlCopyInState() : base()
        { }

        public static NpgsqlCopyInState Instance
        {
            get
            {
                if ( _instance == null )
                {
                    _instance = new NpgsqlCopyInState();
                }
                return _instance;
            }
        }

        override protected void StartCopy( NpgsqlConnector context, NpgsqlCopyHeader copyHeader )
        {
            Stream userFeed = context.Mediator.CopyStream;
            if( userFeed == null )
            {
                context.Mediator.CopyStream = new NpgsqlCopyInStream(context);
            }
            else
            {
                // copy all of user feed to server at once
                int bufsiz = context.Mediator.CopyBufferSize;
                byte[] buf = new byte[bufsiz];
                int len;
                while( (len = userFeed.Read( buf, 0, bufsiz )) > 0 )
                {
                    SendCopyData( context, buf, 0, len );
                }
                SendCopyDone(context);
            }
        }

        override public void SendCopyData( NpgsqlConnector context, byte[] buf, int off, int len)
        {
            Stream toServer = context.Stream;
            toServer.WriteByte( (byte)NpgsqlMessageTypes_Ver_3.CopyData );
            PGUtil.WriteInt32( toServer, len+4 );
            toServer.Write( buf, off, len );
        }
        
        override public void SendCopyDone( NpgsqlConnector context )
        {
            Stream toServer = context.Stream;
            toServer.WriteByte( (byte)NpgsqlMessageTypes_Ver_3.CopyDone );
            PGUtil.WriteInt32( toServer, 4 ); // message without data
            toServer.Flush();
            ProcessBackendResponses(context);
            context.CheckErrorsAndNotifications();
        }
        
        override public void SendCopyFail( NpgsqlConnector context, String message )
        {
            Stream toServer = context.Stream;
            toServer.WriteByte( (byte)NpgsqlMessageTypes_Ver_3.CopyFail );
            byte[] buf = context.Encoding.GetBytes( ( message == null ? "" : message ) + '\x00' );
            PGUtil.WriteInt32( toServer, 4 + buf.Length );
            toServer.Write( buf, 0, buf.Length );
            toServer.Flush();
            ProcessBackendResponses(context);
            context.CheckErrorsAndNotifications();
        }
    }
}
