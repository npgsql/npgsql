# Compatibility Notes

This page centralizes Npgsql's compatibility status with PostgreSQL and other components,
and documents some important gotchas.

We aim to be compatible with all [currently supported PostgreSQL versions](http://www.postgresql.org/support/versioning/), which means 5 years back.
Earlier versions may still work but we don't perform continuous testing on them or commit to resolving issues on them.

## PostgreSQL

We aim to be compatible with all [currently supported PostgreSQL versions](http://www.postgresql.org/support/versioning/), which means 5 years back.
Earlier versions may still work but we don't perform continuous testing on them or commit to resolving issues on them.

## ADO.NET

Npgsql is an ADO.NET-compatible provider, so it has the same APIs as other .NET database drivers and should behave the same.
Please let us know if you notice any non-standard behavior.

## .NET Framework/.NET Core/mono

Npgsql 3.1 targets .NET Framework 4.5 and 4.5.1, as well as the [.NET Standard 1.3](https://github.com/dotnet/corefx) which allows it
to run on .NET Core. It is also tested and runs well on mono.

Here is a sample project.json to get you started with .NET Core:

```json
{
  "buildOptions": {
    "emitEntryPoint": "true"
  },
  "dependencies": {
    "Npgsql" : "3.1.8"
  },
  "frameworks": {
    "net451": {
      "frameworkAssemblies": {
        "System.Data": { "version": "4.0.0.0", "type": "build" }
      }
    },
    "netcoreapp1.0": {
      "dependencies": {
        "Microsoft.NETCore.App": {
          "version": "1.0.1",
          "type": "platform"
        }
      }
    }
  }
}
```

Note that `netcoreapp1.0` can be replaced with `netstandard13` (or up) to create a library.

## Amazon Redshift

Amazon Redshift is a cloud-based data warehouse originally based on PostgreSQL 8.0.2.
In addition, due to its nature some features have been removed and others changed in ways that make them incompatible with PostgreSQL.
We try to support Redshift as much as we can, please let us know about issues you run across.

First, check out Amazon's [page about Redshift and PostgreSQL](http://docs.aws.amazon.com/redshift/latest/dg/c_redshift-and-postgres-sql.html) which
contains lots of useful compatibility information.

Additional known issues:

* If you want to connect over SSL, your connection string must contain `Server Compatibility Mode=Redshift`, otherwise you'll get a connection
  error about `ssl_renegotiation_limit`.
* Entity Framework with database-computed identity values don't work with Redshift, since it doesn't support sequences
(see issue [#544](https://github.com/npgsql/npgsql/issues/544)).

## pgbouncer

Npgsql works well with PgBouncer, but there are some quirks to be aware of.

* In many cases, you'll want to turn off Npgsql's internal connection pool by specifying `Pooling=false` on the connection string.
* If you decide to keep Npgsql pooling on along with PgBouncer, and are using PgBouncer's transaction or statement mode, then you  need to specify `No Reset On Close=true` on the connection string. This disables Npgsql's connection reset logic (`DISCARD ALL`), which gets executed when a connection is return to Npgsql's pool, and which makes no sense in these modes.
* Prior to version 3.1, Npgsql sends the `statement_timeout` startup parameter when it connects, but this parameter isn't supported by pgbouncer.
  You can get around this by specifying `CommandTimeout=0` on the connection string, and then manually setting the `CommandTimeout`
  property on your `NpgsqlCommand` objects. Version 3.1 no longer sends `statement_timeout`.
