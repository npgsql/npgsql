# VS2012,VS2013,and VS2015Pro+NpgsqlDdexProvider+EFv6 how to

(by @kenjiuno)

Reference: [#213](https://github.com/npgsql/Npgsql/pull/213#issuecomment-46892619)

**Note:** Npgsql 3.1 and NpgsqlDdexProvider 3.1.0 documentations are ongoing at github: https://github.com/npgsql/npgsql/issues/1299

## Overview

1. Install Npgsql DDEX (Data Designer Extensibility) provider.
2. Install Npgsql ADO.NET Data Provider.
3. Visual Studio's *Entity Data Model wizard* will be enabled for PostgreSQL servers.

## Prerequisites

Visual Studio 2015 users:

1. *Visual Studio 2015 Professional* or greater editions. Express edition won't work.
2. [Microsoft Visual Studio 2015 Update 1](https://www.microsoft.com/en-us/download/details.aspx?id=49989) is available.

Visual Studio 2013 users:

1. *Visual Studio 2013 Professional Update 2* or greater editions. Express edition won't work.
2. [Microsoft Visual Studio 2013 Update 5](https://www.microsoft.com/en-us/download/details.aspx?id=48129) is available.

Visual Studio 2012 users:

1. *Visual Studio 2012 Professional* or greater editions. Express edition won't work.
2. [Visual Studio 2012 Update 5](http://www.microsoft.com/en-us/download/details.aspx?id=48708) is available.

PostgreSQL server installed:

1. Tested on *PostgreSQL 9.3.4 (win-x64)*

## Install DDEX provider (Npgsql 3.0.x)

1. Grab *Setup_NpgsqlDdexProvider.exe* from [https://github.com/npgsql/npgsql/releases](https://github.com/npgsql/npgsql/releases) and run it.
2. Select all components to install.

Note: The **version** among *Npgsql*, *EntityFramework6.Npgsql* and *NpgsqlDdexProvider* **must be same**. For example, if you select *Npgsql* 3.0.5, it needs *EntityFramework6.Npgsql* 3.0.5. Also *NpgsqlDdexProvider* 3.0.5.

## Install Npgsql ADO.NET Data Provider to Visual Studio (Npgsql 3.0.x)

1. Launch Visual Studio.
2. Open [TOOL] menu, and then click [Setup Npgsql DbProviderFactories...]
3. Click [OK], 2 times.
4. Restart Visual Studio.

It asks permission to modify your devenv.exe.config:

![setup2](https://cloud.githubusercontent.com/assets/5955540/9305488/b7368b7e-452c-11e5-9555-5dba7cb53a5a.png)

This process will add the Npgsql in devenv.exe.config:

```xml
<system.data>
  <DbProviderFactories>
    ...
    <remove invariant="Npgsql" />
    <add name="Npgsql Data Provider" invariant="Npgsql" description=".Net Data Provider for PostgreSQL" type="Npgsql.NpgsqlFactory, Npgsql, Version=3.0.0.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" />
  </DbProviderFactories>
</system.data>
```

Setup succeeded.

![setup3](https://cloud.githubusercontent.com/assets/5955540/9305503/e28f8578-452c-11e5-988c-228cc6792371.png)

Note: It will be prompted if administrative privilege is required to modify your devenv.exe.config.

## Prepare new project for testing

1. Launch Visual Studio.
2. [FILE]→[New]→[Project...]
3. [Console Application]
4. Name is [testef] for example.

## Install Npgsql for Entity Framework 6 (3.0.x) from NuGet

1. Right click project [testef]
2. [Managet NuGet Packages...]
3. Type "Npgsql" at [Search Online (Ctrl+E)]
4. Install "Npgsql for Entity Framework 6" ([EntityFramework6.Npgsql](https://www.nuget.org/packages/EntityFramework6.Npgsql/)). Version is 3.0.5 for now.
5. EntityFramework 6.0.0 and Npgsql are also installed as part of its dependency.

Notice: The assembly versions of Npgsql and NpgsqlDdexProvider **must be same**. If, for some reason, you need to install a version which isn't the latest one from NuGet, you need to use the following command in the NuGet Package Manager Console:

`Install-Package EntityFramework6.Npgsql -Version <version>` where `<version>` is the version you want to install. A list of versions available can be found in the NuGet Npgsql page: [https://www.nuget.org/packages/Npgsql/](https://www.nuget.org/packages/Npgsql/)

## Add Npgsql EFv6 provider

Notice: Recent *EntityFramework6.Npgsql* NuGet package automatically does this process.

1. Open [App.config], or [Web.config] for web projects.
2. Add provider-element into providers-element: `<provider invariantName="Npgsql" type="Npgsql.NpgsqlServices, EntityFramework6.Npgsql" />`

An App.config having Npgsql EFv6 privoder:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
  </configSections>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>
  <entityFramework>
    <defaultConnectionFactory type="System.Data.Entity.Infrastructure.SqlConnectionFactory, EntityFramework" />
    <providers>
      <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
      <provider invariantName="Npgsql" type="Npgsql.NpgsqlServices, EntityFramework6.Npgsql" />
    </providers>
  </entityFramework>
</configuration>
```

## Add Npgsql ADO.NET Data Provider

You need to declare the *Npgsql ADO.NET Data Provider*. Edit one of following config files:

1. `App.config` or `Web.config`
2. `machine.config`

If you are using NuGet for your application, we recommend to edit: App.config or Web.config

machine.config are placed in these locations. Framework64 will exist on 64-bit Windows:

```
C:\Windows\Microsoft.NET\Framework\v4.0.30319\Config\machine.config
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config
```

This is needed part of App.config:

```xml
<system.data>
  <DbProviderFactories>
    <remove invariant="Npgsql"/>
    <add name="Npgsql Data Provider"
         invariant="Npgsql"
         description=".Net Data Provider for PostgreSQL"
         type="Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7"
         support="FF" />
  </DbProviderFactories>
</system.data>
```

Note: `<remove invariant="Npgsql"/>` is important, in case of already having `<add invariant="Npgsql" ... />` in machine.config.

Edited App.config:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <!-- For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
  </configSections>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
  </startup>
  <entityFramework>
    <defaultConnectionFactory type="System.Data.Entity.Infrastructure.SqlConnectionFactory, EntityFramework" />
    <providers>
      <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
      <provider invariantName="Npgsql" type="Npgsql.NpgsqlServices, EntityFramework6.Npgsql" />
    </providers>
  </entityFramework>
  <system.data>
    <DbProviderFactories>
      <remove invariant="Npgsql"/>
      <add name="Npgsql Data Provider"
           invariant="Npgsql"
           description=".Net Data Provider for PostgreSQL"
           type="Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7"
           support="FF" />
    </DbProviderFactories>
  </system.data>
</configuration>
```

## Build once

1. Build your project.

### New ADO.NET Entity Data Model

1. Right click project [testef]
2. [Add]→[New Item...]
3. [ADO.NET Entity Data Model]
4. Default name [Model1] for example.
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
Host=127.0.0.1;Port=5432;Database=npgsql_tests;Username=npgsql_tests;Password=npgsql_tests
~~~

Note: **PreloadReader** and **Compatible** properies are obsoleted since Npgsql 3.0.0. Please remove them before submitting ConnectionString.

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

```c#
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
```

Sample output:  
![ef10](https://cloud.githubusercontent.com/assets/5955540/3362980/57d6c77e-fb0f-11e3-9136-481d547f40d6.png)


## How to check if Npgsql DDEX is working correctly. (Npgsql 3.0.x)

(by @kenjiuno)

Reference: [#718](https://github.com/npgsql/npgsql/pull/718#issuecomment-131815079)

NpgsqlDdexProvider 3.0.4 and later has a feature to check Npgsql installation status of your .NET project.

1. Right click your .NET project
2. Click [Check Npgsql project installation]

<img width="359" alt="checknpgsqlprojectinstallation" src="https://cloud.githubusercontent.com/assets/5955540/11956722/9be2f4de-a8ff-11e5-9ca7-a1264c9a8a58.png">

Click a button to start the check!

<img width="451" alt="test1" src="https://cloud.githubusercontent.com/assets/5955540/11956750/d8556302-a8ff-11e5-95a4-8eb965ad0370.png">

It will suggest them if you need one or more actions:

<img width="451" alt="test3" src="https://cloud.githubusercontent.com/assets/5955540/11956755/dc675270-a8ff-11e5-8265-311e532a285d.png">

[Test and result] shows test cases and their results:

<img width="451" alt="test2" src="https://cloud.githubusercontent.com/assets/5955540/11956753/da1e40fa-a8ff-11e5-9fd2-69469568a07f.png">


## How to check if Npgsql DDEX was correctly loaded. (Npgsql 2.x)

(by @kenjiuno)

Reference: [#67](https://github.com/npgsql/Npgsql/pull/67#issuecomment-40281835)

Here are tips to check.

- Check your connection dialog:  
![addconn](https://cloud.githubusercontent.com/assets/5955540/2687365/0129b2fe-c24f-11e3-8d9e-6e193ca77d92.png)

- Make sure to edit both x86 and x64's machine.config.
VS2013 runs 64bit mode on x64 machine.
C:\Windows\Microsoft.NET\Framework\v4.0.30319\Config\machine.config
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config

```xml
<system.data>
  <DbProviderFactories>
    <add name="Npgsql Data Provider" 
         invariant="Npgsql" 
         description=".Net Data Provider for PostgreSQL" 
         type="Npgsql.NpgsqlFactory, Npgsql, Version=2.0.12.91, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7" 
         support="FF" />
  </DbProviderFactories>
</system.data>
```

Note that the `Version` attribute above should match the version of the Npgsql Assembly you are using.


# NpgsqlDdexProvider build tips

(by @kenjiuno)

Reference: [#213](https://github.com/npgsql/Npgsql/pull/213#issuecomment-42109168)

## VS2010 users

You'll need VS2010 Professional or greater.

SP0 users:

- Install: [Visual Studio 2010 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=2680)

SP1 users:

- Install: [Microsoft Visual Studio 2010 Service Pack 1 (Installer)](http://www.microsoft.com/en-us/download/details.aspx?id=23691)
- Install: [Visual Studio 2010 SP1 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=21835)

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

- Install: [Microsoft Visual Studio 2012 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=30668)
- Install: [Visual Studio 2012 Update 4](http://www.microsoft.com/en-us/download/details.aspx?id=39305)

## VS2013 users

You'll need VS2013 Professional or greater.

- Install: [Microsoft Visual Studio 2013 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=40758)
- Install: [Visual Studio 2013 Update 2](https://www.microsoft.com/en-us/download/details.aspx?id=44921) or later.

## VS2015 users

You'll need VS2015 Professional or greater.

- Check: [Installing the Visual Studio Extensibility Tools (VS SDK)](https://msdn.microsoft.com/en-us/library/bb166441.aspx#Anchor_0)

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

