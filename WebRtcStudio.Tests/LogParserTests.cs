using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebRtcLogAnalyzer.Infrastructure.Parsing;
using WebRtcLogAnalyzer.Application.DTOs;
using Xunit;

namespace WebRtcLogAnalyzer.Tests
{
    public class LogParserTests
    {
        private readonly LogParser _logParser;

        public LogParserTests()
        {
            _logParser = new LogParser();
        }

        [Fact]
        public async Task AnalyzeAsync_ValidLogFile_Returns_CorrectResult()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var rootDirectory = Directory.GetParent(currentDirectory).Parent.FullName;
            var logFilePath = Path.Combine(rootDirectory, "../../logs", "webrtc_studio.log");

            // Act
            var result = await _logParser.AnalyzeAsync(logFilePath);

            // Assert
            Assert.True(result.UniqueUsers > 0);
            Assert.NotEmpty(result.UserActivity);
            Assert.True(result.Errors.Any(e => e.Key == "ERROR" && e.Value > 0),
                "Expected to find an 'ERROR' key with a value greater than 0.");
            Assert.True(result.Errors.Any(e => e.Key == "CRITICAL" && e.Value > 0),
                            "Expected to find an 'CRITICAL' key with a value greater than 0.");
            Assert.True(result.Errors.Any(e => e.Key == "WARNING" && e.Value > 0),
                "Expected to find an 'WARNING' key with a value greater than 0.");
        }

        [Fact]
        public async Task AnalyzeAsync_LogFileNotFound_Returns_EmptyResult()
        {
            // Arrange
            var logFilePath = "invalid_file_path.log"; // Invalid path

            // Act
            var result = await _logParser.AnalyzeAsync(logFilePath);

            // Assert
            Assert.Equal(0, result.UniqueUsers); // Should return 0 unique users since file is not found
            Assert.Empty(result.UserActivity); // No activity should be returned
            Assert.Empty(result.Errors); // No errors should be counted
        }
    }
}
