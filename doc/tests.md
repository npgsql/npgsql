---
layout: doc
title: Tests
---

## Overview

Npgsql comes with an extensive test suite to make sure no regressions occur. All tests are run on our build server on all supported .NET versions (including a recent version of mono) and all supported Postgresql backends.

There is also a growing suite of speed tests to be able to measure performance. These tests are currently marked [Explicit] and aren't executed automatically.

## Simple setup

The Npgsql test suite requires a Postgresql backend to test against. Simply use the latest version of Postgresql on your dev machine on the default port (5432). All tests will be run on database *npgsql_tests*, using user *npgsql_tests*, and password *npgsql_tests*.

To set this up, connect to Postgresql as the admin user as follows:

{% highlight sql %}
psql -h localhost -U postgresql
<enter the admin password>
create user npgsql_tests password 'npgsql_tests' superuser;
create database npgsql_tests owner npgsql_tests;
{% endhighlight %}

And you're done.

Superuser access is needed for some tests, e.g. loading the hstore extension, creatig and dropping test databases in the Entity Framework tests...

## Testing against multiple backends

The test suite uses NUnit parameterized test fixtures to allow for testing against multiple backends. This allows our build server to make sure all tests complete against *all* supported backend major versions.

Setting this up is easy. The test suite checks for the existence of environment variables, which contain the connection string for each backend version. If an environment variable for a given Postgresql major version is missing, testing against that version is skipped.

For example, to test against Postgresql versions 9.3 and 9.1, define NPGSQL_TEST_DB_9.3 and NPGSQL_TEST_DB_9.1, each containing a valid Postgresql connection string.

After that, you have to configure each backend to listen to different tcp ports. Remember to add the port parameter to the connection string you specified above!

## Debugging tests with Visual Studio .Net 

In order to debug the tests running on NUnit under Visual Studio .net, you need to make some small changes to the project. 

### Project properties 

1. First, select the project NpgsqlTests in solution explorer, right-click it and select Properties. 
1. In the Debug tab, set the complete path of nunit-x86.exe in the Start external program box.
1. In the Command line arguments, set the complete path of the NpgsqlTests.dll assembly you want to debug
1. Set the path of the assembly in the working directory box.

### Nunit configuration

If you are using .Net 4.0 and above, you have to edit nunit-x86.exe.config  (or nunit.exe.config if you are running 64-bit version of NUnit) and add the following section inside the configuration element:

{% highlight xml %}
<startup>
    <supportedRuntime version="4.0" />
</startup>
{% endhighlight %}

**This is needed or else, Visual Studio will complain that it can't load debug symbols.**

### Set NpgsqlTests project as startup project

To facilitate debugging, you can specify NpgsqlTests as startup project. This way, when you press F5, Visual studio will start NUnit with the tests loaded.

### Reference

A complete reference tutorial with pictures where those instructions were taken from can be found here: http://erraticdev.blogspot.com/2012/01/running-or-debugging-nunit-tests-from.html
