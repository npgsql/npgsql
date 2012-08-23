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
    
    
    tests.ParametersGetName();
    tests.NoNameParameterAdd();
    tests.FunctionCallFromSelect();
    tests.ExecuteScalar();
    tests.FunctionCallReturnSingleValue();
    tests.FunctionCallReturnSingleValueWithPrepare();
    tests.FunctionCallWithParametersReturnSingleValue();
    tests.FunctionCallWithParametersPrepareReturnSingleValue();
    tests.FunctionCallReturnResultSet();
    tests.CursorStatement();
    tests.PreparedStatementNoParameters();
    tests.PreparedStatementWithParameters();
    
    
          
  }




  	public void ParametersGetName()
  	{
  		NpgsqlCommand command = new NpgsqlCommand();
  		
  		// Add parameters.
  		command.Parameters.Add(new NpgsqlParameter("Parameter1", DbType.Boolean));
  		command.Parameters.Add(new NpgsqlParameter("Parameter2", DbType.Int32));
  		command.Parameters.Add(new NpgsqlParameter("Parameter3", DbType.DateTime));
  		
  		
  		// Get by indexers.
  		
  		Console.WriteLine(command.Parameters["Parameter1"].ParameterName);
  		Console.WriteLine(command.Parameters["Parameter2"].ParameterName);
  		Console.WriteLine(command.Parameters["Parameter3"].ParameterName);
  		
  		Console.WriteLine(command.Parameters[0].ParameterName);
  		Console.WriteLine(command.Parameters[1].ParameterName);
  		Console.WriteLine(command.Parameters[2].ParameterName);
  		
  		
  		
  		
  	}
  	
  	
  	public void NoNameParameterAdd()
  	{
  	  try
  	  {
  	  
  		  NpgsqlCommand command = new NpgsqlCommand();
  		
  		  command.Parameters.Add(new NpgsqlParameter());
  		} 
  		catch( NpgsqlException e){}
  		
  		
  	}
  	
  	
  	public void FunctionCallFromSelect()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("select * from funcB()", _conn);
  		
  		NpgsqlDataReader reader = command.ExecuteReader();
  		
  		Console.WriteLine(reader == null);
  		
  		//Assertion.AssertNotNull(reader);
  		//reader.FieldCount
  		
  		_conn.Close();
  		
  	}
  	
  	
  	public void ExecuteScalar()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("select count(*) from tablea", _conn);
  		
  		Int64 result = (Int64)command.ExecuteScalar();
  		
  		Console.WriteLine(result);
  		//Assertion.AssertEquals(4, result);
  		//reader.FieldCount
  		
  		_conn.Close();
  	}
  
  	
  	public void FunctionCallReturnSingleValue()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("funcC()", _conn);
  		command.CommandType = CommandType.StoredProcedure;
  					
  		Int64 result = (Int64) command.ExecuteScalar();
  		
  		Console.WriteLine(result);
  		
  		//Assertion.AssertEquals(4, result);
  		//reader.FieldCount
  		_conn.Close();
  	}
  	
  	
  	
  	public void FunctionCallReturnSingleValueWithPrepare()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("funcC()", _conn);
  		command.CommandType = CommandType.StoredProcedure;
  		
  		command.Prepare();
  		Int64 result = (Int64) command.ExecuteScalar();
  		
  		Console.WriteLine(result);
  		
  		//Assertion.AssertEquals(4, result);
  		//reader.FieldCount
  		_conn.Close();
  	}
  	
  	
  	public void FunctionCallWithParametersReturnSingleValue()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("funcC(:a)", _conn);
  		command.CommandType = CommandType.StoredProcedure;
  		
  		command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
  					
  		command.Parameters[0].Value = 4;
  					
  		Int64 result = (Int64) command.ExecuteScalar();
  		
  		Console.WriteLine(result);
  		
  		//Assertion.AssertEquals(1, result);
  		
  		_conn.Close();
  	}
  	
  	
  	
  	public void FunctionCallWithParametersPrepareReturnSingleValue()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("funcC(:a)", _conn);
  		command.CommandType = CommandType.StoredProcedure;
  		
  		
  		command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
  		
  		command.Prepare();
  		
  		
  		command.Parameters[0].Value = 4;
  					
  		Int64 result = (Int64) command.ExecuteScalar();
  		
  		Console.WriteLine(result);
  		
  		//Assertion.AssertEquals(1, result);
  		
  		_conn.Close();
  	}
  	
  	
  	public void FunctionCallReturnResultSet()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("funcB()", _conn);
  		command.CommandType = CommandType.StoredProcedure;
  		
  								
  		NpgsqlDataReader dr = command.ExecuteReader();
  		
  		Console.WriteLine(dr == null);
  		
  		_conn.Close();
  	}
  	
  	
  	
  	public void CursorStatement()
  	{
  		
  		_conn.Open();
  		
  		Int32 i = 0;
  		
  		NpgsqlTransaction t = _conn.BeginTransaction();
  		
  		NpgsqlCommand command = new NpgsqlCommand("declare te cursor for select * from tablea;", _conn);
  		
  		command.ExecuteNonQuery();
  		
  		command.CommandText = "fetch forward 3 in te;";
  		
  		NpgsqlDataReader dr = command.ExecuteReader();
  		
  		
  		while (dr.Read())
  		{
  			i++;
  		}
  		
  		Console.WriteLine(i);
  		//Assertion.AssertEquals(3, i);
  		
  		
  		i = 0;
  		
  		command.CommandText = "fetch backward 1 in te;";
  		
  		NpgsqlDataReader dr2 = command.ExecuteReader();
  		
  		while (dr2.Read())
  		{
  			i++;
  		}
  		
  		Console.WriteLine(i);
  		//Assertion.AssertEquals(1, i);
  		
  		command.CommandText = "close te;";
  		
  		command.ExecuteNonQuery();
  		
  		t.Commit();
  		
  		
  		_conn.Close();
  	}
  	
  	
  	public void PreparedStatementNoParameters()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("select * from tablea;", _conn);
  		
  		command.Prepare();
  		
  		command.Prepare();
  		
  		NpgsqlDataReader dr = command.ExecuteReader();
  		
  		Console.WriteLine(dr == null);
  		
  		_conn.Close();
  	}
  	
  	
  	public void PreparedStatementWithParameters()
  	{
  		_conn.Open();
  		
  		NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_int4 = :a and field_int8 = :b;", _conn);
  		
  		command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
  		command.Parameters.Add(new NpgsqlParameter("b", DbType.Int64));
  		
  		command.Prepare();
  		
  		command.Parameters[0].Value = 3;
  		command.Parameters[1].Value = 5;
  		
  		NpgsqlDataReader dr = command.ExecuteReader();
  		
  		Console.WriteLine(dr == null);
  		
  		_conn.Close();
  		
  		
  	}
  	
  
}
