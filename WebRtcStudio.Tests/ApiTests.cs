using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebRtcLogAnalyzer.Application.DTOs;
using Xunit;
namespace WebRtcLogAnalyzer.Api.Tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>> // WebApplicationFactory is now using Program
    {
        private readonly HttpClient _client;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            // Create HttpClient using the WebApplicationFactory (which uses Program.cs)
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_LogAnalysis_Returns_200_OK_With_Correct_Data()
        {
            // Arrange
            var url = "/api/loganalysis";

            // Act
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Ensure 200-299 status code

            // Read the response content
            var content = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON into C# models
            var result = JsonSerializer.Deserialize<LogAnalysisResult>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This allows case-insensitive deserialization
            });

            // Assert: Check correct properties in the response

            // Check if the 'uniqueUsers' property exists and is correct
            Assert.NotNull(result);
            Assert.True(result.UniqueUsers > 1, $"Expected UniqueUsers to be greater than 1, but got {result.UniqueUsers}.");


            // Check the 'userActivity' list
            Assert.NotNull(result.UserActivity);
            Assert.Contains(result.UserActivity, ua => ua.UserId == "152");
            Assert.Contains(result.UserActivity, ua => ua.Event == "JOIN");
            var expectedTimestamp = DateTime.Parse("2023-10-27 10:13:46");
            Assert.Contains(result.UserActivity, ua =>
            {
                // Try parsing the timestamp string into a DateTime and compare
                if (DateTime.TryParse(ua.Timestamp.ToString(), out DateTime parsedTimestamp))
                {
                    return parsedTimestamp == expectedTimestamp;
                }
                return false;
            });

            // Check the 'errors' property
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Any(e => e.Key == "ERROR" && e.Value > 0),
                "Expected to find an 'ERROR' key with a value greater than 0.");
            Assert.True(result.Errors.Any(e => e.Key == "CRITICAL" && e.Value > 0),
                            "Expected to find an 'CRITICAL' key with a value greater than 0.");
            Assert.True(result.Errors.Any(e => e.Key == "WARNING" && e.Value > 0),
                "Expected to find an 'WARNING' key with a value greater than 0.");
        }

        [Fact]
        public async Task Get_LogAnalysis_FileNotFound_Returns_404()
        {
            // Arrange
            var url = "/api/loganalysis-not-found";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode); // Ensure 404 is returned when file is not found
        }
    }
}
