namespace Api.Models;

public sealed class CoupleAiSettings
{
    public const string SectionName = "ApiSettings";
    public string AnthropicApiKey { get; init; } = string.Empty;
}