// Author
//  Kalle Hallivuori <kato@iki.fi>
//
//	Copyright (C) 2007 The Npgsql Development Team
//  npgsql-general@gborg.postgresql.org
//  http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
//  Copyright (c) 2002-2007, The Npgsql Development Team
//  
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//  
//      * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//      * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//      * Neither the name of the Npgsql Development Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


using System;
using System.Data;
using System.IO;
using Npgsql;

public class Test_copy
{
    static NpgsqlConnection conn;
    static NpgsqlCopyIn cin;
    static NpgsqlCopyOut cout;
    static CountStream cs;
    static long InLength = 0;
    static long InSum = 0; // this really is a SUM of the bytes! comparison depends on that.

    public static void Main(String[] args)
    {
        conn = new NpgsqlConnection( NpgsqlTests.getConnectionString() );

        conn.Open();

        // clear our test table
        new NpgsqlCommand("TRUNCATE copy1", conn).ExecuteNonQuery();

        CopyInFromStream();
        CopyInByWriting();
        CopyOutToStream();
        CopyOutByReading();

        FailCopyInFromStream();
        FailCopyInByWriting();
        FailCopyOutToStream();
        FailCopyOutByReading();

        // now test serialization into a table with multiple data types
        new NpgsqlCommand("TRUNCATE copy2", conn).ExecuteNonQuery();

        CopyInWithSerializer();

        // confirm that connection and server states stay valid through failures.
        new NpgsqlCommand("TRUNCATE copy1", conn).ExecuteNonQuery();

        conn.Close();
    }

    // Stream success tests

    static public void CopyInFromStream()
    {
        cs = new CountStream();
        cs.WrapStream = new FileStream("test_copy.cs", FileMode.Open, FileAccess.Read);
        cin = new NpgsqlCopyIn( new NpgsqlCommand("COPY copy1 FROM STDIN DELIMITER '\b'", conn), conn, cs );
        cin.Start();
        if(cin.IsActive)
        {
            throw new Exception("Copy from stream did not complete in single pass");
        }
        InLength += cs.BytesPassed;
        InSum += cs.CheckSum;
        cs.Close();
        Console.Out.WriteLine("Copy from stream ok");
    }

    static public void CopyInByWriting()
    {
        cs = new CountStream();
        cs.WrapStream = new FileStream("test_copy.cs", FileMode.Open, FileAccess.Read);
        cin = new NpgsqlCopyIn( "COPY copy1 FROM STDIN DELIMITER '\b'", conn );
        cin.Start();
        if(! cin.IsActive)
        {
            throw new Exception("Copy started inactive");
        }
        byte[] buf = new byte[8];
        int i;
        while( (i = cs.Read(buf,0,buf.Length)) > 0 )
        {
            cin.CopyStream.Write(buf, 0, i);
        }
        cin.End();
        InLength += cs.BytesPassed;
        InSum += cs.CheckSum;
        cs.Close();
        Console.Out.WriteLine("Copy from writing ok");
    }

    static public void CopyOutToStream()
    {
        cs = new CountStream();
        // cs.WrapStream = new FileStream("test_copy.out", FileMode.Create, FileAccess.Write);
        cout = new NpgsqlCopyOut( new NpgsqlCommand("COPY copy1 TO STDOUT", conn), conn, cs );
        cout.Start();
        if(cout.IsActive)
        {
            throw new Exception("Copy to stream did not complete in single pass");
        }
        Console.Out.WriteLine("Lengths of text written to and read in single pass from database differ by " + (InLength-cs.BytesPassed));
        Console.Out.WriteLine("Sums of characters written to and read in single pass from database differ by " + (InSum-cs.CheckSum));
    }

