namespace WebRtcLogAnalyzer.Domain.Events;

public sealed record UserActivityEvent(
    DateTime Timestamp,
    string Event,
    string UserId);