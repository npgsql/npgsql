using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable 1591

namespace System.Diagnostics.CodeAnalysis
{
#if !NET7_0_OR_GREATER
    /// <summary>
    /// Indicates that the specified method requires the ability to generate new code at runtime,
    /// for example through <see cref="System.Reflection"/>.
    /// </summary>
    /// <remarks>
    /// This allows tools to understand which methods are unsafe to call when compiling ahead of time.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
    sealed class RequiresDynamicCodeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresDynamicCodeAttribute"/> class
        /// with the specified message.
        /// </summary>
        /// <param name="message">
        /// A message that contains information about the usage of dynamic code.
        /// </param>
        public RequiresDynamicCodeAttribute(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets a message that contains information about the usage of dynamic code.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets or sets an optional URL that contains more information about the method,
        /// why it requires dynamic code, and what options a consumer has to deal with it.
        /// </summary>
        public string? Url { get; set; }
    }

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    sealed class SetsRequiredMembersAttribute : Attribute
    {
    }
    [AttributeUsageAttribute(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    sealed class UnscopedRefAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnscopedRefAttribute"/> class.
        /// </summary>
        public UnscopedRefAttribute() { }
    }
#endif
#if NETSTANDARD2_0
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
#endif

#if !NET5_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    sealed class MemberNotNullAttribute : Attribute
    {
        public MemberNotNullAttribute(string member) => Members = new string[]
        {
            member
        };

        public MemberNotNullAttribute(params string[] members) => Members = members;

        public string[] Members { get; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    sealed class MemberNotNullWhenAttribute : Attribute
    {
        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            ReturnValue = returnValue;
            Members = new string[1] { member };
        }

        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            ReturnValue = returnValue;
            Members = members;
        }

        public bool ReturnValue { get; }

        public string[] Members { get; }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
    sealed class RequiresUnreferencedCodeAttribute : Attribute
    {
        public RequiresUnreferencedCodeAttribute(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public string? Url { get; set; }
    }

    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter |
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method |
        AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct,
        Inherited = false)]
    sealed class DynamicallyAccessedMembersAttribute : Attribute
    {
        public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
        {
            MemberTypes = memberTypes;
        }

        public DynamicallyAccessedMemberTypes MemberTypes { get; }
    }

    [Flags]
    enum DynamicallyAccessedMemberTypes
    {
        None = 0,
        PublicParameterlessConstructor = 0x0001,
        PublicConstructors = 0x0002 | PublicParameterlessConstructor,
        NonPublicConstructors = 0x0004,
        PublicMethods = 0x0008,
        NonPublicMethods = 0x0010,
        PublicFields = 0x0020,
        NonPublicFields = 0x0040,
        PublicNestedTypes = 0x0080,
        NonPublicNestedTypes = 0x0100,
        PublicProperties = 0x0200,
        NonPublicProperties = 0x0400,
        PublicEvents = 0x0800,
        NonPublicEvents = 0x1000,
        Interfaces = 0x2000,
        All = ~None
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class UnconditionalSuppressMessageAttribute : Attribute
    {
        public UnconditionalSuppressMessageAttribute(string category, string checkId)
        {
            Category = category;
            CheckId = checkId;
        }

        public string Category { get; }
        public string CheckId { get; }
        public string? Scope { get; set; }
        public string? Target { get; set; }
        public string? MessageId { get; set; }
        public string? Justification { get; set; }
    }
#endif
}

namespace System.Runtime.CompilerServices
{
#if !NET5_0_OR_GREATER
    static class IsExternalInit {}
#endif
#if !NET7_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    sealed class RequiredMemberAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
        {
            FeatureName = featureName;
        }

        /// <summary>
        /// The name of the compiler feature.
        /// </summary>
        public string FeatureName { get; }

        /// <summary>
        /// If true, the compiler can choose to allow access to the location where this attribute is applied if it does not understand <see cref="FeatureName"/>.
        /// </summary>
        public bool IsOptional { get; init; }

        /// <summary>
        /// The <see cref="FeatureName"/> used for the ref structs C# feature.
        /// </summary>
        public const string RefStructs = nameof(RefStructs);

        /// <summary>
        /// The <see cref="FeatureName"/> used for the required members C# feature.
        /// </summary>
        public const string RequiredMembers = nameof(RequiredMembers);
    }
#endif
}
