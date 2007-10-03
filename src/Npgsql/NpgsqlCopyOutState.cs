// Npgsql.NpgsqlCopyOutState.cs
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
    internal sealed class NpgsqlCopyOutState : NpgsqlState
    {
        private static NpgsqlCopyOutState _instance = null;

        private readonly String CLASSNAME = "NpgsqlCopyOutState";

        private NpgsqlCopyOutState() : base()
        { }

        public static NpgsqlCopyOutState Instance
        {
            get
            {
                if ( _instance == null )
                {
                    _instance = new NpgsqlCopyOutState();
                }
                return _instance;
            }
        }

        override protected void StartCopy( NpgsqlConnector context, NpgsqlCopyHeader copyHeader )
        {
            Stream userFeed = context.Mediator.CopyStream;
            if( userFeed == null )
            {
                context.Mediator.CopyStream = new NpgsqlCopyOutStream(context);
            }
            else
            {
              byte[] buf;
              while( (buf=GetCopyData(context)) != null )
                  userFeed.Write( buf, 0, buf.Length );
              userFeed.Close();
            }
        }

        override public byte[] GetCopyData( NpgsqlConnector context )
        {
            ProcessBackendResponses_Ver_3(context); // polling in COPY would take seconds on Windows
            return context.Mediator.ReceivedCopyData;
        }
    }
}
