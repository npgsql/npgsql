---
layout: doc
title: Developer Resources
---

## Tests

We maintain a large regression test suite, if you're planning to submit code, please provide a test
that reproduces the bug or tests your new feature. See [this page](dev/tests.html) for information on the
Npgsql test suite.

## Build Server

We have a [TeamCity build server](https://www.jetbrains.com/teamcity/) running continuous integration builds
on commits pushed to our github repository. The Npgsql testsuite is executed over all officially supported
PostgreSQL versions to catch errors as early as possible. CI NuGet packages are automatically pushed to our
[unstable feed at MyGet](https://www.myget.org/F/npgsql-unstable).

For some information about the build server setup, see [this page](dev/build-server.html).

Thanks to Dave Page at PostgreSQL for donating a VM for this!

## Other stuff

Emil compiled [a list of PostgreSQL types and their wire representations](dev/types.html).

