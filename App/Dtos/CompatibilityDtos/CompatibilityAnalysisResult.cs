namespace App.Dtos.CompatibilityDtos;

public sealed record CompatibilityAnalysisResult(
    CompatibilityPersonAnalysis PersonA,
    CompatibilityPersonAnalysis PersonB,
    string Summary,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> PossibleConflicts,
    IReadOnlyList<string> MissingInfo,
    decimal Score = 0,
    string Compatibility = "unknown"
    );
    
public sealed record CompatibilityPersonAnalysis(string Name, CompatibilityNormalizedProfile Normalized);  
    
public sealed record CompatibilityNormalizedProfile(
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