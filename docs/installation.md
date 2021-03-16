---
layout: doc
title: Installation
---

## Offical Packages

Official releases of Npgsql are always available on [nuget.org](https://www.nuget.org/packages/Npgsql/). This is the recommended way to use Npgsql.

## Unstable Packages

In additional to the official releases, we automatically publish CI packages for every build. You can use these to test new features or bug fixes that haven't been released yet. Two CI nuget feeds are available:

* [The patch feed](https://www.myget.org/gallery/npgsql) contains CI packages for the next hotfix/patch version. These packages are generally very stable and safe.
* [The vNext feed](https://www.myget.org/gallery/npgsql-unstable) contains CI packages for the next minor or major versions. These are less stable and should be tested with care.

## Visual Studio Integration

If you'd like to have Visual Studio Design-Time support, give our [VSIX extension a try](ddex.md).

## Windows MSI Installer

In some cases you'll want to install Npgsql into your [Global Assembly Cache (GAC)](https://msdn.microsoft.com/en-us/library/yf1d93sz%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396). This is usually the case when you're using a generic .NET Framework program that can work with any ADO.NET provider but doesn't come with Npgsql or reference it directly (e.g. Excel, PowerBI...). For these cases, you can download the Npgsql Windows MSI installer for Npgsql 4.1.x from [our Github releases page](https://github.com/npgsql/npgsql/releases): it will install Npgsql (and optionally the Entity Framework providers) into your GAC and add Npgsql's DbProviderFactory into your `machine.config` file.  This is *not* the general recommended method of using Npgsql - always install via Nuget if possible. In addition to Npgsql.dll, this will also install `System.Threading.Tasks.Extensions.dll` into the GAC.

Note that support for the Windows MSI installer has been discontinued since Npgsql 5.0.0. However, it is still available for versions 4.1.x, is updated from time to time and should work well.

## DbProviderFactory in .NET Framework

On .NET Framework, you can register Npgsql's `DbProviderFactory` in your applications `App.Config` (or `Web.Config`), allowing you to use general, provider-independent ADO.NET types in your application (e.g. `DbConnection` instead of `NpgsqlConnection`) - [see this tutorial](https://msdn.microsoft.com/en-us/library/dd0w4a2z%28v=vs.110%29.aspx?f=255&MSPPError=-21472173960). To do this, add the following to your `App.config`:

```xml
<system.data>
  <DbProviderFactories>
    <add name="Npgsql Data Provider" invariant="Npgsql" description=".Net Data Provider for PostgreSQL" type="Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7"/>
  </DbProviderFactories>
</system.data>
```
