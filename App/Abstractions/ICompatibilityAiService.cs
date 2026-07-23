using App.Dtos.CompatibilityDtos;

namespace App.Abstractions;

public interface ICompatibilityAiService
{
    Task<CompatibilityAnalysisResult> AnalyzeAsync(string input, CancellationToken cancellationToken);
}