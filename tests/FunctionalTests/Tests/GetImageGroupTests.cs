using ApiClient.Extensions;
using Core.Testing.Models;
using Core.Testing.Validators;
using FluentAssertions;
using System.Net;
using Xunit;

namespace FunctionalTests.Tests
{
    public class GetImageGroupTests : Test
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
        }

        [Fact]
        public async Task GivenUnexistingImageGroup_WhenGetImageGroup_ExpectedProblemDetails()
        {
            //When
            const long id = 600;
            var response = await ApiClient.Api.GetImageGroup(id: 600);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "ImageGroup", id);
        }
    }
}
