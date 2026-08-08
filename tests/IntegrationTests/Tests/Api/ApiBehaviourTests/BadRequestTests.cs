using AwesomeAssertions;
using Xunit;
using Core.Testing.Builders;
using Core.Testing.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ApiClient.Extensions;
using IntegrationTests.Collections;
using Core.Testing.Validators;
using static IntegrationTests.Tests.Api.ApiBehaviourTests.BadRequestTests;
using Core.Testing.Serializers;
using Xunit.Sdk;

[assembly: RegisterXunitSerializer(typeof(TestCaseSerializer), typeof(BadRequestCase))]

namespace IntegrationTests.Tests.Api.ApiBehaviourTests
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class BadRequestTests : Test
    {
        public class BadRequestCase
        {
            public object Id;
            public object Date;
            public object? Request;
            public string ExpectedInstance;
            public string ExpectedDetail;
        }

        public static TheoryData<BadRequestCase> TestCases()
        {
            var row1 = new TheoryDataRow<BadRequestCase>(new BadRequestCase
            {
                Id = "a", Date = null!, Request = null!,
                ExpectedInstance = "/Test/Post/a",
                ExpectedDetail = "Failed to bind parameter \"long id\" from \"a\"."
            }) { TestDisplayName = "Invalid: route parameter id cannot be parsed as long" };
            var row2 = new TheoryDataRow<BadRequestCase>(new BadRequestCase
            {
                Id = (long)1, Date = "b", Request = null!,
                ExpectedInstance = "/Test/Post/1",
                ExpectedDetail = "Failed to bind parameter \"DateTime date\" from \"b\"."
            }) { TestDisplayName = "Invalid: query string date cannot be parsed as DateTime" };
            var row3 = new TheoryDataRow<BadRequestCase>(new BadRequestCase
            {
                Id = (long)1, Date = "2020-01-01", Request = null!,
                ExpectedInstance = "/Test/Post/1",
                ExpectedDetail = "Required parameter \"PostRequest request\" was not provided from body."
            }) { TestDisplayName = "Missing: body not provided" };
            var row4 = new TheoryDataRow<BadRequestCase>(new BadRequestCase
            {
                Id = (long)1, Date = "2020-01-01", Request = "x",
                ExpectedInstance = "/Test/Post/1",
                ExpectedDetail = "Failed to read parameter \"PostRequest request\" from the request body as JSON. The JSON value could not be converted to Api.Endpoints.TestEndpoints+PostRequest. Path: $ | LineNumber: 0 | BytePositionInLine: 3."
            }) { TestDisplayName = "Invalid: body cannot be converted to expected type" };
            var row5 = new TheoryDataRow<BadRequestCase>(new BadRequestCase
            {
                Id = (long)1, Date = "2020-01-01",
                Request = new Dictionary<string, object?> { ["id2"] = "notanumber" },
                ExpectedInstance = "/Test/Post/1",
                ExpectedDetail = "Failed to read parameter \"PostRequest request\" from the request body as JSON. The JSON value could not be converted to System.Int64. Path: $.id2 | LineNumber: 0 | BytePositionInLine: 19."
            }) { TestDisplayName = "Invalid: body property value cannot be converted to expected type" };

            return new TheoryData<BadRequestCase> { row1, row2, row3, row4, row5 };
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task Cases(BadRequestCase testCase)
        {
            //When
            var response = await ApiClient.Test.Post(testCase.Id, testCase.Date, testCase.Request);

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            TraceIdValidator.IsValid(problemDetails.TraceId!).Should().BeTrue();

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(problemDetails.TraceId!)
                .WithBadHttpRequestException()
                .WithInstance(testCase.ExpectedInstance)
                .WithDetail(testCase.ExpectedDetail)
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}