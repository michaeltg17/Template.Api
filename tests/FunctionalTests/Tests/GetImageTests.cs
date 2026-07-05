using ApiClient.Extensions;
using Core.Testing.Models;
using Core.Testing.Validators;
using FluentAssertions;
using Xunit;

namespace FunctionalTests.Tests
{
    public class GetImageTests : Test
    {
        [Fact(Skip = "Will be deleted")]
        public async Task GivenImageGroup_WhenGetImage_IsGot()
        {
            //Given
            const string imagePath = @"Images\didi.jpeg";
            var imageGroup = await ApiClient.Api.SaveImageGroup(imagePath).To<ImageGroup>();

            //When
            var image = await ApiClient.Api.GetImage(imageGroup.Images.First().Id).To<Image>();

            //Then
            var uploadedImageBytes = File.ReadAllBytes(imagePath);
            var downloadedImageBytes = await ApiClient.HttpClient.GetByteArrayAsync(image!.Url);

            uploadedImageBytes.Should().BeEquivalentTo(downloadedImageBytes);
        }

        [Fact]
        public async Task GivenUnexistingImage_WhenGetImage_ExpectedProblemDetails()
        {
            //When
            const long id = 600;
            var response = await ApiClient.Api.GetImage(id: 600);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "Image", id);
        }
    }
}
