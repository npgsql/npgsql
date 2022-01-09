#if !NET6_0_OR_GREATER

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API

// ReSharper disable once CheckNamespace
namespace System.Runtime.Versioning
{
    [AttributeUsage(AttributeTargets.Assembly |
                    AttributeTargets.Module |
                    AttributeTargets.Class |
                    AttributeTargets.Interface |
                    AttributeTargets.Delegate |
                    AttributeTargets.Struct |
                    AttributeTargets.Enum |
                    AttributeTargets.Constructor |
                    AttributeTargets.Method |
                    AttributeTargets.Property |
                    AttributeTargets.Field |
                    AttributeTargets.Event, Inherited = false)]
    public sealed class RequiresPreviewFeaturesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <seealso cref="RequiresPreviewFeaturesAttribute"/> class.
        /// </summary>
        public RequiresPreviewFeaturesAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <seealso cref="RequiresPreviewFeaturesAttribute"/> class with the specified message.
        /// </summary>
        /// <param name="message">An optional message associated with this attribute instance.</param>
        public RequiresPreviewFeaturesAttribute(string? message)
        {
            Message = message;
        }

        /// <summary>
        /// Returns the optional message associated with this attribute instance.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Returns the optional URL associated with this attribute instance.
        /// </summary>
        public string? Url { get; set; }
    }
}

#endif