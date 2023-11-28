using OperationResult;
using System.Reflection;

namespace HandlingErrors.Core.RequestHandlers.Pipelines;

public static class ResultUtils
{
    internal static MethodInfo ResultError { get; }
    internal static Type TypeResultGeneric { get; } = typeof(Result<>);
    internal static Type TypeResult { get; } = typeof(Result);

    static ResultUtils()
        => ResultError = TypeResult.GetMethods().First(m => m.Name == "Error" && m.IsGenericMethod);

    internal static MethodInfo? GetGenericError(Type type)
        => type.IsGenericType ? ResultError.MakeGenericMethod(type.GetGenericArguments().First()) : null;
}
