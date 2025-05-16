using WebRtcLogAnalyzer.Domain.Events;

namespace WebRtcLogAnalyzer.Application.DTOs
{
    public class LogAnalysisResult
    {
        // Number of unique users
        public int UniqueUsers { get; set; }

        // List of user activity events (JOIN/LEAVE)
        public List<UserActivityEvent> UserActivity { get; set; }

        // Error counts by type (ERROR, CRITICAL, WARNING)
        public Dictionary<string, int> Errors { get; set; }

        // Constructor to initialize collections
        public LogAnalysisResult()
        {
            UserActivity = new List<UserActivityEvent>();
            Errors = new Dictionary<string, int>();
        }
    }
}
