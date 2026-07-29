namespace Infra.Dto;

public sealed class ShipItResultAiDto
{
    public ShipItResultAiPersonAnalysisDto? PersonA { get; init; }
    public ShipItResultAiPersonAnalysisDto? PersonB { get; init; }
    public string? Summary { get; init; }

    public IReadOnlyList<string>? Strengths { get; init; }
    public IReadOnlyList<string>? PossibleConflicts { get; init; }
    public IReadOnlyList<string>? MissingInfo { get; init; }

    public decimal Score { get; init; }
    public string? Compatibility { get; init; }
}

public sealed class ShipItResultAiPersonAnalysisDto
{
    public string? Name { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
    public ShipItResultAiNormalizedProfileDto? Normalized { get; init; }
}

public sealed class ShipItResultAiNormalizedProfileDto
{
    public string? RelationshipGoal { get; init; }
    public string? Lifestyle { get; init; }
    public string? LifestylePace { get; init; }
    public string? AffectionStyle { get; init; }
    public string? FaithImportance { get; init; }
    public string? FamilyOrientation { get; init; }
    public string? SocialEnergy { get; init; }
    public string? CommunicationStyle { get; init; }
    public string? AmbitionLevel { get; init; }
    public string? HumorStyle { get; init; }
    public string? FinancialMindset { get; init; }
}