---
layout: page
title: Tests
---

## Overview

Npgsql has an extensive test suite to guard against regressions. The test suite is run on the official build server for all supported .NET versions (including a recent version of mono) and all supported PostgreSQL backends.

Npgsql also includes a growing suite of performance tests. These tests are marked `[Explicit]` and can be executed manually.

## Getting Started

The Npgsql test suite requires a PostgreSQL backend for tests to run. By default, the test suite expects PostgreSQL to be running on the local machine with the default port (5432). 

### Setup PostgreSQL

1. Install PostgreSQL: https://www.postgresql.org/download
2. Start the PostgreSQL backend.

### Create the `npgsql_tests` account

By default, the test suite expects an account named `npgsql_tests` with a password of `npgsql_tests`. This account is used by the test suite to create a database named `npgsql_tests` and run the tests. 

```
$ psql -h localhost -U postgres
postgres=# CREATE USER npgsql_tests PASSWORD 'npgsql_tests' SUPERUSER;
```

_Note: superuser access is required to create and drop test databases, load extensions (e.g. `hstore`, `postgis`), etc._

### Clone the repository

```
cd ~
git clone git@github.com:npgsql/npgsql.git
```

### Run the test suite

```
cd ~/npgsql
dotnet test ./test/Npgsql.Tests
dotnet test ./test/Npgsql.PluginTests
dotnet test ./test/Npgsql.Benchmarks
```
