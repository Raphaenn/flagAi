public sealed record ShipItResult(
    PersonAnalysis PersonA,
    PersonAnalysis PersonB,
    string Summary,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> PossibleConflicts,
    IReadOnlyList<string> MissingInfo,
    decimal Score,
    string Compatibility
    );

public sealed record PersonAnalysis(
    string Name,
    IReadOnlyList<string> Tags,
    NormalizedProfile Normalized
    );

public sealed record NormalizedProfile(
    string RelationshipGoal,
    string Lifestyle,
    string AffectionStyle,
    string FaithImportance,
    string FamilyOrientation,
    string SocialEnergy,
    string CommunicationStyle,
    string AmbitionLevel,
    string HumorStyle,
    string FinancialMindset
    );