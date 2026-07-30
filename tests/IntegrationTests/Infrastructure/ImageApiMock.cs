using Application.Features.Images;
using AwesomeAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace IntegrationTests.Infrastructure
{
    internal class ImageApiMock
    {
        public readonly WireMockServer Server;

        public ImageApiMock()
        {
            Server = WireMockServer.Start();
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
        /// Validates a post request was received and returns the uploaded bytes.
        /// Also sets a GET mock to get those images.
        /// </summary>
        public byte[] ValidatePostAndSetGetMock(string imageName, byte[] expectedBody)
        {
            var logEntries = Server.LogEntries;
            var uploadEntry = logEntries
                .LastOrDefault(e => e.RequestMessage != null &&
                           e.RequestMessage.Method == "POST" &&
                           e.RequestMessage.Path == ImageService.BuildPathUrl(imageName));

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
                    .WithPath(ImageService.BuildPathUrl(imageName))
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(imageData));
        }

        public void ValidateGetRequest(string imageName)
        {
            var logEntries = Server.LogEntries;
            var downloadEntries = logEntries
                .Where(e => e.RequestMessage != null &&
                           e.RequestMessage.Method == "GET" &&
                           e.RequestMessage.Path == ImageService.BuildPathUrl(imageName));

            downloadEntries.Should().NotBeEmpty($"get of '{imageName}' should have occurred");
        }

        public void ValidateDeleteRequests(IEnumerable<string> imageNames)
        {
            foreach (var imageName in imageNames)
            {
                var logEntries = Server.LogEntries;
                var deleteEntries = logEntries
                    .Where(e => e.RequestMessage != null &&
                               e.RequestMessage.Method == "DELETE" &&
                               e.RequestMessage.Path == ImageService.BuildPathUrl(imageName));

                deleteEntries.Should().NotBeEmpty($"delete of '{imageName}' should have occurred");
            }
        }
    }
}