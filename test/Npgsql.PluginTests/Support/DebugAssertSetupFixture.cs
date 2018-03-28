using NUnit.Framework;

#if NET451 || NET45
// ReSharper disable once CheckNamespace
[SetUpFixture]
public class PluginsDebugAssertSetupFixture : DebugAssertSetupFixture {}
#endif
