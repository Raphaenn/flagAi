namespace Domain.Compatibility;

public static class CalculatorScore
{
    // * Algorithm
    // RelationshipGoal * 0.5 +
    // AffectionStyle * 0.1;
    // Lifestyle * 0.08 +
    // FaithImportance * 0.8;
    // FinancialMindset * 0.6 +
    // AmbitionLevel * 0.6;
    // FamilyOrientation * 0.4;
    // HumorStyle * 0.04 +
    // CommunicationStyle * 0.02 +
    // SocialEnergy * 0.02;
    
    public static Decimal Calculate(CompatibilityScore score1, CompatibilityScore score2)
    {
        decimal initialScore = 0m;
        if (score1.RelationshipGoal == score2.RelationshipGoal)
            initialScore += 0.5m;
        
        if (score1.AffectionStyle == score2.AffectionStyle)
            initialScore += 0.1m;
        
        if (score1.Lifestyle == score2.Lifestyle)
            initialScore += 0.08m;
        
        if (score1.FaithImportance == score2.FaithImportance)
            initialScore += 0.08m;
        
        if (score1.FinancialMindset == score2.FinancialMindset)
            initialScore += 0.06m;
        
        if (score1.AmbitionLevel == score2.AmbitionLevel)
            initialScore += 0.6m;
        
        if (score1.FamilyOrientation == score2.FamilyOrientation)
            initialScore += 0.4m;
        
        if (score1.HumorStyle == score2.HumorStyle)
            initialScore += 0.4m;
        
        if (score1.CommunicationStyle == score2.CommunicationStyle)
            initialScore += 0.2m;
        
        if (score1.SocialEnergy == score2.SocialEnergy)
            initialScore += 0.2m;

        return initialScore;
    }
}