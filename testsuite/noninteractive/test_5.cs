//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Data;
using Npgsql;

public class Test_Connection
{
  
  private NpgsqlConnection _conn = new NpgsqlConnection(NpgsqlTests.getConnectionString());
  
  public static void Main(String[] args)
  {
  
    Test_Connection tests = new Test_Connection();
    
    
    tests.GetBoolean();
    tests.GetChars();
    tests.GetInt32();
    tests.GetInt16();
    tests.GetDecimal();
    tests.GetDouble();
    tests.GetFloat();
    tests.GetString();
    tests.GetValueByName();
    
    
          
  }


  public void GetBoolean()
  {
	  _conn.Open();
		
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 4;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		Boolean result = dr.GetBoolean(4);
		
		Console.WriteLine(result);
		_conn.Close();
		
		
		
	}
  	
	
	public void GetChars()
	{
		_conn.Open();
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		Char[] result = new Char[6];
		
		
		Int64 a = dr.GetChars(1, 0, result, 0, 6);
		
		Console.WriteLine(result);
		_conn.Close();
		
		
		
	}
	
	
	public void GetInt32()
	{
		_conn.Open();
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 2;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		
		
		Int32 result = dr.GetInt32(2);
		
		Console.WriteLine(result);
		_conn.Close();
		
		
		
	}
	
	
	
	public void GetInt16()
	{
		_conn.Open();
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 2;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		
		Int16 result = dr.GetInt16(2);
		
		Console.WriteLine(result);
		_conn.Close();
		
		
		
	}
	
	
	public void GetDecimal()
	{
		_conn.Open();
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 2;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		
		Decimal result = dr.GetDecimal(2);
		
		Console.WriteLine(result);
		_conn.Close();
		
		
		
	}

	
	
	public void GetDouble()
	{
		_conn.Open();
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 2;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		
		Double result = dr.GetDouble(2);
		
		Console.WriteLine(result);
		_conn.Close();
		
		
	}
	
	
	
	public void GetFloat()
	{
		_conn.Open();
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 2;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		
		Single result = dr.GetFloat(2);
		
		Console.WriteLine(result);
		_conn.Close();
		
		
	}
	
	
	
	public void GetString()
	{
		_conn.Open();
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		
		String result = dr.GetString(1);
		
		Console.WriteLine(result);
		_conn.Close();
		
		
		
	}
	
	
	public void GetValueByName()
	{
		_conn.Open();
		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _conn);
		
		NpgsqlDataReader dr = command.ExecuteReader();
		
		dr.Read();
		
		String result = (String) dr["field_text"];
		
		Console.WriteLine(result);
		_conn.Close();
		
		
		
	}
}

