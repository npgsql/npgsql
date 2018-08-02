---
layout: page
title: Tests
---

## Overview

Npgsql comes with an extensive test suite to make sure no regressions occur. All tests are run on our build server on all supported .NET versions (including a recent version of mono) and all supported PostgreSQL backends.

There is also a growing suite of speed tests to be able to measure performance. These tests are currently marked [Explicit] and aren't executed automatically.

## Simple setup

The Npgsql test suite requires a PostgreSQL backend to test against. Simply use the latest version of PostgreSQL on your dev machine on the default port (5432).
By default, all tests will be run using user *npgsql_tests*, and password *npgsql_tests*. Npgsql will automatically create a database called *npgsql_tests* and
run its tests against this.

To set this up, connect to PostgreSQL as the admin user as follows:

```
psql -h localhost -U postgres
<enter the admin password>
create user npgsql_tests password 'npgsql_tests' superuser;
```

And you're done.

Superuser access is needed for some tests, e.g. loading the `hstore` extension, creating and dropping test databases in the Entity Framework tests...
