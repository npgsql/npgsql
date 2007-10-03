// Npgsql.NpgsqlCopyInState.cs
//
// Author:
// 	Kalle Hallivuori <kato@iki.fi>
//
//	Copyright (C) 2007 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
//  INSERT BSD LICENCE HERE :)

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
            // TODO: ProcessBackendResponsesWithoutBlocking();
        }
        
        override public void SendCopyDone( NpgsqlConnector context )
        {
            Stream toServer = context.Stream;
            toServer.WriteByte( (byte)NpgsqlMessageTypes_Ver_3.CopyDone );
            PGUtil.WriteInt32( toServer, 4 ); // message without data
            toServer.Flush();
            ProcessBackendResponses(context);
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
        }
    }
}
