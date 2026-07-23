using App.Abstractions;
using App.Dtos.CompatibilityDtos;

namespace App.Services.Compatibility;

public sealed class AnalyzeCompatibilityAppService
{
    private readonly ICompatibilityAiService _compatibilityAiService;

    public AnalyzeCompatibilityAppService(ICompatibilityAiService compatibilityAiService)
    {
        _compatibilityAiService = compatibilityAiService;
    }

    public Task<CompatibilityAnalysisResult> AnalyzeAsync(
        string description,
        string personAName,
        string personAEmail,
        string personBName,
        string personBEmail,
        CancellationToken cancellationToken)
    {
        var input = BuildInput(description, personAName, personAEmail, personBName, personBEmail);
        return _compatibilityAiService.AnalyzeAsync(input, cancellationToken);
    }

    private static string BuildInput(
        string description,
        string personAName,
        string personAEmail,
        string personBName,
        string personBEmail)
    {
        return $"""
               Person A:
               name: {personAName}
               email: {personAEmail}

               Person B:
               name: {personBName}
               email: {personBEmail}

               Description:
               {description}
               """;
    }
}