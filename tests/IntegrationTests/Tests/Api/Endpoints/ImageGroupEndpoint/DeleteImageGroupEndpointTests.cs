using ApiClient.Extensions;
using Core.Testing.Models;
using Core.Testing.Validators;
using FluentAssertions;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.ImageGroupEndpoint
{
    [Collection(nameof(ApiCollection))]
    public class DeleteImageGroupEndpointTests : Test
    {
        [InlineData(1)]
        [InlineData(2)]
        [Theory]
        public async Task GivenImageGroup_WhenDelete_IsDeleted(int version)
        {
            //Given
            const string imagePath = @"Images\didi.jpeg";
            var imageGroup = await ApiClient.Api.SaveImageGroup(imagePath).To<ImageGroup>();
            var imageGroup2 = await ApiClient.Api.GetImageGroup(imageGroup.Id).To<ImageGroup>();
            imageGroup.Should().BeEquivalentTo(imageGroup2);

            //When
            var deleteResponse = await ApiClient.Api.DeleteImageGroup(imageGroup.Id, version);

            //Then
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResponse = await ApiClient.Api.GetImageGroup(imageGroup.Id);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenDeleteNonexistentImageGroup_ExpectedProblemDetails()
        {
            //When
            const long id = 600;
            var response = await ApiClient.Api.DeleteImageGroup(id);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "ImageGroup", id);
        }
    }
}
