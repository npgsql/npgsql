---
layout: doc
title: Installation
---

## Offical Packages

Official releases of Npgsql are always available on [nuget.org](https://www.nuget.org/packages/Npgsql/). This is the recommended way to use Npgsql.

We occasionally publish previews to nuget.org as well - these are generally quite safe for use, and can help us find issues before official packages are released.

## Daily Builds

In additional to the official releases, we automatically publish CI packages for every build. You can use these to test new features or bug fixes that haven't been released yet. Two CI nuget feeds are available:

* [The patch feed](https://www.myget.org/feed/Packages/npgsql) contains CI packages for the next hotfix/patch version. These packages are generally very stable and safe.
* [The vNext feed](https://www.myget.org/feed/Packages/npgsql-vnext) contains CI packages for the next minor or major versions. These are less stable and should be tested with care.

## Older, unsupported installation methods

### Windows MSI Installer

If you need to use Npgsql as a database provider for PowerBI, Excel or other similar systems, you need to install it into the Windows [Global Assembly Cache (GAC)](https://msdn.microsoft.com/en-us/library/yf1d93sz%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396), and do some special configuration. Npgsql versions prior to 5.0.0 provided a Windows installer (MSI) which does installation for you, and which are still usable and maintained with critical bug fixes. Do not use the Windows MSI installer unless you're sure that your program requires GAC installation - this method is otherwise highly discouraged.

The Npgsql Windows MSI installer for Npgsql 4.1.x can be found on [our Github releases page](https://github.com/npgsql/npgsql/releases): it will install Npgsql (and optionally the Entity Framework providers) into your GAC and add Npgsql's DbProviderFactory into your `machine.config` file. Some additional assemblies which are Npgsql dependencies will be installed into the GAC as well (e.g. `System.Threading.Tasks.Extensions.dll`).

### Visual Studio Integration

Older versions of Npgsql came with a Visual Studio extension (VSIX) which integrated PostgreSQL access into Visual Studio. The extension allowed connecting to PostgreSQL from within Visual Studio's Server Explorer, creating an Entity Framework 6 model from an existing database, etc. The extension had various limitations and known issues, mainly because of problems with Visual Studio's extensibility around database.

Use of the extension is no longer recommended. However, if you're like to give it a try, it can be installed directly from [the Visual Studio Marketplace page](https://marketplace.visualstudio.com/vsgallery/258be600-452d-4387-9a2f-89ae10e84ae0).

### DbProviderFactory in .NET Framework

On .NET Framework, you can register Npgsql's `DbProviderFactory` in your applications `App.Config` (or `Web.Config`), allowing you to use general, provider-independent ADO.NET types in your application (e.g. `DbConnection` instead of `NpgsqlConnection`) - [see this tutorial](https://msdn.microsoft.com/en-us/library/dd0w4a2z%28v=vs.110%29.aspx?f=255&MSPPError=-21472173960). To do this, add the following to your `App.config`:

```xml
<system.data>
  <DbProviderFactories>
    <add name="Npgsql Data Provider" invariant="Npgsql" description=".Net Data Provider for PostgreSQL" type="Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7"/>
  </DbProviderFactories>
</system.data>
```
