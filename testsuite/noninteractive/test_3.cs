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
    
    
    tests.ChangeDatabase();
    tests.ConnectionString();
    
          
  }
  
  
  public void ChangeDatabase()
  {
  	_conn.Open();
  	
  	_conn.ChangeDatabase("template1");
  	
  	NpgsqlCommand command = new NpgsqlCommand("select current_database()", _conn);
  	
  	String result = (String)command.ExecuteScalar();
  	
  	Console.WriteLine(result);
  	
  	//Assertion.AssertEquals("template1", result);
  		
  }
  
  public void ConnectionString()
  {
    try
    {
      _conn.ConnectionString = "User Id=user;Password=password;Database=database";
    }
    catch (ArgumentException e)
    {
      Console.WriteLine(e.Message);
      
    }
    
    try
    {
      _conn.ConnectionString = "Server=server;Password=password;Database=database";
    }
    catch (ArgumentException e)
    {
      Console.WriteLine(e.Message);
      
    }
    
    try
    {
      _conn.ConnectionString = "Server=server;User Id=user;Database=database";
    }
    catch (ArgumentException e)
    {
      Console.WriteLine(e.Message);
      
    }
    
    
  }
  
    
  
}
