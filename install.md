---
layout: page
title: Getting Npgsql
---
The best way to install Npgsql in your project is with our <a href="https://www.nuget.org/packages/Npgsql/">nuget package</a>.
If you need Entity Framework support, install <a href="https://www.nuget.org/packages/Npgsql.EntityFramework/">this package</a>
(or <a href="https://www.nuget.org/packages/Npgsql.EntityFrameworkLegacy/">this one</a> if you're still using Entity Framework 5).

If you'd like to have Visual Studio Design-Time support, you can try our <a href="">experiental installer</a>.
Otherwise follow the <a href="">instructions for manual installation</a> in the documentation.

Our build server publishes CI nuget packages for every build. If you'd like to install an unstable build, add our unstable NuGet
feed at MyGet: [https://www.myget.org/F/npgsql-unstable](https://www.myget.org/F/npgsql-unstable).
