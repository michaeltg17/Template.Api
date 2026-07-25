using AwesomeAssertions;
using System.Collections.Generic;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace IntegrationTests.Infrastructure
{
    public class ImageApiMock
    {
        readonly WireMockServer _server;

        public ImageApiMock()
        {
            _server = WireMockServer.Start(new WireMockServerSettings
            {
                StartAdminInterface = false
            });
            SetupStubs();
        }

        public string BaseUrl => _server.Urls[0];

        void SetupStubs()
        {
            _server
                .Given(Request.Create().WithPath("/api/v1/images/*").UsingPost())
                .RespondWith(Response.Create().WithStatusCode(201));
        }

        // Verifies an upload request was received, returns the uploaded bytes for assertions.
        // Also registers a GET stub to return those bytes for subsequent download validation.
        public byte[] VerifyUploadAndStore(string imageName, byte[] expectedBody)
        {
            var logEntries = _server.LogEntries;
            var uploadEntry = logEntries
                .Where(e => e.RequestMessage != null &&
                           e.RequestMessage.Method == "POST" &&
                           e.RequestMessage.Path == $"/api/v1/images/{imageName}")
                .LastOrDefault();

            uploadEntry.Should().NotBeNull($"upload of '{imageName}' should have been sent");
            var requestMessage = uploadEntry!.RequestMessage!;

            // Verify Api-Key header
            var apiKeys = requestMessage.Headers!["X-Api-Key"];
            apiKeys.Should().NotBeNullOrEmpty("Api-Key header should be present");
            apiKeys[0].Should().Be("test-api-key", "Api-Key should match test value");

            // Verify body
            var uploadedBody = requestMessage.BodyAsBytes!;
            uploadedBody.Should().NotBeNull("uploaded body should not be null");
            uploadedBody.Should().BeEquivalentTo(expectedBody, "uploaded body should match expected bytes");

            // Register GET stub to return these bytes
            _server
                .Given(Request.Create().WithPath($"/api/v1/images/{imageName}").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(200)
                    .WithBody(uploadedBody));

            return uploadedBody;
        }

        // Verifies a download request was made to this image
        public void VerifyDownload(string imageName)
        {
            var logEntries = _server.LogEntries;
            var downloadEntries = logEntries
                .Where(e => e.RequestMessage != null &&
                           e.RequestMessage.Method == "GET" &&
                           e.RequestMessage.Path == $"/api/v1/images/{imageName}");

            downloadEntries.Should().NotBeEmpty($"download of '{imageName}' should have occurred");
        }

        // Verifies a delete request was made to this image
        public void VerifyDelete(string imageName)
        {
            var logEntries = _server.LogEntries;
            var deleteEntries = logEntries
                .Where(e => e.RequestMessage != null &&
                           e.RequestMessage.Method == "DELETE" &&
                           e.RequestMessage.Path == $"/api/v1/images/{imageName}");

            deleteEntries.Should().NotBeEmpty($"delete of '{imageName}' should have occurred");
        }

        // Reset log entries for next test
        public void Reset()
        {
            _server.ResetLogEntries();
        }

        // Stop the server
        public void Dispose()
        {
            _server.Dispose();
        }
    }
}