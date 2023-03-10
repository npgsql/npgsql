using Npgsql.BackendMessages;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal;

namespace Npgsql;

static class ThrowHelper
{
    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException()
        => throw new ArgumentOutOfRangeException();

    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException(string paramName, string message)
        => throw new ArgumentOutOfRangeException(paramName, message);

    [DoesNotReturn]
    internal static void ThrowArgumentOutOfRangeException(string paramName, string message, object argument)
        => throw new ArgumentOutOfRangeException(paramName, string.Format(message, argument));

    [DoesNotReturn]
    internal static void ThrowInvalidOperationException()
        => throw new InvalidOperationException();

    [DoesNotReturn]
    internal static void ThrowInvalidOperationException(string message)
        => throw new InvalidOperationException(message);

    [DoesNotReturn]
    internal static void ThrowInvalidOperationException(string message, object argument)
        => throw new InvalidOperationException(string.Format(message, argument));

    [DoesNotReturn]
    internal static void ThrowObjectDisposedException(string? objectName)
        => throw new ObjectDisposedException(objectName);

    [DoesNotReturn]
    internal static void ThrowObjectDisposedException(string objectName, string message)
        => throw new ObjectDisposedException(objectName, message);

    [DoesNotReturn]
    internal static void ThrowObjectDisposedException(string objectName, Exception? innerException)
        => throw new ObjectDisposedException(objectName, innerException);

    [DoesNotReturn]
    internal static void ThrowInvalidCastException(string message, object argument)
        => throw new InvalidCastException(string.Format(message, argument));

    [DoesNotReturn]
    internal static void ThrowInvalidCastException_NoValue(FieldDescription field) =>
        throw new InvalidCastException($"Column '{field.Name}' is null.");

    [DoesNotReturn]
    internal static void ThrowInvalidOperationException_NoPropertyGetter(Type type, MemberInfo property) =>
        throw new InvalidOperationException($"Composite type '{type}' cannot be written because the '{property}' property has no getter.");

    [DoesNotReturn]
    internal static void ThrowInvalidOperationException_NoPropertySetter(Type type, MemberInfo property) =>
        throw new InvalidOperationException($"Composite type '{type}' cannot be read because the '{property}' property has no setter.");

    [DoesNotReturn]
    internal static void ThrowInvalidOperationException_BinaryImportParametersMismatch(int columnCount, int valueCount) =>
        throw new InvalidOperationException($"The binary import operation was started with {columnCount} column(s), but {valueCount} value(s) were provided.");

    [DoesNotReturn]
    internal static void ThrowNpgsqlException(string message)
        => throw new NpgsqlException(message);

    [DoesNotReturn]
    internal static void ThrowNpgsqlException(string message, Exception? innerException)
        => throw new NpgsqlException(message, innerException);

    [DoesNotReturn]
    internal static void ThrowNpgsqlOperationInProgressException(NpgsqlCommand command)
        => throw new NpgsqlOperationInProgressException(command);
    
    [DoesNotReturn]
    internal static void ThrowNpgsqlOperationInProgressException(ConnectorState state)
        => throw new NpgsqlOperationInProgressException(state);

    [DoesNotReturn]
    internal static void ThrowArgumentException(string message)
        => throw new ArgumentException(message);

    [DoesNotReturn]
    internal static void ThrowArgumentException(string message, string paramName)
        => throw new ArgumentException(message, paramName);

    [DoesNotReturn]
    internal static void ThrowArgumentNullException(string paramName)
        => throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    internal static void ThrowIndexOutOfRangeException(string message)
        => throw new IndexOutOfRangeException(message);

    [DoesNotReturn]
    internal static void ThrowNotSupportedException(string message)
        => throw new NotSupportedException(message);

    [DoesNotReturn]
    internal static void ThrowNpgsqlExceptionWithInnerTimeoutException(string message)
        => throw new NpgsqlException(message, new TimeoutException());
}