namespace App.Dtos;

public class ShipItDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string TargetName1 { get; set; } = String.Empty;
    public string TargetName2 { get; set; } = String.Empty;
    public string TargetEmail1 { get; set; } = String.Empty;
    public string TargetEmail2 { get; set; } = String.Empty;
    public string Status { get; set; } = String.Empty;
    public string? Summary { get; set; }
    public Decimal? Score { get; set; }
    public DateTime InvitedAt { get; set; }
}