    static public void CopyOutByReading()
    {
        cs = new CountStream();
        cout = new NpgsqlCopyOut( "COPY copy1 TO STDOUT", conn );
        cout.Start();
        if(! cout.IsActive)
        {
            throw new Exception("Copy reading started inactive");
        }
        byte[] buf = new byte[9];
        int i;
        while( (i = cout.CopyStream.Read(buf, 0, buf.Length)) > 0 )
        {
            cs.Write(buf, 0, i);
        }
        cs.Close();
        cout.End();
        Console.Out.WriteLine("Lengths of text written to and read via stream from database differ by " + (InLength-cs.BytesPassed));
        Console.Out.WriteLine("Sums of characters written to and read via stream from database differ by " + (InSum-cs.CheckSum));
    }

    // Stream failure tests

    static public void FailCopyInFromStream()
    {
        cs = new CountStream();
        cs.FailAt = 2;
        cs.WrapStream = new FileStream("test_copy.cs", FileMode.Open, FileAccess.Read);
        cin = new NpgsqlCopyIn( new NpgsqlCommand("COPY copy1 FROM STDIN DELIMITER '\b'", conn), conn, cs );
        try
        {
            cin.Start();
        }
        catch(Exception e)
        {
            if( (""+e).Contains("Test Exception handling") )
            {
                Console.Out.WriteLine("Copy from stream failed as requested.");
                return;
            }
            else
            {
                Console.Out.WriteLine("Copy from stream failing failed: " + e);
                throw e;
            }
        }
        finally
        {
            cs.Close();
            cin.End(); // should do nothing
        }
        Console.Out.WriteLine("Copy from stream did not fail as requested");
    }

    static public void FailCopyInByWriting()
    {
        cs = new CountStream();
        cs.FailAt = 2;
        cs.WrapStream = new FileStream("test_copy.cs", FileMode.Open, FileAccess.Read);
        cin = new NpgsqlCopyIn( "COPY copy1 FROM STDIN", conn );
        cin.Start();
        if(! cin.IsActive)
        {
            throw new Exception("Copy started inactive");
        }
        byte[] buf = new byte[8];
        int i;
        try
        {
            while( (i = cs.Read(buf,0,buf.Length)) > 0 )
            {
                cin.CopyStream.Write(buf, 0, i);
            }
        }
        catch(Exception e)
        {
            if( (""+e).Contains("Test Exception handling") )
            {
                try
                {
                    cin.Cancel("Test whether copy in fails correctly");
                }
                catch(Exception e2)
                {
                    if( (""+e2).Contains("Test whether copy in fails correctly") )
                    {
                        Console.Out.WriteLine("Copy from writing failed as requested.");
                        return;
                    }
                    throw e2;
                }
                throw new Exception("CopyIn.Cancel() didn't throw up the expected exception");
            }
            throw e;
        }
        finally
        {
            cs.Close();
            cin.End(); // should do nothing
        }
        throw new Exception("Copy from writing did not fail as requested");
    }

    static public void FailCopyOutToStream()
    {
        cs = new CountStream();
        cs.FailAt = 2;
        try
        {
            cout = new NpgsqlCopyOut( new NpgsqlCommand("COPY copy1 TO STDOUT", conn), conn, cs );
            cout.Start();
        }
        catch(Exception e)
        {
            if( (""+e).Contains("Test Exception handling") )
            {
                Console.Out.WriteLine("Copy to stream failed as requested.");
                return;
            }
            throw e;
        }
        finally
        {
            cs.Close();
            cout.End(); // should silently discard rest of data
        }
        throw new Exception("Copy to stream did not fail as requested");
    }

    static public void FailCopyOutByReading()
    {
        cs = new CountStream();
        cs.FailAt = 2;
        cout = new NpgsqlCopyOut( "COPY copy1 TO STDOUT", conn );
        cout.Start();
        if(! cout.IsActive)
        {
            throw new Exception("Copy reading started inactive");
        }
        byte[] buf = new byte[9];
        int i;
        try
        {
            while( (i = cout.CopyStream.Read(buf, 0, buf.Length)) > 0 )
            {
                cs.Write(buf, 0, i);
            }
        }
        catch(Exception e)
        {
            if( (""+e).Contains("Test Exception handling") )
            {
                Console.Out.WriteLine("Copy to reading failed as requested.");
                return;
            }
            throw e;
        }
        finally
        {
            cs.Close();
            cout.End();
        }
        throw new Exception("Copy reading did not fail as requested");
    }

