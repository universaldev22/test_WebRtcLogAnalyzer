using WebRtcLogAnalyzer.Application.DTOs;

namespace WebRtcLogAnalyzer.Application.Contracts;

public interface ILogAnalysisService
{
    Task<LogAnalysisResult> AnalyzeAsync(string originLogPath, CancellationToken ct = default);
}