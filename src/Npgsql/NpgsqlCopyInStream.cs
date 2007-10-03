// Npgsql.NpgsqlCopyInStream.cs
//
// Author:
//     Kalle Hallivuori <kato@iki.fi>
//
//    Copyright (C) 2007 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
//  INSERT BSD LICENCE HERE :)

using System;
using System.IO;

namespace Npgsql
{
    class NpgsqlCopyInStream : Stream
    {
        private NpgsqlConnector _context;
        private long _bytesPassed = 0;
        
        private bool IsActive
        {
            get
            {
                return _context != null && _context.CurrentState is NpgsqlCopyInState;
            }
        }

        public NpgsqlCopyInStream(NpgsqlConnector context)
        {
            _context = context;
        }
    
        override public bool CanRead
        {
            get
            {
                return false;
            }
        }
    
        override public bool CanWrite
        {
            get
            {
                return true;
            }
        }
        
        override public bool CanSeek
        {
            get
            {
                return false;
            }
        }
    
        override public long Length
        {
            get
            {
                return _bytesPassed;
            }
        }
        
        override public long Position
        {
            get
            {
                return _bytesPassed;
            }
            set
            {
                throw new NotSupportedException("Tried to set Position of network stream " + this);
            }
        }
    
        override public void Close()
        {
            if( _context != null )
            {
                if(IsActive)
                {
                    _context.CurrentState.SendCopyDone( _context );
                }
                if( _context.Mediator.CopyStream == this )
                {
                    _context.Mediator.CopyStream = null;
                }
                _context = null;
            }
        }

        /// <summary>
        /// Withdraws an already started copy operation. The operation will fail with given error message.
        /// </summary>
        public void Cancel(string message)
        {
            if(IsActive)
            {
                _context.CurrentState.SendCopyFail( _context, message );
                _context.Mediator.CopyStream = null;
                _context = null;
            }
        }

        override public void Write(byte[] buf, int off, int len)
        {
            if(! IsActive)
                throw new ObjectDisposedException("Writing to closed " + this);
            _context.CurrentState.SendCopyData( _context, buf, off, len );
            _bytesPassed += len;
        }

        override public void Flush()
        {
            if(! IsActive)
                throw new ObjectDisposedException("Flushing closed " + this);
            _context.Stream.Flush();
        }

        override public int Read(byte[] buf, int off, int len)
        {
            throw new NotSupportedException("Tried to read non-readable " + this);
        }
    
        override public long Seek(long pos, SeekOrigin so)
        {
            throw new NotSupportedException("Tried to seek non-seekable " + this);
        }
    
        override public void SetLength(long len)
        {
            throw new NotSupportedException("Tried to set length of network stream " + this);
        }

        ///<summary>
        ///Like Close() but fails the copy operation instead of finishing succesfully. Causes an error message from server.
        ///</summary>
        public void Fail(String msg)
        {
            if(IsActive)
            {
                _context.CurrentState.SendCopyFail( _context, msg );
                _context = null;
            }
        }
    }
}
