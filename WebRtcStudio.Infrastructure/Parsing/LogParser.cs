using System.Text.RegularExpressions;
using WebRtcLogAnalyzer.Application.Contracts;
using WebRtcLogAnalyzer.Application.DTOs;
using WebRtcLogAnalyzer.Domain.Events;

namespace WebRtcLogAnalyzer.Infrastructure.Parsing
{
    public sealed class LogParser : ILogAnalysisService
    {
        private static readonly Regex LineRx = new(
            @"\[(?<ts>.*?)\]\s+(?<type>\w+)\s+(?<user>\d+)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public async Task<LogAnalysisResult> AnalyzeAsync(string originLogPath, CancellationToken ct = default)
        {
            if (!File.Exists(originLogPath))
            {
                return new LogAnalysisResult
                {
                    UniqueUsers = 0,
                    UserActivity = new List<UserActivityEvent>(),
                    Errors = new Dictionary<string, int>()
                };
            }

            var userEvents = new List<UserActivityEvent>();
            var errorCounts = new Dictionary<string, int>();

            await foreach (var line in File.ReadLinesAsync(originLogPath, ct))
            {
                var match = LineRx.Match(line);
                if (!match.Success) continue;

                var ts = DateTime.Parse(match.Groups["ts"].Value);
                var raw = match.Groups["type"].Value.Replace("USER_", "");
                var uid = match.Groups["user"].Value;

                if (raw is "JOIN" or "LEAVE")
                {
                    userEvents.Add(new UserActivityEvent(
                        ts,
                        raw,
                        uid));
                }

                if (raw is "ERROR" or "CRITICAL" or "WARNING")
                {
                    errorCounts.TryAdd(raw, 0);
                    errorCounts[raw]++;
                }
            }

            return new LogAnalysisResult
            {
                UniqueUsers = userEvents.Select(e => e.UserId).Distinct().Count(),
                UserActivity = userEvents,
                Errors = errorCounts
            };
        }
    }
}