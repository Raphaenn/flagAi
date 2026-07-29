using App.Interfaces;
using App.Mappings;
using Domain.Entities;

namespace App.UseCase;

public sealed class AnalyzeCompatibility
{
    private readonly ICompatibilityAiAnalyzer _compatibilityAiService;

    public AnalyzeCompatibility(ICompatibilityAiAnalyzer compatibilityAiService)
    {
        _compatibilityAiService = compatibilityAiService;
    }

    public async Task<ShipItResult> AnalyzeAsync(
        string description,
        string personAName,
        string personAEmail,
        string personBName,
        string personBEmail,
        string? language,
        CancellationToken cancellationToken)
    {
        var input = BuildInput(description, personAName, personAEmail, personBName, personBEmail, language);
        var result = await _compatibilityAiService.AnalyzeAsync(input, language, cancellationToken);

        return result.ToDomain();
    }

    private static string BuildInput(
        string description,
        string personAName,
        string personAEmail,
        string personBName,
        string personBEmail,
        string? language)
    {
        var languageInstruction = language switch
        {
            null or "" => string.Empty,
            _ => $"\n\nReturn the response in {ResolveLanguageName(language)}."
        };

        return $"""
               Person A:
               name: {personAName}
               email: {personAEmail}

               Person B:
               name: {personBName}
               email: {personBEmail}

               Description:
               {description}{languageInstruction}
               """;
    }

    private static string ResolveLanguageName(string language)
    {
        var normalized = language.Trim().ToLowerInvariant();

        return normalized switch
        {
            "pt" or "pt-br" or "pt-pt" or "portuguese" => "Portuguese",
            "en" or "en-us" or "en-gb" or "english" => "English",
            "es" or "es-es" or "es-mx" or "spanish" => "Spanish",
            _ => "Portuguese"
        };
    }
}