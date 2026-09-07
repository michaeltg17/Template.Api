using Microsoft.AspNetCore.Mvc;

namespace Core.Testing.Extensions
{
    public static class ProblemDetailsExtensions
    {
        public static string? TraceId(this ProblemDetails problemDetails)
        {
            ArgumentNullException.ThrowIfNull(problemDetails);
            return problemDetails.Extensions["traceId"] as string;
        }

        public static string? Exception(this ProblemDetails problemDetails)
        {
            ArgumentNullException.ThrowIfNull(problemDetails);
            return problemDetails.Extensions["exception"] as string;
        }
    }
}
