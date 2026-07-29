using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using App.Dto.Compatibility;
using App.Interfaces;
using Infra.Dto;
using Infra.Mappers;

namespace Infra.Services.AI;

public sealed class CompatibilityAiClient : ICompatibilityAiAnalyzer
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public CompatibilityAiClient(HttpClient client)
    {
        _httpClient = client;
    }
    
    public async Task<CompatibilityResultDto> AnalyzeAsync(string input, string? language, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty.", nameof(input));

        var promptPath = ResolvePromptPath();
        var systemPrompt = await File.ReadAllTextAsync(promptPath, cancellationToken);
        var languageInstruction = BuildLanguageInstruction(language);
        var effectiveSystemPrompt = string.IsNullOrWhiteSpace(languageInstruction)
            ? systemPrompt
            : $"{systemPrompt}\n\n{languageInstruction}";
        
        var payload = new
        {
            model = "claude-haiku-4-5-20251001",
            max_tokens = 1024,
            system = effectiveSystemPrompt,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = input
                }
            }
        };
        
        var jsonPayload = JsonSerializer.Serialize(payload, _jsonOptions);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/messages")
        {
            Content = content
        };

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        response.EnsureSuccessStatusCode();

        var anthropicResponse = JsonSerializer.Deserialize<AnthropicResponse>(responseContent, _jsonOptions)
                               ?? throw new InvalidOperationException("AI response is empty or invalid.");

        var textPart = anthropicResponse.Content?.FirstOrDefault(c => c.Type == "text")?.Text;
        if (string.IsNullOrWhiteSpace(textPart))
            throw new InvalidOperationException("AI response does not contain text content.");

        var jsonText = ExtractJson(textPart);

        var modelResponse = JsonSerializer.Deserialize<ShipItResultAiDto>(jsonText, _jsonOptions)
                           ?? throw new InvalidOperationException("AI response could not be deserialized.");
        
        return modelResponse.ToAppDto();
    }

    private static string BuildLanguageInstruction(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return string.Empty;

        var normalized = language.Trim().ToLowerInvariant();
        var languageName = normalized switch
        {
            "pt" or "pt-br" or "pt-pt" or "portuguese" => "Portuguese",
            "en" or "en-us" or "en-gb" or "english" => "English",
            "es" or "es-es" or "es-mx" or "spanish" => "Spanish",
            _ => null
        };

        return languageName is null
            ? string.Empty
            : $"IMPORTANT: Return all response content in {languageName}.";
    }

    private static string ResolvePromptPath()
    {
        var baseDir = AppContext.BaseDirectory;
        var current = Directory.GetCurrentDirectory();

        var candidates = new[]
        {
            Path.Combine(baseDir, "prompts", "shipit.txt"),
            Path.Combine(current, "prompts", "shipit.txt"),
            Path.Combine(current, "shipit.txt"),
            Path.Combine(current, "..", "Infra", "prompts", "shipit.txt")
        };

        foreach (var candidate in candidates)
        {
            var fullPath = Path.GetFullPath(candidate);
            if (File.Exists(fullPath))
                return fullPath;
        }

        throw new FileNotFoundException("shipit.txt not found. Expected in prompts folder.");
    }

    private static string ExtractJson(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.StartsWith("```") && trimmed.Contains('{'))
        {
            var match = Regex.Match(trimmed, "\\{[\\s\\S]*\\}");
            if (match.Success)
                return match.Value;
        }

        return trimmed;
    }

    private sealed record AnthropicResponse(IReadOnlyList<AnthropicContentPart>? Content);

    private sealed record AnthropicContentPart(string Type, string? Text);
}