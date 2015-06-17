---
layout: page
title: Build Server Notes
---

This page describes the steps used to set up the Npgsql build server.

If you're upgrading the TeamCity version, see "Give agent service start/stop permissions" below.

## Install all supported versions of the Postgresql backend

At the time of writing, this means 8.4, 9.0, 9.1, 9.2, 9.3. They are configured on ports 5484, 5490, 5491, 5492, 5493.

The parameterized unit tests look at the environment variables NPGSQL_TEST_DB_8.4, NPGSQL_TEST_DB_9.0 and so on for the connection strings to the above. Set them up in TeamCity on the Npgsql project level, in the parameters tab.

## Install a TeamCity-dedicated Postgresql cluster

TeamCity itself requires an SQL database, but we don't want it to run in the same environment as that used for the unit tests. So choosing the latest stable Postgresql version (9.3 at time of writing), we create a new Postgresql cluster: initdb -U postgres -W c:\dev\TeamcityPostgresData

Next we set up a Windows service that starts up the new cluster: pg_ctl register -N postgresql-9.3-teamcity -U teamcity -P <password> -D c:\dev\TeamcityPostgresData

Finally, create a a user and database and point TeamCity to it.

## Install .NET SDKs for all supported .NET versions

* .NET 4.0 (Windows 7 SDK): http://www.microsoft.com/en-us/download/details.aspx?id=8279
* .NET 4.5 (Windows 8 SDK): http://msdn.microsoft.com/en-us/windows/hardware/hh852363.aspx
* .NET 4.5.1 (Windows 8.1 SDK): http://msdn.microsoft.com/en-us/windows/hardware/bg162891.aspx

While installing the SDK for .NET 4.0, I had this problem: http://support.microsoft.com/kb/2717426

## Give agent service start/stop permissions

When upgrading TeamCity, the agent needs to be able to stop and start the Windows service. This is how you can grant a normal user specific permissions on specific services:

 * Download and install subinacl from http://www.microsoft.com/en-us/download/details.aspx?id=23510
 * cd C:\Program Files (x86)\Windows Resource Kits\Tools\
 * subinacl /service TCBuildAgent /grant=teamcity_agent=TO

## Update build status back in github

* Download the plugin from https://github.com/jonnyzzz/TeamCity.GitHub, get the ZIP
* Drop the ZIP in the TeamCity content dir's plugins subdir
* Add the Build Feature "Report change status to GitHub". Configure everything appropriately, and be sure the user you set up has push access to the repository!
