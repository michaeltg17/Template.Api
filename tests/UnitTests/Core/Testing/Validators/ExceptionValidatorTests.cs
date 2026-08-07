using Xunit;
using Core.Testing.Validators;
using AwesomeAssertions;

namespace UnitTests.Core.Testing.Validators;

public sealed class ExceptionValidatorTests
{
    public static readonly TheoryDataRow<string?, bool>[] TestCases =
    [
        new("System.Exception: Sensitive data\r\n   at Api.Endpoints.Test.ThrowInternalServerErrorEndpoint.<>c.<Map>b__0_0() in E:\\1\\Repos\\Test\\ThrowInternalServerErrorEndpoint.cs:line 11\r\n   at lambda_method16(Closure, Object, HttpContext)\r\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)", true) { TestDisplayName = "Valid: source file + lambda" },
        new("System.Exception: Sensitive data\r\n   at Api.Endpoints.Test.ThrowInternalServerErrorEndpoint.<>c.<Map>b__0_0() in E:\\test.cs:line 11\r\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)", true) { TestDisplayName = "Valid: source file + generic" },
        new("System.Exception: Sensitive data\r\n   at lambda_method16(Closure, Object, HttpContext)", true) { TestDisplayName = "Valid: lambda only" },
        new("System.Exception: Sensitive data\r\n   at Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddlewareImpl.<Invoke>g__Awaited|10_0(ExceptionHandlerMiddlewareImpl middleware, HttpContext context, Task task)", true) { TestDisplayName = "Valid: compiler-generated" },
        new("System.Exception: Sensitive data\r\n   at ValidSource.Method() in C:\\path\\to\\File.cs:line 42", true) { TestDisplayName = "Valid: source location" },
        new("System.Exception: Sensitive data\r\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)\r\n   at ValidSource.Method() in C:\\path\\to\\File.cs:line 42", true) { TestDisplayName = "Valid: mixed stack + source" },
        new("", false) { TestDisplayName = "Invalid: empty" },
        new(null, false) { TestDisplayName = "Invalid: null" },
        new("   ", false) { TestDisplayName = "Invalid: whitespace" },
        new("random text", false) { TestDisplayName = "Invalid: random text" },
        new("System.Exception: Sensitive data", false) { TestDisplayName = "Invalid: no stack trace" },
        new("System.Exception: Sensitive data\r\n   at NoSourceInfo.Method()", false) { TestDisplayName = "Invalid: stack but no source/lambda" },
    ];

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Cases(string? exceptionText, bool expected)
    {
        //When
        var result = ExceptionValidator.IsValid(exceptionText!);

        //Then
        result.Should().Be(expected);
    }
}