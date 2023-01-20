#if NETSTANDARD2_0

namespace System.Runtime.CompilerServices;

static class RuntimeFeature
{
    public static bool IsDynamicCodeSupported => true;
}

#endif
