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

public class test_executenonquery
{
  public static void Main(String[] args)
  {
    NpgsqlConnection conn = null;
    
    try
		{
			conn = new NpgsqlConnection(NpgsqlTests.getConnectionString());
			conn.Open();
			Console.WriteLine("Connection completed");
			
			// Check whether the insert statement works
			NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values ('Text from Npgsql');", conn);
			Int32 num_rows = command.ExecuteNonQuery();
			Console.WriteLine("{0} rows were added!", num_rows);
			
			// Check whether the update statement works
			command.CommandText = "update tablea set field_text='Updated Text from Npgsql' where field_text='Text from Npgsql';";
			num_rows = command.ExecuteNonQuery();
			Console.WriteLine("{0} rows were updated!", num_rows);

			// Check whether the delete statement works
			command.CommandText = "delete from tablea where field_text = 'Updated Text from Npgsql'";
			num_rows = command.ExecuteNonQuery();
			Console.WriteLine("{0} rows were deleted!", num_rows);

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
