using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using App.Abstractions;
using App.Dtos.CompatibilityDtos;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;

namespace Infra.Services.AI;

public sealed class CompatibilityAiService : ICompatibilityAiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public CompatibilityAiService(HttpClient client)
    {
        _httpClient = client;
    }
    
    public async Task<CompatibilityAnalysisResult> AnalyzeAsync(string input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be empty.", nameof(input));

        var promptPath = ResolvePromptPath();
        var systemPrompt = await File.ReadAllTextAsync(promptPath, cancellationToken);
        
        var payload = new
        {
            model = "claude-haiku-4-5-20251001",
            max_tokens = 1024,
            system = systemPrompt,
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

        var modelResponse = JsonSerializer.Deserialize<CompatibilityAnalysisResult>(jsonText, _jsonOptions)
                           ?? throw new InvalidOperationException("AI response could not be deserialized.");

        return modelResponse;
    }

    private static string ResolvePromptPath()
    {
        var current = Directory.GetCurrentDirectory();

        var localPath = Path.Combine(current, "prompt.txt");
        if (File.Exists(localPath))
            return localPath;

        var infraPath = Path.Combine(current, "..", "Infra", "prompt.txt");
        var fullInfraPath = Path.GetFullPath(infraPath);
        if (File.Exists(fullInfraPath))
            return fullInfraPath;

        var rootInfraPath = Path.Combine(current, "Infra", "prompt.txt");
        var fullRootInfraPath = Path.GetFullPath(rootInfraPath);
        if (File.Exists(fullRootInfraPath))
            return fullRootInfraPath;

        throw new FileNotFoundException("prompt.txt not found. Expected in Infra folder.");
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