using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Reflection;
using System.Resources;

// Additional assembly attributes are defined in GlobalAssemblyInfo.cs

#if ENTITIES6
[assembly: AssemblyTitleAttribute("EntityFramework6.Npgsql")]
[assembly: AssemblyDescriptionAttribute("PostgreSQL provider for Entity Framework 6")]
#else
[assembly: AssemblyTitleAttribute("EntityFramework5.Npgsql")]
[assembly: AssemblyDescriptionAttribute("PostgreSQL provider for Entity Framework 5")]
#endif
