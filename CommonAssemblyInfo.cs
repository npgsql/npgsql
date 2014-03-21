using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Reflection;
using System.Resources;

// Contains assembly attributes shared by all Npgsql projects

[assembly: CLSCompliantAttribute(true)]
[assembly: AllowPartiallyTrustedCallersAttribute()]
#if NET40
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
#endif
[assembly: AssemblyCompanyAttribute("Npgsql Development Team")]
[assembly: AssemblyProductAttribute("Npgsql")]
[assembly: AssemblyCopyrightAttribute("Copyright © 2002 - 2014 Npgsql Development Team")]
[assembly: AssemblyTrademarkAttribute("")]
[assembly: AssemblyVersionAttribute("2.1.1")]
[assembly: AssemblyFileVersionAttribute("2.1.1")]
[assembly: AssemblyInformationalVersionAttribute("2.1.1")]
[assembly: NeutralResourcesLanguageAttribute("en", UltimateResourceFallbackLocation.MainAssembly)]