    // Serializer success test
  
    static public void CopyInWithSerializer()
    {
        NpgsqlCopySerializer sink = new NpgsqlCopySerializer( conn );
        String q = "COPY copy2(field_int4, field_int8, field_text, field_timestamp, field_bool) FROM STDIN";
        cin = new NpgsqlCopyIn( q, conn );
        cin.Start();
        if(! cin.IsActive)
        {
            throw new Exception("Copy started inactive");
        }
        sink.AddInt32(-13);
        sink.AddNull();
        sink.AddString("First row");
        sink.AddDateTime(new DateTime( 2020, 12, 22, 23, 33, 45, 765 ));
        sink.AddBool(true);
        sink.EndRow();
        sink.AddNull();
        sink.AddNull();
        sink.AddString("Second row");
        sink.Close();
        Console.Out.WriteLine("Copy through serializer ok");
    }


    // helper


    internal class CountStream : Stream
    {
        public int BytesPassed = 0;
        public long CheckSum = 0;
        public bool IsOpen = true;
        public bool Escape = false;
        public bool Unescape = false;
        public int FailAt = 0;
        public Stream WrapStream = null;

        override public bool CanRead
        {
            get
            {
                return true;
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
                return BytesPassed;
            }
        }
    
        override public long Position
        {
            get
            {
                return BytesPassed;
            }
            set
            {
                throw new NotSupportedException("Tried to set Position of comparison stream " + this);
            }
        }

        override public void Close()
        {
            if(IsOpen)
            {
                if(WrapStream != null)
                {
                    WrapStream.Close();
                }
                IsOpen = false;
            }
        }

        override public void Write(byte[] buf, int off, int len)
        {
            if(! IsOpen)
            {
                throw new ObjectDisposedException("Writing to closed " + this);
            }
            if(FailAt > 0 && --FailAt == 0)
            {
                // Console.Out.WriteLine("Write failing per test request");
                throw new Exception("Test Exception handling");
            }
            for(int i=0; i < len; i++)
            {
                if( buf[off+i] < 32 && buf[off+i] != '\n' )
                {
                    buf[off+i] = 32;
                }
            }
            if(WrapStream != null)
            {
                WrapStream.Write(buf, off, len);
            }
            Count(buf, off, len);
        }

        override public void Flush()
        {
            if(IsOpen)
            {
                if(WrapStream != null)
                {
                    WrapStream.Flush();
                }
            } else {
                throw new ObjectDisposedException("Flushing closed " + this);
            }
        }

        override public int Read(byte[] buf, int off, int len)
        {
            if(! IsOpen)
            {
                throw new ObjectDisposedException("Reading from closed " + this);
            }
            if(FailAt > 0 && --FailAt == 0)
            {
                // Console.Out.WriteLine("Read failing per test request");
                throw new Exception("Test Exception handling");
            }
            len = (WrapStream != null) ? WrapStream.Read( buf, off, len ) : 0;
            for(int i=0; i < len; i++)
            {
                if( buf[off+i] < 32 && buf[off+i] != '\n' )
                {
                    buf[off+i] = 32;
                }
            }
            Count(buf, off, len);
            return len;
        }

        override public long Seek(long pos, SeekOrigin so)
        {
            throw new NotSupportedException("Tried to seek non-seekable " + this);
        }

        override public void SetLength(long len)
        {
            throw new NotSupportedException("Tried to set length of comparison stream " + this);
        }
        
        private bool _skipNext = false;
        private void Count(byte[] buf, int off, int len)
        {
            for(int i = 0; i<len; i++)
            {
                if(_skipNext)
                {
                    _skipNext = false;
                } 
                else if( buf[off+i] < 32 || buf[off+i] == '\\' )
                { // skip special characters and escape sequences mangled by transfer
                    _skipNext = buf[off+i] == '\\';
                }
                else
                {
                    CheckSum += buf[off+i];
                    BytesPassed++;
                }
            }
        }
    }
}
