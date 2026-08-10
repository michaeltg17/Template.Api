using Application.Features.Images;
using AwesomeAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace IntegrationTests.Infrastructure
{
    public class ImageApiMock : ApiMock
    {
        public ImageApiMock() : base()
        {
            SetMocks();
        }

        void SetMocks()
        {
            Server
                .Given(Request.Create()
                    .WithHeader(ImageService.ImageApiKeyHeaderName, Test.ApiKey)
                    .WithPath($"/{ImageService.ApiPath}/*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(201));

            Server
                .Given(Request.Create()
                    .WithHeader(ImageService.ImageApiKeyHeaderName, Test.ApiKey)
                    .WithPath($"/{ImageService.ApiPath}/*")
                    .UsingDelete())
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        /// <summary>
        /// Asserts a post request was received and returns the uploaded bytes.
        /// Also sets a GET mock to get those images.
        /// </summary>
        public byte[] AssertPostAndSetGetMock(string imageName, byte[] expectedBody)
        {
            var logEntries = Server.LogEntries;
            var uploadEntry = logEntries
                .LastOrDefault(e => e.RequestMessage != null &&
                           e.RequestMessage.Method == "POST" &&
                           e.RequestMessage.Path == ImageService.BuildPathUrl(imageName).ToString());

            uploadEntry.Should().NotBeNull($"upload of '{imageName}' should have been sent");
            var requestMessage = uploadEntry!.RequestMessage!;

            // Verify body
            var uploadedBody = requestMessage.BodyAsBytes!;
            uploadedBody.Should().NotBeNull("uploaded body should not be null");
            uploadedBody.Should().BeEquivalentTo(expectedBody, "uploaded body should match expected bytes");

            SetGetMock(imageName, uploadedBody);

            return uploadedBody;
        }

        public void SetGetMock(string imageName, byte[] imageData)
        {
            Server
                .Given(Request.Create()
                    .WithPath(ImageService.BuildPathUrl(imageName).ToString())
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(imageData));
        }

        public void AssertGetRequest(string imageName)
        {
            var logEntries = Server.LogEntries;
            var downloadEntries = logEntries
                .Where(e => e.RequestMessage != null &&
                           e.RequestMessage.Method == "GET" &&
                           e.RequestMessage.Path == ImageService.BuildPathUrl(imageName).ToString());

            downloadEntries.Should().NotBeEmpty($"get of '{imageName}' should have occurred");
        }

        public void AssertDeleteRequests(IEnumerable<string> imageNames)
        {
            foreach (var imageName in imageNames)
            {
                var logEntries = Server.LogEntries;
                var deleteEntries = logEntries
                    .Where(e => e.RequestMessage != null &&
                               e.RequestMessage.Method == "DELETE" &&
                               e.RequestMessage.Path == ImageService.BuildPathUrl(imageName).ToString());

                deleteEntries.Should().NotBeEmpty($"delete of '{imageName}' should have occurred");
            }
        }
    }
}