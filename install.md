---
layout: page
title: Getting Npgsql
---
The best way to install Npgsql in your project is with our <a href="https://www.nuget.org/packages/Npgsql/">nuget package, Npgsql</a>.

- For Entity Framework 6, install <a href="https://www.nuget.org/packages/EntityFramework6.Npgsql/">EntityFramework6.Npgsql</a>
(In Npgsql 2.2 and earlier the package was <a href="https://www.nuget.org/packages/Npgsql.EntityFramework/">Npgsql.EntityFramework</a>.
- For Entity Framework 5, install <a href="https://www.nuget.org/packages/EntityFramework5.Npgsql/">EntityFramework5.Npgsql</a>
(or <a href="https://www.nuget.org/packages/Npgsql.EntityFrameworkLegacy/">Npgsql.EntityFrameworkLegacy</a> in Npgsql 2.2).
- An Entity Framework 7 provider is in the works but is still unsuitable for actual use. You can try it out via our unstable
feed (see below), package name EntityFramework7.Npgsql.

If you'd like to have Visual Studio Design-Time support, you can try our <a href="">experiental installer</a>.
Otherwise follow the <a href="doc/ddex.html">instructions for manual installation</a> in the documentation.

Our build server publishes CI nuget packages for every build. If you'd like to install an unstable build, add our unstable NuGet
feed at MyGet: [https://www.myget.org/F/npgsql-unstable](https://www.myget.org/F/npgsql-unstable).
