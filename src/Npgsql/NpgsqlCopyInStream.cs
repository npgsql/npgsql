// Npgsql.NpgsqlCopyInStream.cs
//
// Author:
//     Kalle Hallivuori <kato@iki.fi>
//
//    Copyright (C) 2007 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
                NpgsqlConnector c = _context;
                _context = null;
                c.Mediator.CopyStream = null;
                c.CurrentState.SendCopyFail( _context, message != null ? message : "Cancel Copy" );
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
    }
}
