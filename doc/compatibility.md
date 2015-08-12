---
layout: doc
title: Compatibility Notes
---

This page centralizes Npgsql's compatibility status with PostgreSQL and other components,
and documents some important gotchas.

We aim to be compatible with all [currently supported PostgreSQL versions](http://www.postgresql.org/support/versioning/), which means 5 years back.
Earlier versions may still work but we don't perform continuous testing on them or commit to resolving issues on them.

## ADO.NET

Npgsql is an ADO.NET-compatible provider, so it has the same APIs as other .NET database drivers and should behave the same.
Please let us know if you notice any non-standard behavior.

A few known issues:

* Npgsql 3.0 implements most [.NET 4.5 ADO.NET async operations](https://msdn.microsoft.com/en-us/library/hh211418(v=vs.110).aspx), e.g.
  ExecuteReaderAsync.
  However, *connecting* asynchronously is not yet implemented (doing so will transparently fall back to sync). This is a planned feature
  for 3.1, see [#379](https://github.com/npgsql/npgsql/issues/379).

## Amazon Redshift

Amazon Redshift is a cloud-based data warehouse originally based on PostgreSQL 8.0.2.
In addition, due to its nature some features have been removed and others changed in ways that make them incompatible with PostgreSQL.
We try to support Redshift as much as we can, please let us know about issues you run across.

First, check out Amazon's [page about Redshift and PostgreSQL](http://docs.aws.amazon.com/redshift/latest/dg/c_redshift-and-postgres-sql.html) which
contains lots of useful compatibility information.

Additional known issues:

* If you want to connect over SSL, your connection string must contain "Server Compatibility Mode=Redshift", otherwise you'll get a connection
  error about `ssl_renegotiation_limit`.
* Entity Framework with database-computed identity values don't work with Redshift, since it doesn't support sequences
(see issue [#544](https://github.com/npgsql/npgsql/issues/544)).

## pgbouncer

Npgsql works well with pgbouncer, but there are some quirks to be aware of.

* Don't forget to turn off Npgsql's internal connection pool by specifying `Pooling=false` on the connection string.
* Npgsql sends the `statement_timeout` startup parameter when it connects, but this parameter isn't supported by pgbouncer.
  You can get around this by specifying `CommandTimeout=0` on the connection string, and then manually setting the `CommandTimeout`
  property on your `NpgsqlCommand` objects.
