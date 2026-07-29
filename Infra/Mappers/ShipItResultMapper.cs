using App.Dto;
using App.Dto.Compatibility;
using Infra.Dto;

namespace Infra.Mappers;

public static class ShipItResultMapper
{
    public static CompatibilityResultDto ToAppDto(this ShipItResultAiDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new CompatibilityResultDto(
            PersonA: dto.PersonA.ToAppDto(),
            PersonB: dto.PersonB.ToAppDto(),
            Summary: NormalizeText(dto.Summary),
            Strengths: NormalizeList(dto.Strengths),
            PossibleConflicts: NormalizeList(dto.PossibleConflicts),
            MissingInfo: NormalizeList(dto.MissingInfo),
            Score: dto.Score,
            Compatibility: NormalizeText(dto.Compatibility)
        );
    }

    public static CompatibilityPersonAnalysis ToAppDto(this ShipItResultAiPersonAnalysisDto? dto)
    {
        if (dto is null)
        {
            return new CompatibilityPersonAnalysis(
                Name: "unknown",
                Tags: Array.Empty<string>(),
                Normalized: new CompatibilityNormalizedProfile(
                    RelationshipGoal: "unknown",
                    Lifestyle: "unknown",
                    AffectionStyle: "unknown",
                    FaithImportance: "unknown",
                    FamilyOrientation: "unknown",
                    SocialEnergy: "unknown",
                    CommunicationStyle: "unknown",
                    AmbitionLevel: "unknown",
                    HumorStyle: "unknown",
                    FinancialMindset: "unknown"
                )
            );
        }

        return new CompatibilityPersonAnalysis(
            Name: NormalizeText(dto.Name),
            Tags: NormalizeList(dto.Tags).ToArray(),
            Normalized: dto.Normalized.ToAppDto()
        );
    }

    public static CompatibilityNormalizedProfile ToAppDto(this ShipItResultAiNormalizedProfileDto? dto)
    {
        if (dto is null)
        {
            return new CompatibilityNormalizedProfile(
                RelationshipGoal: "unknown",
                Lifestyle: "unknown",
                AffectionStyle: "unknown",
                FaithImportance: "unknown",
                FamilyOrientation: "unknown",
                SocialEnergy: "unknown",
                CommunicationStyle: "unknown",
                AmbitionLevel: "unknown",
                HumorStyle: "unknown",
                FinancialMindset: "unknown"
            );
        }

        return new CompatibilityNormalizedProfile(
            RelationshipGoal: NormalizeText(dto.RelationshipGoal),
            Lifestyle: NormalizeText(dto.Lifestyle ?? dto.LifestylePace),
            AffectionStyle: NormalizeText(dto.AffectionStyle),
            FaithImportance: NormalizeText(dto.FaithImportance),
            FamilyOrientation: NormalizeText(dto.FamilyOrientation),
            SocialEnergy: NormalizeText(dto.SocialEnergy),
            CommunicationStyle: NormalizeText(dto.CommunicationStyle),
            AmbitionLevel: NormalizeText(dto.AmbitionLevel),
            HumorStyle: NormalizeText(dto.HumorStyle),
            FinancialMindset: NormalizeText(dto.FinancialMindset)
        );
    }

    private static string NormalizeText(string? value)
        => string.IsNullOrWhiteSpace(value) ? "unknown" : value.Trim();

    private static IReadOnlyList<string> NormalizeList(IReadOnlyList<string>? values)
    {
        if (values is null || values.Count == 0)
            return Array.Empty<string>();

        return values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Trim())
            .ToArray();
    }
}