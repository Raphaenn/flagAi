using System.Text.Json.Serialization;

namespace Infra.Contracts.CoupleAi;

public sealed record AnthropicUsage(
    [property: JsonPropertyName("input_tokens")]
    int InputTokens,

    [property: JsonPropertyName("output_tokens")]
    int OutputTokens,
        
    [property: JsonPropertyName("cache_creation_input_tokens")]
    int CacheCreationInputTokens,

    [property: JsonPropertyName("cache_read_input_tokens")]
    int CacheReadInputTokens
    );