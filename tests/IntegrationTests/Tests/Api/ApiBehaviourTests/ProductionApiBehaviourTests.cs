//using ApiClient.Extensions;
//using AwesomeAssertions;
//using Core.Testing;
//using Core.Testing.Builders;
//using Core.Testing.Extensions;
//using IntegrationTests.Collections;
//using Microsoft.AspNetCore.Mvc;
//using System.Net;
//using Xunit;

//namespace IntegrationTests.Tests.Api.ApiBehaviourTests
//{
//    [Collection(nameof(ProductionApiCollection))]
//    public class ProductionApiBehaviourTests : Test
//    {
//        [Fact]
//        public async Task InternalServerError_HidesSensitiveData()
//        {
//            //When
//            var response = await ApiClient.Test.ThrowInternalServerError();

//            //Then
//            var problemDetails = await response.To<ProblemDetails>();
//            TraceIdValidator.IsValid(problemDetails.TraceId!).Should().BeTrue();

//            var expected = new ProblemDetailsBuilder()
//                .WithTraceId(problemDetails.TraceId!)
//                .WithHiddenInternalServerError("/Test/ThrowInternalServerError")
//                .Build();

//            problemDetails.Should().BeEquivalentTo(expected);
//            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
//        }
//    }
//}