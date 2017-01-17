# Visual Studio Integration

Npgsql has a Visual Studio extension (VSIX) which integrates PostgreSQL access into Visual Studio. It allows connecting to PostgreSQL from within Visual Studio's Server Explorer, create an Entity Framework 6 model from an existing database, etc.

Note that the extension has been pretty much rewritten for Npgsql 3.2 - if you encountered installation issues with previous versions, these issues should hopefully be gone. A summary of work done for 3.2 [is available here](https://github.com/npgsql/npgsql/issues/1407). If you already have an earlier version of the VSIX (or MSI) installed, it's highly recommended that you uninstall them to avoid conflicts. 

It is no longer necessary or recommended to have Npgsql in your GAC, or to have Npgsql listed in your machines.config. Simply installing the VSIX should work just fine, and a GAC/machines.config may actually cause issues. If you previously installed Npgsql into your GAC/machines.config, it's recommended you uninstall it (unless you have good reasons to have it there).

The VSIX extension has been tested and works on Visual Studio 2015 and 2017. It is probably compatible with versions all the way back to 2012, but these haven't been tested. Note that installing into pre-2015 versions will display a warning, although it should be safe to proceed.

## Entity Framework 6

The extension supports generating a model from an existing database. To do so, install EntityFramework6.Npgsql into your project, and then make sure you have the same version of Npgsql as your extension does. A mismatch between the version installed in your project and the VSIX's may cause issues.

## Development

Development on the extension is currently possible only on Visual Studio 2017. Be sure to install the "Visual Studio extension development" workload.
