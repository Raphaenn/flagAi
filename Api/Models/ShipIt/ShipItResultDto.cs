namespace Api.Models.ShipIt;

public sealed class ShipItResultAiDto
{
    public required ShipItResultAiPersonAnalysisDto PersonA { get; init; }
    public required ShipItResultAiPersonAnalysisDto PersonB { get; init; }
    public required string Summary { get; init; }

    public required IReadOnlyList<string> Strengths { get; init; }
    public required IReadOnlyList<string> PossibleConflicts { get; init; }
    public required IReadOnlyList<string> MissingInfo { get; init; }

    public required decimal Score { get; init; }
    public required string Compatibility { get; init; }
}

public sealed class ShipItResultAiPersonAnalysisDto
{
    public required string Name { get; init; }
    public required IReadOnlyList<string> Tags { get; init; }
    public required ShipItResultAiNormalizedProfileDto Normalized { get; init; }
}

public sealed class ShipItResultAiNormalizedProfileDto
{
    public required string RelationshipGoal { get; init; }
    public required string Lifestyle { get; init; }
    public required string AffectionStyle { get; init; }
    public required string FaithImportance { get; init; }
    public required string FamilyOrientation { get; init; }
    public required string SocialEnergy { get; init; }
    public required string CommunicationStyle { get; init; }
    public required string AmbitionLevel { get; init; }
    public required string HumorStyle { get; init; }
    public required string FinancialMindset { get; init; }
}