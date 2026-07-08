using FluentAssertions;
using System.Collections.Generic;
using Xunit;
using Core.Testing.Builders;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ApiClient.Extensions;
using Core.Testing.Extensions;
using Core.Testing;
using Api.Models.Requests;

namespace IntegrationTests.Tests.Api
{
    [Collection(nameof(ApiCollection))]
    public class ApiBehaviourTests : Test
    {
        [Fact]
        public async Task WhenInternalServerError_ExpectedProblemDetails()
        {
            //When
            var response = await ApiClient.Test.ThrowInternalServerError();

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            var traceId = problemDetails.GetTraceId();
            TraceIdValidator.IsValid(traceId).Should().BeTrue();

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(traceId)
                .WithInternalServerError("/Test/ThrowInternalServerError")
                .Build();

            var responseAsString = (await response.Content.ReadAsStringAsync()).ToLowerInvariant();
            responseAsString.Should().NotContain("Sensitive data".ToLowerInvariant());
            responseAsString.Should().NotContain("Exception".ToLowerInvariant());
            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task WhenNonexistentRoute_ExpectedProblemDetails()
        {
            //When
            var response = await ApiClient.Test.RequestUnexistingRoute();

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            var traceId = problemDetails.GetTraceId();
            TraceIdValidator.IsValid(traceId).Should().BeTrue();

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(traceId)
                .WithNotFound()
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenGoodPostRequest_Ok()
        {
            //When
            var response = await ApiClient.Test.Post(1L, new DateTime(2020, 1, 1), new TestPostRequest { Id2 = 2L });

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public static IEnumerable<object[]> BindingFailureBodyCases() =>
[
    new object[] { (long)1, "2020-01-01", new Dictionary<string, object?> { ["id2"] = "notanumber" }, "Failed to read parameter \"TestPostRequest request\" from the request body as JSON." },
        ];

        [Theory]
        [InlineData("a", null, null, "Failed to bind parameter \"long id\" from \"a\".")]
        [InlineData((long)1, "b", null, "Failed to bind parameter \"DateTime date\" from \"b\".")]
        [InlineData((long)1, "2020-01-01", null, "Required parameter \"TestPostRequest request\" was not provided from body.")]
        [InlineData((long)1, "2020-01-01", "x", "Failed to read parameter \"TestPostRequest request\" from the request body as JSON.")]
        [MemberData(nameof(BindingFailureBodyCases))]
        public async Task WhenBadRequest_ExpectedProblemDetails(
            object id, object date, object? request, string expectedDetail)
        {
            //When
            var response = await ApiClient.Test.Post(id, date, request);

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            var traceId = problemDetails.GetTraceId();
            TraceIdValidator.IsValid(traceId).Should().BeTrue();

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(traceId)
                .WithBadHttpRequestException()
                .WithInstance(problemDetails.Instance!)
                .WithDetail(expectedDetail)
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}