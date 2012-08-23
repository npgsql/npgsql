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


public class test_preparedstatements
{
  public static void Main(String[] args)
  {

	  NpgsqlConnection conn = null;
		try
		{
			conn = new NpgsqlConnection(NpgsqlTests.getConnectionString());
			
			conn.Open();
			Console.WriteLine("Connection completed");
			
			NpgsqlCommand command = new NpgsqlCommand();
			command.CommandText = "select * from tablea where field_int4 = :a;";
			command.Connection = conn;
			command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
			
			command.Prepare();
			command.Parameters[0].Value = 4;
			
			NpgsqlDataReader dr = command.ExecuteReader();
			
			Int32 j;
			
			do
			{
			  j = dr.FieldCount;
			  Console.WriteLine(j);
			  
			  /*DataTable dt = dr.GetSchemaTable();
			  DataRowCollection schemarows = dt.Rows;
			  
			  
			  
			  for (i = 0; i < j; i++)
			  {
			    Console.Write("{0} \t", schemarows[i][0]);
			   
			  }
			  */
			  Int32 i;
			  
  			Console.WriteLine();
  			Console.WriteLine("============================================");
  			  			
			  while(dr.Read())
			  {
  			  for (i = 0; i < j; i++)
  			  {
  			    Console.Write("{0} \t", dr[i] == DBNull.Value ? "null" : dr[i]);
  			   
  			  }
  			  Console.WriteLine();
  			  
  			}
			  
			} while(dr.NextResult());
						
			dr.Close();			
			
		}
		catch(NpgsqlException e)
		{
			Console.WriteLine(e.ToString());
		}
		finally
		{
			
			if (conn != null)
				conn.Close();
		}
	}
}

