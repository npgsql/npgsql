#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    class NotNullWhenTrueAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Parameter)]
    class EnsuresNotNullAttribute : Attribute {}
}
