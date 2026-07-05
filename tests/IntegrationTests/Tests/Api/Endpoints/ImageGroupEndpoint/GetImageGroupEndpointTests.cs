using ApiClient.Extensions;
using Core.Testing.Models;
using Core.Testing.Validators;
using FluentAssertions;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.ImageGroupEndpoint
{
    [Collection(nameof(ApiCollection))]
    public class GetImageGroupEndpointTests : Test
    {
        [Fact]
        public async Task GivenImageGroup_WhenSaveAndGetImageGroup_IsGot()
        {
            //Given
            const string imagePath = @"Images\didi.jpeg";
            var imageGroup = await ApiClient.Api.SaveImageGroup(imagePath).To<ImageGroup>();

            //When
            var response = await ApiClient.Api.GetImageGroup(imageGroup.Id);
            var imageGroup2 = await response.To<ImageGroup>();

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            imageGroup.Should().BeEquivalentTo(imageGroup2);
            imageGroup.Images.Should().HaveCount(3);
        }

        [Fact]
        public async Task WhenGetNonexistentImageGroup_ExpectedProblemDetails()
        {
            //When
            const long id = 600;
            var response = await ApiClient.Api.GetImageGroup(id);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "ImageGroup", id);
        }
    }
}
