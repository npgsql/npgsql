---
layout: doc
title: User Manual
#permalink: /doc/manual2.2/
---

*Note: This documentation covers the older 2.2 version of Npgsql,
if you're starting out it's recommended that you try the [newer 3.0 version](.).*

## 1. What is Npgsql?

Npgsql is a .Net Data Provider for the PostgreSQL Database Server.

It allows a .Net client application (Console, WinForms, ASP.NET, Web Services...) to send and receive data with a PostgreSQL server. It is actively developed based on the guidelines specified in the .Net documentation.

## 2. How to get and compile Npgsql

## 3. Npgsql Usage

This section explains Npgsql usage in a .Net application (Windows or ASP.NET). If you have experience developing data access applications using the Sql Server, OleDB or ODBC.NET providers, you will find that Npgsql is very similar, in most respects equally or more robust, and backed by an active community.

In order to use Npgsql, the PostgreSQL server must be listening to TCP/IP connections. TCP connections are enabled by default on 8.0 + servers. Previous versions should have postmaster started with the "-i" option. Check PostgreSQL Documentation for details: http://www.postgresql.org/docs/7.4/static/postmaster-start.html

Note: Npgsql is still under development. Only features currently supported will be demonstrated. As Npgsql matures, more functionality will become available.

### Adding required namespaces to your source file

First, in order to access Npgsql objects more easily (i.e. Intellisense in Visual Studio .Net), you need to instruct the compiler to use the Npgsql namespace. As you manipulate data retrieved by Npgsql, classes in System.Data will also be required. In C#, add this directive to the appropriate page or class:

