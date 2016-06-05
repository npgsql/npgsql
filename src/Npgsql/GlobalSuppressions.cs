
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1064:Exceptions should be public", Justification = "Internal exceptions areused in several places within Npgsql, other public exceptions are thrown as a result.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "We have several exception classes where this makes no sense")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1710:Identifiers should have correct suffix", Justification = "Disagree")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1707:Remove the underscores from member name", Justification = "Seems to cause some false positives on implicit/explicit cast operators, strange")]
