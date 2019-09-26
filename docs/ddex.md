# Visual Studio Integration

Npgsql has a Visual Studio extension (VSIX) which integrates PostgreSQL access into Visual Studio. It allows connecting to PostgreSQL from within Visual Studio's Server Explorer, create an Entity Framework 6 model from an existing database, etc. The extension can be installed directly from [the Visual Studio Marketplace page](https://marketplace.visualstudio.com/vsgallery/258be600-452d-4387-9a2f-89ae10e84ae0).

The VSIX doesn't automatically add Npgsql to your GAC, `App.config`, `machines.config` or any other project or system-wide resource. It only allows accessing PostgreSQL from Visual Studio itself.

## Visual Studio Compatibility

The VSIX extension has been tested and works on Visual Studio 2015, 2017 and 2019. It is probably compatible with versions all the way back to 2012, but these haven't been tested. Note that installing into pre-2015 versions will display a warning, although it should be safe to proceed.

## Upgrading from an older version

Note that the extension has been pretty much rewritten for Npgsql 3.2 - if you encountered installation issues with previous versions, these issues should hopefully be gone. A summary of work done for 3.2 [is available here](https://github.com/npgsql/npgsql/issues/1407). If you already have an earlier version of the VSIX (or MSI) installed, it's highly recommended that you uninstall them to avoid conflicts. 

It is no longer necessary or recommended to have Npgsql in your GAC, or to have Npgsql listed in your machines.config. Simply installing the VSIX should work just fine, and a GAC/machines.config may actually cause issues. If you previously installed Npgsql into your GAC/machines.config, it's recommended you uninstall it. If you have any entries (binding redirects, DbProviderFactory registrations) in either your `machines.config` or in your Visual Studio setup (e.g. App.config, `devenv.exe.config`), please remove them The VSIX should work on a totally clean setup.

## Features

The provider isn't feature complete - please let us know of missing features or bugs by opening issues.

### Server Explorer

You can add a PostgreSQL database in Server Explorer, explore tables and columns, send ad-hoc queries, etc.

### Entity Framework 6

The extension supports generating a model from an existing database. To do so, install EntityFramework6.Npgsql into your project, and then make sure you have the same version of Npgsql as your extension does. A mismatch between the version installed in your project and the VSIX's may cause issues.

## Development

Development on the VSIX is currently possible only on Visual Studio 2017, 2019. Be sure to install the "Visual Studio extension development" workload.