{% highlight c# %}
using System.Data;
using Npgsql;
{% endhighlight %}

If you are using ASP.NET without code-behind files, you may need to add the following lines in top of your ASPX pages:

{% highlight c# %}
<%@ Assembly name="System.Data" %>
<%@ Assembly name="Npgsql" %>
{% endhighlight %}

### Establishing a connection

To establish a connection to a server located at IP 127.0.0.1, port 5432, as user "joe", with password "secret", on database "joedata", open NpgsqlConnection with the following connection string:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public class NpgsqlUserManual
{
  public static void Main(String[] args)
  {
    NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
    conn.Open();
    conn.Close();
  }
}
{% endhighlight %}

### Connection String parameters
When establishing a connection, NpgsqlConnection accepts many parameters which modify its behavior. Here is the list of current parameters you can tweak: 

- Server 
  * Address/Name of PostgreSQL Server    
- Port
  * Port to connect to
- Protocol 
  * Protocol version to use, instead of automatic; Integer 2 or 3
- Database
  * Database name. Defaults to user name if not specified
- User Id
  * User name
- Integrated Security
  * Set to use windows integrated security. Default=false
- Password
  * Password for clear text authentication
- SSL
  * True or False. Controls whether to attempt a secure connection. Default = False
- ApplicationName
  * The Application Name that will show against this connection in pg_stat_activity
- Pooling
  * True or False. Controls whether connection pooling is used. Default = True
- MinPoolSize
  * Min size of connection pool. Min pool size, when specified, will make NpgsqlConnection pre-allocate the specified number of connections with the server. Default: 1
- MaxPoolSize
  * Max size of connection pool. Pooled connections will be disposed of when returned to the pool if the pool contains more than this number of connections. Default: 20
- Encoding
  * Obsolete. Always returns the string "Unicode", and silently ignores attempts to set it.
- Timeout
  * Time to wait for connection open in seconds. Default is 15.
- CommandTimeout
  * Time to wait for command to finish execution before throw an exception. In seconds. Default is 20.
- Sslmode
  * Mode for ssl connection control. Can be one of the following:
  * Prefer
    * If it is possible to connect via SLL, SSL will be used.
  * Require
    * If an SSL connection cannot be established, an exception is thrown.
  * Allow
    * Not supported yet; connects without SSL.
  * Disable
    * No SSL connection is attempted.
  * The default value is "Disable".
- ConnectionLifeTime
  * Time to wait before closing unused connections in the pool, in seconds. Default is 15.
- SyncNotification
  * Specifies if Npgsql should use synchronous notifications
- SearchPath
  * Changes search path to specified and public schemas.
- Preload Reader
  * If set to "true" (the default is "false") this causes datareaders to be loaded in their entirety before ExecuteReader returns.

  * This results in less performance (especially in the case of very large recordsets, in which case the level of performance could be intolerable), but is left as an option to cover a particular potential backwards-compatibility issue with previous versions of Npgsql.

  * According to the ADO.NET documentation, while an IDataReader is open the IDbConnection used to obtain it is "busy" and cannot be used for any other operations (with a few documented exceptions to this rule). Npgsql enforces this rule and hence while an NpgsqlDataReader is open most other operations on the NpgsqlConnection used to obtain it will result in an InvalidOperationException (Npgsql relaxes the rule in allowing you to use a connection if an NpgsqlDataReader has been read to the end of it's resultset(s) even if it hasn't been closed, since at this point it is no longer using any resources from the connection).

  * Previously however, Npgsql allowed users to completely ignore this rule. This was entirely a side-effect of internal implementation issues, and strictly speaking has never been supported (since it always violates the ADO.NET specification) but that will be little comfort should you suddenly find previously working code is broken. Hence if you find a problem with this change you can use this connection-string option to move back to the previous behaviour.

  * If you do use it however, you should do so as a stop-gap before fixing the code in question for two reasons:

    * Performance and, particularly, scalability is much better without this option.
    * Such code will be likely to fail, should you at any point want to extend to support a different data provider.
- Use Extended Types
  * This option affects whether DataAdaptors expect to use the .NET System.DateTime type or the Npgsql date and time types like NpgsqlTimeStamp which has functionality and ranges beyond that of System.DateTime. Either option allows both the Npgsql and System types to be used, but if set to "true" DataAdaptors will expect to be passed the specific Npgsql type for the field in question, whereas if set to "false" they will expect System.DateTime.

  * This option is experimental and will hopefully its impact will be reduced or removed in later releases.

  * The default is "false".

- Compatibility
  * This version is intended as a simpler method of dealing with breaking changes to adding yet more and more connection string options. It takes a single value in the form of a version number (a.b[.c[.d]]). Changes that could break existing code will, when possible, copy the behaviour prior of version number. The first such version is 2.0.2.1, so a value of "2.0.2" would not have its new behaviour.
Version	Behaviour
  * 2.0.2	
    * GetOrdinal will return -1 if the field name is not found.
GetOrdinal is kana-width sensitive.
  * 2.0.2.1	
    * GetOrdinal will throw IndexOutOfRangeException if the field name is not found.
GetOrdinal is kana-width insensitive.

### Using NpgsqlCommand to add a row in a table

The previous example doesn't do anything useful. It merely connects to the database and disconnects. If there is an error, a NpgsqlException is thrown. Now, suppose you have a table called "table1" with two fields, "fielda" and "fieldb", both of type int. If you want to insert tuple (1, 1) in this table you can send the insert statement:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public static class NpgsqlUserManual
{
  public static void Main(String[] args)
  {
    NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
    conn.Open();
    
    NpgsqlCommand command = new NpgsqlCommand("insert into table1 values(1, 1)", conn);
    Int32 rowsaffected;
    
    try
    {
      rowsaffected = command.ExecuteNonQuery();
     
      Console.WriteLine("It was added {0} lines in table table1", rowsaffected);
    }
    
    finally
    {
      conn.Close();
    }
  }
}
{% endhighlight %}

ExecuteNonQuery() is ideally suited for insert and update queries because it returns an integer indicating the number of rows affected by the last operation.

### Getting a single result value using the NpgsqlCommand.ExecuteScalar() method

In some scenarios, you only need to retrieve a single value (scalar) from a function. Use the ExecuteScalar() method on a Command object :

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public static class NpgsqlUserManual
{
  public static void Main(String[] args)
  {
    NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
    conn.Open();
    
    NpgsqlCommand command = new NpgsqlCommand("select version()", conn);
    String serverversion;
    
    try
    {
      serverversion = (String)command.ExecuteScalar();
      Console.WriteLine("PostgreSQL server version: {0}", serverversion);
    }
    
    
    finally
    {
      conn.Close();
    }
  }
}
{% endhighlight %}

You may also use ExecuteScalar against queries that return a recordset, such as "select count(*) from table1". However, when calling a function that returns a set of one or more records, only the first column of the first row is returned (DataSet.Tables[0].Rows[0][0]). In general, any query that returns a single value should be called with Command.ExecuteScalar.

### Getting a full result set with NpgsqlCommand.ExecuteReader() method and NpgsqlDataReader

There are several ways to return recordsets with Npgsql. When you'd like to pass a SQL statement as command text and access the results with a memory-efficent DataReader, use the ExecuteReader() method of the NpgsqlCommand object:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public static class NpgsqlUserManual
{
  public static void Main(String[] args)
  {
    NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
    conn.Open();
    
    NpgsqlCommand command = new NpgsqlCommand("select * from tablea", conn);

    
    try
    {
	NpgsqlDataReader dr = command.ExecuteReader();
	while(dr.Read())
	{
  		for (i = 0; i < dr.FieldCount; i++)
  		{
  			Console.Write("{0} \t", dr[i]);
  		}
  		Console.WriteLine();
	}

    }
    
    finally
    {
      conn.Close();
    }
  }
}
{% endhighlight %}

Note that you can 'daisy chain' select statements in a command object's commandtext to retrieve more than one record set: "select * from tablea; select * from tableb"

### Using parameters in a query

Parameters let you dynamcially insert values into SQL queries at run-time. Generally speaking, parameter binding is the best way to build dynamic SQL statements in your client code. Other approaches, such as basic string concatenation, are less robust and can be vulerable to SQL injection attacks. To add parameters to your SQL query string, prefix the paramter name with ":". The example below uses a parameter named value1 (see ":value1").

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public static class NpgsqlUserManual
{
    public static void Main(String[] args)
    {
        using(NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;"))
        {
            conn.Open();

            // Declare the parameter in the query string
            using(NpgsqlCommand command = new NpgsqlCommand("select * from tablea where column1 = :value1", conn))
            {
        
                // Now add the parameter to the parameter collection of the command specifying its type.
                command.Parameters.Add(new NpgsqlParameter("value1", NpgsqlDbType.Integer));
                
                // Now, add a value to it and later execute the command as usual.
                command.Parameters[0].Value = 4;
    
                using(NpgsqlDataReader dr = command.ExecuteReader())
                {
                    while(dr.Read())
                    {
                        for (i = 0; i < dr.FieldCount; i++)
                        {
                            Console.Write("{0} \t", dr[i]);
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
{% endhighlight %}

You can also send a parameterized query to the server using NpgsqlParamenter and NpgsqlParamenterCollection objects.) This code assumes a table called "tablea" with at least one column named "column1" of type int4.

### Using prepared statements

The Prepare method lets you optimize the performance of frequently used queries. Prepare() basically "caches" the query plan so that it's used in subsequent calls. (Note that this feature is only available in server 7.3+ versions. If you call it in a server which doesn't support it, Npgsql will silently ignore it.) Simply call the Prepare() method of the NpgsqlCommand before query execution:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public static class NpgsqlUserManual
{
    public static void Main(String[] args)
    {
        using(NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;"))
        {
            conn.Open();

            // Declare the parameter in the query string
            using(NpgsqlCommand command = new NpgsqlCommand("select * from tablea where column1 = :column1", conn))
            {
                // Now add the parameter to the parameter collection of the command specifying its type.
                command.Parameters.Add(new NpgsqlParameter("column1", NpgsqlDbType.Integer);

                // Now, prepare the statement.
                command.Prepare();

                // Now, add a value to it and later execute the command as usual.
                command.Parameters[0].Value = 4;

                using(NpgsqlDataReader dr = command.ExecuteReader())
                {
                    while(dr.Read())
                    {
                        for (i = 0; i < dr.FieldCount; i++)
                        {
                            Console.Write("{0} \t", dr[i]);
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
{% endhighlight %}

This code assumes a table called "tablea" with at least one column named "column1" of type int4.

### Function calling

To call a function, set the CommandType property of the NpgsqlCommand object to CommandType.StoredProcedure and pass the name of the function you want to call as the query string (CommandText property).

{% highlight c# %}
using System;
using System.Data;
using Npgsql;


// This example uses a function called funcC() with the following definition:
// create function funcC() returns int8 as '
// select count(*) from tablea;
// ' language 'sql';

// Note that the return type of select count(*) changed from int4 to int8 in 7.3+ versions. To use this function
// in a 7.2 server, change the return type from int8 to int4.

public static class NpgsqlUserManual
{
  public static void Main(String[] args)
  {
    NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
    conn.Open();
    
      
    try
    {
        NpgsqlCommand command = new NpgsqlCommand("funcC", conn);
        command.CommandType = CommandType.StoredProcedure;
  					
        Object result = command.ExecuteScalar();
  		
        Console.WriteLine(result);
    }
    
    finally
    {
      conn.Close();
    }
  }
}
{% endhighlight %}

Adding parameters to a PostgreSQL function is similar to our previous examples. However, when specifying the CommandText string, you can exclude parameter names. Use only the function name:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;


// This example uses a function called funcC with the following definition:
// create function funcC(int4) returns int8 as '
// select count(*) from tablea where field_int4 = $1;
// ' language 'sql';

public static class NpgsqlUserManual
{
  public static void Main(String[] args)
  {
    NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
    conn.Open();
    
      
    try
    {
        NpgsqlCommand command = new NpgsqlCommand("funcC", conn);
        command.CommandType = CommandType.StoredProcedure;
        
 		command.Parameters.Add(new NpgsqlParameter());
            
		command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Integer;
		command.Parameters[0].Value = 4;
		
        Object result = command.ExecuteScalar();
  		
        Console.WriteLine(result);


    }
    
    finally
    {
      conn.Close();
    }
  }
}
{% endhighlight %}

This code assumes a table called "tablea" with at least one field called "field_int4" of type int4.

### Getting full results in a DataSet object: Using refcursors

Refcursors are one of the most powerful ways to build functions in Postgres that return large result sets to the client. Using refcursors, a single function can return the results of multiple queries to the client in a single round-trip. Most Npgsql developers will learn that refcursors are quite easy to use once you grasp the basic syntax.

This sample returns two result sets from a function using refcursors. With Npgsql's solid refcursor support, you can get many result sets without having to worry about the internal workings of the refcursor in Postgres.

Consider the following refcursor-based function:

{% highlight sql %}
CREATE OR REPLACE FUNCTION testrefcursor(int4) RETURNS SETOF refcursor AS

'DECLARE 
  ref1 refcursor;
  ref2 refcursor;
  ref3 refcursor;
BEGIN

OPEN ref1 FOR 
 SELECT * FROM table1;
RETURN NEXT ref1;

OPEN ref2 FOR 
 SELECT * FROM table2;
RETURN next ref2;

OPEN ref3 FOR EXECUTE 
 'SELECT * FROM table3 WHERE keyfield = ' || $1;
RETURN next ref3;

RETURN;
END;'
LANGUAGE plpgsql;
{% endhighlight %}

This function returns the full results of three select statements. Notice that the last select statement is dynamically created on the server.

Now, to call these function and retrieve the data using a DataReader, you should use the following code:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;

public class c
{
    public static void Main(String[] args)
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Initial Catalog=eeeeee;User id=npgsql_tests;password=npgsql_tests;");
        conn.Open();

        NpgsqlTransaction t = conn.BeginTransaction();          
        NpgsqlCommand command = new NpgsqlCommand("testrefcursor", conn);
        command.CommandType = CommandType.StoredProcedure;   
        NpgsqlDataReader dr = command.ExecuteReader();
    
        while(dr.Read())
            Console.WriteLine(dr.GetValue(0));

        dr.NextResult();
        while(dr.Read())
            Console.WriteLine(dr.GetValue(0));
        dr.Close();
        t.Commit();
        conn.Close();
    }
}
{% endhighlight %}

Alternatively, you can retrieve the results into a DataSet object:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;

public class c
{
    public static void Main(String[] args)
    {
        DataSet myDS;
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Initial Catalog=eeeeee;User id=npgsql_tests;password=npgsql_tests;");
        conn.Open();

        NpgsqlTransaction t = conn.BeginTransaction();          
        NpgsqlCommand command = new NpgsqlCommand("testrefcursor", conn);
        command.CommandType = CommandType.StoredProcedure;
        
        con.Open();
        NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
        da.Fill(myDS);
 
        t.Commit();
        conn.Close();
    }
}
{% endhighlight %}

That's it!. One last thing worth noting is that *you have to use a transaction* in order for this to work. This is necessary to prevent cursors returned by refcursor function from closing after the implicity transaction is finished (just after you do the function call).

If you have parameters in your function, *assign only the function name* to the CommandText property and add parameters to the NpgsqlCommand.Parameters collection as usual. Npgsql will take care of binding your parameters correctly.

### Using output parameters in a query

Output parameters can be used with Npgsql. Note that Npgsql "simulates" output parameter by parsing the first result set from the execution of a query and translating it to output parameters value. This can be done in two ways: mapped or not. A mapped parsing tries to match the column name returned by resultset into a parameter with the same name. If a match is found, only the output parameters which has a match will be updated. If a map is not found, the output parameters are updated based on the order they were added to command parameters collection. This mapping is automatic. When parsing resultset, Npgsql tries to find a match. *Both Output and InputOutput parameter directions are supported*.

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public static class NpgsqlUserManual
{
    public static void Main(String[] args)
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
        conn.Open();
        
        // Send a query to backend.
        NpgsqlCommand command = new NpgsqlCommand("select * from tablea where column1 = 2", conn);
        
        // Now declare an output parameter to receive the first column of the tablea.
        NpgsqlParameter firstColumn = new NpgsqlParameter("firstcolumn", NpgsqlDbType.Integer);
        firstColumn.Direction = ParameterDirection.Output;
        command.Parameters.Add(firstColumn);
            
        try
        {
            command.ExecuteNonQuery();   
            // Now, the firstcolumn parameter will have the value of the first column of the resultset.
            Console.WriteLine(firstColumn.Value);
        }        
        finally
        {
            conn.Close();
        }
    }
}
{% endhighlight %}

### Working with .NET Datasets

Npgsql lets you propogate changes to a .NET DataSet object back to the database. The example below demonstrates the insertion of a record into a DataSet, followed by a call to update the associated database:

This method expects the following table in the backend: create table tableb(field_int2 int2, field_timestamp timestamp, field_numeric numeric);
{% highlight c# %}
void AddWithDataSet(NpgsqlConnection conn)
{	
	conn.Open();			
	DataSet ds = new DataSet();

	NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb", conn);	
	da.InsertCommand = new NpgsqlCommand("insert into tableb(field_int2, field_timestamp, field_numeric) " + 
							" values (:a, :b, :c)", conn);
	da.InsertCommand.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Smallint));
	da.InsertCommand.Parameters.Add(new NpgsqlParameter("b", NpgsqlDbType.Timestamp));
	da.InsertCommand.Parameters.Add(new NpgsqlParameter("c", NpgsqlDbType.Numeric));
	da.InsertCommand.Parameters[0].Direction = ParameterDirection.Input;
	da.InsertCommand.Parameters[1].Direction = ParameterDirection.Input;
	da.InsertCommand.Parameters[2].Direction = ParameterDirection.Input;
	da.InsertCommand.Parameters[0].SourceColumn = "field_int2";
	da.InsertCommand.Parameters[1].SourceColumn = "field_timestamp";
	da.InsertCommand.Parameters[2].SourceColumn = "field_numeric";
	
	da.Fill(ds);
	
	DataTable dt = ds.Tables[0];
	DataRow dr = dt.NewRow();
	dr["field_int2"] = 4;
	dr["field_timestamp"] = new DateTime(2003, 03, 03, 14, 0, 0);
	dr["field_numeric"] = 7.3M;
			
	dt.Rows.Add(dr);
	DataSet ds2 = ds.GetChanges();
	da.Update(ds2);
	ds.Merge(ds2);
	ds.AcceptChanges();
}
{% endhighlight %}

### Working with strongly typed datasets

This example demonstrates the use of a strongly typed dataset generated by XSD. To start, we need an XSD file specifing the appropiate schema. You can generate this file by hand, or you can use an XSD tool to generate it for you. In order to let NpgsqlDataAdapter generate XSD, you need to suppy it with an XML file; the XML file allows the inference of an XML schema.

{% highlight c# %}
public void GenerateXmlFromDataSet(NpgsqlConnection conn)
{
	conn.Open();
	var da = new NpgsqlDataAdapter("select * from tablea", conn);
	var ds = new DataSet();		
	da.Fill(ds);
	ds.WriteXml("StrongDataSetFeed.xml");
}
{% endhighlight %}

The example code results in a file which looks similar to:

{% highlight xml %}
<?xml version="1.0" standalone="yes"?>
<NewDataSet>
  <Table>
    <field_serial>1</field_serial>
    <field_text>Random text</field_text>
  </Table>
  <Table>
    <field_serial>2</field_serial>
    <field_int4>4</field_int4>
  </Table>
  <Table>
    <field_serial>3</field_serial>
    <field_int8>8</field_int8>
  </Table>
  <Table>
    <field_serial>4</field_serial>
    <field_bool>true</field_bool>
  </Table>
  <Table>
    <field_serial>5</field_serial>
    <field_text>Text with ' single quote</field_text>
  </Table>
</NewDataSet>
{% endhighlight %}

The following command uses the file to generate XSD:

xsd StrongDataSetFeed.xml

XSD will produce an XML schema in which all types are specified as string. As a consequence, we need to change the XSD to specify the correct types, resulting in an XSD file similar to:

{% highlight xml %}
<?xml version="1.0" encoding="utf-8"?>
<xs:schema id="NewDataSet" xmlns="" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
  <xs:element name="NewDataSet" msdata:IsDataSet="true" msdata:Locale="pt-BR">
    <xs:complexType>
      <xs:choice maxOccurs="unbounded">
        <xs:element name="Table">
          <xs:complexType>
            <xs:sequence>
              <xs:element name="field_serial" type="xs:int" minOccurs="0" />
              <xs:element name="field_text" type="xs:string" minOccurs="0" />
              <xs:element name="field_int4" type="xs:int" minOccurs="0" />
              <xs:element name="field_int8" type="xs:long" minOccurs="0" />
              <xs:element name="field_bool" type="xs:boolean" minOccurs="0" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:choice>
    </xs:complexType>
  </xs:element>
</xs:schema>
{% endhighlight %}

Given the above file, the following command generates a strongly typed dataset:

xsd StrongDataSetFeed.xsd /dataset

This command generates a file that compiles into an assembly for the strongly typed dataset. It's used in the example below:

{% highlight c# %}
using System;
using Npgsql;

public class t
{
	public static void Main(String[] args)
	{
		NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
		conn.Open();
		
		NpgsqlDataAdapter da = new NpgsqlDataAdapter("Select * from tablea", conn);
		NewDataSet n = new NewDataSet();
		da.Fill(n);

		foreach (NewDataSet._TableRow tr in n._Table) {
			Console.WriteLine(tr.field_serial);
		}
	}
}
{% endhighlight %}

### Working with binary data and bytea datatype

This sample takes a filename as an argument, inserts its contents into a table called "tableByteA". The table contains a field named "field_bytea" of type bytea and a field named "field_serial" of type serial. Next, it retrieves the field contents and writes a new file with the suffix "database".

table schema: create table tableBytea (field_serial serial, field_bytea bytea)

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using System.IO;

public class t
{
	public static void Main(String[] args)
	{
		NpgsqlConnection conn = new NpgsqlConnection("server=localhost;user id=npgsql_tests;password=npgsql_tests");
		conn.Open();

		FileStream fs = new FileStream(args[0], FileMode.Open, FileAccess.Read);
		BinaryReader br = new BinaryReader(new BufferedStream(fs));
		Byte[] bytes = br.ReadBytes((Int32)fs.Length);
		Console.WriteLine(fs.Length);

		br.Close();
		fs.Close();
				
		NpgsqlCommand command = new NpgsqlCommand("insert into tableBytea(field_bytea) values(:bytesData)", conn);
		NpgsqlParameter param = new NpgsqlParameter(":bytesData", NpgsqlDbType.Bytea);
		param.Value = bytes;
		command.Parameters.Add(param);
		command.ExecuteNonQuery();
		command = new NpgsqlCommand("select field_bytea from tableBytea where field_serial = (select max(select field_serial) from tableBytea);", conn);

		Byte[] result = (Byte[])command.ExecuteScalar();
		fs = new FileStream(args[0] + "database", FileMode.Create, FileAccess.Write);
		BinaryWriter bw = new BinaryWriter(new BufferedStream(fs));
		bw.Write(result);

		bw.Flush();
		fs.Close();
		bw.Close();
		conn.Close();
	}
}
{% endhighlight %}

### Working with large object support

This sample is nearly identical to the bytea code above. It stores the retrieved file in Postgresql, and then later removes it. As with the bytea sample, it writes a file with a "database" suffix.

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using System.IO;

public class c
{
    public static void Main(String[] args)
    {
        NpgsqlConnection newconn = new NpgsqlConnection("server=localhost;user id=npgsql_tests;password=npgsql_tests");

        newcon.Open();
        NpgsqlTransaction t = newcon.BeginTransaction();
        LargeObjectManager lbm = new LargeObjectManager(newcon);

        int noid = lbm.Create(LargeObjectManager.READWRITE);
        LargeObject lo =  lbm.Open(noid,LargeObjectManager.READWRITE);

        FileStream fs = File.OpenRead(args[0]);

        byte[] buf = new byte[fs.Length];
        fs.Read(buf,0,(int)fs.Length);

        lo.Write(buf);
        lo.Close();
        t.Commit();
        
        
        t = newcon.BeginTransaction();
        
        lo =  lbm.Open(noid,LargeObjectManager.READWRITE);
        
        FileStream fsout = File.OpenWrite(args[0] + "database");
        
        buf = lo.Read(lo.Size());
        
        fsout.Write(buf, 0, (int)lo.Size());
        fsout.Flush();
        fsout.Close();
        lo.Close();
        t.Commit();
        
        
        DeleteLargeObject(noid);
        
        Console.WriteLine("noid: {0}", noid);
        newcon.Close();
    }
    
    public static void DeleteLargeObject(Int32 noid)
    {
        NpgsqlConnection conn = new NpgsqlConnection("server=localhost;user id=npgsql_tests;password=npgsql_tests");

        newcon.Open();
        NpgsqlTransaction t = newcon.BeginTransaction();
        LargeObjectManager lbm = new LargeObjectManager(newcon);
        lbm.Delete(noid);
        
        t.Commit();
        
        newcon.Close();

    }
}
{% endhighlight %}

Another example, contributed by Mirek (mirek at mascort dot com dot pl), uses large object support to get an image from the database and display it in a form on the client.

{% highlight c# %}
using System;
using Npgsql;
using NpgsqlTypes;
using System.Drawing;
using System.IO;

//metod whos take picture oid  from database
public int takeOID(int id)
{
    //it's a metod whos connect  to database and return picture oid
    
    BazySQL pir = new BazySQL(Login.DaneUzera[8]);
    string pytanko = String.Format("select rysunek from k_rysunki where idtowaru = " + idtowaru.ToString());
    string[] wartosci = pir.OddajSelectArray(pytanko);
    int liczba = int.Parse(wartosci[0].ToString());
    return liczba;
}

//take a picture from database and convert to Image type
public Image pobierzRysunek(int idtowaru)
{
    NpgsqlConnection Polacz = new NpgsqlConnection();
    Polacz.ConnectionString = Login.DaneUzera[8].ToString();  //its metod whos return connection string
    Polacz.Open();
    NpgsqlTransaction t = Polacz.BeginTransaction();
    LargeObjectManager lbm = new LargeObjectManager(Polacz);
    LargeObject lo = lbm.Open(takeOID(idtowaru),LargeObjectManager.READWRITE); //take picture oid from metod takeOID
    byte[] buf = new byte[lo.Size()];
    buf = lo.Read(lo.Size());
    MemoryStream ms = new MemoryStream();
    ms.Write(buf,0,lo.Size());
    lo.Close();
    t.Commit();
    Polacz.Close();
    Polacz.Dispose();
    Image zdjecie = Image.FromStream(ms);
    return zdjecie;
}

//next I just use this metod
pictureBox1.Image = Image pobierzRysunek(1);
{% endhighlight %}

### Retrieving last inserted id on a table with serial values

This example was contributed by Josh Cooley when answering a [user question](http://pgfoundry.org/forum/forum.php?thread_id=943&forum_id=519) on Npgsql Forums. This code assumes you have the following table and function in your database:

{% highlight sql %}
create table test_seq (field_serial serial, test_text text);

CREATE OR REPLACE FUNCTION ins_seq("varchar")
        RETURNS test_seq AS
        'insert into test_seq (test_text) values ($1);
        select * from test_seq where test_text = $1'
        LANGUAGE 'sql' VOLATILE;
{% endhighlight %}

And this is the code:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;

public class c
{
    public static void Main(String[] args)
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;");

        using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("select * from test_seq", conn))
        {
            DataTable table = new DataTable();
            adapter.Fill(table);
            adapter.InsertCommand = new NpgsqlCommand("ins_seq", adapter.SelectCommand.Connection);
            adapter.InsertCommand.Parameters.Add("foo", NpgsqlTypes.NpgsqlDbType.Varchar, 100, "test_text");
            adapter.InsertCommand.CommandType = CommandType.StoredProcedure;

            DataRow row = table.NewRow();
            row["test_text"] = "asdfqwert";
            table.Rows.Add(row);
            adapter.Update(table);

            foreach (DataRow rowItem in table.Rows)
                Console.WriteLine("key {0}, value {1}", rowItem[0], rowItem[1]);

            Console.ReadLine();
        }
    }
}
{% endhighlight %}

### Cancelling a command in progress

Npgsql is able to ask the server to cancel commands in progress. To do this, call the NpgsqlCommand's Cancel method. Note that another thread must handle the request as the main thread will be blocked waiting for command to finish. Also, the main thread will raise an exception as a result of user cancellation. (The error code is 57014.) See the following code which demonstrates the technique:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using System.Threading;

public class c
{
    // This method expects the following table in the backend:
    //
    /*      CREATE OR REPLACE FUNCTION funcwaits() returns integer as
    '
    declare t integer;
    begin
    
    t := 0;
    
    while t < 1000000 loop
    t := t + 1;
    end loop;
    
    return t;
    end;
    '
    */

    static NpgsqlConnection conn = null;
    static NpgsqlCommand command = null;

    public static void Main(String[] args)
    {
        //NpgsqlEventLog.Level = LogLevel.Debug;
        //NpgsqlEventLog.LogName = "NpgsqlTests.LogFile";
        //NpgsqlEventLog.EchoMessages = true;

        try
        {
            conn = new NpgsqlConnection("Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;");
            conn.Open();

            var d = new NpgsqlCommand();
            var t = new Thread(new ThreadStart(CancelRequest));

            command = new NpgsqlCommand("select * from funcwaits()", conn);

            Console.WriteLine("Cancelling command...");
            t.Start();

            Console.WriteLine(command.ExecuteScalar());
            conn.Close();
        }
        catch (NpgsqlException e)
        {
            if (e.Code == "57014")
                Console.WriteLine("Command was cancelled");
        }
    }

    public static void CancelRequest()
    {
        command.Cancel();
        Console.WriteLine("command cancelled");
    }
}
{% endhighlight %}

### Working with Notifications

Npgsql allows user to receive events based on notifications sent by a server. There are two ways to receive notications with Npgsql: asynchronously or synchronously. Synchronous notification is only supported by Npgsql 1.0 and above.

#### Asynchronous Notifications

This is the default notification mechanism used in Npgsql. It is called asynchronous because Npgsql doesn't receive a notification upon execution of the event which generated it on the server. Npgsql receives the notification on the next instance of client interaction with the server. This interaction actually occurs when Npgsql sends a subsequent command to the server- which might consist of a few seconds to many hours later. With this in mind, most users will need to actively poll the server in order to recieve notifications in a timely matter. One approach involves polling via empty commands such as ";"

#### Synchronous Notifications

Starting with Npgsql 1.0, there is support for synchronous notifications. When working in this mode, Npgsql is able to receive a notificaton upon its instantiation and deliver it to client. All this is done without any additional interaction between the client and server (as described above).

*Important notice*: When using Synchronous notification, you can't execute commands inside your notification handler function. If you do so, you will hang Npgsql as the thread which handles the notification is the same which handles Npgsql communication with backend. If you want to use any commands, please, create another connection and use it instead. This is not the best solution but we are studying better ways to do that instead of needing another connection.

The code to receive the notification is the same for both modes:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using System.Threading;

public class c
{
    public static void Main(String[] args)
    {
        var conn = new NpgsqlConnection("Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;");
        conn.Open();
    
        NpgsqlCommand command = new NpgsqlCommand("listen notifytest;", conn);
        command.ExecuteNonQuery();
        
        conn.Notification += new NotificationEventHandler(NotificationSupportHelper);
        
        command = new NpgsqlCommand("notify notifytest;", _conn);
        command.ExecuteNonQuery();

        Console.ReadLine();  // To prevent program termination before notification is handled. 
    }
    
    private void NotificationSupportHelper(Object sender, NpgsqlNotificationEventArgs args)
    {
        // process notification here. 
    }
}
{% endhighlight %}

This code registers to listen for a notification and raises the notification. It will be delivered to the NotificationSupportHelper method.

### Fast bulk data copy into a table

Batched inserts can be time consuming with large amounts of data. PostgreSQL provides an alternative, much faster method of importing raw data. Its syntax and input format options are already explained in [PostgreSQL COPY documentation](http://www.postgresql.org/docs/8.2/interactive/sql-copy.html). To copy data from client-side you need to use the FROM STDIN option.

When feeding straight to COPY IN operation, *you have to provide data using the same encoding as the server uses*.

The simplest method is to provide a readable file handle to the CopyIn operation constructor. Upon start, the copy in operation will read whole contents of given stream and push them to the server. (Refer to COPY statement documentation for different input formats!)

1. See to it that you set SyncNotification=true in your database connection string. This is to catch any anomaly reports during import to prevent deadlock between client and server network buffers.
2.  Create NpgsqCopyIn object with a stream providing the data to input into database
3. Call Start() to initiate copy operation. The operation is completed immediately.
4. If Start() throws an exception, call NpgsqlCopyIn.Cancel() to cancel an ongoing operation and clear connection back to Ready For Query state. *Otherwise your connection may stay in copy mode, unusable for anything else*.

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public class CopyInExample
{

    public static void Main(String[] args)
    {

        conn = new NpgsqlConnection("Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;SyncNotification=true;");
        conn.Open();

        NpgsqlCommand command = new NpgsqlCommand("COPY myCopyTestTable FROM STDIN", conn);
        NpgsqlCopyIn cin = new NpgsqlCopyIn( command, conn, Console.OpenStandardInput() ); // expecting input in server encoding!
        try
        {
            cin.Start();
        }
        catch(Exception e)
        {
            try
            {
                cin.Cancel("Undo copy");
            }
            catch(NpgsqlException e2)
            {
                // we should get an error in response to our cancel request:
                if( ! (""+e2).Contains("Undo copy") )
                {
                    throw new Exception("Failed to cancel copy: " + e2 + " upon failure: " + e);
                }
            }
            throw e;
        }
    }
}
{% endhighlight %}

If you wish to provide the data from inside your application, you can use a normal writable stream:

1. See to it that you set SyncNotification=true in your database connection string. This is to catch any anomaly reports during import to prevent deadlock between client and server network buffers.
2. Create NpgsqCopyIn object without specifying a stream
3. Call Start() to initiate copy operation
4. Write your data in correct format and encoding into NpgsqlCopyIn.CopyStream
5. *During the operation the connection can not be used for anything else.*
6. Call CopyStream.Close() or NpgsqlCopyIn.End() to complete writing
7. To cancel an ongoing operation and clear connection back to Ready For Query state call NpgsqlCopyIn.Cancel().
8. Upon failure call NpgsqlCopyIn.Cancel() to cancel an ongoing operation and clear connection back to Ready For Query state. *Otherwise your connection may stay in copy mode, unusable for anything else.*

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public class CopyInExample
{

    public static void Main(String[] args)
    {

        conn = new NpgsqlConnection("Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;SyncNotification=true;");
        conn.Open();

        NpgsqlCommand command = new NpgsqlCommand("COPY myCopyTestTable FROM STDIN", conn);
        NpgsqlCopyIn cin = new NpgsqlCopyIn( command, conn );
        
        Stream inStream = Console.OpenStandardInput();
        Encoding inEncoding = System.Text.Encoding.ASCII;
        Encoding serverEncoding = System.Text.Encoding.BigEndianUnicode; // example assumption

        try
        {
            cin.Start();
            Stream copyInStream = cin.CopyStream;
            byte[] buf = new byte[9];
            int i;
            while( (i = inStream.Read(buf,0,buf.Length)) > 0 )
            {
                buf = System.Text.Convert( inEncoding, serverEncoding, buf, 0, i );
                copyInStream.Write( buf, 0, i );
            }
            copyInStream.Close(); // or cin.End(), if you wish
        }
        catch(Exception e)
        {
            try
            {
                cin.Cancel("Undo copy"); // Sends CopyFail to server
            }
            catch(Exception e2)
            {
                // we should get an error in response to our cancel request:
                if( ! (""+e2).Contains("Undo copy") )
                {
                    throw new Exception("Failed to cancel copy: " + e2 + " upon failure: " + e);
                }
            }
            throw e;
        }
    }
}
{% endhighlight %}

### Fast bulk data copy from a table or select

Even trivial selections of large data sets can become time consuming when network is the bottleneck. PostgreSQL provides an alternative, much faster method of exporting raw data. Its syntax and input format options are already explained in [PostgreSQL COPY documentation](http://www.postgresql.org/docs/8.2/interactive/sql-copy.html). To copy data to client-side you need to use the TO STDOUT option.

COPY OUT provides data *in server-side encoding*.

The simplest method is to provide a writable stream to the CopyOut operation constructor. Upon start, the operation will then write everything coming down from server right into that sink. (Refer to COPY statement documentation for different output formats!)

1. Create NpgsqCopyOut object with a stream for writing the output received from database
2. Call Start() to initiate copy operation. All requested data is written to specified stream immediately.
3. An ongoing operation may be cancelled by calling CopyStream.Close() or NpgsqlCopyIn.End()
4. Upon failure your connection becomes *unusable unless you cancel the copy operation*.
5. If Start() throws an exception, cancel the ongoing operation. *Otherwise your connection may stay in copy mode, unusable for anything else.*

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public class CopyOutExample
{

    public static void Main(String[] args)
    {

        conn = new NpgsqlConnection("Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;");
        conn.Open();

        NpgsqlCommand command = new NpgsqlCommand("COPY myCopyTestTable TO STDOUT", conn);
        NpgsqlCopyOut cout = new NpgsqlCopyOut( command, conn, Console.OpenStandardOutput() );
        try
        {
            cout.Start();
        }
        catch(Exception e)
        {
            try
            {
                cout.End(); // return connection to Ready for Query state
            }
            catch(Exception e2)
            {
                throw new Exception("Failed to revive from copy: " + e2 + " upon failure: " + e);
            }
            throw e;
        }
    }
}
{% endhighlight %}

You can read COPY OUT data normally from a stream:

1. Create NpgsqCopyOut object without specifying a stream
2. Call Start() to initiate copy operation
3. Read data in server-side encoding from NpgsqlCopyOut.CopyStream or row by row from NpgsqlCopyOut.Read
4. *During the operation the connection may not be used for anything else.*
5. All data has been received when no more comes out (CopyStream.Read(...) returns zero; NpgsqlCopyOut.Read a null pointer)
6. The operation completes automatically upon end
7. An ongoing operation may be cancelled by calling CopyStream.Close() or NpgsqlCopyIn.End()
8. Upon failure cancel the ongoing operation. *Otherwise your connection may stay in copy mode, unusable for anything else.*

{% highlight c# %}
using System;
using System.Data;
using Npgsql;

public class CopyOutExample
{

    public static void Main(String[] args)
    {
        conn = new NpgsqlConnection("Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;");
        conn.Open();

        NpgsqlCommand command = new NpgsqlCommand("COPY myCopyTestTable TO STDOUT", conn);
        NpgsqlCopyOut cout = new NpgsqlCopyOut( command, conn );

        Stream outStream = Console.OpenStandardOutput();
        Encoding serverEncoding = System.Text.Encoding.BigEndianUnicode; // example assumption
        Encoding outEncoding = System.Text.Encoding.ASCII;

        try
        {
            cout.Start();
            Stream copyOutStream = cout.CopyStream;
            byte[] buf = cout.Read; // complete first row
            Console.Out.Write(buf,0,buf.Length);
            int i;
            while( (i = copyOutStream.Read(buf,0,buf.Length)) > 0 )
            {
                buf = System.Text.Convert( serverEncoding, outEncoding, buf, 0, i );
                Console.Out.Write( buf, 0, i );
            }
            copyOutStream.Close(); // or cout.End(), if you wish
        }
        catch(Exception e)
        {
            try
            {
                cout.End(); // return connection to Ready for Query state
            }
            catch(Exception e2)
            {
                throw new Exception("Failed to revive from copy: " + e2 + " upon failure: " + e);
            }
            throw e;
        }
    }
}
{% endhighlight %}

### System.Transactions Support

Thanks Josh Cooley, Npgsql has added initial support for System.Transactions. This code is still in very early stage, so if you have any problems with it, please let us know so we can fix it as soon as possible.

In order to use it, you have to put the following in your connection string:

Enlist=true

False is currently the default, but we will likely make enlist=true the default once System.Transactions support stabilizes.

Here is a sample code which uses System.Transactions support:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using System.Transactions;

public class TransactionExample
{

    public static void Main(String[] args)
    {

            string connectionString = "Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;Enlist=true";
            using (TransactionScope tx = new TransactionScope())
            {
                using (NpgsqlConnection connection = new
NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (NpgsqlCommand command = new
NpgsqlCommand("insert into tablea (cola) values ('b')", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (NpgsqlConnection connection2 = new
NpgsqlConnection(connectionString))
                    {
                        connection2.Open();
                        using (NpgsqlCommand command = new
NpgsqlCommand("insert into tablea (colb) values ('c')", connection2))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                tx.Complete();
            }
    }
}
{% endhighlight %}

### Working with search paths

Npgsql allows you to modify the search path when connecting to a database. In order to do that, just specify it in your connection string with the syntax: searchpath='blablabla,blabla,blabla'. *Note that by specifying a search path in connection string, you may want to add the public schema as it will not be added automatically*.

### Working with Arrays

In order to use Npgsql array support, you may specify your parameter dbtype as an OR'ed operation. Anything that implements IEnumerable<T> where T is a type already supported by npgsql will be treated the same as T[], anything that implements IEnumerable<U>  where U implements IEnumerable<T> will be treated the same as T[,] (but cause an error if it's a "jagged" array, as postgres doesn't support them) and so on. For example, List<ICollection<short>> will be treated as a 2-dimensional array of 16bit-integers. For example, to use an array of Int32 you will use something like that:

{% highlight c# %}
command.Parameters.Add(new NpgsqlParameter("arrayParam", NpgsqlDbType.Array | NpgsqlDbType.Int32));
{% endhighlight %}

Or you can specify directly the value of parameter to be an array:

{% highlight c# %}
Int32 a = new Int32[2];
a[0] = 4;
a[1] = 2;
command.Parameters.Add(new NpgsqlParameter("@parameter")).Value = a;
{% endhighlight %}

Here is a complete example:

{% highlight c# %}
using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;

public class c
{
        public static void Main(String[] args)
        {
                //NpgsqlEventLog.Level = LogLevel.Debug;
                //NpgsqlEventLog.LogName = "NpgsqlTests.LogFile";
                //NpgsqlEventLog.EchoMessages = true;

                NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;");
                conn.Open();

                NpgsqlCommand d = new NpgsqlCommand();

                Int32[] a = new Int32[2];

                a[0] = 4;
                a[1] = 2;



                NpgsqlCommand command = new NpgsqlCommand("select :arrayParam", conn);

                command.Parameters.Add(new NpgsqlParameter("arrayParam", NpgsqlDbType.Array | NpgsqlDbType.Integer));

                command.Parameters[0].Value = a;

                Console.WriteLine(command.ExecuteScalar());

                conn.Close();
        }
}
{% endhighlight %}

This is what postgresql logs:

{% highlight text %}
LOG:  connection received: host=127.0.0.1 port=37356
DEBUG:  forked new backend, pid=10616 socket=6
LOG:  connection authorized: user=npgsql_tests database=npgsql_tests
LOG:  statement: SELECT oid, typname FROM pg_type WHERE typname IN ('oidvector', '_oidvector', 'unknown', '_unknown', 'refcursor', '_refcursor', 'char', '_char', 'bpchar', '_bpchar', 'varchar', '_varchar', 'text', '_text', 'name', '_name', 'bytea', '_bytea', 'bit', '_bit', 'bool', '_bool', 'int2', '_int2', 'int4', '_int4', 'int8', '_int8', 'oid', '_oid', 'float4', '_float4', 'float8', '_float8', 'numeric', '_numeric', 'inet', '_inet', 'money', '_money', 'date', '_date', 'time', '_time', 'timetz', '_timetz', 'timestamp', '_timestamp', 'timestamptz', '_timestamptz', 'point', '_point', 'lseg', '_lseg', 'path', '_path', 'box', '_box', 'circle', '_circle', 'polygon', '_polygon', 'uuid', '_uuid', 'xml', '_xml')

LOG:  statement: select array['4','2']::int4[]

LOG:  disconnection: session time: 0:00:00.342 user=npgsql_tests database=npgsql_tests host=127.0.0.1 port=37356
DEBUG:  server process (PID 10616) exited with exit code 0
{% endhighlight %}

And this is what Npgsql shows in console:

System.Int32[]

### Using Npgsql Logging support

Sometimes it's necessary to trace Npgsql's behaviour to track errors. Npgsql can log messages to a specified file, to the console, or to both.

There are three levels of logging: 
* None
* Normal
* Debug

The following NpgsqlEventLog static properties may also be specified:

* Level - Can be one of the LogLevel enum values: None, Normal, Debug.
* LogName - Full path of the file where to log into.
* EchoMessages - Log to the console.

The example below shows you how to log data to the console and to a file using level "Debug":

{% highlight c# %}
using System.Data;
using Npgsql;

public static class NpgsqlUserManual
{
  public static void Main(String[] args)
  {
    // Enable logging.
    NpgsqlEventLog.Level = LogLevel.Debug;
    NpgsqlEventLog.LogName = "NpgsqlTests.LogFile";
    NpgsqlEventLog.EchoMessages = true;
	  
    NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;");
    conn.Open();
    conn.Close();
  }
}
{% endhighlight %}

Running this code gives the following output:

{% highlight text %}
Set NpgsqlEventLog.EchoMessages = True
Entering NpgsqlConnection.NpgsqlConnection()
Entering NpgsqlConnection.ParseConnectionString()
Connection string option: DATABASE = joedata
Connection string option: SERVER = 127.0.0.1
Connection string option: USER ID = joe
Connection string option: PASSWORD = secret
Entering NpgsqlConnection.Open()
Connected to: 127.0.0.1:5432
Entering NpgsqlConnection.WritestartupPacket()
Entering NpgsqlStartupPacket.NpgsqlStartupPacket()
Entering NpgsqlStartupPacket.WriteToStream()
Entering PGUtil.WriteLimString()
Entering PGUtil.WriteLimString()
Entering PGUtil.WriteLimString()
Entering PGUtil.WriteLimString()
Entering PGUtil.WriteLimString()
Entering NpgsqlConnection.HandleStartupPacketResponse()
AuthenticationRequest message from Server
Server requested cleartext password authentication.
Entering NpgsqlPasswordPacket.NpgsqlPasswordPacket()
Entering NpgsqlPasswordPacket.WriteToStream()
Entering PGUtil.WriteString()
Listening for next message
AuthenticationRequest message from Server
Listening for next message
BackendKeyData message from Server
Entering NpgsqlBackEndKeyData.ReadFromStream()
Got ProcessID. Value: 3116
Got SecretKey. Value: -132883070
Listening for next message
ReadyForQuery message from Server
Listening for next message
Connection completed
Entering NpgsqlConnection.Close()
{% endhighlight %}

I used the "Debug" level to show that a lot of information can be obtained. Of course, the "Normal" level is less verbose. (This data was written to file NpgsqlTests.LogFile.)

### Npgsql design time support - VS.Net support

Npgsql 0.6 and higher provide initial design time support. This means that you can drag and drop a NpgsqlConnection in the Forms Designer of Visual Studio .NET (just like with SqlConnections or OleDbConnections).
In addition, a dialog is available for easily editing and validating the ConnectionString.

To do so you must:

1. Install Npgsql.dll into the GAC
2. Add a new Registry-Key below 'HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\AssemblyFolders' and set its default value to the path of your Npgsql.dll
3. Open Visual Studio .NET
4. Right-click the "Data" tab in the toolbox
5. Click "Add/Remove Element"
6. On the .Net tab, select NpgsqlConnection

For VS.Net 2005 you have to "Add a registry key under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v2.0.50727\AssemblyFoldersEx with whatever name you like ( I named mine PostgreSQL ) with a default value of the directory ( trailing backslash ) where the assembly resides. " Thanks Edward Diener for tip.

As result you will have an icon named NpgsqlConnection in the "Data" tab of the toolbox.

### ConnectionPool considerations

Npgsql will clear all connections from the pool whenever it finds any problems with a connection. This will allow easy recovery from any instability problems which might occur. Although this strategy may not have the best performance implications, it will ensure that the pool remains stay consistent when problems arise. Two methods to clear the pools are available through NpgsqlConnection: ClearPool and ClearAllPools. You can use them to clear the pool manually.

### Using Npgsql with ProviderFactory

Npgsql can be used with Provider Factory pattern which allows you to write code which is independent of database you are using.

In order to do that, you have to use the following configuration:

{% highlight xml %}
<?xml version="1.0" encoding="iso-8859-1" ?>

<configuration>
  <system.data>
    <DbProviderFactories>
      <add name="Npgsql Data Provider" invariant="Npgsql" support="FF" description=".Net Framework Data Provider for Postgresql Server" type="Npgsql.NpgsqlFactory, Npgsql, Version=2.0.1.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" />
    </DbProviderFactories>
  </system.data>
</configuration>
{% endhighlight %}

This configuration can be put inside the global machine.config file or in the application specific .config file.

After that, you can write code like the following:

{% highlight c# %}
using System;
using System.Data;
using System.Data.Common;

public class c
{
    public static void Main(String[] args)
    {
       DbProviderFactory factory = DbProviderFactories.GetFactory("Npgsql");       
       DbConnection conn = factory.CreateConnection();
       conn.ConnectionString = "Server=127.0.0.1;User id=npgsql_tests;password=npgsql_tests;";
       conn.Open();
       conn.Close();
    }
}
{% endhighlight %}

### Supported data types

Npgsql supports the following data types:

Postgresql Type | NpgsqlDbType | System.DbType Enum | .Net System Type
----------------|--------------|--------------------|-----------------
int8            | Bigint       | Int64              | Int64
bool            | Boolean      | Boolean            | Boolean
Box, Circle, Line, LSeg, Path, Point, Polygon	Box, Circle, Line, LSeg, Path, Point, Polygon | Object | Object
bytea           | Bytea        | Binary             | Byte[]
date            | Date         | Date               | DateTime, NpgsqlDate
float8          | Double       | Double             | Double
int4            | Integer      | Int32              | Int32
money           | Money        | Decimal            | Decimal
numeric         | Numeric      | Decimal            | Decimal
float4          | Real         | Single             | Single
int2            | Smallint     | Int16              | Int16
text            | Text         | String             | String
time            | Time         | Time               | DateTime, NpgsqlTime
timetz          | Time         | Time               | DateTime, NpgsqlTimeTZ
timestamp       | Timestamp    | DateTime           | DateTime, NpgsqlTimestamp
timestamptz     | TimestampTZ  | DateTime           | DateTime, NpgsqlTimestampTZ
interval        | Interval     | Object             | TimeSpan, NpgsqlInterval
varchar         | Varchar      | String             | String
inet            | Inet         | Object             | NpgsqlInet, IPAddress (there is an implicity cast operator to convert NpgsqlInet objects into IPAddress if you need to use IPAddress and have only NpgsqlInet)
bit             | Bit          | Boolean            | Boolean, Int32 (If you use an Int32 value, odd values will be translated to bit 1 and even values to bit 0)
uuid            | Uuid         | Guid               | Guid
array           | Array        | Object             | Array In order to explicitly use array type, specify NpgsqlDbType as an 'OR'ed type: NpgsqlDbType.Array | NpgsqlDbType.Integer for an array of Int32 for example.
