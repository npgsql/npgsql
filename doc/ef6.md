---
layout: doc
title: Entity Framework 6
---


**NOTE**: if you want to use Visual Studio Visual Designer, see
[VS integration](ddex.html) instead.

## Guid Support ##

Npgsql EF Migrations support uses `uuid_generate_v4()` function to generate guids.
In order to have access to this function, you have to install the extension uuid-ossp through the following command:

{% highlight sql %}
create extension "uuid-ossp";
{% endhighlight %}

If you don't have this extension installed, when you run Npgsql migrations you will get the following error message:

{% highlight C# %}
ERROR:  function uuid_generate_v4() does not exist
{% endhighlight %}

If the database is being created by Npgsql Migrations, you will need to
[run the `create extension` command in the `template1` database](http://stackoverflow.com/a/11584751).
This way, when the new database is created, the extension will be installed already.

## Samples ##

### ef_db_first_sample instruction

This sample shows how to use Npgsql with Entity Framework Database First development.

This sample was contributed by Kenji Uno: https://github.com/npgsql/Npgsql/pull/100#issuecomment-30314547

This sample uses Kenji's tool called EdmGen06. It is similar to MS.net edmgen with some improvements. 


Tested on:
- Windows 8.1 Pro with Media Center x64
- PostgreSQL 9.3.1, compiled by Visual C++ build 1600, 64-bit

### Create a database

With command line:

{% highlight sql %}
createdb ef_db_first_sample 
{% endhighlight %}

If you can invoke SQL commands, try:

{% highlight sql %}
CREATE DATABASE ef_db_first_sample
  WITH ENCODING='UTF8'
       TEMPLATE=template0
       LC_COLLATE='C'
       LC_CTYPE='C'
       CONNECTION LIMIT=-1;
{% endhighlight %}

### Add role "npgsql_tests" with pass "npgsql_tests"

Invoke SQL commands:

{% highlight sql %}
CREATE ROLE npgsql_tests LOGIN
  PASSWORD 'npgsql_tests'
  NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION;
{% endhighlight %}

### Create sample tables: Blog and Post

Invoke SQL commands:

{% highlight sql %}
CREATE TABLE "Blog"
(
  "BlogId" serial NOT NULL,
  "Name" character varying(255),
  CONSTRAINT "Blog_pkey" PRIMARY KEY ("BlogId")
);

CREATE TABLE "Post"
(
  "PostId" serial NOT NULL,
  "Title" character varying(255),
  "Content" character varying(8000),
  "BlogId" integer,
  CONSTRAINT "Post_pkey" PRIMARY KEY ("PostId"),
  CONSTRAINT "Post_BlogId_fkey" FOREIGN KEY ("BlogId")
      REFERENCES "Blog" ("BlogId") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
);
{% endhighlight %}

Grant permissions to role npgsql_tests:

{% highlight sql %}
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE "Blog" TO npgsql_tests;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE "Post" TO npgsql_tests;

GRANT SELECT, UPDATE ON TABLE "Blog_BlogId_seq" TO npgsql_tests;
GRANT SELECT, UPDATE ON TABLE "Post_PostId_seq" TO npgsql_tests;
{% endhighlight %}

### Generate an edmx file from your database

EdmGen06 is a partially compatible tool to Microsoft's EdmGen.

The latest binary releases will be available at https://github.com/kenjiuno/EdmGen06/wiki

Extract the files somewhere.

Launch command prompt.

Enter EFv4 directory.

Try next command.

~~~
EdmGen06 ^
  /ModelGen ^
  "Port=5432;Encoding=UTF-8;Server=127.0.0.1;Database=ef_db_first_sample;UserId=npgsql_tests;Password=npgsql_tests;Preload Reader=true;" ^
  "Npgsql" ^
  "Blogging" ^
  "public" ^
  "3.0"
~~~

You will see output like following lines:

~~~
EdmGen06 Information: 101 : ModelGen v3
EdmGen06 Information: 101 : Getting System.Data.Common.DbProviderFactory from 'Npgsql'
EdmGen06 Information: 101 : Npgsql.NpgsqlFactory, Npgsql, Version=2.1.0.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7
EdmGen06 Information: 101 : file:///H:/Dev/EdmGen06/test/EFv4/Npgsql.DLL
EdmGen06 Information: 101 : Ok
EdmGen06 Information: 101 : Connecting
EdmGen06 Information: 101 : Connected
EdmGen06 Information: 101 : Getting System.Data.Common.DbProviderServices from 'Npgsql'
EdmGen06 Information: 101 :  from IServiceProvider.GetService method
EdmGen06 Information: 101 : Npgsql.NpgsqlServices, Npgsql.EntityFrameworkLegacy, Version=2.1.0.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7
EdmGen06 Information: 101 : file:///H:/Dev/EdmGen06/test/EFv4/Npgsql.EntityFrameworkLegacy.DLL
EdmGen06 Information: 101 : Ok
EdmGen06 Information: 101 : Get ProviderManifestToken
EdmGen06 Information: 101 : Get ProviderManifest
EdmGen06 Information: 101 : Get StoreSchemaDefinition
EdmGen06 Information: 101 : Get StoreSchemaMapping
EdmGen06 Information: 101 : Write temporary ProviderManifest ssdl
EdmGen06 Information: 101 : Write temporary ProviderManifest msl
EdmGen06 Information: 101 : Checking ProviderManifest version.
EdmGen06 Information: 101 : ProviderManifest v1
EdmGen06 Information: 101 : Write temporary ProviderManifest csdl
EdmGen06 Information: 101 : Getting SchemaInformation
EdmGen06 Information: 101 : Ok
EdmGen06 Information: 101 : Table: public.Blog
EdmGen06 Information: 101 :  TableColumn: BlogId
EdmGen06 Information: 101 :  TableColumn: Name
EdmGen06 Information: 101 : Table: public.Post
EdmGen06 Information: 101 :  TableColumn: PostId
EdmGen06 Information: 101 :  TableColumn: Title
EdmGen06 Information: 101 :  TableColumn: Content
EdmGen06 Information: 101 :  TableColumn: BlogId
EdmGen06 Information: 101 : Constraint: Post_BlogId_fkey
~~~

### Blogging.App.config

Open "Blogging.App.config" generated by EdmGen06.

{% highlight xml %}
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <!--configSections has to be FIRST element!-->
  <configSections>
    <!--for EF6.0.x -->
    <!--you don't need this. your nuget will setup automatically-->
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
  </configSections>
  <system.data>
    <DbProviderFactories>
      <!--for EF4.x and EF6.0.x -->
      <!--you may need this. if you don't modify machine.config-->
      <remove invariant="Npgsql" />
      <add name="Npgsql - .Net Data Provider for PostgreSQL" invariant="Npgsql" description=".Net Data Provider for PostgreSQL" type="Npgsql.NpgsqlFactory, Npgsql, Version=2.1.0.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" />
    </DbProviderFactories>
  </system.data>
  <connectionStrings>
    <!--for EF4.x and EF6.0.x -->
    <add name="BloggingEntities" connectionString="metadata=Blogging.csdl|Blogging.ssdl|Blogging.msl;provider=Npgsql;provider connection string=&quot;Port=5432;Encoding=UTF-8;Server=127.0.0.1;Database=ef_db_first_sample;UserId=npgsql_tests;Password=npgsql_tests;Preload Reader=true;&quot;" providerName="System.Data.EntityClient" />
  </connectionStrings>
  <entityFramework>
    <providers>
      <!--for EF6.0.x -->
      <!--you need this. add it manually-->
      <provider invariantName="Npgsql" type="Npgsql.NpgsqlServices, Npgsql.EntityFrameworkLegacy, Version=2.1.0.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" />
    </providers>
  </entityFramework>
</configuration>

{% endhighlight %}

Take the element and its children: &lt;system.data&gt;
Take the element and its children: &lt;connectionStrings&gt;

### Create an application with VisualStudio2012ExpressForDesktop

Template: Console Application (C#)
Name: DBFirstNewDatabaseSample

### Edit your App.config

1. Add the elements: &lt;system.data&gt;
1. Add the elements: &lt;connectionStrings&gt;

Your App.config will look like:

{% highlight xml %}
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>
  <system.data>
    <DbProviderFactories>
      <!--for EF4.x and EF6.0.x -->
      <!--you may need this. if you don't modify machine.config-->
      <remove invariant="Npgsql" />
      <add name="Npgsql - .Net Data Provider for PostgreSQL" invariant="Npgsql" description=".Net Data Provider for PostgreSQL" type="Npgsql.NpgsqlFactory, Npgsql, Version=2.1.0.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" />
    </DbProviderFactories>
  </system.data>
  <connectionStrings>
    <!--for EF4.x and EF6.0.x -->
    <add name="BloggingEntities" connectionString="metadata=Blogging.csdl|Blogging.ssdl|Blogging.msl;provider=Npgsql;provider connection string=&quot;Port=5432;Encoding=UTF-8;Server=127.0.0.1;Database=ef_db_first_sample;UserId=npgsql_tests;Password=npgsql_tests;Preload Reader=true;&quot;" providerName="System.Data.EntityClient" />
  </connectionStrings>
</configuration>
{% endhighlight %}

### Add Blogging.edmx

Add "Blogging.edmx" generated by EdmGen06, to your project.

### Edit the property of Blogging.edmx

Custom tool: EntityModelCodeGenerator
Build action: EntityDeploy

### Copy Npgsql.dll and Npgsql.EntityFrameworkLegacy.dll

Copy "Npgsql.dll" and "Npgsql.EntityFrameworkLegacy.dll" files into your project folder.

Edit the property:
- Set "Copy to Output Directory" to "Copy if newer".

### Reading & Writing Data
Edit your Program.cs

{% highlight c# %}
using System;
using System.Linq;

namespace DBFirstNewDatabaseSample {
    class Program {
        static void Main(string[] args) {
            using (var db = new BloggingEntities()) {
                // Create and save a new Blog
                Console.Write("Enter a name for a new Blog: ");
                var name = Console.ReadLine();

                var blog = new Blog { Name = name };
                db.Blog.AddObject(blog);
                db.SaveChanges();

                // Display all Blogs from the database
                var query = from b in db.Blog
                            orderby b.Name
                            select b;

                Console.WriteLine("All blogs in the database:");
                foreach (var item in query) {
                    Console.WriteLine(item.Name);
                }

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
{% endhighlight %}

Launch, and enjoy it!

~~~
Enter a name for a new Blog: my first blog
All blogs in the database:
my first blog
Press any key to exit...
~~~

