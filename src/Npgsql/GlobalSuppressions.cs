
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "Internal exceptions are used in several places within Npgsql, other public exceptions are thrown as a result.")]
[assembly: SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "We have several exception classes where this makes no sense")]
[assembly: SuppressMessage("Design", "CA1710:Identifiers should have correct suffix", Justification = "Disagree")]
[assembly: SuppressMessage("Design", "CA1707:Remove the underscores from member name", Justification = "Seems to cause some false positives on implicit/explicit cast operators, strange")]
[assembly: SuppressMessage("Reliability", "CA2007:Do not directly await a Task", Justification = "Npgsql uses NoSynchronizationContextScope instead of ConfigureAwait(false)")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "All I/O methods are both sync and async, avoid clutter")]

