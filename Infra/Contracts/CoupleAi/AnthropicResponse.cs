namespace Infra.Contracts.CoupleAi;

public sealed record AnthropicResponse(IReadOnlyList<AnthropicContentPart>? Content, AnthropicUsage? Usage);
