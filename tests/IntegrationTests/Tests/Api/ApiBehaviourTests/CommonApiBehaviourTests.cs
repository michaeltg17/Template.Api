using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Extensions;
using Core.Testing.Validators;
using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;
using static Api.Endpoints.TestEndpoints;

namespace IntegrationTests.Tests.Api.ApiBehaviourTests
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class CommonApiBehaviourTests(TestFixture testFixture) : Test(testFixture)
    {
        [Fact]
        public async Task NonexistentRoute_ExpectedProblemDetails()
        {
            //When
            var response = await ApiClient.Test.RequestUnexistingRoute();

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            TraceIdValidator.IsValid(problemDetails.TraceId!).Should().BeTrue();

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(problemDetails.TraceId!)
                .WithNotFound()
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ValidRequest_Ok()
        {
            //When
            var response = await ApiClient.Test.Post(1L, new DateTime(2020, 1, 1), new PostRequest(2L));

            //Then
            await response.ValidateOrThrow(HttpStatusCode.OK);
        }
    }
}