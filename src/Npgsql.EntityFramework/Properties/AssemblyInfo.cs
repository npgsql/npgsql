using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Reflection;
using System.Resources;

// Additional assembly attributes are defined in GlobalAssemblyInfo.cs

#if ENTITIES6
[assembly: AssemblyTitleAttribute("Npgsql.EntityFramework")]
[assembly: AssemblyDescriptionAttribute("Postgresql provider for Entity Framework 6 and above")]
#else
[assembly: AssemblyTitleAttribute("Npgsql.EntityFrameworkLegacy")]
[assembly: AssemblyDescriptionAttribute("Postgresql provider for Entity Framework 5 and under")]
#endif
