using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Reflection;
using System.Resources;

// Contains assembly attributes shared by all Npgsql projects

[assembly: CLSCompliantAttribute(true)]
[assembly: AllowPartiallyTrustedCallersAttribute()]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
[assembly: AssemblyCompanyAttribute("Npgsql Development Team")]
[assembly: AssemblyProductAttribute("Npgsql")]
[assembly: AssemblyCopyrightAttribute("Copyright © 2002 - 2014 Npgsql Development Team")]
[assembly: AssemblyTrademarkAttribute("")]
[assembly: AssemblyVersionAttribute("3.0.0")]
[assembly: AssemblyFileVersionAttribute("3.0.0")]
[assembly: AssemblyInformationalVersionAttribute("3.0.0-beta1")]
[assembly: NeutralResourcesLanguageAttribute("en", UltimateResourceFallbackLocation.MainAssembly)]

