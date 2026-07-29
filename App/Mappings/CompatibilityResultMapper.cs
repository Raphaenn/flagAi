using App.Dto.Compatibility;
using Domain.Compatibility;

namespace App.Mappings;

public static class CompatibilityResultMapper
{
    public static ShipItResult ToDomain(this CompatibilityResultDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var personA = dto.PersonA.ToDomain();
        var personB = dto.PersonB.ToDomain();
        var computedScore = ComputeScore(personA.Normalized, personB.Normalized);
        var finalScore = dto.Score > 0 ? dto.Score : computedScore;
        var finalCompatibility = string.IsNullOrWhiteSpace(dto.Compatibility)
            || dto.Compatibility.Equals("unknown", StringComparison.OrdinalIgnoreCase)
            ? ToCompatibilityBand(finalScore)
            : dto.Compatibility;

        return new ShipItResult(
            PersonA: personA,
            PersonB: personB,
            Summary: dto.Summary,
            Strengths: dto.Strengths,
            PossibleConflicts: dto.PossibleConflicts,
            MissingInfo: dto.MissingInfo,
            Score: finalScore,
            Compatibility: finalCompatibility
        );
    }

    private static PersonAnalysis ToDomain(this CompatibilityPersonAnalysis dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new PersonAnalysis(
            Name: dto.Name,
            Tags: dto.Tags,
            Normalized: dto.Normalized.ToDomain()
        );
    }

    private static NormalizedProfile ToDomain(this CompatibilityNormalizedProfile dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new NormalizedProfile(
            RelationshipGoal: dto.RelationshipGoal,
            Lifestyle: dto.Lifestyle,
            AffectionStyle: dto.AffectionStyle,
            FaithImportance: dto.FaithImportance,
            FamilyOrientation: dto.FamilyOrientation,
            SocialEnergy: dto.SocialEnergy,
            CommunicationStyle: dto.CommunicationStyle,
            AmbitionLevel: dto.AmbitionLevel,
            HumorStyle: dto.HumorStyle,
            FinancialMindset: dto.FinancialMindset
        );
    }

    private static decimal ComputeScore(NormalizedProfile a, NormalizedProfile b)
    {
        var scoreA = new CompatibilityScore
        {
            RelationshipGoal = a.RelationshipGoal,
            Lifestyle = a.Lifestyle,
            AffectionStyle = a.AffectionStyle,
            FaithImportance = a.FaithImportance,
            FamilyOrientation = a.FamilyOrientation,
            SocialEnergy = a.SocialEnergy,
            CommunicationStyle = a.CommunicationStyle,
            AmbitionLevel = a.AmbitionLevel,
            HumorStyle = a.HumorStyle,
            FinancialMindset = a.FinancialMindset
        };

        var scoreB = new CompatibilityScore
        {
            RelationshipGoal = b.RelationshipGoal,
            Lifestyle = b.Lifestyle,
            AffectionStyle = b.AffectionStyle,
            FaithImportance = b.FaithImportance,
            FamilyOrientation = b.FamilyOrientation,
            SocialEnergy = b.SocialEnergy,
            CommunicationStyle = b.CommunicationStyle,
            AmbitionLevel = b.AmbitionLevel,
            HumorStyle = b.HumorStyle,
            FinancialMindset = b.FinancialMindset
        };

        var raw = CalculatorScore.Calculate(scoreA, scoreB);
        const decimal maxRaw = 2.62m;
        var normalized = maxRaw == 0m ? 0m : raw / maxRaw;

        return decimal.Round(Math.Clamp(normalized, 0m, 1m), 2, MidpointRounding.AwayFromZero);
    }

    private static string ToCompatibilityBand(decimal score)
    {
        if (score >= 0.75m)
            return "high";

        if (score >= 0.45m)
            return "medium";

        return "low";
    }
}