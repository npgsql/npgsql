//
// Npsql.cs
// Date: 2005.02.11
// author: Hiroshi Saito(z-saito@guitar.ocn.ne.jp)
// Description: It is used by PostgreSQL 8.1
//
using System;
using System.Data;
using Npgsql;

class Npsql {
	static string version = "0.5b";
	public static void supsql(NpgsqlCommand command)
	{
		if(String.Compare(command.CommandText,0,"\\l",0,2) ==0)
			command.CommandText = "SELECT d.datname as \"Name\",u.usename as \"Owner\",pg_catalog.pg_encoding_to_char(d.encoding) as \"Encoding\" FROM pg_catalog.pg_database d LEFT JOIN pg_catalog.pg_user u ON d.datdba = u.usesysid ORDER BY 1;";
		else
		if(String.Compare(command.CommandText,0,"\\u",0,2) ==0)
			command.CommandText = "SELECT r.rolname AS \"Role name\",CASE WHEN r.rolsuper THEN 'yes' ELSE 'no' END AS \"Superuser\",CASE WHEN r.rolcreaterole THEN 'yes' ELSE 'no' END AS \"Create role\",CASE WHEN r.rolcreatedb THEN 'yes' ELSE 'no' END AS \"Create DB\",CASE WHEN r.rolconnlimit < 0 THEN CAST('no limit' AS pg_catalog.text) ELSE CAST(r.rolconnlimit AS pg_catalog.text) END AS \"Connections\" FROM pg_catalog.pg_roles r ORDER BY 1;";
		else
		if(String.Compare(command.CommandText,0,"\\d",0,2) ==0)
		{
			if (command.CommandText.Length > 3)
			    command.CommandText = "SELECT a.attname AS \"Field\",t.typname AS \"Type\" FROM pg_class c, pg_attribute a, pg_type t WHERE c.relname = '" + command.CommandText.Substring(3,command.CommandText.Length-3) + "' AND a.attnum > 0 AND a.attrelid = c.oid AND a.atttypid = t.oid;";
			else
			    command.CommandText = "SELECT n.nspname as \"Schema\", c.relname as \"Name\",CASE c.relkind WHEN 'r' THEN 'table' WHEN 'v' THEN 'view' WHEN 'i' THEN 'index' WHEN 'S' THEN 'sequence' WHEN 's' THEN 'special' END as \"Type\", u.usename as \"Owner\" FROM pg_catalog.pg_class c LEFT JOIN pg_catalog.pg_user u ON u.usesysid = c.relowner LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace WHERE c.relkind IN ('r','v','S','') AND n.nspname NOT IN ('pg_catalog', 'pg_toast') AND pg_catalog.pg_table_is_visible(c.oid) ORDER BY 1,2;";
		}
	}
	public static void welcommsg()
	{
		Console.WriteLine("Welcome to Npsql {0} , the PostgreSQL mini terminal.",version);
		Console.WriteLine("Type:  \\l list all databases");
		Console.WriteLine("       \\d [NAME] describe table, index, sequence, or view");
		Console.WriteLine("       \\u list users");
		Console.WriteLine("       \\q to quit");
		Console.WriteLine("       \\? this help");

	}
	public static void helpmsg()
	{
		Console.WriteLine("usage:");
		Console.WriteLine("  Npsql [HOSTNAME[:PORT]] [[DBNAME] USERNAME[/PASSWORD]]\n");
		Console.WriteLine("  DBNAME    specify database name to connect to (default: template1)");
		Console.WriteLine("  USERNAME  user name to connect to (default: postgres)");
		Console.WriteLine("");
	}
        public static int Main(string[] args)
        {

		String url = "localhost";
		String dbn = "template1";
		String usr = "postgres";

		if (args.Length >= 1)
		{
			url = args[0].Replace(":",";PORT=");
		}
		
		if (args.Length == 2)
		{
			usr = args[1].Replace("/",";PWD=");
		}
		else
		if (args.Length == 3)
		{
			dbn = args[1];
			usr = args[2].Replace("/",";PWD=");
		}

		NpgsqlConnection cnDB;

                // Connect to database
                Console.WriteLine("Connecting to ... " + url);
				String constr = "DATABASE=" + dbn + ";SERVER=" + url + ";UID=" + usr + ";Encoding=UNICODE;";
                cnDB = new NpgsqlConnection(constr);

		try 
		{
			cnDB.Open();
		} 
		catch (NpgsqlException ex)
		{
			Console.WriteLine(ex.ToString());
			helpmsg();
			return 1;
		}

		// Get the PostgreSQL version number as proof
		try
		{
			NpgsqlCommand cmdVer = new NpgsqlCommand("SELECT version()", cnDB);
			Object ObjVer = cmdVer.ExecuteScalar();
			Console.WriteLine(ObjVer.ToString());
		}
		catch(NpgsqlException ex)
		{
			Console.WriteLine(ex.ToString());
			return 1;
		}

		welcommsg();

		//
		try
		{
		  do{
			NpgsqlCommand command = new NpgsqlCommand();
			Console.Write("\nNpsql>");
			command.CommandText = Console.ReadLine();
			if(String.Compare(command.CommandText,0,"\\q",0,2) ==0)
				break;
			if(String.Compare(command.CommandText,0,"\\?",0,2) ==0)
			{
				welcommsg();
				continue;
			}
			// command helper
			supsql(command);
			command.Connection = cnDB;
			NpgsqlDataReader dr;
			try
			{
				dr = command.ExecuteReader();
			}
			catch(NpgsqlException ex)
			{
				Console.WriteLine(ex.ToString());
				continue;
			}
			do
			{
				Int32 j,i;

				//  this is empty result check.
				if (!dr.HasRows)
					continue;

				j = dr.FieldCount;
				DataTable dt = dr.GetSchemaTable();
				DataRowCollection schemarows = dt.Rows;
				for (i = 0; i < j; i++)
				{
					Console.Write("{0} \t", schemarows[i][0]);
				}
				Console.WriteLine("\n============================================");
				while(dr.Read())
				{
					for (i = 0; i < j; i++)
						Console.Write("{0} \t", dr[i]);
					Console.WriteLine();
				}
                       } while(dr.NextResult());
                    } while(true);
		}
		catch(NpgsqlException ex)
		{
			Console.WriteLine(ex.ToString());
		}
		if (cnDB != null)
		{
			if (cnDB.State != ConnectionState.Closed)
			{
				try
				{
					cnDB.Close();
				}
				catch (NpgsqlException ex)
				{
				}
			}
		} 
		return 1;
	}
}
