using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("NpgsqlVSIX")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following is necessary to make Visual Studio load our new version of Npgsql.
// Specifically, the EF6 provider usuall depends on some old Npgsql, and trying to generate an EDM
// model from an existing database will fail because of this. The following line redirects the EF6
// to use our own Npgsql instead.
[assembly: ProvideBindingRedirection(AssemblyName = "Npgsql", NewVersion = "4.0.2.0", OldVersionLowerBound = "0.0.0.0", OldVersionUpperBound = "4.0.2.0")]
