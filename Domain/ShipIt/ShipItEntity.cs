namespace Domain.ShipIt;

public class ShipItEntity
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string TargetName1 { get; private set; }
    public string TargetName2 { get; private set; }
    public string TargetEmail1 { get; private set; }
    public string TargetEmail2 { get; private set; }
    public string Status { get; private set; } = String.Empty;
    public string? Summary { get; set; }
    public Decimal? Score { get; set; }
    public DateTime InvitedAt { get; set; }

    private ShipItEntity(Guid id, Guid ownerId, string targetName1, string targetName2, string targetEmail1, string targetEmail2)
    {
        this.Id = id;
        this.OwnerId = ownerId;
        this.TargetName1 = targetName1;
        this.TargetName2 = targetName2;
        this.TargetEmail1 = targetEmail1;
        this.TargetEmail2 = targetEmail2;
        this.InvitedAt = DateTime.UtcNow;
    }

    public static ShipItEntity CreateOne(Guid ownerId, string targetName1, string targetName2, string targetEmail1, string targetEmail2)
    {
        if (targetName1.Length <= 4 || targetName2.Length <= 4)
        {
            throw new Exception("Invalid user name");
        }
        
        return new ShipItEntity(Guid.NewGuid(), ownerId, targetName1, targetName2, targetEmail1, targetEmail2);
    }

    public void AddSummary(string summary)
    {
        if (summary.Length <= 50)
        {
            throw new Exception("Summary is too small");
        }
        this.Summary = summary;
    }
    
    public void SetCompatibility(decimal compatibilityScore)
    {
        Score = compatibilityScore;
        Status = "Calculated";
    }
}