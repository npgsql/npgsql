---
layout: doc
title: Visual Studio Support (DDEX)
---

## VS2013Pro+NpgsqlDdexProvider+EF6 how to

(by @kenjiuno)

Reference: [#213](https://github.com/npgsql/Npgsql/pull/213#issuecomment-46892619)

Prerequisites:

- *Visual Studio 2013 Professional* or greater edition
- *Microsoft Visual Studio 2013 Update 2* or later installed. Update4 is available: https://www.microsoft.com/en-us/download/details.aspx?id=44921
- *PostgreSQL server*. Tested on *PostgreSQL 9.3.4 (win-x64)*

### Install assemblies into GAC

1. Grab the latest setup program (.exe-file) from https://github.com/npgsql/npgsql/releases and run it.
2. Select all components to install.

### Prepare new project for testing
1. Launch Visual Studio 2013.
2. [FILE]→[New]→[Project...]
3. [Console Application]
4. Name is [testef] for example.

### Install EntityFramework 6, Npgsql and Npgsql.EntityFramework from NuGet
1. Right click project [testef]
2. [Managet NuGet Packages...]
3. Type "EntityFramework" at [Search Online (Ctrl+E)]
4. Install "EntityFramework". Version is 6.1.1 for now.
5. Do the same for Npgsql and Npgsql.EntityFramework.

Notice: The versions of Npgsql in your project and the one installed in the GAC **must be the same**. The GAC version is used by visual studio and the EntityFramework Wizard and tools. If, for some reason, you need to install a version which isn't the latest one from NuGet, you need to use the following command in the NuGet Package Manager Console:

`Install-Package Npgsql -Version <version>` where `<version>` is the version you want to install. A list of versions available can be found in the NuGet Npgsql page: https://www.nuget.org/packages/Npgsql/

### Add EF6 provider into App.config

1. Open [App.config]
2. Add provider-element into providers-element.

{% highlight xml %}
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  ...
  <entityFramework>
    ...
    <providers>
      <provider invariantName="Npgsql" type="Npgsql.NpgsqlServices, Npgsql.EntityFramework" />
	  ...
    </providers>
  </entityFramework>
  ...
</configuration>
{% endhighlight %}

If you want to be able to run your code on machines that have not installed Npgsql with the installer (or manually added Npgsql and Npgsql.EntityFramework to the GAC and added a Npgsql DbProviderFactory to machine.config) you also need the following:

{% highlight xml %}
<system.data>
  <DbProviderFactories>
    ...
    <remove invariant="Npgsql"/>
    <add name="Npgsql Data Provider" 
         invariant="Npgsql" 
         description=".Net Data Provider for PostgreSQL" 
         type="Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" 
         support="FF" />
  </DbProviderFactories>
</system.data>`
{% endhighlight %}

### Build once

1. Build your project.

### New ADO.NET Entity Data Model

1. Right click project [testef]
2. [Add]→[New Item...]
3. [ADO.NET Entity Data Model]
4. Name is [Model1] for example.
5. Click [Add]
6. [EF Designer from database] at Choose Model Contents.  
![ef1](https://cloud.githubusercontent.com/assets/5955540/3362619/bc30d222-fb0b-11e3-91d9-feddd811a164.png)
7. [New Connection] at Choose Your Data Connection.  
![ef2](https://cloud.githubusercontent.com/assets/5955540/3362638/dcba3574-fb0b-11e3-855d-a946a5bd48b1.png)
8. [PostgreSQL Database] at Change Data Source.  
![ef3](https://cloud.githubusercontent.com/assets/5955540/3362786/74ff7b5e-fb0d-11e3-87cb-462a5f28d0e7.png)
9. Fill properties in Connection Properties. It is easy to fill everything by setting [ConnectionString].  
![ef4](https://cloud.githubusercontent.com/assets/5955540/3362805/b00f5cc8-fb0d-11e3-98d7-afd1f6ec11e0.png)

My sample ConnectionString:

~~~
PORT=5432;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=20;COMMANDTIMEOUT=20;COMPATIBLE=2.1.3.0;HOST=g45;DATABASE=npgsql_tests_ef;PASSWORD=npgsql_tests;USER ID=npgsql_tests;PRELOADREADER=True
~~~

Note: Don't forget to set **PreloadReader** to **true**.

10. Select [Yes, include the sensitive data in the connection string.] in this case for easy setup.  
![ef5](https://cloud.githubusercontent.com/assets/5955540/3362833/f8a3514c-fb0d-11e3-8675-8147125ad10b.png)

11. Select tables which you want, at Choose Your Database Objects and Settings.  
![ef6](https://cloud.githubusercontent.com/assets/5955540/3362842/07a02e2c-fb0e-11e3-9f91-a3a84377018d.png)

Note: remember the text **npgsql_tests_efModel** at [Model Namespace].

12. Click OK for Security Warning. T4 Templates generator warns you as it contains just runnable C# code.  
![ef7](https://cloud.githubusercontent.com/assets/5955540/3362849/28e8805c-fb0e-11e3-9015-e36514e5ccc8.png)

13. You will get a generated model.  
![ef8](https://cloud.githubusercontent.com/assets/5955540/3362912/c2c4b1be-fb0e-11e3-911f-e7f12c39b162.png)

### Edit your program.cs

Just my sample code for npgsql_tests_ef database.

About the name of "npgsql_tests_efEntities" class, check your [Model Namespace] entered above. Replace "Model" with "Entities", like it is "npgsql_tests_efModel".

{% highlight c# %}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testef
{
    class Program
    {
        static void Main(string[] args)
        {
            using (npgsql_tests_efEntities Context = new npgsql_tests_efEntities())
            {
                foreach (Blogs blog in Context.Blogs)
                {
                    Console.WriteLine(blog.BlogId + " " + blog.Name);
                }
            }
        }
    }
}
{% endhighlight %}

Sample output:  
![ef10](https://cloud.githubusercontent.com/assets/5955540/3362980/57d6c77e-fb0f-11e3-9136-481d547f40d6.png)

# NpgsqlDdexProvider build tips

(by @kenjiuno)
Reference: https://github.com/npgsql/Npgsql/pull/213#issuecomment-42109168

## VS2010 users

You'll need VS2010 Professional or greater.

SP0 users:
- Install: **Visual Studio 2010 SDK**
  http://www.microsoft.com/en-us/download/details.aspx?id=2680

SP1 users:
- Install: **Microsoft Visual Studio 2010 Service Pack 1 (Installer)**
  http://www.microsoft.com/en-us/download/details.aspx?id=23691
- Install: **Visual Studio 2010 SP1 SDK**
  http://www.microsoft.com/en-us/download/details.aspx?id=21835

If you need newer NpgsqlDdexProvider2010.pkgdef, create your own manually.
pkgdef is a kind of registry file for our DDEX registration.
*Note: It is needed only if you change contents of NpgsqlDataProviderRegistration class.*

Command example:

	H:\Dev\Npgsql\NpgsqlDdexProvider>"H:\Program Files (x86)\Microsoft Visual Studio 2010 SDK SP1\VisualStudioIntegration\Tools\Bin\CreatePkgDef.exe" /out=NpgsqlDdexProvider2010.pkgdef /codebase bin\Release-net40\NpgsqlDdexProvider.dll

Output:

~~~
	Visual Studio (R) PkgDef Creation Utility.
	Copyright (c) Microsoft Corporation. All rights reserved.
	
	CreatePkgDef : warning : The Assembly specified at 'bin\Release-net40\NpgsqlDdexProvider.dll' cannot be loaded because an alternate copy with the same identity
	exists in the Assembly probing path at 'H:\Dev\Npgsql\NpgsqlDdexProvider\bin\Release-net40\NpgsqlDdexProvider.dll'. The Assembly at 'H:\Dev\Npgsql\NpgsqlDdexPro
	vider\bin\Release-net40\NpgsqlDdexProvider.dll' will be loaded instead.
	Assembly: NpgsqlDdexProvider 1.0.0.0
	Output file: NpgsqlDdexProvider2010.pkgdef
	
	インストールされている製品:   NpgsqlDdexProviderPackage、Version 1.0
	パッケージ:          NpgsqlDdexProviderPackage {958b9481-2712-4670-9a62-8fe65e5beea7}
	サービス:          PostgreSQL Provider Object Factory
	
	SUCCEEDED:        NpgsqlDdexProvider
~~~

Check: **How to create a pkgdef file for your Visual Studio Package**    http://blogs.msdn.com/b/dsvst/archive/2010/03/08/how-to-create-a-pkgdef-file-for-your-visual-studio-package.aspx

## VS2012 users

You'll need VS2012 Professional or greater.

- Install: **Microsoft Visual Studio 2012 SDK**
  http://www.microsoft.com/en-us/download/details.aspx?id=30668
- Install: **Visual Studio 2012 Update 4**
  http://www.microsoft.com/en-us/download/details.aspx?id=39305

## VS2013 users

You'll need VS2013 Professional or greater.

- Install: **Microsoft Visual Studio 2013 SDK**
  http://www.microsoft.com/en-us/download/details.aspx?id=40758
- Install: **Visual Studio 2013 Update 2** or later. Try Update 4:
  https://www.microsoft.com/en-us/download/details.aspx?id=44921

## VS2015RC users

You'll need [VS2015Pro RC](https://www.microsoft.com/en-us/download/details.aspx?id=46876), [VS2015 RC Downloads](https://www.visualstudio.com/en-us/downloads/visual-studio-2015-downloads-vs.aspx) or such.

- Install: **Microsoft Visual Studio 2015 RC SDK**
  https://www.microsoft.com/en-us/download/details.aspx?id=46850

## How to check if Npgsql DDEX was correctly loaded.

(by @kenjiuno)
Reference: https://github.com/npgsql/Npgsql/pull/67#issuecomment-40281835

Here are tips to check.

- Check your connection dialog:  
![addconn](https://cloud.githubusercontent.com/assets/5955540/2687365/0129b2fe-c24f-11e3-8d9e-6e193ca77d92.png)

- Make sure to edit both x86 and x64's machine.config.
VS2013 runs 64bit mode on x64 machine.
C:\Windows\Microsoft.NET\Framework\v4.0.30319\Config\machine.config
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config

{% highlight xml %}
<system.data>
  <DbProviderFactories>
    <add name="Npgsql Data Provider" 
         invariant="Npgsql" 
         description=".Net Data Provider for PostgreSQL" 
         type="Npgsql.NpgsqlFactory, Npgsql, Version=2.0.12.91, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" 
         support="FF" />
  </DbProviderFactories>
</system.data>
{% endhighlight %}

Note that the `Version` attribute above should match the version of the Npgsql Assembly you are using.

## How to debug Npgsql DDEX extension

In order to debug it, you will need to use the Experimental Instance of Visual Studio.

- In the NpgsqlDdex project, right click and select properties.
- Go to Debug tab
- Click on the radio button for Start External Program. Point it to the devenv.exe binary. On my machine it's located at

`C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe`

Then set the command line arguments to `/rootsuffix Exp`

Save everything and now, just right click the NpgsqlDdex project -> Debug -> Run in a new instance.
A new Visual Studio instance should be run where the extension will be made available and you can debug it in the first visual studio instance.

Reference: http://stackoverflow.com/questions/9281662/how-to-debug-visual-studio-extensions

