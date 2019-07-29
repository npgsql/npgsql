#if NET461 || NETSTANDARD2_0
#pragma warning disable 1591
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    sealed partial class AllowNullAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    sealed partial class DisallowNullAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Method, Inherited=false)]
    sealed partial class DoesNotReturnAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter, Inherited=false)]
    sealed partial class DoesNotReturnIfAttribute : Attribute
    {
        public DoesNotReturnIfAttribute(bool parameterValue) => ParameterValue = parameterValue;
        public bool ParameterValue { get; }
    }

    [AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    sealed partial class ExcludeFromCodeCoverageAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    sealed partial class MaybeNullAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter, Inherited=false)]
    sealed partial class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }

    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    sealed partial class NotNullAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    sealed partial class NotNullIfNotNullAttribute : Attribute
    {
        public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;
        public string ParameterName { get; }
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter, Inherited=false)]
    sealed partial class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }
}
#endif
