#if NETSTANDARD2_0

#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    sealed class AllowNullAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    sealed class DisallowNullAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Method)]
    sealed class DoesNotReturnAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter)]
    sealed class DoesNotReturnIfAttribute : Attribute
    {
        public DoesNotReturnIfAttribute(bool parameterValue) => ParameterValue = parameterValue;
        public bool ParameterValue { get; }
    }

    [AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct, AllowMultiple = false)]
    sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    sealed class MaybeNullAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter)]
    sealed class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }

    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    sealed class NotNullAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
    sealed class NotNullIfNotNullAttribute : Attribute
    {
        public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;
        public string ParameterName { get; }
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter)]
    sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
        public bool ReturnValue { get; }
    }
}
#endif
