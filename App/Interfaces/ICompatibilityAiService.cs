using App.Dto.Compatibility;

namespace App.Interfaces;

public interface ICompatibilityAiAnalyzer
{
    Task<CompatibilityResultDto> AnalyzeAsync(string input, string? language, CancellationToken cancellationToken);
}