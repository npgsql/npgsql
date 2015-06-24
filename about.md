---
layout: page
title: About
---
## General

Npgsql is an open source ADO.NET Data Provider for PostgreSQL, it allows programs written in C#, Visual Basic, F# to access the PostgreSQL database server.
It allows any program developed for .NET framework to access database server. It is implemented in 100% C# code. Works with PostgreSQL 9.x and above.

## Compatibility

We aim to be compatible with all [currently supported PostgreSQL versions](http://www.postgresql.org/support/versioning/), which means 5 years back.
Earlier versions may still work but we don't perform continuous testing on them or commit to resolving issues on them.

For more compatibility information please see [this page](doc/compatibility.html).

## Non-Windows Platforms

Full compatibility on mono is a major goal of Npgsql. We run regular regression tests on mono and even contribute fixes to mono itself from time to time.
Please report any issues you find.

## License

Npgsql uses the [PostgreSQL License](https://github.com/npgsql/npgsql/blob/develop/LICENSE.txt), a liberal OSI-approved open source license.
