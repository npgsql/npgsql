---
layout: doc
title: Documentation
---

<p id="badges">
  <a href="https://www.nuget.org/packages/Npgsql/">
    <img src="https://img.shields.io/nuget/v/Npgsql.svg?label=Stable&amp;style=plastic;maxAge=600" />
  </a>

  <a href="https://www.myget.org/gallery/npgsql-unstable">
    <img src="https://img.shields.io/myget/npgsql-unstable/vpre/npgsql.svg?label=Unstable&amp;style=plastic;maxAge=600" />
  </a>

  <a href="https://ci.appveyor.com/project/roji/npgsql">
    <img src="https://img.shields.io/appveyor/ci/roji/npgsql/dev.svg" />
  </a>

  <a href="https://travis-ci.org/npgsql/npgsql" >
    <img src="https://img.shields.io/travis/npgsql/npgsql.svg" />
  </a>

  <a href="https://gitter.im/npgsql/npgsql">
    <img src="https://img.shields.io/badge/GITTER-JOIN%20CHAT-brightgreen.svg?style=plastic;maxAge=600" />
  </a>
</p>

## Getting Started

The best way to use Npgsql is to install its [nuget package](https://www.nuget.org/packages/Npgsql/).

Npgsql aims to be fully ADO.NET-compatible, its API should feel almost identical to other .NET database drivers.
Here's a basic code snippet to get you started.

```c#
var connString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

using (var conn = new NpgsqlConnection(connString))
{
    conn.Open();

    // Insert some data
    using (var cmd = new NpgsqlCommand())
    {
        cmd.Connection = conn;
        cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
        cmd.Parameters.AddWithValue("p", "Hello world");
        cmd.ExecuteNonQuery();
    }

    // Retrieve all rows
    using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
    using (var reader = cmd.ExecuteReader())
        while (reader.Read())
            Console.WriteLine(reader.GetString(0));
}
```

You can find more info about the ADO.NET API in the [MSDN docs](https://msdn.microsoft.com/en-us/library/h43ks021(v=vs.110).aspx)
or in many tutorials on the Internet.

## DbProviderFactory

The example above involves some Npgsql-specific types (`NpgsqlConnection`, `NpgsqlCommand`...), which makes your application Npgsql-specific. If your code needs to be database-portable, you should use the ADO.NET `DbProviderFactory` API instead ([see this tutorial](https://msdn.microsoft.com/en-us/library/dd0w4a2z%28v=vs.110%29.aspx?f=255&MSPPError=-21472173960)). In a nutshell, you register Npgsql's provider factory in your application's `App.config` (or `machines.config`) file, and then obtain it in your code without referencing any Npgsql-specific types. You can then use the factory to create a `DbConnection` (which `NpgsqlConnection` extends), and from there a `DbCommand` and so on.

To do this, add the following to your `App.config`:

```xml
<system.data>
  <DbProviderFactories>
    <add name="Npgsql Data Provider" invariant="Npgsql" description=".Net Data Provider for PostgreSQL" type="Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7"/>
  </DbProviderFactories>
</system.data>
```

## GAC Installation

In some cases you'll want to install Npgsql into your [Global Assembly Cache (GAC)](https://msdn.microsoft.com/en-us/library/yf1d93sz%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396). This is usually the case when you're using a generic database program that can work with any ADO.NET provider but doesn't come with Npgsql or reference it directly. For these cases, you can download the Npgsql Windows installer from [our Github releases page](https://github.com/npgsql/npgsql/releases): it will install Npgsql (and optionally the Entity Framework providers) into your GAC and add Npgsql's DbProviderFactory into your `machine.config` file.  This is *not* the general recommended method of using Npgsql - always install via Nuget if possible. In addition to Npgsql.dll, this will also install `System.Threading.Tasks.Extensions.dll` into the GAC.

## Visual Studio Integration

If you'd like to have Visual Studio Design-Time support, give our [VSIX extension a try](ddex.md).

## Unstable Packages

The Npgsql build server publishes CI nuget packages for every build. If a bug affecting you was fixed but there hasn't yet been a patch release,
you can get a CI nuget at our [stable MyGet feed](https://www.myget.org/gallery/npgsql). These packages are generally stable and
safe to use (although it's better to wait for a release).

We also publish CI packages for the next minor/major version at our [unstable MyGet feed](https://www.myget.org/gallery/npgsql-unstable).
These are definitely unstable and should be used with care.
