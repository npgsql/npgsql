using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Reflection;
using System.Resources;

// Contains assembly attributes shared by all Npgsql projects

[assembly: CLSCompliant(false)]
[assembly: AllowPartiallyTrustedCallers()]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
[assembly: AssemblyCompany("Npgsql Development Team")]
[assembly: AssemblyProduct("Npgsql")]
[assembly: AssemblyCopyright("Copyright © 2002 - 2016 Npgsql Development Team")]
[assembly: AssemblyTrademark("")]
[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.MainAssembly)]

// The following version attributes get rewritten by GitVersion as part of the build
[assembly: AssemblyVersion("3.1.2")]
[assembly: AssemblyFileVersion("3.1.2")]
[assembly: AssemblyInformationalVersion("3.1.2-ci")]
