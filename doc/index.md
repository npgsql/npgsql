---
layout: doc
title: Documentation
---

## Getting Started

The best way to use Npgsql is to install its [nuget package](https://www.nuget.org/packages/Npgsql/).

Npgsql aims to be fully ADO.NET-compatible, its API should feel almost identical to other .NET database drivers.
Here's a basic code snippet to get you started.

```c#
using (var conn = new NpgsqlConnection("Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase"))
{
    conn.Open();
    using (var cmd = new NpgsqlCommand())
    {
        cmd.Connection = conn;

        // Insert some data
        cmd.CommandText = "INSERT INTO data (some_field) VALUES ('Hello world')";
        cmd.ExecuteNonQuery();

        // Retrieve all rows
        cmd.CommandText = "SELECT some_field FROM data";
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
}
```

You can find more info about the ADO.NET API in the [MSDN docs](https://msdn.microsoft.com/en-us/library/h43ks021(v=vs.110).aspx)
or in many tutorials on the Internet.

## GAC Installation

In some cases you'll want to install Npgsql into your [Global Assembly Cache (GAC)](https://msdn.microsoft.com/en-us/library/yf1d93sz%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396). This is usually the case when you're using a generic database program that can work with any ADO.NET provider but doesn't come with Npgsql or reference it directly. For these cases, you can download the Npgsql Windows installer from [our Github releases page](https://github.com/npgsql/npgsql/releases): it will install Npgsql (and optionally the Entity Framework providers) into your GAC and add Npgsql's DbProviderFactory into your `machine.config` file.  This is *not* the general recommended method of using Npgsql - always install via Nuget if possible. In addition to Npgsql.dll, this will also install `System.Threading.Tasks.Extensions.dll` and `Microsoft.Extensions.Logging.dll` into the GAC.

## Visual Studio Integration

If you'd like to have Visual Studio Design-Time support, you can try our [experimental DDEX installer](https://github.com/npgsql/npgsql/releases).
Follow the [instructions](ddex.md) in the documentation.

## Unstable Packages

The Npgsql build server publishes CI nuget packages for every build. If a bug affecting you was fixed but there hasn't yet been a patch release,
you can get a CI nuget at our [stable MyGet feed](https://www.myget.org/gallery/npgsql). These packages are generally stable and
safe to use (although it's better to wait for a release).

We also publish CI packages for the next minor/major version at our [unstable MyGet feed](https://www.myget.org/gallery/npgsql-unstable).
These are definitely unstable and should be used with care.
