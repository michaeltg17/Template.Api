using Core.Testing;
using AwesomeAssertions;
using Xunit;

namespace UnitTests.Core.Testing.Validators
{
    public class ExceptionValidatorTests
    {
        [Theory]
        [InlineData("System.Exception: Sensitive data\r\n   at Api.Endpoints.Test.ThrowInternalServerErrorEndpoint.<>c.<Map>b__0_0() in E:\\1\\Repos\\Test\\ThrowInternalServerErrorEndpoint.cs:line 11\r\n   at lambda_method16(Closure, Object, HttpContext)\r\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)", true)]
        [InlineData("System.Exception: Sensitive data\r\n   at Api.Endpoints.Test.ThrowInternalServerErrorEndpoint.<>c.<Map>b__0_0() in E:\\test.cs:line 11\r\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)", true)]
        [InlineData("System.Exception: Sensitive data\r\n   at lambda_method16(Closure, Object, HttpContext)", true)]
        [InlineData("System.Exception: Sensitive data\r\n   at Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddlewareImpl.<Invoke>g__Awaited|10_0(ExceptionHandlerMiddlewareImpl middleware, HttpContext context, Task task)", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        [InlineData("random text", false)]
        [InlineData("System.Exception: Sensitive data", false)]
        [InlineData("System.Exception: Sensitive data\r\n   at NoSourceInfo.Method()", false)]
        [InlineData("System.Exception: Sensitive data\r\n   at ValidSource.Method() in C:\\path\\to\\File.cs:line 42", true)]
        [InlineData("System.Exception: Sensitive data\r\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.Invoke(HttpContext httpContext)\r\n   at ValidSource.Method() in C:\\path\\to\\File.cs:line 42", true)]
        public void IsValid_ShouldMatchExpected(string? exceptionText, bool expected)
        {
            ExceptionValidator.IsValid(exceptionText!).Should().Be(expected);
        }
    }
}